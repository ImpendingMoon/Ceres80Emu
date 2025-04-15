using System.Diagnostics;

namespace Ceres80Emu.Emulator
{
    internal class Ceres80
    {
        /// <summary>
        /// Start the emulator.
        /// Will lock up the UI thread if run on the main thread.
        /// </summary>
        public void Start()
        {
            _running = true;
            while (_running)
            {
                RunFrame();
            }
        }

        /// <summary>
        /// Stop the emulator.
        /// </summary>
        public void Stop()
        {
            _running = false;
        }

        /// <summary>
        /// Execute a single instruction.
        /// </summary>
        public void Step()
        {
            Tick(1);
        }

        /// <summary>
        /// Set the speed of the emulator.
        /// </summary>
        /// <param name="multiplier">Percent of normal speed. 100x is default. 0x is single-step mode.</param>
        /// <exception cref="ArgumentException">When multiplier is less than 0 or greater than 500</exception>
        public void SetSpeed(int multiplier)
        {
            if (multiplier < 0 || multiplier > 500)
            {
                throw new ArgumentException("Speed must be between 0x and 500x");
            }
            _instructionsPerFrame = (int)(_instructionsPerSecond * (multiplier / 100f)) / _framesPerSecond;
        }

        /// <summary>
        /// Load a ROM into the emulator.
        /// </summary>
        /// <param name="data">Data to load</param>
        /// <exception cref="ArgumentException">If data size > 32768 bytes</exception>
        public void LoadROM(byte[] data)
        {
            if (data.Length > _rom.Size)
            {
                throw new ArgumentException("File size exceeds ROM size.");
            }
            _rom.LoadState(data);
        }

        /// <summary>
        /// Load a RAM image into the emulator.
        /// </summary>
        /// <param name="data">Data to load</param>
        /// <exception cref="ArgumentException">If data size > 32768 bytes</exception>
        public void LoadRAM(byte[] data)
        {
            if (data.Length > _ram.Size)
            {
                throw new ArgumentException("File size exceeds RAM size.");
            }
            _ram.LoadState(data);
        }

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

            _bus.AddMemoryDevice(_rom, 0x0000, 0x7FFF);
            _bus.AddMemoryDevice(_ram, 0x8000, 0xFFFF);
            _bus.AddPortDevice(_ctc, 0x0000, 0x0003);
            _bus.AddPortDevice(_pio, 0x0004, 0x0007);
            _bus.AddPortDevice(_lcd, 0x0008, 0x000B);

            _cpu = new Z80(_bus, _interruptManager);

        }

        public void Reset()
        {
            _rom.Reset();
            _ram.Reset();
            _cpu.Reset();
            _ctc.Reset();
            _pio.Reset();
            _lcd.Reset();
        }


        private void Tick(int cycles)
        {
            int cyclesElapsed = 0;
            while(cyclesElapsed < cycles)
            {
                int currentCycles = _cpu.Tick();
                for (int i = 0; i < currentCycles; i++)
                {
                    _ctc.Tick();
                    _pio.Tick();
                    _lcd.Tick();
                }
                cyclesElapsed += currentCycles;
            }
        }

        private void RunFrame()
        {
            _stopwatch.Restart();
            // TODO: Process input
            Tick(_instructionsPerFrame);
            // TODO: Update display
            _stopwatch.Stop();
            // Wait for the next frame
            long elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
            if (elapsedMilliseconds < _secondsPerFrame * 1000)
            {
                Thread.Sleep((int)(_secondsPerFrame * 1000) - (int)elapsedMilliseconds);
            }
        }

        private Z80 _cpu;
        private MemoryBus _bus;
        private InterruptManager _interruptManager;
        private Memory _rom;
        private Memory _ram;
        private CTC _ctc;
        private PIO _pio;
        private LCD _lcd;

        private Stopwatch _stopwatch = new Stopwatch();

        private const int _framesPerSecond = 60;
        private const float _secondsPerFrame = 1f / _framesPerSecond;
        private const int _instructionsPerSecond = 6144000;
        private int _instructionsPerFrame = _instructionsPerSecond / _framesPerSecond;
        private bool _running = false;

    }
}
