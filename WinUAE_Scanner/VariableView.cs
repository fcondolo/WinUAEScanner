using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
Amiga-Side example:

DBG_BYTE	EQU 1
DBG_WORD	EQU 2
DBG_LONG	EQU 4
DBG_CPER    EQU 8
DBG_BIN		EQU (1<<8)
DBG_DEC		EQU (1<<9)
DBG_HEX		EQU (1<<10)

	dc.b			'[STRT-DBG]'
debugHeader:
			dc.w	3
			dc.w	counterl-debugHeader,DBG_LONG|DBG_HEX
			dc.w	counterw-debugHeader,DBG_WORD|DBG_DEC
			dc.w	counterb-debugHeader,DBG_BYTE|DBG_BIN

counterl:	dc.l	$DEADBEEF
counterw:	dc.w	1000
counterb:	dc.b	10
	even

	dc.b			'[END-DBG ]'
 
*/

namespace WinUAE_Scanner
{
    public partial class VariableView : Form
    {
        const int DBG_BYTE = 1;
        const int DBG_WORD = 2;
        const int DBG_LONG = 4;
        const int DBG_CPER = 8;
        const int DBG_BIN = 1<<8;
        const int DBG_DEC = 1<<9;
        const int DBG_HEX = 1<<10;

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private bool pauseBackgroundWorker = true;
        private Dictionary<ushort, string> customReg;

