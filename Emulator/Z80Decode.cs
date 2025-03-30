using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    // Handles decoding of Z80 instructions
    internal partial class Z80
    {
        /// <summary>
        /// Executes a single instruction
        /// </summary>
        /// <returns>Number of cycles taken</returns>
        public int Step()
        {
            int cycles = 0;
            string instruction = "";
            byte opcode = _memoryBus.Read(_registers.PC);

            // TODO: Check interrupt status

            switch (opcode)
            {
                // Handle opcode prefixes
                case 0xCB:
                {
                    _registers.PC++;
                    (cycles, instruction) = DecodeBit();
                    break;
                }
                case 0xDD:
                {
                    _registers.PC++;
                    (cycles, instruction) = DecodeIX();
                    break;
                }
                case 0xED:
                {
                    _registers.PC++;
                    (cycles, instruction) = DecodeMisc();
                    break;
                }
                case 0xFD:
                {
                    _registers.PC++;
                    (cycles, instruction) = DecodeIY();
                    break;
                }
                default:
                {
                    (cycles, instruction) = DecodeMain();
                    break;
                }
            }

            Console.WriteLine($"Executed instruction: {instruction} with opcode {opcode:X02} at {_registers.PC:X04}");
            return cycles;
        }



        private (int, string) DecodeMain()
        {
            int cycles = 0;
            string instruction = "";
            int opcode = _memoryBus.Read(_registers.PC);

            switch(opcode)
            {
                default:
                {
                    cycles = 0;
                    instruction = "Unknown Instruction";
                    break;
                }
            }

            return (cycles, instruction);
        }




        private (int, string) DecodeMisc()
        {
            // Took 4 cycles to read first byte of opcode
            int cycles = 4;
            string instruction = "";
            int opcode = _memoryBus.Read(_registers.PC);

            switch (opcode)
            {
                default:
                {
                    cycles = 0;
                    instruction = "Unknown Instruction";
                    break;
                }
            }

            return (cycles, instruction);
        }



        private (int, string) DecodeBit()
        {
            // Took 4 cycles to read first byte of opcode
            int cycles = 4;
            string instruction = "";
            int opcode = _memoryBus.Read(_registers.PC);

            switch (opcode)
            {
                default:
                {
                    cycles = 0;
                    instruction = "Unknown Instruction";
                    break;
                }
            }

            return (cycles, instruction);
        }



        private (int, string) DecodeIX()
        {
            // Took 4 cycles to read first byte of opcode
            int cycles = 4;
            string instruction = "";
            int opcode = _memoryBus.Read(_registers.PC);

            switch (opcode)
            {
                case 0xCB:
                {
                    _registers.PC++;
                    (cycles, instruction) = DecodeIXBit();
                    break;
                }
                default:
                {
                    cycles = 0;
                    instruction = "Unknown Instruction";
                    break;
                }
            }

            return (cycles, instruction);
        }



        private (int, string) DecodeIXBit()
        {
            // Took 8 cycles to read first two bytes of opcode
            int cycles = 8;
            string instruction = "";
            int opcode = _memoryBus.Read(_registers.PC);

            switch (opcode)
            {
                default:
                {
                    cycles = 0;
                    instruction = "Unknown Instruction";
                    break;
                }
            }

            return (cycles, instruction);
        }



        private (int, string) DecodeIY()
        {
            // Took 4 cycles to read first byte of opcode
            int cycles = 4;
            string instruction = "";
            int opcode = _memoryBus.Read(_registers.PC);

            switch (opcode)
            {
                case 0xCB:
                {
                    _registers.PC++;
                    (cycles, instruction) = DecodeIYBit();
                    break;
                }
                default:
                {
                    cycles = 0;
                    instruction = "Unknown Instruction";
                    break;
                }
            }

            return (cycles, instruction);
        }



        private (int, string) DecodeIYBit()
        {
            // Took 8 cycles to read first two bytes of opcode
            int cycles = 8;
            string instruction = "";
            int opcode = _memoryBus.Read(_registers.PC);

            switch (opcode)
            {
                default:
                {
                    cycles = 0;
                    instruction = "Unknown Instruction";
                    break;
                }
            }

            return (cycles, instruction);
        }
    }
}
