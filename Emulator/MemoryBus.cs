using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    internal class MemoryBus
    {
        public MemoryBus() { }

        public byte Read(ushort address, bool port = false)
        {
            if(port)
            {
                return 0x00; // Open bus
            }

            if (address <= 0x7FFF)
            {
                return _rom.Read(address);
            }
            return _ram.Read((ushort)(address - 0x8000));
        }

        public void Write(ushort address, byte data, bool port = false)
        {
            if(port)
            {
                return;
            }

            if (address <= 0x7FFF)
            {
                _rom.Write(address, data);
            }
            else
            {
                _ram.Write((ushort)(address - 0x8000), data);
            }
        }

        // Could use dependency injection, but memory map will never change
        private Memory _rom = new Memory(32768, false);
        private Memory _ram = new Memory(32768, true);
    }
}
