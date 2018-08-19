using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ShipFastTurn
{
    public class Kernel32
    {
        [DllImport("kernel32")]
        public static extern bool QueryPerformanceCounter(out long counter);

        [DllImport("kernel32")]
        public static extern bool QueryPerformanceFrequency(out long frequency);
    }
}
