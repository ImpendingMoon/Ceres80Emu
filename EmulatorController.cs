namespace Ceres80Emu
{
    internal class EmulatorController
    {
        public event Action? FrameUpdated;
        public Bitmap CurrentFrame => _ceres80.GetBitmap();

        public EmulatorController()
        {
            _ceres80 = new Emulator.Ceres80();
            _ceres80.FrameRendered += () => FrameUpdated?.Invoke();
        }

        public void LoadROM(byte[] rom)
        {
            _ceres80.LoadROM(rom);
        }

        public void LoadRAM(byte[] ram)
        {
            _ceres80.LoadRAM(ram);
        }

        public void Start()
        {
            if (_emulationTask != null && !_emulationTask.IsCompleted)
            {
                throw new InvalidOperationException("Emulator is already running.");
            }

            _cts = new CancellationTokenSource();
            _emulationTask = Task.Run(() => _ceres80.Start(_cts.Token), _cts.Token);
            _paused = false;
        }

        public void Stop()
        {
            if (_emulationTask == null || _emulationTask.IsCompleted) { return; }
            _ceres80.Pause();
            _ceres80.Reset();
            _cts?.Cancel();
            _emulationTask?.Wait();
        }

        public bool TogglePause()
        {
            _paused = !_paused;
            if(_paused)
            {
                _ceres80.Pause();
            }
            else
            {
                _ceres80.Resume();
            }
            return _paused;
        }


        private Emulator.Ceres80 _ceres80;
        private Task? _emulationTask;
        private CancellationTokenSource? _cts;
        private bool _paused;
    }
}
