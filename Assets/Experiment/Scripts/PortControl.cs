using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ParallelPortControl
{
    class PortControl
    {
        [DllImport("inpoutx64")]
        public static extern short Inp32(int address);
        [DllImport("inpoutx64", EntryPoint = "Out32")]
        public static extern void Output(int adress, int value); // decimal
    }
}
