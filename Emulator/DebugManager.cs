using System.Text;

namespace Ceres80Emu.Emulator
{
    internal class DebugManager
    {
        public DebugManager() { }

        public void StartInstruction()
        {
            _accesses.Clear();
            _instruction = String.Empty;
            _instructionAddress = 0;
            _cycles = 0;
        }

        public void StopInstruction()
        {
            // List the opcode bytes + address + mnemonic (sub in immediate bytes) + cycles
            // List the other reads/writes in order
            // List interrupts

            StringBuilder sb = new StringBuilder();

            List<byte> opcodeBytes = new List<byte>();
            List<byte> immediateBytes = new List<byte>();
            List<AccessEvent> otherAccesses = new List<AccessEvent>();

            foreach(var access in _accesses)
            {
                if(access.AccessType == MemoryAccessType.Execute)
                {
                    opcodeBytes.Add(access.Value);
                }
                else if (access.AccessType == MemoryAccessType.Immediate)
                {
                    immediateBytes.Add(access.Value);
                }
                else
                {
                    otherAccesses.Add(access);
                }
            }

            // "Executed 0x<opcode> <mnemonic> <immediate bytes> @ 0x<address>\n"
            sb.Append("Executed 0x");

            foreach(var b in opcodeBytes)
            {
                sb.AppendFormat("{0:X2} ", b);
            }

            // Fill in mnemonic placeholders with actual data
            sb.Append(FillInInstruction(_instruction, immediateBytes));
            sb.AppendFormat(" @ 0x{0:X4}\n", _instructionAddress);

            // List the other accesses under this instruction
            foreach(var access in otherAccesses)
            {
                // Example:
                // "    Read Value 0xA1 @ 0xF800"
                sb.Append("\t");
                sb.Append(access.Read ? "Read Value " : "Wrote Value ");
                sb.AppendFormat("0x{0:X2} ", access.Value);

                if(access.Port)
                {
                    sb.AppendFormat("@ Port 0x{0:X2}", access.Address);
                }
                else
                {
                    sb.AppendFormat("@ Address 0x{0:X4}", access.Address);
                }

                sb.Append("\n");
            }

            if(_interruptRaised)
            {
                sb.Append("\tInterrupt was raised.\n");
            }

            if(_interruptAcknowledged)
            {
                sb.Append("\tInterrupt was acknowledged.\n");
            }

            Console.WriteLine(sb);
        }

        public void AddMemoryAccess(int address, byte value, bool read, MemoryAccessType accessType)
        {
            var accessEvent = new AccessEvent
            {
                Address = address,
                Value = value,
                Read = read,
                Port = false,
                AccessType = accessType
            };
            _accesses.Add(accessEvent);
        }

        public void AddPortAccess(byte port, byte value, bool read)
        {
            var accessEvent = new AccessEvent
            {
                Address = port,
                Value = value,
                Read = read,
                Port = true,
                AccessType = MemoryAccessType.Standard
            };
            _accesses.Add(accessEvent);
        }

        public void AddInstruction(string instruction, ushort address, int cycles)
        {
            _instruction = instruction;
            _instructionAddress = address;
            _cycles = cycles;
        }

        public void AddInterrupt(bool raised)
        {
            _interruptRaised = raised;
            _interruptAcknowledged = !raised;
        }

        private string FillInInstruction(string mnemonic, List<byte> immediateBytes)
        {
            if (string.IsNullOrEmpty(mnemonic) || immediateBytes.Count == 0)
                return mnemonic;

            StringBuilder result = new StringBuilder();
            int i = 0; // Index into immediateBytes

            for (int j = 0; j < mnemonic.Length; j++)
            {
                // Signed values 'd' are first
                // Then 'nn' for 16-bit immediate values
                // Then 'n' for 8-bit immediate values

                if (mnemonic[j] == 'd')
                {
                    if (i >= immediateBytes.Count)
                        throw new ArgumentOutOfRangeException("Not enough immediate bytes for mnemonic.");

                    sbyte signedValue = (sbyte)immediateBytes[i];
                    // Signed hexadecimal looks weird so we don't do it.
                    result.AppendFormat("{0}", signedValue);
                    i++;
                }
                else if (j < mnemonic.Length - 1 && mnemonic.Substring(j, 2) == "nn")
                {
                    if (i + 1 >= immediateBytes.Count)
                        throw new ArgumentOutOfRangeException("Not enough immediate bytes for mnemonic.");

                    ushort nn = (ushort)((immediateBytes[i] << 8) | immediateBytes[i + 1]);
                    result.AppendFormat("0x{0:X4}", nn);
                    i += 2;
                }
                else if (mnemonic[j] == 'n')
                {
                    if (i >= immediateBytes.Count)
                        throw new ArgumentOutOfRangeException("Not enough immediate bytes for mnemonic.");

                    result.AppendFormat("0x{0:X2}", immediateBytes[i]);
                    i++;
                }
                else
                {
                    result.Append(mnemonic[j]);
                }
            }

            return result.ToString();
        }

        private List<AccessEvent> _accesses = new();

        private String _instruction = String.Empty;
        private int _instructionAddress = 0;
        private int _cycles = 0;
        private bool _interruptRaised = false;
        private bool _interruptAcknowledged = false;

        private struct AccessEvent
        {
            public int Address;
            public byte Value;
            public bool Read;
            public bool Port;
            public MemoryAccessType AccessType;
        }
    }
}
