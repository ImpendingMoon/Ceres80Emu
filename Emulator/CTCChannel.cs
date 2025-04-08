using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    internal class CTCChannel
    {
        public CTCChannel(InterruptManager interruptManager)
        {
            _interruptManager = interruptManager;
        }

        public byte Read()
        {
            return _counter;
        }

        public void Write(byte data)
        {
            if (_waitingForTimeConstant)
            {
                _timeConstant = data;
                _waitingForTimeConstant = false;
                _running = true;
            }
            else if ((data & 1) != 0)
            {
                if ((data & 0b10) != 0)
                    Reset();

                if ((data & 0b100) != 0)
                    _waitingForTimeConstant = true;
                else
                    _running = true;

                if ((data & 0b100000) == 0)
                {
                    _prescaler = 16;
                    _internalCounter = 16;
                }
                else
                {
                    _prescaler = 0;
                    _internalCounter = 0;
                }

                _interruptEnabled = (data & 0b10000000) != 0;
            }
        }

        public void Tick()
        {
            if (_waitingForInterrupt && _interruptManager.IsAcknowledgePending())
            {
                _waitingForInterrupt = false;
            }

            if (!_running || _waitingForInterrupt)
                return;

            _internalCounter--;
            if (_internalCounter == 0)
            {
                _internalCounter = _prescaler;
                _counter--;
                if (_counter == 0)
                {
                    _counter = _timeConstant;
                    if (_interruptEnabled)
                    {
                        _interruptManager.RaiseInterrupt();
                        _waitingForInterrupt = true;
                    }
                }
            }
        }

        public void Reset()
        {
            _prescaler = 16;
            _counter = 0x00;
            _internalCounter = 16;
            _timeConstant = 0x00;
            _running = false;
            _interruptEnabled = false;
            _waitingForInterrupt = false;
            _waitingForTimeConstant = false;
        }

        private byte _prescaler = 16;
        private byte _counter = 0x00;
        private byte _internalCounter = 16;
        private byte _timeConstant = 0x00;
        private bool _running = false;
        private bool _interruptEnabled = false;
        private bool _waitingForInterrupt = false;
        private bool _waitingForTimeConstant = false;

        private InterruptManager _interruptManager;
    }
}
