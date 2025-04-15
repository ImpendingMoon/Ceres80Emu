using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Ceres80Emu.Emulator
{
    internal class LCD: IMemoryDevice
    {
        public LCD()
        {
            _lcdBitmap = new Bitmap(128, 64, PixelFormat.Format8bppIndexed);
            _palette = _lcdBitmap.Palette;
            _palette.Entries[0] = Color.Black;
            _palette.Entries[1] = Color.White;
            _lcdBitmap.Palette = _palette;
        }

        public ushort Size { get; } = 4;

        public byte Read(ushort address)
        {
            switch(address % Size)
            {
                case 0: return _left.ReadStatus();
                case 1: return _left.ReadData();
                case 2: return _right.ReadStatus();
                case 3: return _right.ReadData();
                default: return 0xFF;
            }
        }

        public void Write(ushort address, byte data)
        {
            switch(address % Size)
            {
                case 0: _left.WriteCommand(data); break;
                case 1: _left.WriteData(data); break;
                case 2: _right.WriteCommand(data); break;
                case 3: _right.WriteData(data); break;
            }
        }

        public void Reset()
        {
            _left.Reset();
            _right.Reset();
        }

        public void Tick()
        {
        }

        public Bitmap GetBitmap()
        {
            byte[,] leftPixels = _left.GetPixelData();
            byte[,] rightPixels = _right.GetPixelData();

            BitmapData data = _lcdBitmap.LockBits(
                new Rectangle(0, 0, 128, 64),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed
            );

            int stride = data.Stride;
            int height = data.Height;
            int width = data.Width;

            // Combine two halves into one array
            byte[] pixelBytes = new byte[stride * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pixelIndex = y * stride + x;
                    byte value = (x < 64) ? leftPixels[x, y] : rightPixels[x - 64, y];
                    pixelBytes[pixelIndex] = value;
                }
            }

            Marshal.Copy(pixelBytes, 0, data.Scan0, pixelBytes.Length);
            _lcdBitmap.UnlockBits(data);

            return _lcdBitmap;
        }

        public byte[] SaveState()
        {
            return new byte[0];
        }

        public void LoadState(byte[] state)
        {
        }

        LCDSection _left = new LCDSection();
        LCDSection _right = new LCDSection();

        Bitmap _lcdBitmap = new Bitmap(128, 64, PixelFormat.Format8bppIndexed);
        ColorPalette _palette;
    }
}