        private void createDico()
        {
            try
            {
                customReg = new Dictionary<ushort, string>();

                customReg.Add(0x000, " BLTDDAT Blitter destination early read (unusable)");
                customReg.Add(0x002, " DMACONR DMA control(and blitter status) read");
                customReg.Add(0x004, " VPOSR   Read vertical raster position bit 9(and interlace odd / even frame)");
                customReg.Add(0x006, " VHPOSR  Rest of raster XY position - High byte: vertical, low byte: horizontal");
                customReg.Add(0x008, " DSKDATR Disk data early read (unusable)");
                customReg.Add(0x00a, " JOY0DAT Joystick / mouse 0 data");
                customReg.Add(0x00c, " JOT1DAT Joystick / mouse 1 data");
                customReg.Add(0x00e, " CLXDAT  Poll(read and clear) sprite collision state");
                customReg.Add(0x010, " ADKCONR Audio, disk control register read");
                customReg.Add(0x012, " POT0DAT Pot counter pair 0 data");
                customReg.Add(0x014, " POT1DAT Pot counter pair 1 data");
                customReg.Add(0x016, " POTGOR  Pot pin data read");
                customReg.Add(0x018, " SERDATR Serial port data and status read");
                customReg.Add(0x01a, " DSKBYTR Disk data byte and status read");
                customReg.Add(0x01c, " INTENAR Interrupt enable bits read");
                customReg.Add(0x01e, " INTREQR Interrupt request bits read");
                customReg.Add(0x020, " DSKPTH  Disk track buffer pointer(high 5 bits)");
                customReg.Add(0x022, " DSKPTL  Disk track buffer pointer(low 15 bits)");
                customReg.Add(0x024, " DSKLEN  Disk track buffer length");
                customReg.Add(0x026, " DSKDAT  Disk DMA data write");
                customReg.Add(0x028, " REFPTR  AGA: Refresh pointer");
                customReg.Add(0x02a, " VPOSW   Write vert most sig.bits(and frame flop)");
                customReg.Add(0x02c, " VHPOSW  Write vert and horiz pos of beam");
                customReg.Add(0x02e, " COPCON  Coprocessor control register(CDANG)");
                customReg.Add(0x030, " SERDAT  Serial port data and stop bits write");
                customReg.Add(0x032, " SERPER  Serial port period and control");
                customReg.Add(0x034, " POTGO   Pot count start, pot pin drive enable data");
                customReg.Add(0x036, " JOYTEST Write to all 4 joystick / mouse counters at once");
                customReg.Add(0x038, " STREQU  Strobe for horiz sync with VBLANK and EQU");
                customReg.Add(0x03a, " STRVBL  Strobe for horiz sync with VBLANK");
                customReg.Add(0x03c, " STRHOR  Strobe for horiz sync");
                customReg.Add(0x03e, " STRLONG Strobe for identification of long / short horiz line");
                customReg.Add(0x040, " BLTCON0 Blitter control reg 0");
                customReg.Add(0x042, " BLTCON1 Blitter control reg 1");
                customReg.Add(0x044, " BLTAFWM Blitter first word mask for source A");
                customReg.Add(0x046, " BLTALWM Blitter last word mask for source A");
                customReg.Add(0x048, " BLTCPTH Blitter pointer to source C(high 5 bits)");
                customReg.Add(0x04a, " BLTCPTL Blitter pointer to source C(low 15 bits)");
                customReg.Add(0x04c, " BLTBPTH Blitter pointer to source B(high 5 bits)");
                customReg.Add(0x04e, " BLTBPTL Blitter pointer to source B(low 15 bits)");
                customReg.Add(0x050, " BLTAPTH Blitter pointer to source A(high 5 bits)");
                customReg.Add(0x052, " BLTAPTL Blitter pointer to source A(low 15 bits)");
                customReg.Add(0x054, " BLTDPTH Blitter pointer to destination D(high 5 bits)");
                customReg.Add(0x056, " BLTDPTL Blitter pointer to destination D(low 15 bits)");
                customReg.Add(0x058, " BLTSIZE Blitter start and size(win / width, height)");
                customReg.Add(0x05a, " BLTCON0L    Blitter control 0 lower 8 bits(minterms)");
                customReg.Add(0x05c, " BLTSIZV Blitter V size(for 15 bit vert size)");
                customReg.Add(0x05e, " BLTSIZH ECS: Blitter H size & start(for 11 bit H size)");
                customReg.Add(0x060, " BLTCMOD Blitter modulo for source C");
                customReg.Add(0x062, " BLTBMOD Blitter modulo for source B");
                customReg.Add(0x064, " BLTAMOD Blitter modulo for source A");
                customReg.Add(0x066, " BLTDMOD Blitter modulo for destination D");
                customReg.Add(0x068, " RESERVED    Reserved");
                customReg.Add(0x06a, " RESERVED    Reserved");
                customReg.Add(0x06c, " RESERVED    Reserved");
                customReg.Add(0x06e, " RESERVED    Reserved");
                customReg.Add(0x070, " BLTCDAT Blitter source C data reg");
                customReg.Add(0x072, " BLTBDAT Blitter source B data reg");
                customReg.Add(0x074, " BLTADAT Blitter source A data reg");
                customReg.Add(0x076, " RESERVED    Reserved");
                customReg.Add(0x078, " SPRHDAT AGA: Ext logic UHRES sprite pointer and data identifier");
                customReg.Add(0x07a, " BPLHDAT AGA: Ext logic UHRES bit plane identifier");
                customReg.Add(0x07c, " LISAID  AGA: Chip revision level for Denise / Lisa");
                customReg.Add(0x07e, " DSKSYNC Disk sync pattern");
                customReg.Add(0x080, " COP1LCH Write Copper pointer 1(high 5 bits)");
                customReg.Add(0x082, " COP1LCL Write Copper pointer 1(low 15 bits)");
                customReg.Add(0x084, " COP2LCH Write Copper pointer 2(high 5 bits)");
                customReg.Add(0x086, " COP2LCL Write Copper pointer 2(low 15 bits)");
                customReg.Add(0x088, " COPJMP1 Trigger Copper 1(any value)");
                customReg.Add(0x08a, " COPJMP2 Trigger Copper 2(any value)");
                customReg.Add(0x08c, " COPINS  Coprocessor inst fetch identify");
                customReg.Add(0x08e, " DIWSTRT Display window start(upper left vert - hor pos)");
                customReg.Add(0x090, " DIWSTOP Display window stop(lower right vert - hor pos)");
                customReg.Add(0x092, " DDFSTRT Display bitplane data fetch start.hor pos");
                customReg.Add(0x094, " DDFSTOP Display bitplane data fetch stop.hor pos");
                customReg.Add(0x096, " DMACON  DMA control write(clear or set)");
                customReg.Add(0x098, " CLXCON  Write Sprite collision control bits");
                customReg.Add(0x09a, " INTENA  Interrupt enable bits(clear or set bits)");
                customReg.Add(0x09c, " INTREQ  Interrupt request bits(clear or set bits)");
                customReg.Add(0x09e, " ADKCON  Audio, disk and UART control");
                customReg.Add(0x0a0, " AUD0LCH Audio channel 0 pointer(high 5 bits)");
                customReg.Add(0x0a2, " AUD0LCL Audio channel 0 pointer(low 15 bits)");
                customReg.Add(0x0a4, " AUD0LEN Audio channel 0 length");
                customReg.Add(0x0a6, " AUD0PER Audio channel 0 period");
                customReg.Add(0x0a8, " AUD0VOL Audio channel 0 volume");
                customReg.Add(0x0aa, " AUD0DAT Audio channel 0 data");
                customReg.Add(0x0ac, " RESERVED    Reserved");
                customReg.Add(0x0ae, " RESERVED    Reserved");
                customReg.Add(0x0b0, " AUD1LCH Audio channel 1 pointer(high 5 bits)");
                customReg.Add(0x0b2, " AUD1LCL Audio channel 1 pointer(low 15 bits)");
                customReg.Add(0x0b4, "AUD1LEN Audio channel 1 length");
                customReg.Add(0x0b6, " AUD1PER Audio channel 1 period");
                customReg.Add(0x0b8, " AUD1VOL Audio channel 1 volume");
                customReg.Add(0x0ba, " AUD1DAT Audio channel 1 data");
                customReg.Add(0x0bc, " RESERVED    Reserved");
                customReg.Add(0x0be, " RESERVED    Reserved");
                customReg.Add(0x0c0, " AUD2LCH Audio channel 2 pointer(high 5 bits)");
                customReg.Add(0x0c2, " AUD2LCL Audio channel 2 pointer(low 15 bits)");
                customReg.Add(0x0c4, " AUD2LEN Audio channel 2 length");
                customReg.Add(0x0c6, " AUD2PER Audio channel 2 period");
                customReg.Add(0x0c8, " AUD2VOL Audio channel 2 volume");
                customReg.Add(0x0ca, " AUD2DAT Audio channel 2 data");
                customReg.Add(0x0cc, " RESERVED    Reserved");
                customReg.Add(0x0ce, " RESERVED    Reserved");
                customReg.Add(0x0d0, " AUD3LCH Audio channel 3 pointer(high 5 bits)");
                customReg.Add(0x0d2, " AUD3LCL Audio channel 3 pointer(low 15 bits)");
                customReg.Add(0x0d4, "AUD3LEN Audio channel 3 length");
                customReg.Add(0x0d6, " AUD3PER Audio channel 3 period");
                customReg.Add(0x0d8, " AUD3VOL Audio channel 3 volume");
                customReg.Add(0x0da, " AUD3DAT Audio channel 3 data");
                customReg.Add(0x0dc, " RESERVED    Reserved");
                customReg.Add(0x0de, " RESERVED    Reserved");
                customReg.Add(0x0e0, " BPL1PTH Bitplane pointer 1(high 5 bits)");
                customReg.Add(0x0e2, " BPL1PTL Bitplane pointer 1(low 15 bits)");
                customReg.Add(0x0e4, " BPL2PTH Bitplane pointer 2(high 5 bits)");
                customReg.Add(0x0e6, " BPL2PTL Bitplane pointer 2(low 15 bits)");
                customReg.Add(0x0e8, " BPL3PTH Bitplane pointer 3(high 5 bits)");
                customReg.Add(0x0ea, " BPL3PTL Bitplane pointer 3(low 15 bits)");
                customReg.Add(0x0ec, " BPL4PTH Bitplane pointer 4(high 5 bits)");
                customReg.Add(0x0ee, " BPL4PTL Bitplane pointer 4(low 15 bits)");
                customReg.Add(0x0f0, " BPL5PTH Bitplane pointer 5(high 5 bits)");
                customReg.Add(0x0f2, " BPL5PTL Bitplane pointer 5(low 15 bits)");
                customReg.Add(0x0f4, " BPL6PTH Bitplane pointer 6(high 5 bits)");
                customReg.Add(0x0f6, " BPL6PTL Bitplane pointer 6(low 15 bits)");
                customReg.Add(0x0f8, " BPL7PTH AGA: Bitplane pointer 7(high 5 bits)");
                customReg.Add(0x0fa, " BPL7PTL AGA: Bitplane pointer 7(low 15 bits)");
                customReg.Add(0x0fc, " BPL8PTH AGA: Bitplane pointer 8(high 5 bits)");
                customReg.Add(0x0fe, " BPL8PTL AGA: Bitplane pointer 8(low 15 bits)");
                customReg.Add(0x100, " BPLCON0 Bitplane depth and screen mode)");
                customReg.Add(0x102, " BPLCON1 Bitplane / playfield horizontal scroll values");
                customReg.Add(0x104, " BPLCON2 Sprites vs. Playfields priority");
                customReg.Add(0x106, " BPLCON3 AGA: Bitplane control reg(enhanced features)");
                customReg.Add(0x108, " BPL1MOD Bitplane modulo (odd planes)");
                customReg.Add(0x10a, " BPL2MOD Bitplane modulo (even planes)");
                customReg.Add(0x10c, " BPLCON4 AGA: Bitplane control reg(bitplane & sprite masks)");
                customReg.Add(0x10e, " CLXCON2 AGA: Write Extended sprite collision control bits");
                customReg.Add(0x110, " BPL1DAT Bitplane 1 data(parallel to serial convert)");
                customReg.Add(0x112, " BPL2DAT Bitplane 2 data(parallel to serial convert)");
                customReg.Add(0x114, " BPL3DAT Bitplane 3 data(parallel to serial convert)");
                customReg.Add(0x116, " BPL4DAT Bitplane 4 data(parallel to serial convert)");
                customReg.Add(0x118, " BPL5DAT Bitplane 5 data(parallel to serial convert)");
                customReg.Add(0x11a, " BPL6DAT Bitplane 6 data(parallel to serial convert)");
                customReg.Add(0x11c, " BPL7DAT AGA: Bitplane 7 data(parallel to serial convert)");
                customReg.Add(0x11e, " BPL8DAT AGA: Bitplane 8 data(parallel to serial convert)");
                customReg.Add(0x120, " SPR0PTH Sprite 0 pointer(high 5 bits)");
                customReg.Add(0x122, " SPR0PTL Sprite 0 pointer(low 15 bits)");
                customReg.Add(0x124, " SPR1PTH Sprite 1 pointer(high 5 bits)");
                customReg.Add(0x126, " SPR1PTL Sprite 1 pointer(low 15 bits)");
                customReg.Add(0x128, " SPR2PTH Sprite 2 pointer(high 5 bits)");
                customReg.Add(0x12a, " SPR2PTL Sprite 2 pointer(low 15 bits)");
                customReg.Add(0x12c, " SPR3PTH Sprite 3 pointer(high 5 bits)");
                customReg.Add(0x12e, " SPR3PTL Sprite 3 pointer(low 15 bits)");
                customReg.Add(0x130, " SPR4PTH Sprite 4 pointer(high 5 bits)");
                customReg.Add(0x132, " SPR4PTL Sprite 4 pointer(low 15 bits)");
                customReg.Add(0x134, " SPR5PTH Sprite 5 pointer(high 5 bits)");
                customReg.Add(0x136, " SPR5PTL Sprite 5 pointer(low 15 bits)");
                customReg.Add(0x138, " SPR6PTH Sprite 6 pointer(high 5 bits)");
                customReg.Add(0x13a, " SPR6PTL Sprite 6 pointer(low 15 bits)");
                customReg.Add(0x13c, " SPR7PTH Sprite 7 pointer(high 5 bits)");
                customReg.Add(0x13e, " SPR7PTL Sprite 7 pointer(low 15 bits)");
                customReg.Add(0x140, " SPR0POS Sprite 0 vert - horiz start pos data");
                customReg.Add(0x142, " SPR0CTL Sprite 0 position and control data");
                customReg.Add(0x144, " SPR0DATA    Sprite 0 low bitplane data");
                customReg.Add(0x146, " SPR0DATB    Sprite 0 high bitplane data");
                customReg.Add(0x148, " SPR1POS Sprite 1 vert - horiz start pos data");
                customReg.Add(0x14a, " SPR1CTL Sprite 1 position and control data");
                customReg.Add(0x14c, " SPR1DATA    Sprite 1 low bitplane data");
                customReg.Add(0x14e, " SPR1DATB    Sprite 1 high bitplane data");
                customReg.Add(0x150, " SPR2POS Sprite 2 vert - horiz start pos data");
                customReg.Add(0x152, " SPR2CTL Sprite 2 position and control data");
                customReg.Add(0x154, " SPR2DATA    Sprite 2 low bitplane data");
                customReg.Add(0x156, " SPR2DATB    Sprite 2 high bitplane data");
                customReg.Add(0x158, " SPR3POS Sprite 3 vert - horiz start pos data");
                customReg.Add(0x15a, " SPR3CTL Sprite 3 position and control data");
                customReg.Add(0x15c, " SPR3DATA    Sprite 3 low bitplane data");
                customReg.Add(0x15e, " SPR3DATB    Sprite 3 high bitplane data");
                customReg.Add(0x160, " SPR4POS Sprite 4 vert - horiz start pos data");
                customReg.Add(0x162, " SPR4CTL Sprite 4 position and control data");
                customReg.Add(0x164, " SPR4DATA    Sprite 4 low bitplane data");
                customReg.Add(0x166, " SPR4DATB    Sprite 4 high bitplane data");
                customReg.Add(0x168, " SPR5POS Sprite 5 vert - horiz start pos data");
                customReg.Add(0x16a, " SPR5CTL Sprite 5 position and control data");
                customReg.Add(0x16c, " SPR5DATA    Sprite 5 low bitplane data");
                customReg.Add(0x16e, " SPR5DATB    Sprite 5 high bitplane data");
                customReg.Add(0x170, " SPR6POS Sprite 6 vert - horiz start pos data");
                customReg.Add(0x172, " SPR6CTL Sprite 6 position and control data");
                customReg.Add(0x174, " SPR6DATA    Sprite 6 low bitplane data");
                customReg.Add(0x176, " SPR6DATB    Sprite 6 high bitplane data");
                customReg.Add(0x178, " SPR7POS Sprite 7 vert - horiz start pos data");
                customReg.Add(0x17a, " SPR7CTL Sprite 7 position and control data");
                customReg.Add(0x17c, " SPR7DATA    Sprite 7 low bitplane data");
                customReg.Add(0x17e, " SPR7DATB    Sprite 7 high bitplane data");
                customReg.Add(0x180, " COLOR00 Palette color 00");
                customReg.Add(0x182, " COLOR01 Palette color 1");
                customReg.Add(0x184, " COLOR02 Palette color 2");
                customReg.Add(0x186, " COLOR03 Palette color 3");
                customReg.Add(0x188, " COLOR04 Palette color 4");
                customReg.Add(0x18a, " COLOR05 Palette color 5");
                customReg.Add(0x18c, "	COLOR06	Palette color 6");
                customReg.Add(0x18e, "	COLOR07	Palette color 7");
                customReg.Add(0x190, "	COLOR08	Palette color 8");
                customReg.Add(0x192, "	COLOR09	Palette color 9");
                customReg.Add(0x194, "	COLOR10	Palette color 10");
                customReg.Add(0x196, "	COLOR11	Palette color 11");
                customReg.Add(0x198, "	COLOR12	Palette color 12");
                customReg.Add(0x19a, "	COLOR13	Palette color 13");
                customReg.Add(0x19c, "	COLOR14	Palette color 14");
                customReg.Add(0x19e, "	COLOR15	Palette color 15");
                customReg.Add(0x1a0, "	COLOR16	Palette color 16");
                customReg.Add(0x1a2, "	COLOR17	Palette color 17");
                customReg.Add(0x1a4, "	COLOR18	Palette color 18");
                customReg.Add(0x1a6, "	COLOR19	Palette color 19");
                customReg.Add(0x1a8, "	COLOR20	Palette color 20");
                customReg.Add(0x1aa, "	COLOR21	Palette color 21");
                customReg.Add(0x1ac, "	COLOR22	Palette color 22");
                customReg.Add(0x1ae, "	COLOR23	Palette color 23");
                customReg.Add(0x1b0, "	COLOR24	Palette color 24");
                customReg.Add(0x1b2, "COLOR25	Palette color 25");
                customReg.Add(0x1b4, "	COLOR26	Palette color 26");
                customReg.Add(0x1b6, "COLOR27	Palette color 27");
                customReg.Add(0x1b8, "	COLOR28	Palette color 28");
                customReg.Add(0x1ba, "	COLOR29	Palette color 29");
                customReg.Add(0x1bc, "	COLOR30	Palette color 30");
                customReg.Add(0x1be, "	COLOR31	Palette color 31");
                customReg.Add(0x1c0, "	HTOTAL	AGA: Highest number count in horiz line (VARBEAMEN = 1)");
                customReg.Add(0x1c2, "	HSSTOP	AGA: Horiz line pos for HSYNC stop");
                customReg.Add(0x1c4, "	HBSTRT	AGA: Horiz line pos for HBLANK start");
                customReg.Add(0x1c6, "	HBSTOP	AGA: Horiz line pos for HBLANK stop");
                customReg.Add(0x1c8, "	VTOTAL	AGA: Highest numbered vertical line (VARBEAMEN = 1)");
                customReg.Add(0x1ca, "	VSSTOP	AGA: Vert line for Vsync stop");
                customReg.Add(0x1cc, "	VBSTRT	AGA: Vert line for VBLANK start");
                customReg.Add(0x1ce, "	VBSTOP	AGA: Vert line for VBLANK stop");
                customReg.Add(0x1d0, "	SPRHSTRT	AGA: UHRES sprite vertical start");
                customReg.Add(0x1d2, "	SPRHSTOP	AGA: UHRES sprite vertical stop");
                customReg.Add(0x1d4, "	BPLHSTRT	AGA: UHRES bit plane vertical start");
                customReg.Add(0x1d6, "	BPLHSTOP	AGA: UHRES bit plane vertical stop");
                customReg.Add(0x1d8, "	HHPOSW	AGA: DUAL mode hires H beam counter write");
                customReg.Add(0x1da, "	HHPOSR	AGA: DUAL mode hires H beam counter read");
                customReg.Add(0x1dc, "	BEAMCON0	Beam counter control register");
                customReg.Add(0x1de, "	HSSTRT	AGA: Horizontal sync start (VARHSY)");
                customReg.Add(0x1e0, "	VSSTRT	AGA: Vertical sync start (VARVSY)");
                customReg.Add(0x1e2, "	HCENTER	AGA: Horizontal pos for vsync on interlace");
                customReg.Add(0x1e4, "	DIWHIGH	AGA: Display window upper bits for start/stop");
                customReg.Add(0x1e6, "	BPLHMOD	AGA: UHRES bit plane modulo");
                customReg.Add(0x1e8, "	SPRHPTH	AGA: UHRES sprite pointer (high 5 bits)");
                customReg.Add(0x1ea, "	SPRHPTL	AGA: UHRES sprite pointer (low 15 bits)");
                customReg.Add(0x1ec, "	BPLHPTH	AGA: VRam (UHRES) bitplane pointer (high 5 bits)");
                customReg.Add(0x1ee, "	BPLHPTL	AGA: VRam (UHRES) bitplane pointer (low 15 bits)");
                customReg.Add(0x1f0, "	RESERVED	Reserved");
                customReg.Add(0x1f2, "	RESERVED	Reserved");
                customReg.Add(0x1f4, "	RESERVED	Reserved");
                customReg.Add(0x1f6, "	RESERVED	Reserved");
                customReg.Add(0x1f8, "	RESERVED	Reserved");
                customReg.Add(0x1fa, "RESERVED	Reserved");
                customReg.Add(0x1fc, "	FMODE	AGA: Write Fetch mode (0=OCS compatible)");
                customReg.Add(0x1fe, "	NO-OP	No operation/NULL (Copper NOP instruction)");
            } catch (Exception ex)
            {
                MessageBox.Show("create dico: " + ex.Message);
            }
}

private void formClosingCallback(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                pauseBackgroundWorker = true;
                backgroundWorker1.CancelAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender,
            DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            while (worker.CancellationPending != true)
            {
                if (pauseBackgroundWorker == false)
                {
                    Scanner.refreshLastZone();
                    listView1.Invoke((MethodInvoker)delegate {
                        fillData();
                    });
                }
            }
        }

