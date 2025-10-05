namespace Ceres80Emu.Emulator;

public struct DecodedInstruction
{
    public ushort Address;
    public int Size;
    public ushort? WillReadFromMemory;
    public ushort? WillWriteToMemory;
    public byte? WillReadFromPort;
    public byte? WillWriteToPort;
    public string Disassembled;
    public delegate int Execute();
}
