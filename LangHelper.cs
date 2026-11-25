using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Keytr
{
    public static class LangHelper
    {

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowThreadProcessId(
    [In] IntPtr hWnd,
    [Out, Optional] IntPtr lpdwProcessId
);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        static extern ushort GetKeyboardLayout(
            [In] int idThread
        );



        public static ushort GetKeyboardLayout()
        {
            return GetKeyboardLayout(GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero));
        }

          public enum Languages : ushort
        {
            Russian = 1049,
            English = 1033
        }

    }
}
