using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceres80Emu.Emulator
{
    // Very simple implementation. Works for the single interrupt device of the Ceres80.
    internal class InterruptManager
    {
        public void RaiseInterrupt()
        {
            _interruptLine = true;
            _acknowledgeLine = false;
        }

        public void AcknowledgeInterrupt()
        {
            _acknowledgeLine = true;
            _interruptLine = false;
        }

        public bool IsInterruptPending()
        {
            return _interruptLine;
        }

        public bool IsAcknowledgePending()
        {
            return _acknowledgeLine;
        }

        private bool _interruptLine = false;
        private bool _acknowledgeLine = false;
    }
}
