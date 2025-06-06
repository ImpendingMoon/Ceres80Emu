﻿using System;

namespace Ceres80Emu.Emulator
{
    // Handles execution of Z80 instructions
    // All methods return the number of cycles taken
    internal partial class Z80
    {
        /************************* Load and Exchange *************************/

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
        /// Loads the value pointed to by an immediate value into a register pair
        /// <br/>Example: LD IX, (nn)
        /// </summary>
        private int Load_Reg16_ImmPtr(ref ushort reg)
        {
            ushort address = ReadImm16();
            reg = ReadShort(address);
            return 16;
        }

        /// <summary>
        /// Stores a register into the memory location pointed to by a register pair
        /// <br/>Example: LD (HL), A
        /// </summary>
        private int Load_Reg16Ptr_Reg(ushort dest, byte src)
        {
            _memoryBus.WriteMemory(dest, src);
            return 7;
        }

        /// <summary>
        /// Loads the value pointed to by a register pair into a register
        /// <br/>Example: LD A, (HL)
        /// </summary>
        private int Load_Reg_Reg16Ptr(ref byte dest, ushort src)
        {
            dest = _memoryBus.ReadMemory(src);
            return 7;
        }

        /// <summary>
        /// Loads an immediate value into the memory location pointed to by a register pair
        /// <br/>Example: LD (HL), n
        /// </summary>
        private int Load_Reg16Ptr_Imm(ushort dest)
        {
            _memoryBus.WriteMemory(dest, ReadImm());
            return 10;
        }

        /// <summary>
        /// Stores a register into the memory location pointed to by an immediate value
        /// <br/>Example: LD (nn), A
        /// </summary>
        private int Load_ImmPtr_Reg(byte src)
        {
            ushort address = ReadImm16();
            _memoryBus.WriteMemory(address, src);
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
        private int Load_Index_Reg(ushort index, byte src)
        {
            int offset = ReadImm();
            ushort address = (ushort)(index + offset);
            _memoryBus.WriteMemory(address, src);
            return 15;
        }

        /// <summary>
        /// Stores an immediate value into the memory location pointed to by an index + offset
        /// <br/>Example: LD (IX+d), n
        /// </summary>
        private int Load_Index_Imm(ushort index)
        {
            byte offset = ReadImm();
            byte value = ReadImm();
            ushort address = (ushort)(index + offset);
            _memoryBus.WriteMemory(address, value);
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
            dest = _memoryBus.ReadMemory(address);
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
            WriteShort(_registers.SP, reg, MemoryAccessType.Stack);
            return 11;
        }

        /// <summary>
        /// Pops a register pair from the stack
        /// <br/>Example: POP BC
        /// </summary>
        private int Pop_Reg16(ref ushort reg)
        {
            reg = ReadShort(_registers.SP, MemoryAccessType.Stack);
            _registers.SP += 2;
            return 10;
        }



        /********************* Block Transfer and Search *********************/

        /// <summary>
        /// Loads a value in memory from (HL) to (DE), incrementing both HL and DE and decrementing BC
        /// <br/>Example: LDI, LDIR
        /// </summary>
        private int Load_Increment()
        {
            byte value = _memoryBus.ReadMemory(_registers.HL);
            _memoryBus.WriteMemory(_registers.DE, value);
            _registers.HL++;
            _registers.DE++;
            _registers.BC--;

            _registers.Subtract = false;
            _registers.HalfCarry = false;
            _registers.ParityOverflow = _registers.BC != 0;

            return 12;
        }

        /// <summary>
        /// Loads a value in memory from (HL) to (DE), decrementing HL, DE, and BC
        /// <br/>Example: LDD, LDDR
        /// </summary>
        private int Load_Decrement()
        {
            byte value = _memoryBus.ReadMemory(_registers.HL);
            _memoryBus.WriteMemory(_registers.DE, value);
            _registers.HL--;
            _registers.DE--;
            _registers.BC--;

            _registers.Subtract = false;
            _registers.HalfCarry = false;
            _registers.ParityOverflow = _registers.BC != 0;

            return 12;
        }

        /// <summary>
        /// Compares the value at (HL) with A, incrementing HL and decrementing BC
        /// <br/>Example: CPI, CPIR
        /// </summary>
        private int Compare_Increment()
        {
            byte value = _memoryBus.ReadMemory(_registers.HL);
            byte result = (byte)(_registers.A - value);
            _registers.HL++;
            _registers.BC--;

            _registers.Subtract = true;
            _registers.ParityOverflow = _registers.BC != 0;
            _registers.HalfCarry = (result & 0x0F) > (_registers.A & 0x0F);
            _registers.Zero = result == 0;
            _registers.Sign = (result & 0x80) != 0;

            return 12;
        }

        /// <summary>
        /// Compares the value at (HL) with A, decrementing HL and BC
        /// <br/>Example: CPD, CPDR
        /// </summary>
        private int Compare_Decrement()
        {
            byte value = _memoryBus.ReadMemory(_registers.HL);
            byte result = (byte)(_registers.A - value);
            _registers.HL--;
            _registers.BC--;

            _registers.Subtract = true;
            _registers.ParityOverflow = _registers.BC != 0;
            _registers.HalfCarry = (result & 0x0F) > (_registers.A & 0x0F);
            _registers.Zero = result == 0;
            _registers.Sign = (result & 0x80) != 0;

            return 12;
        }



        /********************** Arithmetic and Logical ***********************/

        /// <summary>
        /// Adds a register to another register
        /// <br/>Example: ADD A, B
        /// </summary>
        private int Add_Reg_Reg(ref byte dest, byte src, bool carry = false)
        {
            dest = Add8(dest, src, carry);
            return 4;
        }

        /// <summary>
        /// Adds an immediate value to a register
        /// <br/>Example: ADD A, n
        /// </summary>
        private int Add_Reg_Imm(ref byte dest, bool carry = false)
        {
            dest = Add8(dest, ReadImm(), carry);
            return 7;
        }

        /// <summary>
        /// Adds the value at a memory location pointed to by a register pair to a register
        /// <br/>Example: ADD A, (HL)
        /// </summary>
        private int Add_Reg_Reg16Ptr(ref byte dest, ushort src, bool carry = false)
        {
            dest = Add8(dest, _memoryBus.ReadMemory(src), carry);
            return 7;
        }

        /// <summary>
        /// Adds the value at a memory location pointed to by an index + offset to a register
        /// <br/>Example: ADD A, (IX+d)
        /// </summary>
        private int Add_Reg_Index(ref byte dest, ushort index, bool carry = false)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            dest = Add8(dest, _memoryBus.ReadMemory(address), carry);
            return 15;
        }

