using System.Runtime.InteropServices;

namespace Ceres80Emu
{
    public partial class MainWindow : Form
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();


        private EmulatorController _emulator;

        public MainWindow()
        {
            InitializeComponent();
            AllocConsole();
            _emulator = new EmulatorController();
            _emulator.FrameUpdated += UpdateFrame;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            pictureBoxInterpolationMode1.Image = _emulator.CurrentFrame;
        }

        private void loadFirmwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var data = FileLoader.LoadBinaryFile("Open Firmware Image");
            if(data != null)
            {
                try
                {
                    _emulator.Stop();
                    _emulator.LoadROM(data);
                    _emulator.Start();
                    SetPauseState(true, false);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void loadProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var data = FileLoader.LoadBinaryFile("Open Program Image");
            if (data != null)
            {
                try
                {
                    _emulator.Stop();
                    _emulator.LoadRAM(data);
                    _emulator.Start();
                    SetPauseState(true, false);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateFrame()
        {
            pictureBoxInterpolationMode1.Image = _emulator.CurrentFrame;
            pictureBoxInterpolationMode1.Invalidate();
        }

        private void SetPauseState(bool enabled, bool check)
        {
            pauseResumeToolStripMenuItem.Enabled = enabled;
            pauseResumeToolStripMenuItem.Checked = check;
        }

        private void pauseResumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool paused = _emulator.TogglePause();
            SetPauseState(true, paused);
        }
    }
}
