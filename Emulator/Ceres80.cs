using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    internal class Ceres80
    {
        public Ceres80()
        {
            // Initialization order matters. Do not change.
            _interruptManager = new InterruptManager();
            _rom = new Memory(32768, false);
            _ram = new Memory(32768, true);
            _ctc = new CTC(_interruptManager);
            _pio = new PIO();
            _lcd = new LCD();
            _bus = new MemoryBus();

            _bus.AddDevice(_rom, 0x0000, 0x7FFF, false);
            _bus.AddDevice(_ram, 0x8000, 0xFFFF, false);
            _bus.AddDevice(_ctc, 0x0000, 0x0003, true);
            _bus.AddDevice(_pio, 0x0004, 0x0007, true);
            _bus.AddDevice(_lcd, 0x0008, 0x000B, true);

            _cpu = new Z80(_bus, _interruptManager);
        }

        private Z80 _cpu;
        private MemoryBus _bus;
        private InterruptManager _interruptManager;
        private Memory _rom;
        private Memory _ram;
        private LCD _lcd;
        private CTC _ctc;
        private PIO _pio;
    }
}