        private void disposed(object sender, System.EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            pauseBackgroundWorker = true;
        }

        public VariableView()
        {
            InitializeComponent();
        }

        byte getByte(byte[] _data, ref uint _index)
        {
            return _data[_index++];
        }

        ushort  getWord(byte[] _data, ref uint _index)
        {
            byte b1 = getByte(_data, ref _index);
            byte b2 = getByte(_data, ref _index);
            return (ushort)((b1 << 8) | b2);
        }

        uint getLong(byte[] _data, ref uint _index)
        {
            ushort b1 = getWord(_data, ref _index);
            ushort b2 = getWord(_data, ref _index);
            return (uint)((b1 << 16) | b2);
        }

        void dumpCopper(ref uint readIndex)
        {
            byte[] data = Scanner.ScannerBuffer;
            while (readIndex < data.Length)
            {
                string instr = "";
                string param = "";
                ushort word1 = getWord(data, ref readIndex);
                ushort word2 = getWord(data, ref readIndex);
                if ((word1 & 1) != 0)
                {
                    if ((word2 & 1) == 0)
                        instr = "WAIT";
                    else
                        instr = "SKIP";
                    int copper_xToWait = word1 & 255;
                    int copper_yToWait = word1 >> 8;
                    param = word1.ToString("X4") + "," + word2.ToString("X4");
                }
                else
                { // move
                    instr = "MOVE";
                    param = word1.ToString("X4") + "," + word2.ToString("X4");
                    if (customReg.ContainsKey(word1))
                    {
                        param += " -- " + customReg[word1];
                    }
                }

                ListViewItem row = new ListViewItem(readIndex.ToString());
                row.SubItems.Add(new ListViewItem.ListViewSubItem(row, instr.ToString() + " " + param));
                listView1.Items.Add(row);
                if (word1 == 0xffff)
                    break;
            }
        }


