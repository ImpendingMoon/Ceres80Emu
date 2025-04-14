namespace Ceres80Emu
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            pictureBoxInterpolationMode1 = new PictureBoxInterpolationMode();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxInterpolationMode1).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(103, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // pictureBoxInterpolationMode1
            // 
            pictureBoxInterpolationMode1.BackColor = Color.Black;
            pictureBoxInterpolationMode1.Dock = DockStyle.Fill;
            pictureBoxInterpolationMode1.Image = Properties.Resources.smail;
            pictureBoxInterpolationMode1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            pictureBoxInterpolationMode1.Location = new Point(0, 24);
            pictureBoxInterpolationMode1.Margin = new Padding(0);
            pictureBoxInterpolationMode1.MinimumSize = new Size(128, 64);
            pictureBoxInterpolationMode1.Name = "pictureBoxInterpolationMode1";
            pictureBoxInterpolationMode1.Size = new Size(800, 426);
            pictureBoxInterpolationMode1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxInterpolationMode1.TabIndex = 1;
            pictureBoxInterpolationMode1.TabStop = false;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pictureBoxInterpolationMode1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainWindow";
            Text = "Ceres80Emu";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxInterpolationMode1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private PictureBoxInterpolationMode pictureBoxInterpolationMode1;
    }
}
