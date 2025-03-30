using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    // Handles execution of Z80 instructions
    // All methods return the number of cycles taken
    internal partial class  Z80
    {
        // Load and Exchange

        /// <summary>
        /// Loads a register into another register
        /// <br/>Example: LD A, B
        /// </summary>
        private int Load_Reg_Reg(ref byte dest, byte src)
        {
            dest = src;
            return 4;
        }

        /// <summary>
        /// Loads a register pair into another register pair
        /// <br/>Example: LD SP, HL
        /// </summary>
        private int Load_Reg16_Reg16(ref ushort dest, ushort src)
        {
            dest = src;
            return 6;
        }

        /// <summary>
        /// Loads an immediate value into a register
        /// <br/>Example: LD A, n
        /// </summary>
        private int Load_Reg_Imm(ref byte reg)
        {
            reg = ReadImm();
            return 7;
        }

        /// <summary>
        /// Loads an immediate value into a register pair
        /// <br/>Example: LD BC, nn
        /// </summary>
        private int Load_Reg16_Imm(ref ushort reg)
        {
            reg = ReadImm16();
            return 10;
        }

        /// <summary>
        /// Stores a register into the memory location pointed to by a register pair
        /// <br/>Example: LD (HL), A
        /// </summary>
        private int Load_Reg16Ptr_Reg(ushort dest, byte src)
        {
            _memoryBus.Write(dest, src);
            return 7;
        }

        /// <summary>
        /// Loads the value pointed to by a register pair into a register
        /// <br/>Example: LD A, (HL)
        /// </summary>
        private int Load_Reg_Reg16Ptr(ref byte dest, ushort src)
        {
            dest = _memoryBus.Read(src);
            return 7;
        }

        /// <summary>
        /// Loads an immediate value into the memory location pointed to by a register pair
        /// <br/>Example: LD (HL), n
        /// </summary>
        private int Load_Reg16Ptr_Imm(ushort dest)
        {
            _memoryBus.Write(dest, ReadImm());
            return 10;
        }

        /// <summary>
        /// Stores a register into the memory location pointed to by an immediate value
        /// <br/>Example: LD (nn), A
        /// </summary>
        private int Load_ImmPtr_Reg(ushort dest, byte src)
        {
            _memoryBus.Write(dest, src);
            return 13;
        }

        /// <summary>
        /// Stores a register pair into the memory location pointed to by an immediate value
        /// <br/>Example: LD (nn), HL
        /// </summary>
        private int Load_ImmPtr_Reg16(ushort src)
        {
            ushort address = ReadImm16();
            WriteShort(address, src);
            return 16;
        }

        /// <summary>
        /// Stores a register into the memory location pointed to by an index + offset
        /// <br/>Example: LD (IX+d), A
        /// </summary>
        private int Load_Index_Reg(ref ushort index, byte src)
        {
            int offset = ReadImm();
            ushort address = (ushort)(index + offset);
            _memoryBus.Write(address, src);
            return 15;
        }

        /// <summary>
        /// Stores an immediate value into the memory location pointed to by an index + offset
        /// <br/>Example: LD (IX+d), n
        /// </summary>
        private int Load_Index_Imm(ref ushort index)
        {
            byte offset = ReadImm();
            byte value = ReadImm();
            ushort address = (ushort)(index + offset);
            _memoryBus.Write(address, value);
            return 15;
        }

        /// <summary>
        /// Loads a value from the memory location pointed to by an index + offset into a register
        /// <br/>Example: LD A, (IX+d)
        /// </summary>
        private int Load_Reg_Index(ref byte dest, ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            dest = _memoryBus.Read(address);
            return 15;
        }

        /// <summary>
        /// Exchanges AD and AF'
        /// <br/>Example: EX AF, AF'
        /// </summary>
        private int Exchange_AF()
        {
            (_registers.AF, _registers.AltAF) = (_registers.AltAF, _registers.AF);
            return 4;
        }

        /// <summary>
        /// Exchanges DE and HL
        /// <br/>Example: EX DE, HL
        /// </summary>
        private int Exchange_DE_HL()
        {
            (_registers.DE, _registers.HL) = (_registers.HL, _registers.DE);
            return 4;
        }

        /// <summary>
        /// Exchanges general register pairs with their alternate pairs
        /// <br/>Example: EXX
        /// </summary>
        private int Exchange_X()
        {
            (_registers.BC, _registers.AltBC) = (_registers.AltBC, _registers.BC);
            (_registers.DE, _registers.AltDE) = (_registers.AltDE, _registers.DE);
            (_registers.HL, _registers.AltHL) = (_registers.AltHL, _registers.HL);
            return 4;
        }

        /// <summary>
        /// Exchanges the value of a register pair with the value from the memory location pointed to by the stack pointer
        /// <br/>Example: EX (SP), HL
        /// </summary>
        private int Exchange_SPPtr_Reg16(ref ushort reg)
        {
            ushort value = ReadShort(_registers.SP);
            WriteShort(_registers.SP, reg);
            reg = value;

            return 19;
        }

        /// <summary>
        /// Pushes a register pair onto the stack
        /// <br/>Example: PUSH BC
        /// </summary>
        private int Push_Reg16(ushort reg)
        {
            _registers.SP -= 2;
            WriteShort(_registers.SP, reg);
            return 11;
        }

        /// <summary>
        /// Pops a register pair from the stack
        /// <br/>Example: POP BC
        /// </summary>
        private int Pop_Reg16(ref ushort reg)
        {
            reg = ReadShort(_registers.SP);
            _registers.SP += 2;
            return 10;
        }

        // Block Transfer and Search

        // Arithmetic and Logical

        // Rotate and Shift

        // Bit Manipulation

        // Jump, Call, and Return

        // Input/Output

        // CPU Control

        /// <summary>
        /// Does Nothing
        /// <br/>Example: NOP
        /// </summary>
        private int No_Operation()
        {
            return 4;
        }

        /// <summary>
        /// Halts the CPU until an interrupt occurs
        /// <br/>Example: HALT
        /// </summary>
        private int Halt()
        {
            _halted = true;
            return 4;
        }

        // Internal helper methods
        private ushort ReadShort(ushort address)
        {
            byte low = _memoryBus.Read(address);
            byte high = _memoryBus.Read((ushort)(address + 1));
            return (ushort)((high << 8) | low);
        }

        private void WriteShort(ushort address, ushort data)
        {
            byte low = (byte)data;
            byte high = (byte)(data >> 8);
            _memoryBus.Write(address, low);
            _memoryBus.Write((ushort)(address + 1), high);
        }

        private byte ReadImm()
        {
            byte value = _memoryBus.Read((ushort)(_registers.PC + 1));
            _registers.PC += 1;
            return value;
        }

        private ushort ReadImm16()
        {
            ushort value = ReadShort((ushort)(_registers.PC + 1));
            _registers.PC += 2;
            return value;
        }
    }
}