        void fillData()
        {
            try {
                listView1.Items.Clear();
                byte[] data = Scanner.ScannerBuffer;
                uint readIndex = 0;
                ushort count = getWord(data, ref readIndex);
                string HexFormat = "X";
                for (int i = 0; i< count; i++)
                {
                    uint offset = getLong(data, ref readIndex);
                    ushort meta = getWord(data, ref readIndex);
                    uint value = 0;
                    switch (meta&255)
                    {
                        case DBG_BYTE:
                            value = getByte(data, ref offset);
                            break;
                        case DBG_WORD:
                            value = getWord(data, ref offset);
                            HexFormat = "X2";
                            break;
                        case DBG_LONG:
                            value = getLong(data, ref offset);
                            HexFormat = "X4";
                            break;
                        case DBG_CPER:
                            break;
                        default:
                            MessageBox.Show("ERROR: Variable type not identified at offset " + offset.ToString());
                            break;
                    }
                    string valueStr="ERROR";
                    switch (meta&0xff00)
                    {
                        case DBG_BIN:
                            valueStr = "%"+Convert.ToString(value,2);
                            break;
                        case DBG_DEC:
                            valueStr = value.ToString();
                            break;
                        case DBG_HEX:
                            valueStr = "$"+value.ToString(HexFormat);
                            break;
                    }
                    if ((meta&255) == DBG_CPER)
                    {
                        dumpCopper(ref offset);
                    } else
                    {
                        ListViewItem row = new ListViewItem(offset.ToString());
                        row.SubItems.Add(new ListViewItem.ListViewSubItem(row, valueStr));
                        listView1.Items.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("fillData: " + ex.Message);
            }
        }

        private void VariableView_Load(object sender, EventArgs e)
        {
            this.FormClosing += new FormClosingEventHandler(formClosingCallback);
            try
            {
                backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
                backgroundWorker1.WorkerReportsProgress = false;
                backgroundWorker1.WorkerSupportsCancellation = true;
                backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            } catch (Exception ex)
            {
                MessageBox.Show("backgroundWorker1 creation: " + ex.Message);
            }
            pauseBackgroundWorker = true;
            listView1.Items.Clear();
            listView1.Columns.Add("Offset", 80);
            listView1.Columns.Add("Value", 500);
            createDico();
            fillData();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bool auto = checkBox1.Checked;
            if (auto)
            {
                if (backgroundWorker1.IsBusy != true)
                {
                    backgroundWorker1.RunWorkerAsync();
                }
                pauseBackgroundWorker = false;
            }
            else
            {
                pauseBackgroundWorker = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Scanner.refreshLastZone();
            fillData();
        }
    }
}
