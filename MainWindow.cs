namespace Ceres80Emu
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
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
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                byte[] romData = File.ReadAllBytes(filePath);
                Ceres80.LoadROM(romData);
                Ceres80.Start();
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Ceres80 = new Emulator.Ceres80();
        }

        private Emulator.Ceres80 Ceres80;
    }
}
