using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu
{
    internal class PictureBoxInterpolationMode : PictureBox
    {
        [DefaultValue(InterpolationMode.Bilinear)]
        public InterpolationMode InterpolationMode { get; set; }

        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.InterpolationMode = InterpolationMode;
            paintEventArgs.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            base.OnPaint(paintEventArgs);
        }
    }
}
