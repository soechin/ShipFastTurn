using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ShipFastTurn
{
    public class Winmm
    {
        public delegate void TIMECALLBACK(int uTimerID, int uMsg, IntPtr dwUser, IntPtr dw1, IntPtr dw2);

        [DllImport("winmm.dll")]
        public static extern int timeSetEvent(int uDelay, int uResolution, TIMECALLBACK lpTimeProc, IntPtr dwUser, int fuEvent);

        [DllImport("winmm.dll")]
        public static extern int timeKillEvent(int uTimerID);
    }
}
