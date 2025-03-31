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
                case 0x40:
                {
                    cycles = Load_Reg_Reg(ref _registers.B, _registers.B);
                    instruction = "LD B, B";
                    break;
                }
                case 0x41:
                {
                    cycles = Load_Reg_Reg(ref _registers.B, _registers.C);
                    instruction = "LD B, C";
                    break;
                }
                case 0x42:
                {
                    cycles = Load_Reg_Reg(ref _registers.B, _registers.D);
                    instruction = "LD B, D";
                    break;
                }
                case 0x43:
                {
                    cycles = Load_Reg_Reg(ref _registers.B, _registers.E);
                    instruction = "LD B, E";
                    break;
                }
                case 0x44:
                {
                    cycles = Load_Reg_Reg(ref _registers.B, _registers.H);
                    instruction = "LD B, H";
                    break;
                }
                case 0x45:
                {
                    cycles = Load_Reg_Reg(ref _registers.B, _registers.L);
                    instruction = "LD B, L";
                    break;
                }
                case 0x46:
                {
                    cycles = Load_Reg_Reg16Ptr(ref _registers.B, _registers.HL);
                    instruction = "LD B, (HL)";
                    break;
                }
                case 0x47:
                {
                    cycles = Load_Reg_Reg(ref _registers.B, _registers.A);
                    instruction = "LD B, A";
                    break;
                }
                case 0x48:
                {
                    cycles = Load_Reg_Reg(ref _registers.C, _registers.B);
                    instruction = "LD C, B";
                    break;
                }
                case 0x49:
                {
                    cycles = Load_Reg_Reg(ref _registers.C, _registers.C);
                    instruction = "LD C, C";
                    break;
                }
                case 0x4A:
                {
                    cycles = Load_Reg_Reg(ref _registers.C, _registers.D);
                    instruction = "LD C, D";
                    break;
                }
                case 0x4B:
                {
                    cycles = Load_Reg_Reg(ref _registers.C, _registers.E);
                    instruction = "LD C, E";
                    break;
                }
                case 0x4C:
                {
                    cycles = Load_Reg_Reg(ref _registers.C, _registers.H);
                    instruction = "LD C, H";
                    break;
                }
                case 0x4D:
                {
                    cycles = Load_Reg_Reg(ref _registers.C, _registers.L);
                    instruction = "LD C, L";
                    break;
                }
                case 0x4E:
                {
                    cycles = Load_Reg_Reg16Ptr(ref _registers.C, _registers.HL);
                    instruction = "LD C, (HL)";
                    break;
                }
                case 0x4F:
                {
                    cycles = Load_Reg_Reg(ref _registers.C, _registers.A);
                    instruction = "LD C, A";
                    break;
                }
                case 0x50:
                {
                    cycles = Load_Reg_Reg(ref _registers.D, _registers.B);
                    instruction = "LD D, B";
                    break;
                }
                case 0x51:
                {
                    cycles = Load_Reg_Reg(ref _registers.D, _registers.C);
                    instruction = "LD D, C";
                    break;
                }
                case 0x52:
                {
                    cycles = Load_Reg_Reg(ref _registers.D, _registers.D);
                    instruction = "LD D, D";
                    break;
                }
                case 0x53:
                {
                    cycles = Load_Reg_Reg(ref _registers.D, _registers.E);
                    instruction = "LD D, E";
                    break;
                }
                case 0x54:
                {
                    cycles = Load_Reg_Reg(ref _registers.D, _registers.H);
                    instruction = "LD D, H";
                    break;
                }
                case 0x55:
                {
                    cycles = Load_Reg_Reg(ref _registers.D, _registers.L);
                    instruction = "LD D, L";
                    break;
                }
                case 0x56:
                {
                    cycles = Load_Reg_Reg16Ptr(ref _registers.D, _registers.HL);
                    instruction = "LD D, (HL)";
                    break;
                }
                case 0x57:
                {
                    cycles = Load_Reg_Reg(ref _registers.D, _registers.A);
                    instruction = "LD D, A";
                    break;
                }
                case 0x58:
                {
                    cycles = Load_Reg_Reg(ref _registers.E, _registers.B);
                    instruction = "LD E, B";
                    break;
                }
                case 0x59:
                {
                    cycles = Load_Reg_Reg(ref _registers.E, _registers.C);
                    instruction = "LD E, C";
                    break;
                }
                case 0x5A:
                {
                    cycles = Load_Reg_Reg(ref _registers.E, _registers.D);
                    instruction = "LD E, D";
                    break;
                }
                case 0x5B:
                {
                    cycles = Load_Reg_Reg(ref _registers.E, _registers.E);
                    instruction = "LD E, E";
                    break;
                }
                case 0x5C:
                {
                    cycles = Load_Reg_Reg(ref _registers.E, _registers.H);
                    instruction = "LD E, H";
                    break;
                }
                case 0x5D:
                {
                    cycles = Load_Reg_Reg(ref _registers.E, _registers.L);
                    instruction = "LD E, L";
                    break;
                }
                case 0x5E:
                {
                    cycles = Load_Reg_Reg16Ptr(ref _registers.E, _registers.HL);
                    instruction = "LD E, (HL)";
                    break;
                }
                case 0x5F:
                {
                    cycles = Load_Reg_Reg(ref _registers.E, _registers.A);
                    instruction = "LD E, A";
                    break;
                }
                case 0x60:
                {
                    cycles = Load_Reg_Reg(ref _registers.H, _registers.B);
                    instruction = "LD H, B";
                    break;
                }
                case 0x61:
                {
                    cycles = Load_Reg_Reg(ref _registers.H, _registers.C);
                    instruction = "LD H, C";
                    break;
                }
                case 0x62:
                {
                    cycles = Load_Reg_Reg(ref _registers.H, _registers.D);
                    instruction = "LD H, D";
                    break;
                }
                case 0x63:
                {
                    cycles = Load_Reg_Reg(ref _registers.H, _registers.E);
                    instruction = "LD H, E";
                    break;
                }
                case 0x64:
                {
                    cycles = Load_Reg_Reg(ref _registers.H, _registers.H);
                    instruction = "LD H, H";
                    break;
                }
                case 0x65:
                {
                    cycles = Load_Reg_Reg(ref _registers.H, _registers.L);
                    instruction = "LD H, L";
                    break;
                }
                case 0x66:
                {
                    cycles = Load_Reg_Reg16Ptr(ref _registers.H, _registers.HL);
                    instruction = "LD H, (HL)";
                    break;
                }
                case 0x67:
                {
                    cycles = Load_Reg_Reg(ref _registers.H, _registers.A);
                    instruction = "LD H, A";
                    break;
                }
                case 0x68:
                {
                    cycles = Load_Reg_Reg(ref _registers.L, _registers.B);
                    instruction = "LD L, B";
                    break;
                }
                case 0x69:
                {
                    cycles = Load_Reg_Reg(ref _registers.L, _registers.C);
                    instruction = "LD L, C";
                    break;
                }
                case 0x6A:
                {
                    cycles = Load_Reg_Reg(ref _registers.L, _registers.D);
                    instruction = "LD L, D";
                    break;
                }
                case 0x6B:
                {
                    cycles = Load_Reg_Reg(ref _registers.L, _registers.E);
                    instruction = "LD L, E";
                    break;
                }
                case 0x6C:
                {
                    cycles = Load_Reg_Reg(ref _registers.L, _registers.H);
                    instruction = "LD L, H";
                    break;
                }
                case 0x6D:
                {
                    cycles = Load_Reg_Reg(ref _registers.L, _registers.L);
                    instruction = "LD L, L";
                    break;
                }
                case 0x6E:
                {
                    cycles = Load_Reg_Reg16Ptr(ref _registers.L, _registers.HL);
                    instruction = "LD L, (HL)";
                    break;
                }
                case 0x6F:
                {
                    cycles = Load_Reg_Reg(ref _registers.L, _registers.A);
                    instruction = "LD L, A";
                    break;
                }
                case 0x70:
                {
                    cycles = Load_Reg16Ptr_Reg(_registers.HL, _registers.B);
                    instruction = "LD (HL), B";
                    break;
                }
                case 0x71:
                {
                    cycles = Load_Reg16Ptr_Reg(_registers.HL, _registers.C);
                    instruction = "LD (HL), C";
                    break;
                }
                case 0x72:
                {
                    cycles = Load_Reg16Ptr_Reg(_registers.HL, _registers.D);
                    instruction = "LD (HL), D";
                    break;
                }
                case 0x73:
                {
                    cycles = Load_Reg16Ptr_Reg(_registers.HL, _registers.E);
                    instruction = "LD (HL), E";
                    break;
                }
                case 0x74:
                {
                    cycles = Load_Reg16Ptr_Reg(_registers.HL, _registers.H);
                    instruction = "LD (HL), H";
                    break;
                }
                case 0x75:
                {
                    cycles = Load_Reg16Ptr_Reg(_registers.HL, _registers.L);
                    instruction = "LD (HL), L";
                    break;
                }
                case 0x76:
                {
                    cycles = Halt();
                    instruction = "HALT";
                    break;
                }
                case 0x77:
                {
                    cycles = Load_Reg16Ptr_Reg(_registers.HL, _registers.A);
                    instruction = "LD (HL), A";
                    break;
                }
                case 0x78:
                {
                    cycles = Load_Reg_Reg(ref _registers.A, _registers.B);
                    instruction = "LD A, B";
                    break;
                }
                case 0x79:
                {
                    cycles = Load_Reg_Reg(ref _registers.A, _registers.C);
                    instruction = "LD A, C";
                    break;
                }
                case 0x7A:
                {
                    cycles = Load_Reg_Reg(ref _registers.A, _registers.D);
                    instruction = "LD A, D";
                    break;
                }
                case 0x7B:
                {
                    cycles = Load_Reg_Reg(ref _registers.A, _registers.E);
                    instruction = "LD A, E";
                    break;
                }
                case 0x7C:
                {
                    cycles = Load_Reg_Reg(ref _registers.A, _registers.H);
                    instruction = "LD A, H";
                    break;
                }
                case 0x7D:
                {
                    cycles = Load_Reg_Reg(ref _registers.A, _registers.L);
                    instruction = "LD A, L";
                    break;
                }
                case 0x7E:
                {
                    cycles = Load_Reg_Reg16Ptr(ref _registers.A, _registers.HL);
                    instruction = "LD A, (HL)";
                    break;
                }
                case 0x7F:
                {
                    cycles = Load_Reg_Reg(ref _registers.A, _registers.A);
                    instruction = "LD A, A";
                    break;
                }
                case 0x80:
                {
                    cycles = Add_Reg_Reg(ref _registers.A, _registers.B);
                    instruction = "ADD A, B";
                    break;
                }
                case 0x81:
                {
                    cycles = Add_Reg_Reg(ref _registers.A, _registers.C);
                    instruction = "ADD A, C";
                    break;
                }
                case 0x82:
                {
                    cycles = Add_Reg_Reg(ref _registers.A, _registers.D);
                    instruction = "ADD A, D";
                    break;
                }
                case 0x83:
                {
                    cycles = Add_Reg_Reg(ref _registers.A, _registers.E);
                    instruction = "ADD A, E";
                    break;
                }
                case 0x84:
                {
                    cycles = Add_Reg_Reg(ref _registers.A, _registers.H);
                    instruction = "ADD A, H";
                    break;
                }
                case 0x85:
                {
                    cycles = Add_Reg_Reg(ref _registers.A, _registers.L);
                    instruction = "ADD A, L";
                    break;
                }
                case 0x86:
                {
                    cycles = Add_Reg_Reg16Ptr(ref _registers.A, _registers.HL);
                    instruction = "ADD A, (HL)";
                    break;
                }
                case 0x87:
                {
                    cycles = Add_Reg_Reg(ref _registers.A, _registers.A);
                    instruction = "ADD A, A";
                    break;
                }
                case 0x88:
                {
                    cycles = Add_Reg_Reg(ref _registers.A, _registers.B, true);
                    instruction = "ADC A, B";
                    break;
                }
                case 0x89:
                {
                    cycles = Add_Reg_Reg(ref _registers.A, _registers.C, true);
                    instruction = "ADC A, C";
                    break;
                }
                case 0x8A:
                {
                    cycles = Add_Reg_Reg(ref _registers.A, _registers.D, true);
                    instruction = "ADC A, D";
                    break;
                }
                case 0x8B:
                {
                    cycles = Add_Reg_Reg(ref _registers.A, _registers.E, true);
                    instruction = "ADC A, E";
                    break;
                }
                case 0x8C:
                {
                    cycles = Add_Reg_Reg(ref _registers.A, _registers.H, true);
                    instruction = "ADC A, H";
                    break;
                }
                case 0x8D:
                {
                    cycles = Add_Reg_Reg(ref _registers.A, _registers.L, true);
                    instruction = "ADC A, L";
                    break;
                }
                case 0x8E:
                {
                    cycles = Add_Reg_Reg16Ptr(ref _registers.A, _registers.HL, true);
                    instruction = "ADC A, (HL)";
                    break;
                }
                case 0x8F:
                {
                    cycles = Add_Reg_Reg(ref _registers.A, _registers.A, true);
                    instruction = "ADC A, A";
                    break;
                }
                case 0x90:
                {
                    cycles = Sub_Reg_Reg(ref _registers.A, _registers.B);
                    instruction = "SUB A, B";
                    break;
                }
                case 0x91:
                {
                    cycles = Sub_Reg_Reg(ref _registers.A, _registers.C);
                    instruction = "SUB A, C";
                    break;
                }
                case 0x92:
                {
                    cycles = Sub_Reg_Reg(ref _registers.A, _registers.D);
                    instruction = "SUB A, D";
                    break;
                }
                case 0x93:
                {
                    cycles = Sub_Reg_Reg(ref _registers.A, _registers.E);
                    instruction = "SUB A, E";
                    break;
                }
                case 0x94:
                {
                    cycles = Sub_Reg_Reg(ref _registers.A, _registers.H);
                    instruction = "SUB A, H";
                    break;
                }
                case 0x95:
                {
                    cycles = Sub_Reg_Reg(ref _registers.A, _registers.L);
                    instruction = "SUB A, L";
                    break;
                }
                case 0x96:
                {
                    cycles = Sub_Reg_Reg16Ptr(ref _registers.A, _registers.HL);
                    instruction = "SUB A, (HL)";
                    break;
                }
                case 0x97:
                {
                    cycles = Sub_Reg_Reg(ref _registers.A, _registers.A);
                    instruction = "SUB A, A";
                    break;
                }
                case 0x98:
                {
                    cycles = Sub_Reg_Reg(ref _registers.A, _registers.B, true);
                    instruction = "SBC A, B";
                    break;
                }
                case 0x99:
                {
                    cycles = Sub_Reg_Reg(ref _registers.A, _registers.C, true);
                    instruction = "SBC A, C";
                    break;
                }
                case 0x9A:
                {
                    cycles = Sub_Reg_Reg(ref _registers.A, _registers.D, true);
                    instruction = "SBC A, D";
                    break;
                }
                case 0x9B:
                {
                    cycles = Sub_Reg_Reg(ref _registers.A, _registers.E, true);
                    instruction = "SBC A, E";
                    break;
                }
                case 0x9C:
                {
                    cycles = Sub_Reg_Reg(ref _registers.A, _registers.H, true);
                    instruction = "SBC A, H";
                    break;
                }
                case 0x9D:
                {
                    cycles = Sub_Reg_Reg(ref _registers.A, _registers.L, true);
                    instruction = "SBC A, L";
                    break;
                }
                case 0x9E:
                {
                    cycles = Sub_Reg_Reg16Ptr(ref _registers.A, _registers.HL, true);
                    instruction = "SBC A, (HL)";
                    break;
                }
                case 0x9F:
                {
                    cycles = Sub_Reg_Reg(ref _registers.A, _registers.A, true);
                    instruction = "SBC A, A";
                    break;
                }
                case 0xA0:
                {
                    cycles = And_Reg_Reg(ref _registers.A, _registers.B);
                    instruction = "AND A, B";
                    break;
                }
                case 0xA1:
                {
                    cycles = And_Reg_Reg(ref _registers.A, _registers.C);
                    instruction = "AND A, C";
                    break;
                }
                case 0xA2:
                {
                    cycles = And_Reg_Reg(ref _registers.A, _registers.D);
                    instruction = "AND A, D";
                    break;
                }
                case 0xA3:
                {
                    cycles = And_Reg_Reg(ref _registers.A, _registers.E);
                    instruction = "AND A, E";
                    break;
                }
                case 0xA4:
                {
                    cycles = And_Reg_Reg(ref _registers.A, _registers.H);
                    instruction = "AND A, H";
                    break;
                }
                case 0xA5:
                {
                    cycles = And_Reg_Reg(ref _registers.A, _registers.L);
                    instruction = "AND A, L";
                    break;
                }
                case 0xA6:
                {
                    cycles = And_Reg_Reg16Ptr(ref _registers.A, _registers.HL);
                    instruction = "AND A, (HL)";
                    break;
                }
                case 0xA7:
                {
                    cycles = And_Reg_Reg(ref _registers.A, _registers.A);
                    instruction = "AND A, A";
                    break;
                }
                case 0xA8:
                {
                    cycles = Xor_Reg_Reg(ref _registers.A, _registers.B);
                    instruction = "XOR A, B";
                    break;
                }
                case 0xA9:
                {
                    cycles = Xor_Reg_Reg(ref _registers.A, _registers.C);
                    instruction = "XOR A, C";
                    break;
                }
                case 0xAA:
                {
                    cycles = Xor_Reg_Reg(ref _registers.A, _registers.D);
                    instruction = "XOR A, D";
                    break;
                }
                case 0xAB:
                {
                    cycles = Xor_Reg_Reg(ref _registers.A, _registers.E);
                    instruction = "XOR A, E";
                    break;
                }
                case 0xAC:
                {
                    cycles = Xor_Reg_Reg(ref _registers.A, _registers.H);
                    instruction = "XOR A, H";
                    break;
                }
                case 0xAD:
                {
                    cycles = Xor_Reg_Reg(ref _registers.A, _registers.L);
                    instruction = "XOR A, L";
                    break;
                }
                case 0xAE:
                {
                    cycles = Xor_Reg_Reg16Ptr(ref _registers.A, _registers.HL);
                    instruction = "XOR A, (HL)";
                    break;
                }
                case 0xAF:
                {
                    cycles = Xor_Reg_Reg(ref _registers.A, _registers.A);
                    instruction = "XOR A, A";
                    break;
                }
                case 0xB0:
                {
                    cycles = Or_Reg_Reg(ref _registers.A, _registers.B);
                    instruction = "OR A, B";
                    break;
                }
                case 0xB1:
                {
                    cycles = Or_Reg_Reg(ref _registers.A, _registers.C);
                    instruction = "OR A, C";
                    break;
                }
                case 0xB2:
                {
                    cycles = Or_Reg_Reg(ref _registers.A, _registers.D);
                    instruction = "OR A, D";
                    break;
                }
                case 0xB3:
                {
                    cycles = Or_Reg_Reg(ref _registers.A, _registers.E);
                    instruction = "OR A, E";
                    break;
                }
                case 0xB4:
                {
                    cycles = Or_Reg_Reg(ref _registers.A, _registers.H);
                    instruction = "OR A, H";
                    break;
                }
                case 0xB5:
                {
                    cycles = Or_Reg_Reg(ref _registers.A, _registers.L);
                    instruction = "OR A, L";
                    break;
                }
                case 0xB6:
                {
                    cycles = Or_Reg_Reg16Ptr(ref _registers.A, _registers.HL);
                    instruction = "OR A, (HL)";
                    break;
                }
                case 0xB7:
                {
                    cycles = Or_Reg_Reg(ref _registers.A, _registers.A);
                    instruction = "OR A, A";
                    break;
                }
                case 0xB8:
                {
                    cycles = Compare_Reg_Reg(_registers.A, _registers.B);
                    instruction = "CP A, B";
                    break;
                }
                case 0xB9:
                {
                    cycles = Compare_Reg_Reg(_registers.A, _registers.C);
                    instruction = "CP A, C";
                    break;
                }
                case 0xBA:
                {
                    cycles = Compare_Reg_Reg(_registers.A, _registers.D);
                    instruction = "CP A, D";
                    break;
                }
                case 0xBB:
                {
                    cycles = Compare_Reg_Reg(_registers.A, _registers.E);
                    instruction = "CP A, E";
                    break;
                }
                case 0xBC:
                {
                    cycles = Compare_Reg_Reg(_registers.A, _registers.H);
                    instruction = "CP A, H";
                    break;
                }
                case 0xBD:
                {
                    cycles = Compare_Reg_Reg(_registers.A, _registers.L);
                    instruction = "CP A, L";
                    break;
                }
                case 0xBE:
                {
                    cycles = Compare_Reg_Reg16Ptr(_registers.A, _registers.HL);
                    instruction = "CP A, (HL)";
                    break;
                }
                case 0xBF:
                {
                    cycles = Compare_Reg_Reg(_registers.A, _registers.A);
                    instruction = "CP A, A";
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
