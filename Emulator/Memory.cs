using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    // Implements basic memory
    internal class Memory : IMemoryDevice
    {
        public Memory(ushort size, bool canWrite)
        {
            Size = size;
            CanWrite = canWrite;
            _data = new byte[size];
        }

        public ushort Size { get; }

        public bool CanWrite { get; }

        public bool CanRead { get; } = true;

        public byte Read(ushort address)
        {
            return _data[address % Size];
        }

        public void Write(ushort address, byte data)
        {
            if(CanWrite)
            {
                _data[address % Size] = data;
            }
        }

        public void Reset()
        {
            Array.Clear(_data, 0, Size);
        }

        public void Tick()
        {
        }

        public void LoadState(byte[] data)
        {
            Array.Copy(data, _data, Math.Min(Size, data.Length));
        }

        public byte[] SaveState()
        {
            return _data;
        }

        private byte[] _data;
    }
}
