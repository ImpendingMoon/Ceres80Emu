using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
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

        public void Reset()
        {
            foreach (var channel in _channels)
            {
                channel.Reset();
            }
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
            List<byte> state = new List<byte>();
            foreach (var channel in _channels)
            {
                state.AddRange(channel.SaveState());
            }
            return state.ToArray();
        }

        public void LoadState(byte[] state)
        {
            int offset = 0;
            foreach (var channel in _channels)
            {
                byte[] channelState = new byte[channel.SaveState().Length];
                Array.Copy(state, offset, channelState, 0, channelState.Length);
                channel.LoadState(channelState);
                offset += channelState.Length;
            }
        }

        private readonly CTCChannel[] _channels;
    }
}