        /// <summary>
        /// Adds a register pair to another register pair
        /// <br/>Example: ADD HL, BC
        /// </summary>
        private int Add_Reg16_Reg16(ref ushort dest, ushort src, bool carry = false)
        {
            bool overflow = WillOverflow<ushort>(dest, src);
            dest += (ushort)(src + (carry ? 1 : 0));

            _registers.Carry = overflow;
            _registers.Subtract = false;
            _registers.HalfCarry = (dest & 0x0FFF) < (src & 0x0FFF);

            // ADC rr, rr' has different behavior than ADD rr, rr'
            if (carry)
            {
                _registers.ParityOverflow = overflow;
                _registers.Zero = dest == 0;
                _registers.Sign = (dest & 0x8000) != 0;
            }

            return 11;
        }

        /// <summary>
        /// Increments a register
        /// <br/>Example: INC A
        /// </summary>
        private int Inc_Reg(ref byte reg)
        {
            reg = Add8(reg, 1, false, false);
            return 4;
        }

        /// <summary>
        /// Increments the value at a memory location pointed to by a register pair
        /// <br/>Example: INC (HL)
        /// </summary>
        private int Inc_Reg16Ptr(ushort address)
        {
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, Add8(value, 1, false, false));
            return 11;
        }

        /// <summary>
        /// Increments a register pair
        /// <br/>Example: INC BC
        /// </summary>
        private int Inc_Reg16(ref ushort reg)
        {
            reg++;
            return 6;
        }

        /// <summary>
        /// Increments the value at a memory location pointed to by an index + offset
        /// <br/>Example: INC (IX+d)
        /// </summary>
        private int Inc_Index(ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, Add8(value, 1, false, false));

