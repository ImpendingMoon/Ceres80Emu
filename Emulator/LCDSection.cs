namespace Ceres80Emu.Emulator
{
    // https://support.newhavendisplay.com/hc/en-us/article_attachments/4414521335447
    internal class LCDSection
    {
        public byte ReadData()
        {
            byte value = _framebuffer[_y, _x];
            // Increment not documented on command overview, but is in details (13)
            _y = (byte)((_y + 1) % 64);
            return value;
        }

        public void WriteData(byte data)
        {
            _framebuffer[_y, _x] = data;
            _y = (byte)((_y + 1) % 64);
        }

        public byte ReadStatus()
        {
            // Not emulating busy states or display corruption
            return (byte)(_displayOn ? 0b00100000 : 0);
        }

        public void WriteCommand(byte data)
        {
            // Display On/Off
            if ((data & 0b11111110) == 0b00111110)
            {
                _displayOn = (data & 1) == 1;
            }
            // Set Address (Y address)
            else if ((data & 0b11000000) == 0b01000000)
            {
                _y = (byte)(data & 0b00111111);
            }
            // Set Page (X address)
            else if ((data & 0b11111000) == 0b10111000)
            {
                _x = (byte)(data & 0b00000111);
            }
            // Display start line (Z address)
            else if ((data & 0b11000000) == 0b11000000)
            {
                _z = (byte)(data & 0b00111111);
            }
        }

        public void Reset()
        {
            _framebuffer = new byte[64, 8];
            _y = 0;
            _x = 0;
            _z = 0;
            _displayOn = false;
        }

        public byte[,] GetPixelData()
        {
            byte[,] data = new byte[64, 64];

            if(_displayOn)
            {
                // Screen is laid out in a very weird way
                // X pages are vertical, but X=0 is still at the top.
                // LSB of pages are at the top.
                // Y and Z are horizontal.
                // There is no orientation where this makes sense, but that's how it is.

                for(int srcY = 0; srcY < height; srcY++)
                {
                    for(int srcX = 0; srcX < width; srcX++)
                    {
                        for(int bit = 0; bit < 8; bit++)
                        {
                            int destX = (srcY + _z) % height; // Same X = 1 column
                            int destY = (srcX * 8) + bit; // Different Y = 8 rows
                            byte value = (byte)((_framebuffer[srcY, srcX] >> bit) & 1);
                            data[destX, destY] = value;
                        }
                    }
                }
            }

            return data;
        }

        private byte[,] _framebuffer = new byte[64, 8];
        private byte _y = 0; // Row (0-64)
        private byte _x = 0; // 8-Pixel Column (0-8)
        private byte _z = 0; // Vertical Scroll (0-64)
        private bool _displayOn = false;

        private const int width = 8;
        private const int height = 64;
    }
}
