using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Common.Windows
{
    /// <summary>
    /// Contains helpers methods for Windows console
    /// </summary>
    public static class ConsoleHelper
    {
        #region Extern methods

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", EntryPoint = "FreeConsole", SetLastError = true, CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        [return:MarshalAs(UnmanagedType.I1)]
        private static extern bool FreeConsole();

        #endregion

        #region Methods

        /// <summary>
        /// Opens a console for the current process
        /// </summary>
        public static void OpenConsole()
        {            
            bool result = AllocConsole();
            if (!result)
            {
                Win32Exception ex = new Win32Exception(Marshal.GetLastWin32Error());
                throw new Exception(string.Format("Failed to open console: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Closes the console associate to the current process
        /// </summary>
        public static void CloseConsole()
        {
            bool result = FreeConsole();
            if (!result)
            {
                Win32Exception ex = new Win32Exception(Marshal.GetLastWin32Error());
                throw new Exception(string.Format("Failed to close console: {0}", ex.Message));
            }
        }

        #endregion
    }
}
