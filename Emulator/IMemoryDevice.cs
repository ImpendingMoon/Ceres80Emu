using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    internal interface IMemoryDevice
    {

        public byte Read(ushort address);

        public void Write(ushort address, byte data);

        public void Reset();

        public void Tick();

        public byte[] SaveState();

        public void LoadState(byte[] state);
    }
}
