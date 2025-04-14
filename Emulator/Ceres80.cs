using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    internal class Ceres80
    {
        public Ceres80()
        {
            // Initialization order matters. Do not change.
            _interruptManager = new InterruptManager();
            _rom = new Memory(32768, false);
            _ram = new Memory(32768, true);
            _ctc = new CTC(_interruptManager);
            _pio = new PIO();
            _lcd = new LCD();
            _bus = new MemoryBus();

            _bus.AddDevice(_rom, 0x0000, 0x7FFF, false);
            _bus.AddDevice(_ram, 0x8000, 0xFFFF, false);
            _bus.AddDevice(_ctc, 0x0000, 0x0003, true);
            _bus.AddDevice(_pio, 0x0004, 0x0007, true);
            _bus.AddDevice(_lcd, 0x0008, 0x000B, true);

            _cpu = new Z80(_bus, _interruptManager);

            _timer = new System.Timers.Timer(1000 / _framesPerSecond);

            _timer = new System.Timers.Timer(1000 / _framesPerSecond);
        }

        public void Reset()
        {
            _cpu.Reset();
            _ctc.Reset();
            _pio.Reset();
            _lcd.Reset();
        }

        public void Start()
        {
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void SetSpeed(int speed)
        {
            if (speed < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(speed), "Speed must be greater than 0.");
            }
            _framesPerSecond = speed;
            _instructionsPerFrame = _instructionsPerSecond / _framesPerSecond;
            _timer.Interval = 1000 / _framesPerSecond;
        }

        public void SetSingleStep(bool singleStep)
        {
            _singleStep = singleStep;
        }

        public void LoadROM(byte[] data)
        {
            if (data.Length > _rom.Size)
            {
                throw new ArgumentException("ROM size exceeds memory size.");
            }
            _rom.LoadState(data);
        }

        public byte[] SaveState()
        {
            List<byte> state = new List<byte>();
            state.AddRange(_cpu.SaveState());
            state.AddRange(_ctc.SaveState());
            state.AddRange(_pio.SaveState());
            state.AddRange(_lcd.SaveState());
            return state.ToArray();
        }

        public void LoadState(byte[] state)
        {
            int offset = 0;
            _cpu.LoadState(state.Skip(offset).Take(_cpu.SaveState().Length).ToArray());
            offset += _cpu.SaveState().Length;
            _ctc.LoadState(state.Skip(offset).Take(_ctc.SaveState().Length).ToArray());
            offset += _ctc.SaveState().Length;
            _pio.LoadState(state.Skip(offset).Take(_pio.SaveState().Length).ToArray());
            offset += _pio.SaveState().Length;
            _lcd.LoadState(state.Skip(offset).Take(_lcd.SaveState().Length).ToArray());
        }

        private void Tick(int cycles)
        {
            int cyclesElapsed = 0;
            while(cyclesElapsed < cycles)
            {
                int instructionCycles = _cpu.Tick();
                for (int i = 0; i < instructionCycles; i++)
                {
                    _ctc.Tick();
                    _pio.Tick();
                    _lcd.Tick();
                }
                cyclesElapsed += instructionCycles;
            }
        }

        private void OnTimedEvent(Object? source, System.Timers.ElapsedEventArgs e)
        {
            if (_singleStep)
            {
                Tick(1);
            }
            else
            {
                Tick(_instructionsPerFrame);
            }
        }


        private System.Timers.Timer _timer;
        private Z80 _cpu;
        private MemoryBus _bus;
        private InterruptManager _interruptManager;
        private Memory _rom;
        private Memory _ram;
        private CTC _ctc;
        private PIO _pio;
        private LCD _lcd;

        private const int _instructionsPerSecond = 6144000;
        private int _framesPerSecond = 60;
        private int _instructionsPerFrame = _instructionsPerSecond / 60;
        bool _singleStep = false;
    }
}
