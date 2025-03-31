using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    internal interface IMemoryDevice
    {
        /// <summary>
        /// Size of the device in bytes
        /// </summary>
        public ushort Size { get; }

        /// <summary>
        /// Can the CPU write to this device?
        /// </summary>
        public bool CanWrite { get; }

        /// <summary>
        /// Can the CPU read from this device?
        /// </summary>
        public bool CanRead { get; }

        /// <summary>
        /// Reads a byte from the device
        /// </summary>
        /// <param name="address">Device address to read from</param>
        /// <returns>Byte at address</returns>
        public byte Read(ushort address);

        /// <summary>
        /// Writes a byte to the device
        /// </summary>
        /// <param name="address">Device address to write to</param>
        /// <param name="data">Value to write</param>
        public void Write(ushort address, byte data);

        /// <summary>
        /// Loads a device state from a byte array
        /// </summary>
        /// <param name="data">Data to write</param>
        public void LoadState(byte[] data);

        /// <summary>
        /// Saves the device state to a byte array
        /// </summary>
        /// <returns>The device state as a byte array</returns>
        public byte[] SaveState();
    }
}
