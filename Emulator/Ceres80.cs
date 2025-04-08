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
            _lcd = new LCD();
            _ctc = new CTC(_interruptManager);
            _pio = new PIO();
            _bus = new MemoryBus(_rom, _ram, _lcd, _ctc, _pio);
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
