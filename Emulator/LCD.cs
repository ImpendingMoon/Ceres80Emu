﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    internal class LCD: IMemoryDevice
    {
        public ushort Size { get; } = 4;
        public bool CanWrite { get; } = true;
        public bool CanRead { get; } = true;

        public byte Read(ushort address)
        {
            return 0x00;
        }

        public void Write(ushort address, byte data)
        {
        }

        public byte[] SaveState()
        {
            throw new NotImplementedException();
        }

        public void LoadState(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
