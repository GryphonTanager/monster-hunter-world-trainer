using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MonsterHunterWorldTrainer.Core
{
    /// <summary>
    /// Manages low-level memory operations on the MonsterHunterWorld process.
    /// Uses Win32 API for reading/writing process memory.
    /// </summary>
    public class GameMemoryManager : IDisposable
    {
        private IntPtr _processHandle;
        private int _processId;
        private bool _disposed;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        private const uint PROCESS_VM_READ = 0x0010;
        private const uint PROCESS_VM_WRITE = 0x0020;
        private const uint PROCESS_VM_OPERATION = 0x0008;

        /// <summary>
        /// Indicates whether the manager is successfully attached to the target process.
        /// </summary>
        public bool IsAttached => _processHandle != IntPtr.Zero;

        /// <summary>
        /// Attempts to attach to a process by name.
        /// </summary>
        /// <param name="processName">The name of the process (without .exe).</param>
        public GameMemoryManager(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
            {
                Console.WriteLine($"Process '{processName}' not found.");
                _processHandle = IntPtr.Zero;
                return;
            }

            var process = processes[0];
            _processId = process.Id;
            _processHandle = OpenProcess(PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION, false, _processId);

            if (_processHandle == IntPtr.Zero)
            {
                Console.WriteLine("Failed to open process handle. Try running as administrator.");
            }
        }

        /// <summary>
        /// Reads a block of memory from the target process.
        /// </summary>
        public byte[] ReadBytes(IntPtr address, int size)
        {
            if (_processHandle == IntPtr.Zero)
                throw new InvalidOperationException("Not attached to any process.");

            byte[] buffer = new byte[size];
            if (!ReadProcessMemory(_processHandle, address, buffer, size, out int bytesRead))
                throw new Exception($"Failed to read memory at 0x{address.ToInt64():X}. Error code: {Marshal.GetLastWin32Error()}");

            if (bytesRead != size)
                throw new Exception($"Read only {bytesRead} of {size} bytes.");

            return buffer;
        }

        /// <summary>
        /// Writes a block of memory to the target process.
        /// </summary>
        public void WriteBytes(IntPtr address, byte[] data)
        {
            if (_processHandle == IntPtr.Zero)
                throw new InvalidOperationException("Not attached to any process.");

            if (!WriteProcessMemory(_processHandle, address, data, data.Length, out int bytesWritten))
                throw new Exception($"Failed to write memory at 0x{address.ToInt64():X}. Error code: {Marshal.GetLastWin32Error()}");

            if (bytesWritten != data.Length)
                throw new Exception($"Wrote only {bytesWritten} of {data.Length} bytes.");
        }

        /// <summary>
        /// Reads a 32-bit integer (float) from memory.
        /// </summary>
        public float ReadFloat(IntPtr address)
        {
            byte[] bytes = ReadBytes(address, 4);
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Writes a 32-bit float to memory.
        /// </summary>
        public void WriteFloat(IntPtr address, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            WriteBytes(address, bytes);
        }

        /// <summary>
        /// Reads a 32-bit integer from memory.
        /// </summary>
        public int ReadInt32(IntPtr address)
        {
            byte[] bytes = ReadBytes(address, 4);
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// Writes a 32-bit integer to memory.
        /// </summary>
        public void WriteInt32(IntPtr address, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            WriteBytes(address, bytes);
        }

        public void Dispose()
        {
            if (!_disposed && _processHandle != IntPtr.Zero)
            {
                CloseHandle(_processHandle);
                _processHandle = IntPtr.Zero;
                _disposed = true;
            }
        }
    }
}
