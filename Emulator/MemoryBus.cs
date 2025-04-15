namespace Ceres80Emu.Emulator
{
    internal class MemoryBus
    {
        public void AddMemoryDevice(IMemoryDevice device, ushort startAddress, ushort endAddress)
        {
            _memoryDevices.Add(new DeviceMapping(device, startAddress, endAddress));
        }

        public void AddPortDevice(IMemoryDevice device, ushort startAddress, ushort endAddress)
        {
            _portDevices.Add(new DeviceMapping(device, startAddress, endAddress));
        }

        public byte ReadMemory(ushort address, MemoryAccessType accessType = MemoryAccessType.Standard)
        {
            var device = _memoryDevices.FirstOrDefault(d => d.Contains(address));
            if (device == null)
                return 0xFF; // Open bus
            return device.Device.Read(address);
        }

        public byte ReadPort(byte port)
        {
            var device = _portDevices.FirstOrDefault(d => d.Contains(port));
            if (device == null)
                return 0xFF; // Open bus
            return device.Device.Read(port);
        }

        public void WriteMemory(ushort address, byte data, MemoryAccessType accessType = MemoryAccessType.Standard)
        {
            var device = _memoryDevices.FirstOrDefault(d => d.Contains(address));
            if (device == null)
                return; // Open bus
            device.Device.Write(address, data);
        }

        public void WritePort(byte port, byte data)
        {
            var device = _portDevices.FirstOrDefault(d => d.Contains(port));
            if (device == null)
                return; // Open bus
            device.Device.Write(port, data);
        }

        private List<DeviceMapping> _memoryDevices = new();
        private List<DeviceMapping> _portDevices = new();
    }



    internal enum MemoryAccessType
    {
        Standard,
        Execute,
        Immediate,
        Stack,
    }



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
