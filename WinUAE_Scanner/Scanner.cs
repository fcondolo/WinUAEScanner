using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Principal;

namespace WinUAE_Scanner
{
    class Scanner
    {
        // REQUIRED CONSTS

        const int PROCESS_QUERY_INFORMATION = 0x0400;
        const int MEM_COMMIT = 0x00001000;
        const int PAGE_READWRITE = 0x04;
        const int PROCESS_WM_READ = 0x0010;

        // REQUIRED METHODS

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess
             (int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory
        (int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int VirtualQueryEx(IntPtr hProcess,
        IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);



        // REQUIRED STRUCTS

        public struct MEMORY_BASIC_INFORMATION
        {
            public int BaseAddress;
            public int AllocationBase;
            public int AllocationProtect;
            public int RegionSize;
            public int State;
            public int Protect;
            public int lType;
        }

        public struct SYSTEM_INFO
        {
            public ushort processorArchitecture;
            ushort reserved;
            public uint pageSize;
            public IntPtr minimumApplicationAddress;
            public IntPtr maximumApplicationAddress;
            public IntPtr activeProcessorMask;
            public uint numberOfProcessors;
            public uint processorType;
            public uint allocationGranularity;
            public ushort processorLevel;
            public ushort processorRevision;
        }

        public static byte[] ScannerBuffer;
        public static Process process = null;
        public static IntPtr processHandle;
        static MEMORY_BASIC_INFORMATION mem_basic_info;
        static SYSTEM_INFO sys_info;
        static IntPtr proc_min_address;
        static IntPtr proc_max_address;
        static long proc_min_address_l;
        static long proc_max_address_l;
        public static int BASE_Index = -1;

        static bool Connect(string _processName)
        {
            if (process != null)
                return true;

            sys_info = new SYSTEM_INFO();
            GetSystemInfo(out sys_info);

            proc_min_address = sys_info.minimumApplicationAddress;
            proc_max_address = sys_info.maximumApplicationAddress;

            // saving the values as long ints so I won't have to do a lot of casts later
            proc_min_address_l = (long)proc_min_address;
            proc_max_address_l = (long)proc_max_address;


            // notepad better be runnin'
            Process[] processList = Process.GetProcessesByName(_processName);
            if (processList.Length == 0)
            {
                MessageBox.Show("ERROR: could not find any process called " + _processName);
                return false;
            }

            process = processList[0];

            // opening the process with desired access level
            processHandle =
            OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_WM_READ, false, process.Id);
            if (processHandle == null)
            {
                MessageBox.Show("ERROR: could not open process.");
                return false;
            }

            //            StreamWriter sw = new StreamWriter("dump.txt");

            // this will store any information we get from VirtualQueryEx()
            mem_basic_info = new MEMORY_BASIC_INFORMATION();
            return true;
        }

        public static bool FindTags(string _processName)
        {
            if (!Connect(_processName)) return false;
            int bytesRead = 0;  // number of bytes read with ReadProcessMemory

            while (proc_min_address_l < proc_max_address_l)
            {
                // 28 = sizeof(MEMORY_BASIC_INFORMATION)
                VirtualQueryEx(processHandle, proc_min_address, out mem_basic_info, 28);

                // if this memory chunk is accessible
                if (mem_basic_info.Protect ==
                PAGE_READWRITE && mem_basic_info.State == MEM_COMMIT)
                {
                    byte[] buffer = new byte[mem_basic_info.RegionSize];

                    // read everything in the buffer above
                    ReadProcessMemory((int)processHandle,
                    mem_basic_info.BaseAddress, buffer, mem_basic_info.RegionSize, ref bytesRead);

                    // then output this in the file
                    int writeStartIndex = -1;
                    int writeEndIndex = -1;
                    for (int i = 0; i < mem_basic_info.RegionSize; i++)
                    {
                        if (
                            (char)buffer[i + 0] == '[' &&
                            (char)buffer[i + 1] == 'E' &&
                            (char)buffer[i + 2] == 'N' &&
                            (char)buffer[i + 3] == 'D' &&
                            (char)buffer[i + 4] == '-' &&
                            (char)buffer[i + 5] == 'D' &&
                            (char)buffer[i + 6] == 'B' &&
                            (char)buffer[i + 7] == 'G' &&
                            (char)buffer[i + 8] == ' ' &&
                            (char)buffer[i + 9] == ']'
                            )
                        {
                            writeEndIndex = i;
                            break;
                        }
                        if (
                            (char)buffer[i + 0] == '[' && 
                            (char)buffer[i + 1] == 'S' && 
                            (char)buffer[i + 2] == 'T' &&
                            (char)buffer[i + 3] == 'R' &&
                            (char)buffer[i + 4] == 'T' &&
                            (char)buffer[i + 5] == '-' &&
                            (char)buffer[i + 6] == 'D' &&
                            (char)buffer[i + 7] == 'B' &&
                            (char)buffer[i + 8] == 'G' &&
                            (char)buffer[i + 9] == ']'
                            )
                        {
                            writeStartIndex = i+10; // +10 is strlen of the tag (yeah, ugly!, demomaker code)
                        }
                    }
                    if (writeStartIndex >= 0) {
                        mem_basic_info.BaseAddress += writeStartIndex;
                        mem_basic_info.RegionSize = writeEndIndex - writeStartIndex;
                        int windex = 0;
                        ScannerBuffer = new byte[writeEndIndex - writeStartIndex];
                        while (writeStartIndex< writeEndIndex)
                        {
                            ScannerBuffer[windex++] = buffer[writeStartIndex++];
                        }
                        return true;
                    }
                }

                // move to the next memory chunk
                proc_min_address_l += mem_basic_info.RegionSize;
                proc_min_address = new IntPtr(proc_min_address_l);
            }
            MessageBox.Show("ERROR: could not find [STRT-DBG] tag in process memory");
            return false;
        }

        public static bool FindROM(string _processName)
        {
            if (!Connect(_processName)) return false;
            int bytesRead = 0;  // number of bytes read with ReadProcessMemory

            while (proc_min_address_l < proc_max_address_l)
            {
                // 28 = sizeof(MEMORY_BASIC_INFORMATION)
                VirtualQueryEx(processHandle, proc_min_address, out mem_basic_info, 28);

                // if this memory chunk is accessible
                if (mem_basic_info.Protect ==
                PAGE_READWRITE && mem_basic_info.State == MEM_COMMIT)
                {
                    byte[] buffer = new byte[mem_basic_info.RegionSize];

                    // read everything in the buffer above
                    ReadProcessMemory((int)processHandle,
                    mem_basic_info.BaseAddress, buffer, mem_basic_info.RegionSize, ref bytesRead);

                    // then output this in the file
                    for (int i = 0; i < mem_basic_info.RegionSize; i++)
                    {
                        if (
                            (char)buffer[i + 0] == 'A' &&
                            (char)buffer[i + 1] == 'M' &&
                            (char)buffer[i + 2] == 'I' &&
                            (char)buffer[i + 3] == 'G' &&
                            (char)buffer[i + 4] == 'A' &&
                            (char)buffer[i + 5] == ' ' &&
                            (char)buffer[i + 6] == 'R' &&
                            (char)buffer[i + 7] == 'O' &&
                            (char)buffer[i + 8] == 'M' &&
                            (char)buffer[i + 9] == ' ' &&
                            (char)buffer[i + 10] == 'O' &&
                            (char)buffer[i + 11] == 'p' &&
                            (char)buffer[i + 12] == 'e' &&
                            (char)buffer[i + 13] == 'r' &&
                            (char)buffer[i + 14] == 'a' &&
                            (char)buffer[i + 15] == 't' &&
                            (char)buffer[i + 16] == 'i' &&
                            (char)buffer[i + 17] == 'n' &&
                            (char)buffer[i + 18] == 'g' &&
                            (char)buffer[i + 19] == ' ' &&
                            (char)buffer[i + 20] == 'S' &&
                            (char)buffer[i + 21] == 'y' &&
                            (char)buffer[i + 22] == 's' &&
                            (char)buffer[i + 23] == 't' &&
                            (char)buffer[i + 24] == 'e' &&
                            (char)buffer[i + 25] == 'm'
                            )
                        {
                            BASE_Index = i - 3 * 16 - 11 - 0xfc0000;
                            return true;
                        }
                    }
                }

                // move to the next memory chunk
                proc_min_address_l += mem_basic_info.RegionSize;
                proc_min_address = new IntPtr(proc_min_address_l);
            }
            MessageBox.Show("ERROR: could not find [STRT-DBG] tag in process memory");
            return false;
        }

        public static void refreshLastZone()
        {
            ScannerBuffer = new byte[mem_basic_info.RegionSize];

            int bytesRead = 0;
            ReadProcessMemory((int)processHandle,
            mem_basic_info.BaseAddress, ScannerBuffer, mem_basic_info.RegionSize, ref bytesRead);
        }
    }
}
