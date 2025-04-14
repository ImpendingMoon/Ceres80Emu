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

        /// <summary>
        /// Executes a single instruction
        /// </summary>
        /// <returns>Number of cycles taken</returns>
        public int Tick()
        {
            // Only implement Interrupt Mode 1.
            if (_interruptManager.IsInterruptPending() && _registers.IFF1 && !_justEnabledInterrupts)
            {
                _halted = false;
                Push_Reg16(_registers.PC);
                _registers.PC = 0x0038;
                _interruptManager.AcknowledgeInterrupt();
                Console.WriteLine("Z80: Received interrupt");
                return 13;
            }

            if (_halted)
            {
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

            // Handle EI instruction delay
            if (_justEnabledInterrupts)
            {
                _justEnabledInterrupts = false;
            }

            // Memory Refresh Register (7-bit)
            if (_registers.R == 127)
            {
                _registers.R &= 0b10000000; // Bit 7 is preserved on hardware
            }
            _registers.R++;

            // Repeaat instructions (LDIR, LDDR, etc.) stay on the same instruction until BC is 0
            if (_runningRepeatInstruction) { _registers.PC = startAddress; }

            Console.WriteLine($"Z80: Executed instruction: {instruction} at {startAddress:X04}");
            return cycles;
        }

        public void Reset()
        {
            // Every register starts at 0 (corrected by Z80 Undocumented Features by Sean Young)
            foreach (var field in _registers.GetType().GetFields())
            {
                field.SetValue(_registers, 0);
            }
            _halted = false;
            _justEnabledInterrupts = false;
            _runningRepeatInstruction = false;
        }

        public byte[] SaveState()
        {
            return new byte[0];
        }

        public void LoadState()
        {

        }

        private MemoryBus _memoryBus;
        private InterruptManager _interruptManager;
        private RegisterSet _registers;
        private bool _halted = false;
        // EI instruction is delayed by one instruction for returning from an isr
        private bool _justEnabledInterrupts = false;
        private bool _runningRepeatInstruction = false;
    }
}
