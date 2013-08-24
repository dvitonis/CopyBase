using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CopyBase.DataTier
{
    public static class WindowWatcher
    {
            [DllImport("user32.dll")]
            static extern IntPtr GetForegroundWindow();

            public static IntPtr LastHandle
            {
                get
                {
                    return previousHandle;
                }
            }

            public static void setLastActive()
            {
                previousHandle = GetForegroundWindow();
            }

            private static IntPtr previousHandle = IntPtr.Zero;
        }
}