            return 19;
        }

        /// <summary>
        /// Subtracts a register from another register
        /// <br/>Example: SUB A, B
        /// </summary>
        private int Sub_Reg_Reg(ref byte dest, byte src, bool carry = false)
        {
            dest = Sub8(dest, src, carry);
            return 4;
        }

        /// <summary>
        /// Subtracts an immediate value from a register
        /// <br/>Example: SUB A, n
        /// </summary>
        private int Sub_Reg_Imm(ref byte dest, bool carry = false)
        {
            dest = Sub8(dest, ReadImm(), carry);
            return 7;
        }

        /// <summary>
        /// Subtracts the value at a memory location pointed to by a register pair from a register
        /// <br/>Example: SUB A, (HL)
        /// </summary>
        private int Sub_Reg_Reg16Ptr(ref byte dest, ushort src, bool carry = false)
        {
            dest = Sub8(dest, _memoryBus.ReadMemory(src), carry);
            return 7;
        }

        /// <summary>
        /// Subtracts the value at a memory location pointed to by an index + offset from a register
        /// <br/>Example: SUB A, (IX+d)
        /// </summary>
        private int Sub_Reg_Index(ref byte dest, ushort index, bool carry = false)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            dest = Sub8(dest, _memoryBus.ReadMemory(address), carry);
            return 15;
        }

        /// <summary>
        /// Subtracts a register pair from another register pair
        /// Subtracts a register pair from another register pair
        /// <br/>Example: SUB HL, BC
        /// </summary>
        private int Sub_Reg16_Reg16(ref ushort dest, ushort src)
        {
            bool underflow = WillUnderflow<ushort>(dest, src);
            dest -= (ushort)(src - 1);

            _registers.Carry = underflow;
            _registers.Subtract = true;
            _registers.ParityOverflow = underflow;
            _registers.HalfCarry = (dest & 0x0FFF) > (src & 0x0FFF);
            _registers.Zero = dest == 0;
            _registers.Sign = (dest & 0x8000) != 0;

            return 11;
        }

        /// <summary>
        /// Decrements a register
        /// <br/>Example: DEC A
        /// </summary>
        private int Dec_Reg(ref byte reg)
        {
            reg = Sub8(reg, 1, false, false);
            return 4;
        }

        /// <summary>
        /// Decrements the value at a memory location pointed to by a register pair
        /// <br/>Example: DEC (HL)
        /// </summary>
        private int Dec_Reg16Ptr(ushort addressess)
        {
            byte value = _memoryBus.ReadMemory(addressess);
            _memoryBus.WriteMemory(addressess, Sub8(value, 1, false, false));
            return 11;
        }

        /// <summary>
        /// Decrements a register pair
        /// <br/>Example: DEC BC
        /// </summary>
        private int Dec_Reg16(ref ushort reg)
        {
            reg--;
            return 6;
        }

        /// <summary>
        /// Decrements the value at a memory location pointed to by an index + offset
        /// <br/>Example: DEC (IX+d)
        /// </summary>
        private int Dec_Index(ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, Sub8(value, 1, false, false));

            return 19;
        }

        /// <summary>
        /// Compares a register with another register
        /// <br/>Example: CP A, B
        /// </summary>
        private int Compare_Reg_Reg(byte dest, byte src)
        {
            Sub8(dest, src, false);
            return 4;
        }

        /// <summary>
        /// Compares a register with an immediate value
        /// <br/>Example: CP A, n
        /// </summary>
        private int Compare_Reg_Imm(byte dest)
        {
            Sub8(dest, ReadImm(), false);
            return 7;
        }

        /// <summary>
        /// Compares a register with the value at a memory location pointed to by a register pair
        /// <br/>Example: CP A, (HL)
        /// </summary>
        private int Compare_Reg_Reg16Ptr(byte dest, ushort src)
        {
            Sub8(dest, _memoryBus.ReadMemory(src), false);
            return 7;
        }

        /// <summary>
        /// Compares a register with the value at a memory location pointed to by an index + offset
        /// <br/>Example: CP A, (IX+d)
        /// </summary>
        private int Compare_Reg_Index(ref byte dest, ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            Sub8(dest, _memoryBus.ReadMemory(address), false);
            return 15;
        }

        /// <summary>
        /// Performs a bitwise AND operation on a register and another register
        /// <br/>Example: AND A, B
        /// </summary>
        private int And_Reg_Reg(ref byte dest, byte src)
        {
            dest = And8(dest, src);
            return 4;
        }

        /// <summary>
        /// Performs a bitwise AND operation on a register and an immediate value
        /// <br/>Example: AND A, n
        /// </summary>
        private int And_Reg_Imm(ref byte dest, bool carry = false)
        {
            dest = And8(dest, ReadImm());
            return 7;
        }

        /// <summary>
        /// Performs a bitwise AND operation on a register and the value at a memory location pointed to by a register pair
        /// <br/>Example: AND A, (HL)
        /// </summary>
        private int And_Reg_Reg16Ptr(ref byte dest, ushort src)
        {
            dest = And8(dest, _memoryBus.ReadMemory(src));
            return 7;
        }

        /// <summary>
        /// Performs a bitwise AND operation on a register and the value at a memory location pointed to by an index + offset
        /// <br/>Example: AND A, (IX+d)
        /// </summary>
        private int And_Reg_Index(ref byte dest, ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            dest = And8(dest, _memoryBus.ReadMemory(address));
            return 15;
        }

        /// <summary>
        /// Performs a bitwise OR operation on a register and another register
        /// <br/>Example: OR A, B
        /// </summary>
        private int Or_Reg_Reg(ref byte dest, byte src)
        {
            dest = Or8(dest, src);
            return 4;
        }

        /// <summary>
        /// Performs a bitwise OR operation on a register and an immediate value
        /// <br/>Example: OR A, n
        /// </summary>
        private int Or_Reg_Imm(ref byte dest, bool carry = false)
        {
            dest = Or8(dest, ReadImm());
            return 7;
        }

        /// <summary>
        /// Performs a bitwise OR operation on a register and the value at a memory location pointed to by a register pair
        /// <br/>Example: OR A, (HL)
        /// </summary>
        private int Or_Reg_Reg16Ptr(ref byte dest, ushort src)
        {
            dest = Or8(dest, _memoryBus.ReadMemory(src));
            return 7;
        }

        /// <summary>
        /// Performs a bitwise OR operation on a register and the value at a memory location pointed to by an index + offset
        /// <br/>Example: OR A, (IX+d)
        /// </summary>
        private int Or_Reg_Index(ref byte dest, ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            dest = Or8(dest, _memoryBus.ReadMemory(address));
            return 15;
        }

        /// <summary>
        /// Performs a bitwise XOR operation on a register and another register
        /// <br/>Example: XOR A, B
        /// </summary>
        private int Xor_Reg_Reg(ref byte dest, byte src)
        {
            dest = Xor8(dest, src);
            return 4;
        }

        /// <summary>
        /// Performs a bitwise XOR operation on a register and an immediate value
        /// <br/>Example: XOR A, n
        /// </summary>
        private int Xor_Reg_Imm(ref byte dest, bool carry = false)
        {
            dest = Xor8(dest, ReadImm());
            return 7;
        }

        /// <summary>
        /// Performs a bitwise XOR operation on a register and the value at a memory location pointed to by a register pair
        /// <br/>Example: XOR A, (HL)
        /// </summary>
        private int Xor_Reg_Reg16Ptr(ref byte dest, ushort src)
        {
            dest = Xor8(dest, _memoryBus.ReadMemory(src));
            return 7;
        }

        /// <summary>
        /// Performs a bitwise XOR operation on a register and the value at a memory location pointed to by an index + offset
        /// <br/>Example: XOR A, (IX+d)
        /// </summary>
        private int Xor_Reg_Index(ref byte dest, ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            dest = Xor8(dest, _memoryBus.ReadMemory(address));
            return 15;
        }

        /// <summary>
        /// Complements the value in A (invert all bits)
        /// <br/>Example: CPL
        /// </summary>
        private int Complement_A()
        {
            _registers.A = (byte)~_registers.A;
            _registers.Subtract = true;
            _registers.HalfCarry = true;
            return 4;
        }

        /// <summary>
        /// Negates the value in A (two's complement)
        /// <br./>Example: NEG
        /// </summary>
        private int Negate_A()
        {
            _registers.A = Sub8(0, _registers.A, false);
            return 4;
        }

        /// <summary>
        /// Inverts the carry flag
        /// <br/>Example: CCF
        /// </summary>
        private int Complement_Carry()
        {
            _registers.Carry = !_registers.Carry;
            return 4;
        }

        /// <summary>
        /// Sets the carry flag
        /// <br/>Example: SCF
        /// </summary>
        private int Set_Carry()
        {
            _registers.Carry = true;
            return 4;
        }

        /// <summary>
        /// BCD corrects the value in A
        /// <br/>Example: DAA
        /// Implementation taken from "The Undocumented Z80 Documented" by Sean Young
        /// </summary>
        private int Decimal_Adjust_A()
        {
            int t = 0;

            if (_registers.HalfCarry || ((_registers.A & 0xF) > 9))
            {
                t++;
            }

            if (_registers.Carry || (_registers.A > 0x99))
            {
                t += 2;
                _registers.Carry = true;
            }


            if (_registers.Subtract && !_registers.HalfCarry)
            {
                _registers.HalfCarry = false;
            }
            else
            {
                if (_registers.Subtract && _registers.HalfCarry)
                    _registers.HalfCarry = (((_registers.A & 0x0F)) < 6);
                else
                    _registers.HalfCarry = ((_registers.A & 0x0F) >= 0x0A);
            }

            switch (t)
            {
                case 1:
                {
                    _registers.A += (byte)((_registers.Subtract) ? 0xFA : 0x06); // -6:6
                    break;
                }
                case 2:
                {
                    _registers.A += (byte)((_registers.Subtract) ? 0xA0 : 0x60); // -0x60:0x60
                    break;
                }
                case 3:
                {
                    _registers.A += (byte)((_registers.Subtract) ? 0x9A : 0x66); // -0x66:0x66
                    break;
                }
            }

            _registers.Sign = (_registers.A & 0x80) != 0;
            _registers.Zero = _registers.A == 0;
            _registers.ParityOverflow = Parity(_registers.A);

            return 4;
        }



        /************************* Rotate and Shift **************************/

        /// <summary>
        /// Rotates a register left, bit 7 to carry and to bit 0
        /// <br/>Example: RLC A
        /// </summary>
        private int RLC_Reg(ref byte reg)
        {
            reg = RLC8(reg);
            return 4;
        }

        /// <summary>
        /// Rotates the value at a memory location pointed to by a register pair left, bit 7 to carry and to bit 0
        /// <br/>Example: RLC (HL)
        /// </summary>
        private int RLC_Reg16Ptr(ushort address)
        {
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, RLC8(value));
            return 11;
        }

        private int RLC_Index(ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, RLC8(value));
            return 15;
        }

        /// <summary>
        /// Rotates a register right, bit 0 to carry and to bit 7
        /// <br/>Example: RRC A
        /// </summary>
        private int RRC_Reg(ref byte reg)
        {
            reg = RRC8(reg);
            return 4;
        }

        /// <summary>
        /// Rotates the value at a memory location pointed to by a register pair right, bit 0 to carry and to bit 7
        /// <br/>Example: RRC (HL)
        /// </summary>
        private int RRC_Reg16Ptr(ushort address)
        {
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, RRC8(value));
            return 11;
        }

        private int RRC_Index(ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, RRC8(value));
            return 15;
        }

        /// <summary>
        /// Rotates a register left, bit 7 to carry and carry to bit 0
        /// <br/>Example: RL A
        /// </summary>
        private int RL_Reg(ref byte reg)
        {
            reg = RL8(reg);
            return 4;
        }

        /// <summary>
        /// Rotates the value at a memory location pointed to by a register pair left, bit 7 to carry and carry to bit 0
        /// <br/>Example: RL (HL)
        /// </summary>
        private int RL_Reg16Ptr(ushort address)
        {
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, RL8(value));
            return 11;
        }

        private int RL_Index(ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, RL8(value));
            return 15;
        }

        /// <summary>
        /// Rotates a register right, bit 0 to carry and carry to bit 7
        /// <br/>Example: RR A
        /// </summary>
        private int RR_Reg(ref byte reg)
        {
            reg = RR8(reg);
            return 4;
        }

        /// <summary>
        /// Rotates the value at a memory location pointed to by a register pair right, bit 0 to carry and carry to bit 7
        /// <br/>Example: RR (HL)
        /// </summary>
        private int RR_Reg16Ptr(ushort address)
        {
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, RR8(value));
            return 11;
        }

        private int RR_Index(ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, RR8(value));
            return 15;
        }

        /// <summary>
        /// Shifts a register left, bit 7 to carry and 0 to bit 0
        /// <br/>Example: SLA A
        /// </summary>
        private int SLA_Reg(ref byte reg)
        {
            reg = SLA8(reg);
            return 4;
        }

        /// <summary>
        /// Shifts the value at a memory location pointed to by a register pair left, bit 7 to carry and 0 to bit 0
        /// <br/>Example: SLA (HL)
        /// </summary>
        private int SLA_Reg16Ptr(ushort address)
        {
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, SLA8(value));
            return 11;
        }

        private int SLA_Index(ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, SLA8(value));
            return 15;
        }

        /// <summary>
        /// Shifts a register right, bit 0 to carry and bit 7 unchanged
        /// <br/>Example: SRA A
        /// </summary>
        private int SRA_Reg(ref byte reg)
        {
            reg = SRA8(reg);
            return 4;
        }

        /// <summary>
        /// Shifts the value at a memory location pointed to by a register pair right, bit 0 to carry and bit 7 unchanged
        /// <br/>Example: SRA (HL)
        /// </summary>
        private int SRA_Reg16Ptr(ushort address)
        {
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, SRA8(value));
            return 11;
        }

        private int SRA_Index(ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, SRA8(value));
            return 15;
        }

        /// <summary>
        /// Shifts a register right, bit 0 to carry and 0 to bit 7
        /// <br/>Example: SRL A
        /// </summary>
        private int SRL_Reg(ref byte reg)
        {
            reg = SRL8(reg);
            return 4;
        }

        /// <summary>
        /// Shifts the value at a memory location pointed to by a register pair right, bit 0 to carry and 0 to bit 7
        /// <br/>Example: SRL (HL)
        /// </summary>
        private int SRL_Reg16Ptr(ushort address)
        {
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, SRL8(value));
            return 11;
        }

        private int SRL_Index(ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, SRL8(value));
            return 15;
        }

        /// <summary>
        /// BCD Rotate Right A through (HL)
        /// <br/>Example: RRD
        /// </summary>
        private int RRD()
        {
            byte value = _memoryBus.ReadMemory(_registers.HL);
            byte lowNibble = (byte)(_registers.A & 0x0F);
            byte highNibble = (byte)(value & 0x0F);
            _registers.A = (byte)((_registers.A & 0xF0) | (value >> 4));
            value = (byte)((value << 4) | lowNibble);

            _memoryBus.WriteMemory(_registers.HL, value);
            _registers.HalfCarry = false;
            _registers.Subtract = false;
            _registers.Zero = _registers.A == 0;
            _registers.Sign = (_registers.A & 0x80) != 0;
            _registers.ParityOverflow = Parity(_registers.A);

            return 18;
        }

        /// <summary>
        /// BCD Rotate Left A through (HL)
        /// <br/>Example: RLD
        /// </summary>
        private int RLD()
        {
            byte value = _memoryBus.ReadMemory(_registers.HL);
            byte lowNibble = (byte)(_registers.A & 0x0F);
            byte highNibble = (byte)(value >> 4);
            _registers.A = (byte)((_registers.A & 0xF0) | highNibble);
            value = (byte)((value << 4) | lowNibble);

            _memoryBus.WriteMemory(_registers.HL, value);
            _registers.HalfCarry = false;
            _registers.Subtract = false;
            _registers.Zero = _registers.A == 0;
            _registers.Sign = (_registers.A & 0x80) != 0;
            _registers.ParityOverflow = Parity(_registers.A);

            return 18;
        }



        /************************* Bit Manipulation **************************/

        /// <summary>
        /// Tests a bit in a register
        /// <br/>Example: BIT 0, B
        /// </summary>
        private int Bit_Reg(byte bit, ref byte reg)
        {
            Bit8(bit, reg);
            return 4;
        }

        /// <summary>
        /// Tests a bit in the value at a memory location pointed to by a register pair
        /// <br/>Example: BIT 0, (HL)
        /// </summary>
        private int Bit_Reg16Ptr(byte bit, ushort address)
        {
            byte value = _memoryBus.ReadMemory(address);
            Bit8(bit, value);
            return 8;
        }

        private int Bit_Index(byte bit, ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            byte value = _memoryBus.ReadMemory(address);
            Bit8(bit, value);
            return 12;
        }

        /// <summary>
        /// Sets a bit in a register
        /// <br/>Example: SET 0, B
        /// </summary>
        private int Set_Reg(byte bit, ref byte reg)
        {
            reg = Set8(bit, reg);
            return 4;
        }

        /// <summary>
        /// Sets a bit in the value at a memory location pointed to by a register pair
        /// <br/>Example: SET 0, (HL)
        /// </summary>
        private int Set_Reg16Ptr(byte bit, ushort address)
        {
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, Set8(bit, value));
            return 8;
        }

        private int Set_Index(byte bit, ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, Set8(bit, value));
            return 12;
        }

        /// <summary>
        /// Resets a bit in a register
        /// <br/>Example: RES 0, B
        /// </summary>
        private int Reset_Reg(byte bit, ref byte reg)
        {
            reg = Reset8(bit, reg);
            return 4;
        }

        /// <summary>
        /// Resets a bit in the value at a memory location pointed to by a register pair
        /// <br/>Example: RES 0, (HL)
        /// </summary>
        private int Reset_Reg16Ptr(byte bit, ushort address)
        {
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, Reset8(bit, value));
            return 8;
        }

        private int Reset_Index(byte bit, ushort index)
        {
            byte offset = ReadImm();
            ushort address = (ushort)(index + offset);
            byte value = _memoryBus.ReadMemory(address);
            _memoryBus.WriteMemory(address, Reset8(bit, value));
            return 12;
        }


        /********************** Jump, Call, and Return ***********************/

        /// <summary>
        /// Jumps to an address
        /// <br/>Example: JP 1234h
        /// </summary>
        private int Jump(bool condition = true)
        {
            ushort address = ReadImm16();
            if (condition)
            {
                _registers.PC = address;
            }
            return 10;
        }

        /// <summary>
        /// Jumps to the address in HL
        /// <br/>Example: JP (HL)
        /// </summary>
        private int Jump_Reg16(ushort reg)
        {
            _registers.PC = reg;
            return 4;
        }

        /// <summary>
        /// Adds a signed offset to the address
        /// <br/>Example: JR 10
        /// </summary>
        private int Jump_Relative(bool condition = true)
        {
            sbyte offset = (sbyte)ReadImm();
            if (condition)
            {
                _registers.PC = (ushort)(_registers.PC + offset);
                return 12;
            }
            return 7;
        }

        /// <summary>
        /// Decrements B, then adds a signed offset to the address if B is not zero
        /// <br/>Example: DJNZ -10
        /// </summary>
        private int Dec_Jump_Not_Zero()
        {
            sbyte offset = (sbyte)ReadImm();
            _registers.B--;
            if (_registers.B != 0)
            {
                _registers.PC = (ushort)(_registers.PC + offset);
                return 13;
            }
            return 10;
        }

        /// <summary>
        /// Pushes the address of the next instruction onto the stack, then jumps to an address
        /// <br/>Example: CALL 1234h
        /// </summary>
        private int Call(bool condition = true)
        {
            ushort address = ReadImm16();
            if (condition)
            {
                _registers.SP -= 2;
                WriteShort(_registers.SP, _registers.PC, MemoryAccessType.Stack);
                _registers.PC = address;
                return 17;
            }
            return 10;
        }

        /// <summary>
        /// Pushes the address of the next instruction onto the stack, then jumps to a reset vector
        /// <br/>Example: RST 00h
        /// </summary>
        private int Reset(byte address)
        {
            _registers.SP -= 2;
            WriteShort(_registers.SP, _registers.PC, MemoryAccessType.Stack);
            _registers.PC = address;
            return 11;
        }

        /// <summary>
        /// Pops an address from the stack and jumps to it
        /// <br/>Example: RET
        /// </summary>
        private int Return(bool? condition = null)
        {
            // RET without a condition is faster than RET cc
            if (condition == null)
            {
                _registers.PC = ReadShort(_registers.SP, MemoryAccessType.Stack);
                _registers.SP += 2;
                return 10;
            }

            if (condition == true)
            {
                _registers.PC = ReadShort(_registers.SP);
                _registers.SP += 2;
                return 11;
            }
            return 5;
        }

        /// <summary>
        /// Pops an address from the stack and jumps to it, theoretically sending an interrupt acknowledge signal
        /// <br/>Example: RETI
        /// </summary>
        private int Return_From_Interrupt()
        {
            _registers.PC = ReadShort(_registers.SP, MemoryAccessType.Stack);
            _registers.SP += 2;
            return 10;
        }

        /// <summary>
        /// Pops an address from the stack and jumps to it, restoring the IFF1 flag
        /// <br/>Example: RETN
        /// </summary>
        private int Return_From_Nonmaskable_Interrupt()
        {
            _registers.PC = ReadShort(_registers.SP, MemoryAccessType.Stack);
            _registers.SP += 2;
            _registers.IFF1 = _registers.IFF2;
            return 10;
        }


        /*************************** Input/Output ****************************/

        /// <summary>
        /// Loads a value from a port to a register
        /// <br/>Example: IN A, (00h)
        /// </summary>
        private int In_Reg(ref byte reg)
        {
            byte port = ReadImm();
            reg = _memoryBus.ReadPort(port);
            return 11;
        }

        /// <summary>
        /// Loads a value from a register to a port
        /// <br/>Example: OUT (00h), A
        /// </summary>
        private int Out_Reg(byte reg)
        {
            byte port = ReadImm();
            _memoryBus.WritePort(port, reg);
            return 11;
        }

        /// <summary>
        /// Loads a value from Port (C) to a register
        /// <br/>Example: IN A, (C)
        /// </summary>
        private int In_C_Reg(ref byte reg)
        {
            reg = _memoryBus.ReadPort(_registers.C);
            return 8;
        }

        /// <summary>
        /// Loads a value from a register to Port (C)
        /// <br/>Example: OUT (C), A
        /// </summary>
        private int Out_C_Reg(byte reg)
        {
            _memoryBus.WritePort(_registers.C, reg);
            return 8;
        }

        /// <summary>
        /// Loads a value from Port (C) to (HL), incrementing HL and decrementing B
        /// <br/>Example: INI, INIR
        /// <br/>Note: This has some undocumented behavior that needs to be researched
        /// </summary>
        private int In_Increment()
        {
            byte value = _memoryBus.ReadPort(_registers.C);
            _memoryBus.WriteMemory(_registers.HL, value);
            _registers.HL++;
            _registers.B--;

            _registers.Subtract = true;
            _registers.Zero = _registers.BC == 0; // ??? (Need to find documentation)

            return 12;
        }

        /// <summary>
        /// Loads a value from Port (C) to (HL), decrementing HL and B
        /// <br/>Example: IND, INDR
        /// <br/>Note: This has some undocumented behavior that needs to be researched
        /// </summary>
        private int In_Decrement()
        {
            byte value = _memoryBus.ReadPort(_registers.C);
            _memoryBus.WriteMemory(_registers.HL, value);
            _registers.HL--;
            _registers.B--;

            _registers.Subtract = true;
            _registers.Zero = _registers.BC == 0; // ??? (Need to find documentation)

            return 12;
        }

        /// <summary>
        /// Loads a value from (HL) to Port (C), incrementing HL and decrementing B
        /// <br/>Example: OUTI, OTIR
        /// <br/>Note: This has some undocumented behavior that needs to be researched
        /// </summary>
        private int Out_Increment()
        {
            _registers.B--;
            byte value = _memoryBus.ReadMemory(_registers.HL);
            _memoryBus.WritePort(_registers.C, value);
            _registers.HL++;

            _registers.Subtract = true;
            _registers.Zero = _registers.BC == 0; // ??? (Need to find documentation)

            return 12;
        }

        /// <summary>
        /// Loads a value from (HL) to Port (C), decrementing HL and B
        /// <br/>Example: OUTD, OTDR
        /// <br/>Note: This has some undocumented behavior that needs to be researched
        /// </summary>
        private int Out_Decrement()
        {
            _registers.B--;
            byte value = _memoryBus.ReadMemory(_registers.HL);
            _memoryBus.WritePort(_registers.C, value);
            _registers.HL--;

            _registers.Subtract = true;
            _registers.Zero = _registers.BC == 0; // ??? (Need to find documentation)

            return 12;
        }



        /**************************** CPU Control ****************************/

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

        private int Enable_Interrupts()
        {
            _registers.IFF1 = _registers.IFF2 = true;
            _justEnabledInterrupts = true;
            return 4;
        }

        private int Disable_Interrupts()
        {
            _registers.IFF1 = _registers.IFF2 = false;
            return 4;
        }

        private int Set_Interrupt_Mode(byte mode)
        {
            // Ceres80 only uses IM 1, because my physical Z80's IM 2 is broken.
            if (mode != 1)
            {
                Console.WriteLine("WARNING! This emulator only supports IM 1!");
            }
            return 4;
        }

        /************************** Internal Helpers *************************/
        private ushort ReadShort(ushort address, MemoryAccessType accessType = MemoryAccessType.Standard)
        {
            byte low = _memoryBus.ReadMemory(address, accessType);
            byte high = _memoryBus.ReadMemory((ushort)(address + 1), accessType);
            return (ushort)((high << 8) | low);
        }

        private void WriteShort(ushort address, ushort data, MemoryAccessType accessType = MemoryAccessType.Standard)
        {
            byte low = (byte)data;
            byte high = (byte)(data >> 8);
            _memoryBus.WriteMemory(address, low, accessType);
            _memoryBus.WriteMemory((ushort)(address + 1), high, accessType);
        }

        private byte ReadImm()
        {
            byte value = _memoryBus.ReadMemory(_registers.PC, MemoryAccessType.Immediate);
            _registers.PC += 1;
            return value;
        }

        private ushort ReadImm16()
        {
            ushort value = ReadShort(_registers.PC, MemoryAccessType.Immediate);
            _registers.PC += 2;
            return value;
        }

        private byte Add8(byte a, byte b, bool carry, bool setCarry = true)
        {
            byte result = (byte)(a + b + (carry ? 1 : 0));
            bool overflow = WillOverflow<byte>(a, (byte)(b + (carry ? 1 : 0)));

            // Increment instructions don't set carry. For some reason.
            if (setCarry)
                _registers.Carry = overflow;
            _registers.Subtract = false;
            _registers.ParityOverflow = overflow;
            _registers.HalfCarry = (a & 0x0F) < (b & 0x0F);
            _registers.Zero = result == 0;
            _registers.Sign = (result & 0x80) != 0;

            return result;
        }

        private byte Sub8(byte a, byte b, bool carry, bool setCarry = true)
        {
            byte result = (byte)(a - b - (carry ? 1 : 0));
            bool underflow = WillUnderflow<byte>(a, (byte)(b + (carry ? 1 : 0)));

            if (setCarry)
                _registers.Carry = underflow;
            _registers.Subtract = true;
            _registers.ParityOverflow = underflow;
            _registers.HalfCarry = (a & 0x0F) > (b & 0x0F);
            _registers.Zero = result == 0;
            _registers.Sign = (result & 0x80) != 0;

            return result;
        }

        private byte And8(byte a, byte b)
        {
            byte result = (byte)(a & b);
            _registers.Carry = false;
            _registers.Subtract = false;
            _registers.ParityOverflow = Parity(result);
            _registers.HalfCarry = true;
            _registers.Zero = result == 0;
            _registers.Sign = (result & 0x80) != 0;
            return result;
        }

        private byte Or8(byte a, byte b)
        {
            byte result = (byte)(a | b);
            _registers.Carry = false;
            _registers.Subtract = false;
            _registers.ParityOverflow = Parity(result);
            _registers.HalfCarry = false;
            _registers.Zero = result == 0;
            _registers.Sign = (result & 0x80) != 0;
            return result;
        }

        private byte Xor8(byte a, byte b)
        {
            byte result = (byte)(a ^ b);
            _registers.Carry = false;
            _registers.Subtract = false;
            _registers.ParityOverflow = Parity(result);
            _registers.HalfCarry = false;
            _registers.Zero = result == 0;
            _registers.Sign = (result & 0x80) != 0;
            return result;
        }

        private byte RLC8(byte a)
        {
            byte bit7 = (byte)(a & 0x80);
            a = (byte)((a << 1) | (bit7 >> 7));

            _registers.Carry = bit7 != 0;
            _registers.Subtract = false;
            _registers.ParityOverflow = Parity(a);
            _registers.HalfCarry = false;
            _registers.Zero = a == 0;
            _registers.Sign = (a & 0x80) != 0;

            return a;
        }

        private byte RRC8(byte a)
        {
            byte bit0 = (byte)(a & 0x01);
            a = (byte)((a >> 1) | (bit0 << 7));

            _registers.Carry = bit0 != 0;
            _registers.Subtract = false;
            _registers.ParityOverflow = Parity(a);
            _registers.HalfCarry = false;
            _registers.Zero = a == 0;
            _registers.Sign = (a & 0x80) != 0;

            return a;
        }

        private byte RL8(byte a)
        {
            byte bit7 = (byte)(a & 0x80);
            a = (byte)((a << 1) | (_registers.Carry ? 1 : 0));

            _registers.Carry = bit7 != 0;
            _registers.Subtract = false;
            _registers.ParityOverflow = Parity(a);
            _registers.HalfCarry = false;
            _registers.Zero = a == 0;
            _registers.Sign = (a & 0x80) != 0;

            return a;
        }

        private byte RR8(byte a)
        {
            byte bit0 = (byte)(a & 0x01);
            a = (byte)((a >> 1) | (_registers.Carry ? 0x80 : 0));

            _registers.Carry = bit0 != 0;
            _registers.Subtract = false;
            _registers.ParityOverflow = Parity(a);
            _registers.HalfCarry = false;
            _registers.Zero = a == 0;
            _registers.Sign = (a & 0x80) != 0;

            return a;
        }

        private byte SLA8(byte a)
        {
            _registers.Carry = (a & 0x80) != 0;
            a = (byte)(a << 1);

            _registers.Subtract = false;
            _registers.ParityOverflow = Parity(a);
            _registers.HalfCarry = false;
            _registers.Zero = a == 0;
            _registers.Sign = (a & 0x80) != 0;

            return a;
        }

        private byte SRA8(byte a)
        {
            _registers.Carry = (a & 0x01) != 0;
            a = (byte)((a & 0x80) | (a >> 1));

            _registers.Subtract = false;
            _registers.ParityOverflow = Parity(a);
            _registers.HalfCarry = false;
            _registers.Zero = a == 0;
            _registers.Sign = (a & 0x80) != 0;

            return a;
        }

        private byte SRL8(byte a)
        {
            _registers.Carry = (a & 0x01) != 0;
            a = (byte)(a >> 1);

            _registers.Subtract = false;
            _registers.ParityOverflow = Parity(a);
            _registers.HalfCarry = false;
            _registers.Zero = a == 0;
            _registers.Sign = false;

            return a;
        }

        private void Bit8(byte bit, byte a)
        {
            _registers.Subtract = false;
            _registers.HalfCarry = true;
            _registers.Zero = (a & (1 << bit)) == 0;
        }

        private byte Set8(byte bit, byte a)
        {
            return (byte)(a | (1 << bit));
        }

        private byte Reset8(byte bit, byte a)
        {
            return (byte)(a & ~(1 << bit));
        }

        private bool Parity(byte a)
        {
            int count = 0;
            for (int i = 0; i < 8; i++)
            {
                if ((a & (1 << i)) != 0)
                    count++;
            }
            return count % 2 == 0;
        }

        private bool WillOverflow<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(default(T)) > 0 && b.CompareTo(default(T)) > 0 && a.CompareTo(default(T)) + b.CompareTo(default(T)) < 0;
        }

        private bool WillUnderflow<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(default(T)) < 0 && b.CompareTo(default(T)) < 0 && a.CompareTo(default(T)) + b.CompareTo(default(T)) > 0;
        }
    }
}
