using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu
{
    internal static class FileLoader
    {
        public static byte[]? LoadBinaryFile(string title)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*",
                Title = title,
                RestoreDirectory = true,
                CheckFileExists = true,
                Multiselect = false
            };

            return dialog.ShowDialog() == DialogResult.OK
                ? File.ReadAllBytes(dialog.FileName)
                : null;
        }
    }
}
