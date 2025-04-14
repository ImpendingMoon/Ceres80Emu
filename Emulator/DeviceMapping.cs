namespace Ceres80Emu.Emulator
{
    internal class DeviceMapping
    {
        public IMemoryDevice Device { get; }
        public ushort StartAddress { get; }
        public ushort EndAddress { get; }

        public DeviceMapping(IMemoryDevice device, ushort startAddress, ushort endAddress)
        {
            Device = device;
            StartAddress = startAddress;
            EndAddress = endAddress;
        }

        public bool Contains(ushort address) => address >= StartAddress && address <= EndAddress;
    }
}
