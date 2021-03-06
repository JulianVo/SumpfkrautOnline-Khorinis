﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using WinApi.Kernel.Exception;

namespace WinApi.Kernel
{
    class Error
    {
        public static void GetLastError(string message = "")
        {
            int errorcode = Marshal.GetLastWin32Error();
            switch (errorcode)
            {
                case 5:
                    throw new AccessDenied();
                case 299:
                    break;
                default:
                    throw new System.Exception(errorcode + " " + message);
            }
        }
    }
}
