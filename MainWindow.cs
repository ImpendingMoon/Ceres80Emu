using System.Runtime.InteropServices;

namespace Ceres80Emu
{
    public partial class MainWindow : Form
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        private Emulator.Ceres80 Ceres80;
        private Task _emulationTask;
        private CancellationTokenSource _cts;

        public MainWindow()
        {
            InitializeComponent();
            AllocConsole();
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
                    Ceres80.Start();
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }, _cts.Token);
        }

        private void StopEmulator()
        {
            if (_emulationTask == null || _emulationTask.IsCompleted)
            {
                return;
            }
            _cts.Cancel();
            _emulationTask.Wait();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Ceres80 = new Emulator.Ceres80();
        }

        private void loadFirmwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "ROM files (*.bin)|*.bin|All files (*.*)|*.*",
                Title = "Open ROM File"
            };
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
                } catch(ArgumentException ex)
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
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
    }
}
