using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    internal partial class Z80
    {
        public Z80(MemoryBus memoryBus, InterruptManager interruptManager, DebugManager debugManager)
        {
            _memoryBus = memoryBus;
            _interruptManager = interruptManager;
            _debugManager = debugManager;
            _registers = new RegisterSet();
        }

        /// <summary>
        /// Executes a single instruction
        /// </summary>
        /// <returns>Number of cycles taken</returns>
        public int Tick()
        {
            // Only implement Interrupt Mode 1.
            // Don't implement NMI
            if (_interruptManager.InterruptLine && _registers.IFF1 && !_justEnabledInterrupts)
            {
                _halted = false;
                Push_Reg16(_registers.PC);
                _registers.PC = 0x0038;
                _interruptManager.AcknowledgeLine = true;
                _interruptManager.InterruptLine = false;
                _registers.IFF1 = false;
                _debugManager.AddInterrupt(false);
                return 13;
            }

            // Handle EI instruction delay
            if (_justEnabledInterrupts)
            {
                _justEnabledInterrupts = false;
            }

            if (_halted)
            {
                _debugManager.AddHalted();
                return 4;
            }

            int cycles = 0;
            string instruction = "";

            ushort startAddress = _registers.PC;
            byte opcode = FetchInstruction();

            switch (opcode)
            {
                // Handle opcode prefixes
                case 0xCB:
                {
                    (cycles, instruction) = DecodeBit(opcode);
                    break;
                }
                case 0xDD:
                {
                    (cycles, instruction) = DecodeIX(opcode);
                    break;
                }
                case 0xED:
                {
                    (cycles, instruction) = DecodeMisc(opcode);
                    break;
                }
                case 0xFD:
                {
                    (cycles, instruction) = DecodeIY(opcode);
                    break;
                }
                default:
                {
                    (cycles, instruction) = DecodeMain(opcode);
                    break;
                }
            }

            // Memory Refresh Register (7-bit)
            if (_registers.R == 127)
            {
                _registers.R &= 0b10000000; // Bit 7 is preserved on hardware
            }
            _registers.R++;

            // Repeaat instructions (LDIR, LDDR, etc.) stay on the same instruction until BC is 0
            if (_runningRepeatInstruction) { _registers.PC = startAddress; }

            _debugManager.AddInstruction(instruction, startAddress, cycles);

            return cycles;
        }

        public void Reset()
        {
            _registers.Reset();
            _halted = false;
            _justEnabledInterrupts = false;
            _runningRepeatInstruction = false;
        }

        private MemoryBus _memoryBus;
        private InterruptManager _interruptManager;
        private RegisterSet _registers;
        private DebugManager _debugManager;

        private bool _halted = false;
        // EI instruction is delayed by one instruction for returning from an isr
        private bool _justEnabledInterrupts = false;
        private bool _runningRepeatInstruction = false;
    }
}
