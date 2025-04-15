namespace Ceres80Emu.Emulator
{
    // Only handles one device. No IEI/IEO chaining.
    internal class InterruptManager
    {
        public bool InterruptLine { get; set; } = false;
        public bool NMInterruptLine { get; set; } = false;
        public bool AcknowledgeLine { get; set; } = false;
        public bool NMAcknowledgeLine { get; set; } = false;
    }
}
