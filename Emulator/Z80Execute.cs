using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    // Handles execution of Z80 instructions
    // All methods return the number of cycles taken
    internal partial class  Z80
    {
        public int Nop()
        {
            return 4;
        }

        
    }
}
