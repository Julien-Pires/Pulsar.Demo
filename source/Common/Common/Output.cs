#if WINDOWS
using System;
#endif

#if XBOX || XBOX360
using System.Diagnostics;
#endif

namespace Common
{
    public static class Output
    {
        #region Static methods

        public static void WriteLine(string message, params object[] args)
        {
#if WINDOWS
            Console.WriteLine(message, args);
#elif XBOX || XBOX360
            Debug.WriteLine(message);
#endif
        }

        #endregion
    }
}
