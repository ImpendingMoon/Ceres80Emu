using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    internal class PIO : IMemoryDevice
    {
        public ushort Size { get; } = 4;

        public bool CanWrite { get; } = true;

        public bool CanRead { get; } = true;

        public byte Read(ushort address)
        {
            switch (address)
            {
                // Channel A data
                case 0: return _buttonState;
                default: return 0;
            }
        }

        public void Write(ushort address, byte data)
        {
        }

        public void Reset()
        {
        }

        public void Tick()
        {
        }

        public void SetButtonState(byte buttonState)
        {
            _buttonState = buttonState;
        }

        public byte[] SaveState()
        {
            return new byte[0];
        }

        public void LoadState(byte[] state)
        {
        }

        private byte _buttonState = 0xFF;
    }
}
