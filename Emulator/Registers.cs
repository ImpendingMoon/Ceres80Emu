using System.Runtime.InteropServices;

namespace Ceres80Emu.Emulator
{
    // Does not work on Big Endian devices.
    [StructLayout(LayoutKind.Explicit)]
    internal struct Registers
    {
        // Primary Registers
        [FieldOffset(0)] public ushort AF;
        [FieldOffset(1)] public byte A;
        [FieldOffset(0)] public byte F;

        [FieldOffset(2)] public ushort BC;
        [FieldOffset(3)] public byte B;
        [FieldOffset(2)] public byte C;

        [FieldOffset(4)] public ushort DE;
        [FieldOffset(5)] public byte D;
        [FieldOffset(4)] public byte E;

        [FieldOffset(6)] public ushort HL;
        [FieldOffset(7)] public byte H;
        [FieldOffset(6)] public byte L;

        // Alternate Registers
        [FieldOffset(8)] public ushort AF_alt;
        [FieldOffset(9)] public byte A_alt;
        [FieldOffset(8)] public byte F_alt;

        [FieldOffset(10)] public ushort BC_alt;
        [FieldOffset(11)] public byte B_alt;
        [FieldOffset(10)] public byte C_alt;

        [FieldOffset(12)] public ushort DE_alt;
        [FieldOffset(13)] public byte D_alt;
        [FieldOffset(12)] public byte E_alt;

        [FieldOffset(14)] public ushort HL_alt;
        [FieldOffset(15)] public byte H_alt;
        [FieldOffset(14)] public byte L_alt;

        // Other 16-bit Registers
        [FieldOffset(16)] public ushort IX;
        [FieldOffset(18)] public ushort IY;
        [FieldOffset(20)] public ushort SP;
        [FieldOffset(22)] public ushort PC;

        // Special Registers
        [FieldOffset(24)] public byte I;
        [FieldOffset(25)] public byte R;
        [FieldOffset(26)] public bool IFF1;
        [FieldOffset(27)] public bool IFF2;

        // Flag Properties
        public bool Carry
        {
            get => (F & (1 << 0)) == 1;
            set => F = (byte)((F & ~(1 << 0)) | (Convert.ToByte(value) << 0));
        }

        public bool Subtract
        {
            get => (F & (1 << 1)) == 1;
            set => F = (byte)((F & ~(1 << 1)) | (Convert.ToByte(value) << 1));
        }

        public bool Parity
        {
            get => (F & (1 << 2)) == 1;
            set => F = (byte)((F & ~(1 << 2)) | (Convert.ToByte(value) << 2));
        }

        public bool HalfCarry
        {
            get => (F & (1 << 4)) == 1;
            set => F = (byte)((F & ~(1 << 4)) | (Convert.ToByte(value) << 4));
        }

        public bool Zero
        {
            get => (F & (1 << 6)) == 1;
            set => F = (byte)((F & ~(1 << 6)) | (Convert.ToByte(value) << 6));
        }

        public bool Sign
        {
            get => (F & (1 << 7)) == 1;
            set => F = (byte)((F & ~(1 << 7)) | (Convert.ToByte(value) << 7));
        }
    }
}
