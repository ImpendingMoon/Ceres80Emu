namespace Ceres80Emu.Emulator
{
    internal class MemoryBus
    {
        public void AddDevice(IMemoryDevice device, ushort startAddress, ushort endAddress, bool ioDevice)
        {
            if(ioDevice)
                _ioDevices.Add(new DeviceMapping(device, startAddress, endAddress));
            else
                _memoryDevices.Add(new DeviceMapping(device, startAddress, endAddress));
        }

        public void RemoveDevice(IMemoryDevice device)
        {
            _memoryDevices.RemoveAll(d => d.Device == device);
            _ioDevices.RemoveAll(d => d.Device == device);
        }

        public byte Read(ushort address, bool atPort = false, bool execute = false)
        {
            var list = atPort ? _ioDevices : _memoryDevices;
            var device = list.FirstOrDefault(d => d.Contains(address));
            if (device == null)
                return 0x00; // Open bus

            return device.Device.Read(address);
        }

        public void Write(ushort address, byte data, bool atPort = false)
        {
            var list = atPort ? _ioDevices : _memoryDevices;
            var device = list.FirstOrDefault(d => d.Contains(address));
            if (device == null)
                return;
            device.Device.Write(address, data);
        }

        private List<DeviceMapping> _memoryDevices = new();
        private List<DeviceMapping> _ioDevices = new();
    }
}
