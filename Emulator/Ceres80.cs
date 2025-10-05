namespace Ceres80Emu.Emulator;

public class Ceres80
{
    public DecodedInstruction DecodeInstruction(ushort address)
    {
        return new();
    }

    public void Reset()
    {

    }

    public void Load(Span<byte> data, ushort address)
    {

    }

    public byte PeekMemory(ushort address)
    {
        return 0;
    }

    public void PokeMemory(ushort address, byte value)
    {

    }

    public byte PeekPort(byte address)
    {
        return 0;
    }

    public void PokePort(byte address, byte value)
    {

    }

    public ushort PeekRegister(Registers.RegisterTarget register)
    {
        return 0;
    }

    public void PokeRegister(Registers.RegisterTarget register)
    {

    }
}
