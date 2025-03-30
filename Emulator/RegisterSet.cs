using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    internal class RegisterSet
    {
        // General purpose registers and alternate registers
        public byte A, F, B, C, D, E, H, L;
        public byte AltA, AltF, AltB, AltC, AltD, AltE, AltH, AltL;
        // Index, stack pointer, and program counter
        public ushort IX, IY, SP, PC;
        // Interrupt vector and refresh
        public byte I, R;

        public ushort AF
        {
            get => ByteToShort(A, F);
            set => (A, F) = ShortToByte(value);
        }
        public ushort BC
        {
            get => ByteToShort(B, C);
            set => (B, C) = ShortToByte(value);
        }
        public ushort DE
        {
            get => ByteToShort(D, E);
            set => (D, E) = ShortToByte(value);
        }
        public ushort HL
        {
            get => ByteToShort(H, L);
            set => (H, L) = ShortToByte(value);
        }
        public ushort AltAF
        {
            get => ByteToShort(AltA, AltF);
            set => (AltA, AltF) = ShortToByte(value);
        }
        public ushort AltBC
        {
            get => ByteToShort(AltB, AltC);
            set => (AltB, AltC) = ShortToByte(value);
        }
        public ushort AltDE
        {
            get => ByteToShort(AltD, AltE);
            set => (AltD, AltE) = ShortToByte(value);
        }
        public ushort AltHL
        {
            get => ByteToShort(AltH, AltL);
            set => (AltH, AltL) = ShortToByte(value);
        }

        private static ushort ByteToShort(byte high, byte low)
        {
            return (ushort)((high << 8) | low);
        }

        private static (byte, byte) ShortToByte(ushort value)
        {
            return ((byte)(value >> 8), (byte)(value & 0xFF));
        }
    }
}
