using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ShipFastTurn
{
    public class User32
    {
        #region Native Structs
        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public int type;
            public INPUTUNION union;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUTUNION
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }
        #endregion

        #region Native Methods
        [DllImport("user32.dll")]
        private static extern int SendInput(int nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
        #endregion

        public static int SendInput(MOUSEINPUT mi)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = 0/*INPUT_MOUSE*/;
            inputs[0].union.mi = mi;
            return SendInput(1, inputs, Marshal.SizeOf(inputs[0]));
        }

        public static int SendInput(KEYBDINPUT ki)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = 1/*INPUT_KEYBOARD*/;
            inputs[0].union.ki = ki;
            return SendInput(1, inputs, Marshal.SizeOf(inputs[0]));
        }
    }
}
