﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using WinApi.User.Enumeration;

namespace WinApi.User
{
    public class Input
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(VirtualKeys vKey);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(long hWnd, int msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)] // used for button-down & button-up
        private static extern int PostMessage(int hWnd, int msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int ToUnicode(uint virtualKeyCode, uint scanCode,
            byte[] keyboardState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
    StringBuilder receivingBuffer,
            int bufferSize, uint flags);



        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, StringBuilder lParam);
        //If you use '[Out] StringBuilder', initialize the string builder with proper length first.

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPStr)] string lParam);
        //Also can add 'ref' or 'out' ahead of 'String lParam', don't use CharSet.Auto since we use MarshalAs(..) in this
        //example.

        //Works for unicode. One can also use CharSet = CharSet.Unicode instead of [MarshalAs(UnmanagedType.LPWStr)]
        [DllImport("user32.dll", EntryPoint = "SendMessageW")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);


        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, ref POINT lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);



        
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern int ShowCursor(bool bShow);
    }
}
