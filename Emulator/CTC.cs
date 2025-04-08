using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    // NOTE: This does not handle chained interrupts.
    internal class CTC : IMemoryDevice
    {
        public CTC(InterruptManager interruptManager)
        {
            _channels = new CTCChannel[4];
            for (int i = 0; i < 4; i++)
            {
                _channels[i] = new CTCChannel(interruptManager);
            }
        }

        public ushort Size { get; } = 4;
        public bool CanWrite { get; } = true;
        public bool CanRead { get; } = true;

        public byte Read(ushort address)
        {
            int channel = address % 4;
            return _channels[channel].Read();
        }

        public void Write(ushort address, byte data)
        {
            int channel = address % 4;
            _channels[channel].Write(data);
        }

        public void Tick()
        {
            foreach (var channel in _channels)
            {
                channel.Tick();
            }
        }

        public byte[] SaveState()
        {
            throw new NotImplementedException();
        }

        public void LoadState(byte[] data)
        {
            throw new NotImplementedException();
        }

        private readonly CTCChannel[] _channels;
    }
}
