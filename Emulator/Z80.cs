using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    /// <summary>
    /// Emulates a Z80 CPU.
    /// Z80.cs handles public-facing methods and members.
    /// Z80Decode.cs handles decoding of instructions.
    /// Z80Execute.cs handles execution of instructions.
    /// This is because decoding takes several thousand lines of code.
    /// </summary>
    internal partial class Z80
    {
        public Z80(MemoryBus memoryBus, InterruptManager interruptManager)
        {
            _memoryBus = memoryBus;
            _interruptManager = interruptManager;
            _registers = new RegisterSet();
        }

        public Z80(MemoryBus memoryBus, InterruptManager interruptManager, RegisterSet registers)
        {
            _memoryBus = memoryBus;
            _interruptManager = interruptManager;
            _registers = registers;
        }



        /// <summary>
        /// Executes a certain number of machine cycles (T-states)
        /// </summary>
        /// <param name="steps">Minimum number of cycles to execute</param>
        /// <returns>Number of cycles executed</returns>
        public int Run(int steps)
        {
            int cycles = 0;

            while (cycles < steps)
            {
                cycles += Step();
            }

            return cycles;
        }



        /// <summary>
        /// Resets the CPU to its initial state.
        /// </summary>
        public void Reset()
        {
            // Every register starts at 0 (corrected by Z80 Undocumented Features by Sean Young)
            foreach (var field in _registers.GetType().GetFields())
            {
                field.SetValue(_registers, 0);
            }
        }



        private MemoryBus _memoryBus;
        private InterruptManager _interruptManager;
        private RegisterSet _registers;
        private bool _halted = false;
    }
}
