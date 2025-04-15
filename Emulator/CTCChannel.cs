namespace Ceres80Emu.Emulator
{
    internal class CTCChannel
    {
        public CTCChannel(InterruptManager interruptManager, DebugManager debugManager)
        {
            _interruptManager = interruptManager;
            _debugManager = debugManager;
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
            if (_waitingForInterrupt && _interruptManager.AcknowledgeLine)
            {
                _waitingForInterrupt = false;
                _interruptManager.AcknowledgeLine = false;
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
                        _interruptManager.InterruptLine = true;
                        _waitingForInterrupt = true;
                        _debugManager.AddInterrupt(true);
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

        public byte[] SaveState()
        {
            List<byte> state = new List<byte>
            {
                _prescaler,
                _counter,
                _internalCounter,
                _timeConstant,
                (byte)(_running ? 1 : 0),
                (byte)(_interruptEnabled ? 1 : 0),
                (byte)(_waitingForInterrupt ? 1 : 0),
                (byte)(_waitingForTimeConstant ? 1 : 0)
            };
            return state.ToArray();
        }

        public void LoadState(byte[] state)
        {
            if (state.Length != 8)
                throw new ArgumentException("Invalid state length.");
            _prescaler = state[0];
            _counter = state[1];
            _internalCounter = state[2];
            _timeConstant = state[3];
            _running = state[4] != 0;
            _interruptEnabled = state[5] != 0;
            _waitingForInterrupt = state[6] != 0;
            _waitingForTimeConstant = state[7] != 0;
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
        private DebugManager _debugManager;
    }
}
