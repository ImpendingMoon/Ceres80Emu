using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    internal class MemoryBus
    {
        public MemoryBus(Memory rom, Memory ram, LCD lcd, CTC ctc, PIO pio)
        {
            _rom = rom;
            _ram = ram;
            _lcd = lcd;
            _ctc = ctc;
            _pio = pio;
        }

        /// <summary>
        /// Reads a byte from the memory bus.
        /// </summary>
        /// <param name="address">Address to read from. Ports only use the lower byte (0-255)</param>
        /// <param name="port">Address a device through a port</param>
        /// <param name="fetch">Whether this is an instruction fetch. Used for breakpoints</param>
        /// <returns></returns>
        public byte Read(ushort address, bool port = false, bool fetch = false)
        {
            if(port)
            {
                switch(address & 0xFF)
                {
                    case <= 0x03:
                        return _ctc.Read(address);
                    case <= 0x07:
                        return _pio.Read(address);
                    case <= 0x0B:
                        return _lcd.Read(address);
                }
                return 0x00; // Open bus
            }

            if (address <= 0x7FFF)
            {
                return _rom.Read(address);
            }
            return _ram.Read((ushort)(address - 0x8000));
        }

        /// <summary>
        /// Writes a byte to the memory bus.
        /// </summary>
        /// <param name="address">Address to write to. Ports only use the lower byte (0-255)</param>
        /// <param name="data">Value to write</param>
        /// <param name="port">Address a device through a port</param>
        public void Write(ushort address, byte data, bool port = false)
        {
            if(port)
            {
                switch(address & 0xFF)
                {
                    case <= 0x03:
                        _ctc.Write(address, data);
                        break;
                    case <= 0x07:
                        _pio.Write(address, data);
                        break;
                    case <= 0x0B:
                        _lcd.Write(address, data);
                        break;
                }
            }
            else if (address <= 0x7FFF)
            {
                _rom.Write(address, data);
            }
            else
            {
                _ram.Write((ushort)(address - 0x8000), data);
            }
        }

        // For a more extensive memory bus, we could add a list of devices and check if the address is in the range of any device.
        // But the Ceres80 is simple and stable, so this is simpler and faster.
        private Memory _rom;
        private Memory _ram;
        private LCD _lcd;
        private CTC _ctc;
        private PIO _pio;
    }
}
