﻿// Long boi file.
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
                case 0x00:
                {
                    cycles = No_Operation();
                    instruction = "NOP";
                    break;
                }
                case 0x01:
                {
                    // Cannot pass method get property by reference
                    ushort temp = _registers.BC;
                    cycles = Load_Reg16_Imm(ref temp);
                    _registers.BC = temp;
                    instruction = "LD BC, nn";
                    break;
                }
                case 0x02:
                {
                    cycles = Load_Reg16Ptr_Reg(_registers.BC, _registers.A);
                    instruction = "LD (BC), A";
                    break;
                }
                case 0x03:
                {
                    ushort temp = _registers.BC;
                    cycles = Inc_Reg16(ref temp);
                    _registers.BC = temp;
                    instruction = "INC BC";
                    break;
                }
                case 0x04:
                {
                    cycles = Inc_Reg(ref _registers.B);
                    instruction = "INC B";
                    break;
                }
                case 0x05:
                {
                    cycles = Dec_Reg(ref _registers.B);
                    instruction = "DEC B";
                    break;
                }
                case 0x06:
                {
                    cycles = Load_Reg_Imm(ref _registers.B);
                    instruction = "LD B, n";
                    break;
                }
                case 0x07:
                {
                    cycles = RLC_Reg(ref _registers.A);
                    instruction = "RLCA";
                    break;
                }
                case 0x08:
                {
                    cycles = Exchange_AF();
                    instruction = "EX AF, AF'";
                    break;
                }
                case 0x09:
                {
                    ushort temp = _registers.HL;
                    cycles = Add_Reg16_Reg16(ref temp, _registers.BC);
                    _registers.HL = temp;
                    instruction = "ADD HL, BC";
                    break;
                }
                case 0x0A:
                {
                    cycles = Load_Reg_Reg16Ptr(ref _registers.A, _registers.BC);
                    instruction = "LD A, (BC)";
                    break;
                }
                case 0x0B:
                {
                    ushort temp = _registers.BC;
                    cycles = Dec_Reg16(ref temp);
                    _registers.BC = temp;
                    instruction = "DEC BC";
                    break;
                }
                case 0x0C:
                {
                    cycles = Inc_Reg(ref _registers.C);
                    instruction = "INC C";
                    break;
                }
                case 0x0D:
                {
                    cycles = Dec_Reg(ref _registers.C);
                    instruction = "DEC C";
                    break;
                }
                case 0x0E:
                {
                    cycles = Load_Reg_Imm(ref _registers.C);
                    instruction = "LD C, n";
                    break;
                }
                case 0x0F:
                {
                    cycles = RRC_Reg(ref _registers.A);
                    instruction = "RRCA";
                    break;
                }
                case 0x10:
                {
                    cycles = Dec_Jump_Not_Zero();
                    instruction = "DJNZ d";
                    break;
                }
                case 0x11:
                {
                    ushort temp = _registers.DE;
                    cycles = Load_Reg16_Imm(ref temp);
                    _registers.DE = temp;
                    instruction = "LD DE, nn";
                    break;
                }
                case 0x12:
                {
                    cycles = Load_Reg16Ptr_Reg(_registers.DE, _registers.A);
                    instruction = "LD (DE), A";
                    break;
                }
                case 0x13:
                {
                    ushort temp = _registers.DE;
                    cycles = Inc_Reg16(ref temp);
                    _registers.DE = temp;
                    instruction = "INC DE";
                    break;
                }
                case 0x14:
                {
                    cycles = Inc_Reg(ref _registers.D);
                    instruction = "INC D";
                    break;
                }
                case 0x15:
                {
                    cycles = Dec_Reg(ref _registers.D);
                    instruction = "DEC D";
                    break;
                }
                case 0x16:
                {
                    cycles = Load_Reg_Imm(ref _registers.D);
                    instruction = "LD D, n";
                    break;
                }
                case 0x17:
                {
                    cycles = RL_Reg(ref _registers.A);
                    instruction = "RLA";
                    break;
                }
                case 0x18:
                {
                    cycles = Jump_Relative();
                    instruction = "JR d";
                    break;
                }
                case 0x19:
                {
                    ushort temp = _registers.HL;
                    cycles = Add_Reg16_Reg16(ref temp, _registers.DE);
                    _registers.HL = temp;
                    instruction = "ADD HL, DE";
                    break;
                }
                case 0x1A:
                {
                    cycles = Load_Reg_Reg16Ptr(ref _registers.A, _registers.DE);
                    instruction = "LD A, (DE)";
                    break;
                }
                case 0x1B:
                {
                    ushort temp = _registers.DE;
                    cycles = Dec_Reg16(ref temp);
                    _registers.DE = temp;
                    instruction = "DEC DE";
                    break;
                }
                case 0x1C:
                {
                    cycles = Inc_Reg(ref _registers.E);
                    instruction = "INC E";
                    break;
                }
                case 0x1D:
                {
                    cycles = Dec_Reg(ref _registers.E);
                    instruction = "DEC E";
                    break;
                }
                case 0x1E:
                {
                    cycles = Load_Reg_Imm(ref _registers.E);
                    instruction = "LD E, n";
                    break;
                }
                case 0x1F:
                {
                    cycles = RR_Reg(ref _registers.A);
                    instruction = "RRA";
                    break;
                }
                case 0x20:
                {
                    cycles = Jump_Relative(!_registers.Zero);
                    instruction = "JR NZ, d";
                    break;
                }
                case 0x21:
                {
                    ushort temp = _registers.HL;
                    cycles = Load_Reg16_Imm(ref temp);
                    _registers.HL = temp;
                    instruction = "LD HL, nn";
                    break;
                }
                case 0x22:
                {
                    cycles = Load_ImmPtr_Reg16(_registers.HL);
                    instruction = "LD (nn), HL";
                    break;
                }
                case 0x23:
                {
                    ushort temp = _registers.HL;
                    cycles = Inc_Reg16(ref temp);
                    _registers.HL = temp;
                    instruction = "INC HL";
                    break;
                }
                case 0x24:
                {
                    cycles = Inc_Reg(ref _registers.H);
                    instruction = "INC H";
                    break;
                }
                case 0x25:
                {
                    cycles = Dec_Reg(ref _registers.H);
                    instruction = "DEC H";
                    break;
                }
                case 0x26:
                {
                    cycles = Load_Reg_Imm(ref _registers.H);
                    instruction = "LD H, n";
                    break;
                }
                case 0x27:
                {
                    cycles = Decimal_Adjust_A();
                    instruction = "DAA";
                    break;
                }
                case 0x28:
                {
                    cycles = Jump_Relative(_registers.Zero);
                    instruction = "JR Z, d";
                    break;
                }
                case 0x29:
                {
                    ushort temp = _registers.HL;
                    cycles = Add_Reg16_Reg16(ref temp, _registers.HL);
                    _registers.HL = temp;
                    instruction = "ADD HL, HL";
                    break;
                }
                case 0x2A:
                {
                    ushort temp = _registers.HL;
                    cycles = Load_Reg16_Imm(ref temp);
                    _registers.HL = temp;
                    instruction = "LD HL, (nn)";
                    break;
                }
                case 0x2B:
                {
                    ushort temp = _registers.HL;
                    cycles = Dec_Reg16(ref temp);
                    _registers.HL = temp;
                    instruction = "DEC HL";
                    break;
                }
                case 0x2C:
                {
                    cycles = Inc_Reg(ref _registers.L);
                    instruction = "INC L";
                    break;
                }
                case 0x2D:
                {
                    cycles = Dec_Reg(ref _registers.L);
                    instruction = "DEC L";
                    break;
                }
                case 0x2E:
                {
                    cycles = Load_Reg_Imm(ref _registers.L);
                    instruction = "LD L, n";
                    break;
                }
                case 0x2F:
                {
                    cycles = Complement_A();
                    instruction = "CPL";
                    break;
                }
                case 0x30:
                {
                    cycles = Jump_Relative(!_registers.Carry);
                    instruction = "JR NC, d";
                    break;
                }
                case 0x31:
                {
                    cycles = Load_Reg16_Imm(ref _registers.SP);
                    instruction = "LD SP, nn";
                    break;
                }
                case 0x32:
                {
                    cycles = Load_ImmPtr_Reg(_registers.HL, _registers.A);
                    instruction = "LD (nn), A";
                    break;
                }
                case 0x33:
                {
                    ushort temp = _registers.SP;
                    cycles = Inc_Reg16(ref temp);
                    _registers.SP = temp;
                    instruction = "INC SP";
                    break;
                }
                case 0x34:
                {
                    cycles = Inc_Reg16Ptr(_registers.HL);
                    instruction = "INC (HL)";
                    break;
                }
                case 0x35:
                {
                    cycles = Dec_Reg16Ptr(_registers.HL);
                    instruction = "DEC (HL)";
                    break;
                }
                case 0x36:
                {
                    cycles = Load_Reg16Ptr_Imm(_registers.HL);
                    instruction = "LD (HL), n";
                    break;
                }
                case 0x37:
                {
                    cycles = Set_Carry();
                    instruction = "SCF";
                    break;
                }
                case 0x38:
                {
                    cycles = Jump_Relative(_registers.Carry);
                    instruction = "JR C, d";
                    break;
                }
                case 0x39:
                {
                    ushort temp = _registers.HL;
                    cycles = Add_Reg16_Reg16(ref temp, _registers.SP);
                    _registers.HL = temp;
                    instruction = "ADD HL, SP";
                    break;
                }
                case 0x3A:
                {
                    cycles = Load_Reg_Reg16Ptr(ref _registers.A, _registers.HL);
                    instruction = "LD A, (nn)";
                    break;
                }
                case 0x3B:
                {
                    ushort temp = _registers.SP;
                    cycles = Dec_Reg16(ref temp);
                    _registers.SP = temp;
                    instruction = "DEC SP";
                    break;
                }
                case 0x3C:
                {
                    cycles = Inc_Reg(ref _registers.A);
                    instruction = "INC A";
                    break;
                }
                case 0x3D:
                {
                    cycles = Dec_Reg(ref _registers.A);
                    instruction = "DEC A";
                    break;
                }
                case 0x3E:
                {
                    cycles = Load_Reg_Imm(ref _registers.A);
                    instruction = "LD A, n";
                    break;
                }
                case 0x3F:
                {
                    cycles = Complement_Carry();
                    instruction = "CCF";
                    break;
                }
                // Load Register to Register
                case >= 0x40 and <= 0x7F:
                {

                        byte srcCode = (byte)(opcode & 0b111);
                        byte destCode = (byte)((opcode & 0b111000) >> 3);
                        if (srcCode == 6) // (HL)
                        {
                            if (destCode == 6) // (HL)
                            {
                                cycles = Halt();
                                instruction = "HALT";
                            }
                            else
                            {
                                cycles = Load_Reg_Reg16Ptr(ref GetReg(destCode), _registers.HL);
                                instruction = $"LD {RegCodeToString(destCode)}, (HL)";
                            }
                        }
                        else
                        {
                            if (destCode == 6) // (HL)
                            {
                                cycles = Load_Reg16Ptr_Reg(_registers.HL, GetReg(srcCode));
                                instruction = $"LD (HL), {RegCodeToString(srcCode)}";
                            }
                            else
                            {
                                cycles = Load_Reg_Reg(ref GetReg(destCode), GetReg(srcCode));
                                instruction = $"LD {RegCodeToString(destCode)}, {RegCodeToString(srcCode)}";
                            }
                        }
                        break;
                }
                case >= 0x80 and <= 0x87:
                {
                    byte regCode = (byte)(opcode & 0b111);

                    if (regCode == 6) // (HL)
                    {
                        cycles = Add_Reg_Reg16Ptr(ref _registers.A, _registers.HL);
                        instruction = "ADD A, (HL)";
                    }
                    else
                    {
                        cycles = Add_Reg_Reg(ref _registers.A, GetReg(regCode));
                        instruction = $"ADD A, {RegCodeToString(regCode)}";
                    }
                    break;
                }

                case >= 0x88 and <= 0x8F:
                {
                    byte regCode = (byte)(opcode & 0b111);

                    if (regCode == 6) // (HL)
                    {
                        cycles = Add_Reg_Reg16Ptr(ref _registers.A, _registers.HL, true);
                        instruction = "ADC A, (HL)";
                    }
                    else
                    {
                        cycles = Add_Reg_Reg(ref _registers.A, GetReg(regCode), true);
                        instruction = $"ADC A, {RegCodeToString(regCode)}";
                    }
                    break;
                }

                case >= 0x90 and <= 0x97:
                {
                    byte regCode = (byte)(opcode & 0b111);

                    if (regCode == 6) // (HL)
                    {
                        cycles = Sub_Reg_Reg16Ptr(ref _registers.A, _registers.HL);
                        instruction = "SUB A, (HL)";
                    }
                    else
                    {
                        cycles = Sub_Reg_Reg(ref _registers.A, GetReg(regCode));
                        instruction = $"SUB A, {RegCodeToString(regCode)}";
                    }
                    break;
                }

                case >= 0x98 and <= 0x9F:
                {
                    byte regCode = (byte)(opcode & 0b111);

                    if (regCode == 6) // (HL)
                    {
                        cycles = Sub_Reg_Reg16Ptr(ref _registers.A, _registers.HL, true);
                        instruction = "SBC A, (HL)";
                    }
                    else
                    {
                        cycles = Sub_Reg_Reg(ref _registers.A, GetReg(regCode), true);
                        instruction = $"SBC A, {RegCodeToString(regCode)}";
                    }
                    break;
                }

                case >= 0xA0 and <= 0xA7:
                {
                    byte regCode = (byte)(opcode & 0b111);

                    if (regCode == 6) // (HL)
                    {
                        cycles = And_Reg_Reg16Ptr(ref _registers.A, _registers.HL);
                        instruction = "AND A, (HL)";
                    }
                    else
                    {
                        cycles = And_Reg_Reg(ref _registers.A, GetReg(regCode));
                        instruction = $"AND A, {RegCodeToString(regCode)}";
                    }
                    break;
                }

                case >= 0xA8 and <= 0xAF:
                {
                    byte regCode = (byte)(opcode & 0b111);

                    if (regCode == 6) // (HL)
                    {
                        cycles = Xor_Reg_Reg16Ptr(ref _registers.A, _registers.HL);
                        instruction = "XOR A, (HL)";
                    }
                    else
                    {
                        cycles = Xor_Reg_Reg(ref _registers.A, GetReg(regCode));
                        instruction = $"XOR A, {RegCodeToString(regCode)}";
                    }
                    break;
                }

                case >= 0xB0 and <= 0xB7:
                {
                    byte regCode = (byte)(opcode & 0b111);

                    if (regCode == 6) // (HL)
                    {
                        cycles = Or_Reg_Reg16Ptr(ref _registers.A, _registers.HL);
                        instruction = "OR A, (HL)";
                    }
                    else
                    {
                        cycles = Or_Reg_Reg(ref _registers.A, GetReg(regCode));
                        instruction = $"OR A, {RegCodeToString(regCode)}";
                    }
                    break;
                }

                case >= 0xB8 and <= 0xBF:
                {
                    byte regCode = (byte)(opcode & 0b111);

                    if (regCode == 6) // (HL)
                    {
                        cycles = Compare_Reg_Reg16Ptr(_registers.A, _registers.HL);
                        instruction = "CP A, (HL)";
                    }
                    else
                    {
                        cycles = Compare_Reg_Reg(_registers.A, GetReg(regCode));
                        instruction = $"CP A, {RegCodeToString(regCode)}";
                    }
                    break;
                }

                case 0xC0:
                {
                    cycles = Return(!_registers.Zero);
                    instruction = "RET NZ";
                    break;
                }
                case 0xC1:
                {
                    ushort temp = _registers.BC;
                    cycles = Pop_Reg16(ref temp);
                    _registers.BC = temp;
                    instruction = "POP BC";
                    break;
                }
                case 0xC2:
                {
                    cycles = Jump(!_registers.Zero);
                    instruction = "JP NZ, nn";
                    break;
                }
                case 0xC3:
                {
                    cycles = Jump();
                    instruction = "JP nn";
                    break;
                }
                case 0xC4:
                {
                    cycles = Call(!_registers.Zero);
                    instruction = "CALL NZ, nn";
                    break;
                }
                case 0xC5:
                {
                    cycles = Push_Reg16(_registers.BC);
                    instruction = "PUSH BC";
                    break;
                }
                case 0xC6:
                {
                    cycles = Add_Reg_Imm(ref _registers.A);
                    instruction = "ADD A, n";
                    break;
                }
                case 0xC7:
                {
                    cycles = Reset(0x00);
                    instruction = "RST 00H";
                    break;
                }
                case 0xC8:
                {
                    cycles = Return(_registers.Zero);
                    instruction = "RET Z";
                    break;
                }
                case 0xC9:
                {
                    cycles = Return();
                    instruction = "RET";
                    break;
                }
                case 0xCA:
                {
                    cycles = Jump(_registers.Zero);
                    instruction = "JP Z, nn";
                    break;
                }
                case 0xCC:
                {
                    cycles = Call(_registers.Zero);
                    instruction = "CALL Z, nn";
                    break;
                }
                case 0xCD:
                {
                    cycles = Call();
                    instruction = "CALL nn";
                    break;
                }
                case 0xCE:
                {
                    cycles = Add_Reg_Imm(ref _registers.A, true);
                    instruction = "ADC A, n";
                    break;
                }
                case 0xCF:
                {
                    cycles = Reset(0x08);
                    instruction = "RST 08H";
                    break;
                }
                case 0xD0:
                {
                    cycles = Return(!_registers.Carry);
                    instruction = "RET NC";
                    break;
                }
                case 0xD1:
                {
                    ushort temp = _registers.DE;
                    cycles = Pop_Reg16(ref temp);
                    _registers.DE = temp;
                    instruction = "POP DE";
                    break;
                }
                case 0xD2:
                {
                    cycles = Jump(!_registers.Carry);
                    instruction = "JP NC, nn";
                    break;
                }
                case 0xD3:
                {
                    cycles = Out_Reg(_registers.A);
                    instruction = "OUT (n), A";
                    break;
                }
                case 0xD4:
                {
                    cycles = Call(!_registers.Carry);
                    instruction = "CALL NC, nn";
                    break;
                }
                case 0xD5:
                {
                    cycles = Push_Reg16(_registers.DE);
                    instruction = "PUSH DE";
                    break;
                }
                case 0xD6:
                {
                    cycles = Sub_Reg_Imm(ref _registers.A);
                    instruction = "SUB A, n";
                    break;
                }
                case 0xD7:
                {
                    cycles = Reset(0x10);
                    instruction = "RST 10H";
                    break;
                }
                case 0xD8:
                {
                    cycles = Return(_registers.Carry);
                    instruction = "RET C";
                    break;
                }
                case 0xD9:
                {
                    cycles = Exchange_X();
                    instruction = "EXX";
                    break;
                }
                case 0xDA:
                {
                    cycles = Jump(_registers.Carry);
                    instruction = "JP C, nn";
                    break;
                }
                case 0xDB:
                {
                    cycles = In_Reg(ref _registers.A);
                    instruction = "IN A, (n)";
                    break;
                }
                case 0xDC:
                {
                    cycles = Call(_registers.Carry);
                    instruction = "CALL C, nn";
                    break;
                }
                case 0xDE:
                {
                    cycles = Sub_Reg_Imm(ref _registers.A, true);
                    instruction = "SBC A, n";
                    break;
                }
                case 0xDF:
                {
                    cycles = Reset(0x18);
                    instruction = "RST 18H";
                    break;
                }
                case 0xE0:
                {
                    cycles = Return(!_registers.ParityOverflow);
                    instruction = "RET PO";
                    break;
                }
                case 0xE1:
                {
                    ushort temp = _registers.HL;
                    cycles = Pop_Reg16(ref temp);
                    _registers.HL = temp;
                    instruction = "POP HL";
                    break;
                }
                case 0xE2:
                {
                    cycles = Jump(!_registers.ParityOverflow);
                    instruction = "JP PO, nn";
                    break;
                }
                case 0xE3:
                {
                    ushort temp = _registers.HL;
                    cycles = Exchange_SPPtr_Reg16(ref temp);
                    _registers.HL = temp;
                    instruction = "EX (SP), HL";
                    break;
                }
                case 0xE4:
                {
                    cycles = Call(!_registers.ParityOverflow);
                    instruction = "CALL PO, nn";
                    break;
                }
                case 0xE5:
                {
                    cycles = Push_Reg16(_registers.HL);
                    instruction = "PUSH HL";
                    break;
                }
                case 0xE6:
                {
                    cycles = And_Reg_Imm(ref _registers.A);
                    instruction = "AND A, n";
                    break;
                }
                case 0xE7:
                {
                    cycles = Reset(0x20);
                    instruction = "RST 20H";
                    break;
                }
                case 0xE8:
                {
                    cycles = Return(_registers.ParityOverflow);
                    instruction = "RET PE";
                    break;
                }
                case 0xE9:
                {
                    cycles = Jump_HL();
                    instruction = "JP (HL)";
                    break;
                }
                case 0xEA:
                {
                    cycles = Jump(_registers.ParityOverflow);
                    instruction = "JP PE, nn";
                    break;
                }
                case 0xEB:
                {
                    cycles = Exchange_DE_HL();
                    instruction = "EX DE, HL";
                    break;
                }
                case 0xEC:
                {
                    cycles = Call(_registers.ParityOverflow);
                    instruction = "CALL PE, nn";
                    break;
                }
                case 0xEE:
                {
                    cycles = Xor_Reg_Imm(ref _registers.A);
                    instruction = "XOR A, n";
                    break;
                }
                case 0xEF:
                {
                    cycles = Reset(0x28);
                    instruction = "RST 28H";
                    break;
                }
                case 0xF0:
                {
                    cycles = Return(!_registers.Sign);
                    instruction = "RET P";
                    break;
                }
                case 0xF1:
                {
                    ushort temp = _registers.AF;
                    cycles = Pop_Reg16(ref temp);
                    _registers.AF = temp;
                    instruction = "POP AF";
                    break;
                }
                case 0xF2:
                {
                    cycles = Jump(!_registers.Sign);
                    instruction = "JP P, nn";
                    break;
                }
                case 0xF3:
                {
                    cycles = Disable_Interrupts();
                    instruction = "DI";
                    break;
                }
                case 0xF4:
                {
                    cycles = Call(!_registers.Sign);
                    instruction = "CALL P, nn";
                    break;
                }
                case 0xF5:
                {
                    cycles = Push_Reg16(_registers.AF);
                    instruction = "PUSH AF";
                    break;
                }
                case 0xF6:
                {
                    cycles = Or_Reg_Imm(ref _registers.A);
                    instruction = "OR A, n";
                    break;
                }
                case 0xF7:
                {
                    cycles = Reset(0x30);
                    instruction = "RST 30H";
                    break;
                }
                case 0xF8:
                {
                    cycles = Return(_registers.Sign);
                    instruction = "RET M";
                    break;
                }
                case 0xF9:
                {
                    cycles = Load_Reg16_Reg16(ref _registers.SP, _registers.HL);
                    instruction = "LD SP, HL";
                    break;
                }
                case 0xFA:
                {
                    cycles = Jump(_registers.Sign);
                    instruction = "JP M, nn";
                    break;
                }
                case 0xFB:
                {
                    cycles = Enable_Interrupts();
                    instruction = "EI";
                    break;
                }
                case 0xFC:
                {
                    cycles = Call(_registers.Sign);
                    instruction = "CALL M, nn";
                    break;
                }
                case 0xFE:
                {
                    cycles = Compare_Reg_Imm(_registers.A);
                    instruction = "CP A, n";
                    break;
                }
                case 0xFF:
                {
                    cycles = Reset(0x38);
                    instruction = "RST 38H";
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




        private (int, string) DecodeMisc()
        {
            // Took 4 cycles to read first byte of opcode
            int cycles = 4;
            string instruction = "";
            int opcode = _memoryBus.Read(_registers.PC);

            switch (opcode)
            {
                case 0x40:
                {
                    cycles += In_C_Reg(ref _registers.B);
                    instruction = "IN B, (C)";
                    break;
                }
                case 0x41:
                {
                    cycles += Out_C_Reg(_registers.C);
                    instruction = "OUT (C), B";
                    break;
                }
                case 0x42:
                {
                    ushort temp = _registers.HL;
                    cycles += Sub_Reg16_Reg16(ref temp, _registers.BC);
                    _registers.HL = temp;
                    instruction = "SBC HL, BC";
                    break;
                }
                case 0x43:
                {
                    cycles += Load_ImmPtr_Reg16(_registers.BC);
                    instruction = "LD (nn), BC";
                    break;
                }
                case 0x44:
                {
                    cycles += Negate_A();
                    instruction = "NEG";
                    break;
                }
                case 0x45:
                {
                    cycles += Return_From_Nonmaskable_Interrupt();
                    instruction = "RETN";
                    break;
                }
                case 0x46:
                {
                    cycles += Set_Interrupt_Mode(0);
                    instruction = "IM 0";
                    break;
                }
                case 0x47:
                {
                    cycles += Load_Reg_Reg(ref _registers.I, _registers.A);
                    instruction = "LD I, A";
                    break;
                }
                case 0x48:
                {
                    cycles += In_C_Reg(ref _registers.C);
                    instruction = "IN C, (C)";
                    break;
                }
                case 0x49:
                {
                    cycles += Out_C_Reg(_registers.C);
                    instruction = "OUT (C), C";
                    break;
                }
                case 0x4A:
                {
                    ushort temp = _registers.HL;
                    cycles += Add_Reg16_Reg16(ref temp, _registers.BC, true);
                    _registers.HL = temp;
                    instruction = "ADC HL, BC";
                    break;
                }
                case 0x4B:
                {
                    cycles += Load_Reg16Ptr_Imm(_registers.BC);
                    instruction = "LD BC, (nn)";
                    break;
                }
                case 0x4D:
                {
                    cycles += Return_From_Interrupt();
                    instruction = "RETI";
                    break;
                }
                case 0x4F:
                {
                    cycles += Load_Reg_Reg(ref _registers.R, _registers.A);
                    instruction = "LD R, A";
                    break;
                }
                case 0x50:
                {
                    cycles += In_C_Reg(ref _registers.D);
                    instruction = "IN D, (C)";
                    break;
                }
                case 0x51:
                {
                    cycles += Out_C_Reg(_registers.D);
                    instruction = "OUT (C), D";
                    break;
                }
                case 0x52:
                {
                    ushort temp = _registers.HL;
                    cycles += Sub_Reg16_Reg16(ref temp, _registers.DE);
                    _registers.HL = temp;
                    instruction = "SBC HL, DE";
                    break;
                }
                case 0x53:
                {
                    cycles += Load_ImmPtr_Reg16(_registers.DE);
                    instruction = "LD (nn), DE";
                    break;
                }
                case 0x56:
                {
                    cycles += Set_Interrupt_Mode(1);
                    instruction = "IM 1";
                    break;
                }
                case 0x57:
                {
                    cycles += Load_Reg_Reg(ref _registers.A, _registers.I);
                    instruction = "LD A, I";
                    break;
                }
                case 0x58:
                {
                    cycles += In_C_Reg(ref _registers.E);
                    instruction = "IN E, (C)";
                    break;
                }
                case 0x59:
                {
                    cycles += Out_C_Reg(_registers.E);
                    instruction = "OUT (C), E";
                    break;
                }
                case 0x5A:
                {
                    ushort temp = _registers.HL;
                    cycles += Add_Reg16_Reg16(ref temp, _registers.DE, true);
                    _registers.HL = temp;
                    instruction = "ADC HL, DE";
                    break;
                }
                case 0x5B:
                {
                    cycles += Load_Reg16Ptr_Imm(_registers.DE);
                    instruction = "LD DE, (nn)";
                    break;
                }
                case 0x5E:
                {
                    cycles += Set_Interrupt_Mode(2);
                    instruction = "IM 2";
                    break;
                }
                case 0x5F:
                {
                    cycles += Load_Reg_Reg(ref _registers.A, _registers.R);
                    instruction = "LD A, R";
                    break;
                }
                case 0x60:
                {
                    cycles += In_C_Reg(ref _registers.H);
                    instruction = "IN H, (C)";
                    break;
                }
                case 0x61:
                {
                    cycles += Out_C_Reg(_registers.H);
                    instruction = "OUT (C), H";
                    break;
                }
                case 0x62:
                {
                    ushort temp = _registers.HL;
                    cycles += Sub_Reg16_Reg16(ref temp, _registers.HL);
                    _registers.HL = temp;
                    instruction = "SBC HL, HL";
                    break;
                }
                case 0x67:
                {
                    cycles += RRD();
                    instruction = "RRD";
                    break;
                }
                case 0x68:
                {
                    cycles += In_C_Reg(ref _registers.L);
                    instruction = "IN L, (C)";
                    break;
                }
                case 0x69:
                {
                    cycles += Out_C_Reg(_registers.L);
                    instruction = "OUT (C), L";
                    break;
                }
                case 0x6A:
                {
                    ushort temp = _registers.HL;
                    cycles += Add_Reg16_Reg16(ref temp, _registers.HL, true);
                    _registers.HL = temp;
                    instruction = "ADC HL, HL";
                    break;
                }
                case 0x6F:
                {
                    cycles += RLD();
                    instruction = "RLD";
                    break;
                }
                case 0x72:
                {
                    ushort temp = _registers.HL;
                    cycles += Sub_Reg16_Reg16(ref temp, _registers.SP);
                    _registers.HL = temp;
                    instruction = "SBC HL, SP";
                    break;
                }
                case 0x73:
                {
                    cycles += Load_ImmPtr_Reg16(_registers.SP);
                    instruction = "LD (nn), SP";
                    break;
                }
                case 0x78:
                {
                    cycles += In_C_Reg(ref _registers.A);
                    instruction = "IN A, (C)";
                    break;
                }
                case 0x79:
                {
                    cycles += Out_C_Reg(_registers.A);
                    instruction = "OUT (C), A";
                    break;
                }
                case 0x7A:
                {
                    ushort temp = _registers.HL;
                    cycles += Add_Reg16_Reg16(ref temp, _registers.SP, true);
                    _registers.HL = temp;
                    instruction = "ADC HL, SP";
                    break;
                }
                case 0x7B:
                {
                    cycles += Load_Reg16Ptr_Imm(_registers.SP);
                    instruction = "LD SP, (nn)";
                    break;
                }
                case 0xA0:
                {
                    cycles += Load_Increment();
                    instruction = "LDI";
                    break;
                }
                case 0xA1:
                {
                    cycles += Compare_Increment();
                    instruction = "CPI";
                    break;
                }
                case 0xA2:
                {
                    cycles += In_Increment();
                    instruction = "INI";
                    break;
                }
                case 0xA3:
                {
                    cycles += Out_Increment();
                    instruction = "OUTI";
                    break;
                }
                case 0xA8:
                {
                    cycles += Load_Decrement();
                    instruction = "LDD";
                    break;
                }
                case 0xA9:
                {
                    cycles += Compare_Decrement();
                    instruction = "CPD";
                    break;
                }
                case 0xAA:
                {
                    cycles += In_Decrement();
                    instruction = "IND";
                    break;
                }
                case 0xAB:
                {
                    cycles += Out_Decrement();
                    instruction = "OUTD";
                    break;
                }
                case 0xB0:
                {
                    cycles += Load_Increment();
                    instruction = "LDIR";
                    if(_registers.BC != 0)
                    {
                        // HACK: Negate PC increment to stay on the same instruction
                        _registers.PC--;
                        // Takes 5 cycles to jump
                        cycles += 5;
                    }
                    break;
                }
                case 0xB1:
                {
                    cycles += Compare_Increment();
                    instruction = "CPIR";
                    if(_registers.BC != 0)
                    {
                        _registers.PC--;
                        cycles += 5;
                    }
                    break;
                }
                case 0xB2:
                {
                    cycles += In_Increment();
                    instruction = "INIR";
                    if (_registers.BC != 0)
                    {
                        _registers.PC--;
                        cycles += 5;
                    }
                    break;
                }
                case 0xB3:
                {
                    cycles += Out_Increment();
                    instruction = "OTIR";
                    if (_registers.BC != 0)
                    {
                        _registers.PC--;
                        cycles += 5;
                    }
                    break;
                }
                case 0xB8:
                {
                    cycles += Load_Decrement();
                    instruction = "LDDR";
                    if (_registers.BC != 0)
                    {
                        _registers.PC--;
                        cycles += 5;
                    }
                    break;
                }
                case 0xB9:
                {
                    cycles += Compare_Decrement();
                    instruction = "CPDR";
                    if (_registers.BC != 0)
                    {
                        _registers.PC--;
                        cycles += 5;
                    }
                    break;
                }
                case 0xBA:
                {
                    cycles += In_Decrement();
                    instruction = "INDR";
                    if (_registers.BC != 0)
                    {
                        _registers.PC--;
                        cycles += 5;
                    }
                    break;
                }
                case 0xBB:
                {
                    cycles += Out_Decrement();
                    instruction = "OTDR";
                    if (_registers.BC != 0)
                    {
                        _registers.PC--;
                        cycles += 5;
                    }
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



        private (int, string) DecodeBit()
        {
            // Took 4 cycles to read first byte of opcode
            int cycles = 4;
            string instruction = "";
            int opcode = _memoryBus.Read(_registers.PC);

            switch(opcode)
            {
                case >= 0x00 and <= 0x07:
                {
                    byte regCode = (byte)(opcode & 0b111);
                    cycles = RLC_Reg(ref GetReg(regCode));
                    instruction = $"RLC {RegCodeToString(regCode)}";
                    break;
                }

                case >= 0x08 and <= 0x0F:
                {
                    byte regCode = (byte)(opcode & 0b111);
                    cycles = RRC_Reg(ref GetReg(regCode));
                    instruction = $"RRC {RegCodeToString(regCode)}";
                    break;
                }

                case >= 0x10 and <= 0x17:
                {
                    byte regCode = (byte)(opcode & 0b111);
                    cycles = RL_Reg(ref GetReg(regCode));
                    instruction = $"RL {RegCodeToString(regCode)}";
                    break;
                }

                case >= 0x18 and <= 0x1F:
                {
                    byte regCode = (byte)(opcode & 0b111);
                    cycles = RR_Reg(ref GetReg(regCode));
                    instruction = $"RR {RegCodeToString(regCode)}";
                    break;
                }

                case >= 0x20 and <= 0x27:
                {
                    byte regCode = (byte)(opcode & 0b111);
                    cycles = SLA_Reg(ref GetReg(regCode));
                    instruction = $"SLA {RegCodeToString(regCode)}";
                    break;
                }

                case >= 0x28 and <= 0x2F:
                {
                    byte regCode = (byte)(opcode & 0b111);
                    cycles = SRA_Reg(ref GetReg(regCode));
                    instruction = $"SRA {RegCodeToString(regCode)}";
                    break;
                }

                case >= 0x30 and <= 0x37:
                {
                    byte regCode = (byte)(opcode & 0b111);
                    cycles = SLA_Reg(ref GetReg(regCode));
                    instruction = $"SLL {RegCodeToString(regCode)}";
                    break;
                }

                case >= 0x38 and <= 0x3F:
                {
                    byte regCode = (byte)(opcode & 0b111);
                    cycles = SRL_Reg(ref GetReg(regCode));
                    instruction = $"SRL {RegCodeToString(regCode)}";
                    break;
                }

                case >= 0x40 and <= 0x7F:
                {
                    byte bit = (byte)((opcode & 0b00111000) >> 3);
                    byte regCode = (byte)(opcode & 0b111);

                    if (regCode == 6) // (HL)
                    {
                        cycles = Bit_Reg16Ptr(bit, _registers.HL);
                        instruction = $"BIT {bit}, (HL)";
                    }
                    else
                    {
                        cycles = Bit_Reg(bit, ref GetReg(regCode));
                        instruction = $"BIT {bit}, {RegCodeToString(regCode)}";
                    }
                    break;
                }

                case >= 0x80 and <= 0xBF:
                {
                    byte bit = (byte)((opcode & 0b00111000) >> 3);
                    byte regCode = (byte)(opcode & 0b111);

                    if (regCode == 6) // (HL)
                    {
                        cycles = Reset_Reg16Ptr(bit, _registers.HL);
                        instruction = $"RES {bit}, (HL)";
                    }
                    else
                    {
                        cycles = Reset_Reg(bit, ref GetReg(regCode));
                        instruction = $"RES {bit}, {RegCodeToString(regCode)}";
                    }
                    break;
                }

                case >= 0xC0 and <= 0xFF:
                {
                    byte bit = (byte)((opcode & 0b00111000) >> 3);
                    byte regCode = (byte)(opcode & 0b111);

                    if (regCode == 6) // (HL)
                    {
                        cycles = Set_Reg16Ptr(bit, _registers.HL);
                        instruction = $"SET {bit}, (HL)";
                    }
                    else
                    {
                        cycles = Set_Reg(bit, ref GetReg(regCode));
                        instruction = $"SET {bit}, {RegCodeToString(regCode)}";
                    }
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

        private ref byte GetReg(byte reg)
        {
            switch (reg)
            {
                case 0:
                return ref _registers.B;
                case 1:
                return ref _registers.C;
                case 2:
                return ref _registers.D;
                case 3:
                return ref _registers.E;
                case 4:
                return ref _registers.H;
                case 5:
                return ref _registers.L;
                case 7:
                return ref _registers.A;
                default:
                throw new ArgumentOutOfRangeException();
            }
        }

        private string RegCodeToString(byte reg)
        {
            switch (reg)
            {
                case 0:
                return "B";
                case 1:
                return "C";
                case 2:
                return "D";
                case 3:
                return "E";
                case 4:
                return "H";
                case 5:
                return "L";
                case 7:
                return "A";
                default:
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
