using System.Runtime.InteropServices;

namespace Ceres80Emu
{
    public partial class MainWindow : Form
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        private Emulator.Ceres80 Ceres80;
        private Task? _emulationTask;
        private CancellationTokenSource? _cts;

        private bool _paused = false;

        public MainWindow()
        {
            InitializeComponent();
            AllocConsole();
            Ceres80 = new Emulator.Ceres80();
        }

        private void StartEmulator()
        {
            if (_emulationTask != null && !_emulationTask.IsCompleted)
            {
                MessageBox.Show("Emulator is already running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _cts = new CancellationTokenSource();
            _emulationTask = Task.Run(() =>
            {
                try
                {
                    Ceres80.Start(_cts.Token);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }, _cts.Token);
            pauseResumeToolStripMenuItem.Enabled = true;
            pauseResumeToolStripMenuItem.Checked = false;
        }

        private void StopEmulator()
        {
            if (_emulationTask == null || _emulationTask.IsCompleted)
            {
                return;
            }
            Ceres80.Pause();
            Ceres80.Reset();

            _cts.Cancel();
            _emulationTask.Wait();

            pauseResumeToolStripMenuItem.Enabled = false;
            pauseResumeToolStripMenuItem.Checked = false;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            pictureBoxInterpolationMode1.Image = Ceres80.GetBitmap();
            Ceres80.FrameRendered += OnFrameRendered;
        }

        private void loadFirmwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "ROM files (*.bin)|*.bin|All files (*.*)|*.*",
                Title = "Open ROM File"
            };
            openFileDialog.RestoreDirectory = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                byte[] romData = File.ReadAllBytes(filePath);
                try
                {
                    StopEmulator();
                    Ceres80.LoadROM(romData);
                    StartEmulator();
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void loadProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Program files (*.bin)|*.bin|All files (*.*)|*.*",
                Title = "Open Program File"
            };
            openFileDialog.RestoreDirectory = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                byte[] ramData = File.ReadAllBytes(filePath);
                try
                {
                    StopEmulator();
                    Ceres80.LoadRAM(ramData);
                    StartEmulator();
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnFrameRendered()
        {
            pictureBoxInterpolationMode1.Image = Ceres80.GetBitmap();
            pictureBoxInterpolationMode1.Invalidate();
        }

        private void pauseResumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _paused = !_paused;
            if(_paused)
            {
                Ceres80.Pause();
            }
            else
            {
                Ceres80.Resume();
            }
        }
    }
}
