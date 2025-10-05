using System.Runtime.InteropServices;

namespace Ceres80Emu.Emulator;

// Does not work on Big Endian devices.
[StructLayout(LayoutKind.Explicit)]
internal class Registers
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
    [FieldOffset(8)] public ushort AltAF;
    [FieldOffset(9)] public byte AltA;
    [FieldOffset(8)] public byte AltF;

    [FieldOffset(10)] public ushort AltBC;
    [FieldOffset(11)] public byte AltB;
    [FieldOffset(10)] public byte AltC;

    [FieldOffset(12)] public ushort AltDE;
    [FieldOffset(13)] public byte AltD;
    [FieldOffset(12)] public byte AltE;

    [FieldOffset(14)] public ushort AltHL;
    [FieldOffset(15)] public byte AltH;
    [FieldOffset(14)] public byte AltL;

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

    // Flags
    public bool Carry
    {
        get => (F & 0x01) != 0;
        set => F = (byte)((F & ~0x01) | (value ? 0x01 : 0x00));
    }

    public bool Subtract
    {
        get => (F & 0x02) != 0;
        set => F = (byte)((F & ~0x02) | (value ? 0x02 : 0x00));
    }

    public bool Parity
    {
        get => (F & 0x04) != 0;
        set => F = (byte)((F & ~0x04) | (value ? 0x04 : 0x00));
    }

    public bool HalfCarry
    {
        get => (F & 0x10) != 0;
        set => F = (byte)((F & ~0x10) | (value ? 0x10 : 0x00));
    }

    public bool Zero
    {
        get => (F & 0x40) != 0;
        set => F = (byte)((F & ~0x40) | (value ? 0x40 : 0x00));
    }

    public bool Sign
    {
        get => (F & 0x80) != 0;
        set => F = (byte)((F & ~0x80) | (value ? 0x80 : 0x00));
    }

    public enum RegisterTarget
    {
        A, F, B, C, D, E, H, L,
        AltA, AltF, AltB, AltC, AltD, AltE, AltH, AltL,
        I, R,
        AF, BC, DE, HL,
        AltAF, AltBC, AltDE, AltHL,
        IX, IY, PC, SP,
        IFF1, IFF2,
    }

    public ref byte GetByteRegisterRef(RegisterTarget target)
    {
        switch (target)
        {
        case RegisterTarget.A: return ref A;
        case RegisterTarget.F: return ref F;
        case RegisterTarget.B: return ref B;
        case RegisterTarget.C: return ref C;
        case RegisterTarget.D: return ref D;
        case RegisterTarget.E: return ref E;
        case RegisterTarget.H: return ref H;
        case RegisterTarget.L: return ref L;
        case RegisterTarget.AltA: return ref AltA;
        case RegisterTarget.AltF: return ref AltF;
        case RegisterTarget.AltB: return ref AltB;
        case RegisterTarget.AltC: return ref AltC;
        case RegisterTarget.AltD: return ref AltD;
        case RegisterTarget.AltE: return ref AltE;
        case RegisterTarget.AltH: return ref AltH;
        case RegisterTarget.AltL: return ref AltL;
        case RegisterTarget.I: return ref I;
        case RegisterTarget.R: return ref R;
        default: throw new ArgumentException($"Target {target} is not a byte register.");
        }
    }

    public ref ushort GetUShortRegisterRef(RegisterTarget target)
    {
        switch (target)
        {
        case RegisterTarget.AF: return ref AF;
        case RegisterTarget.BC: return ref BC;
        case RegisterTarget.DE: return ref DE;
        case RegisterTarget.HL: return ref HL;
        case RegisterTarget.AltAF: return ref AltAF;
        case RegisterTarget.AltBC: return ref AltBC;
        case RegisterTarget.AltDE: return ref AltDE;
        case RegisterTarget.AltHL: return ref AltHL;
        case RegisterTarget.IX: return ref IX;
        case RegisterTarget.IY: return ref IY;
        case RegisterTarget.SP: return ref SP;
        case RegisterTarget.PC: return ref PC;
        default: throw new ArgumentException($"Target {target} is not a ushort register.");
        }
    }

    public ref bool GetBoolRegisterRef(RegisterTarget target)
    {
        switch (target)
        {
        case RegisterTarget.IFF1: return ref IFF1;
        case RegisterTarget.IFF2: return ref IFF2;
        default: throw new ArgumentException($"Target {target} is not a bool register.");
        }
    }
}