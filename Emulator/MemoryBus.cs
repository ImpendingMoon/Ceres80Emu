using System.Net;

namespace Ceres80Emu.Emulator
{
    internal class MemoryBus
    {
        public MemoryBus(DebugManager debugManager)
        {
            _debugManager = debugManager;
        }

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
            byte value;
            if (device == null)
                value = 0xFF; // Open bus
            else 
                value = device.Device.Read((ushort)(address - device.StartAddress));

            _debugManager.AddMemoryAccess(address, value, true, accessType);

            return value;
        }

        public byte ReadPort(byte port)
        {
            var device = _portDevices.FirstOrDefault(d => d.Contains(port));
            byte value;
            if (device == null)
                value = 0xFF; // Open bus
            else
                value = device.Device.Read((byte)(port - device.StartAddress));

            _debugManager.AddPortAccess(port, value, true);

            return value;
        }

        public void WriteMemory(ushort address, byte data, MemoryAccessType accessType = MemoryAccessType.Standard)
        {
            _debugManager.AddMemoryAccess(address, data, false, accessType);

            var device = _memoryDevices.FirstOrDefault(d => d.Contains(address));
            if (device == null)
                return; // Open bus
            device.Device.Write((ushort)(address - device.StartAddress), data);
        }

        public void WritePort(byte port, byte data)
        {
            _debugManager.AddPortAccess(port, data, false);

            var device = _portDevices.FirstOrDefault(d => d.Contains(port));
            if (device == null)
                return; // Open bus
            device.Device.Write((byte)(port - device.StartAddress), data);
        }

        private List<DeviceMapping> _memoryDevices = new();
        private List<DeviceMapping> _portDevices = new();

        private DebugManager _debugManager;
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
