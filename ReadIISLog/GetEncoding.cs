using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConvertFromIISLogFile
{
    /// <summary>
    /// http://stackoverflow.com/questions/22349139/utf-8-output-from-powershell
    /// </summary>
    public class GetEncoding
    {
        private const UInt32 CP_OEMCP = 1;

        public static Encoding GetDefaultOemCodePageEncoding()

        {
            CPINFOEX cpInfoEx;


            if (GetCPInfoEx(CP_OEMCP, 0, out cpInfoEx) == 0)

                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "GetCPInfoEx() failed with error code {0}",
                    Marshal.GetLastWin32Error()));


            return Encoding.GetEncoding((int) cpInfoEx.CodePage);
        }


        [DllImport("Kernel32.dll", EntryPoint = "GetCPInfoExW", SetLastError = true)]
        public static extern Int32 GetCPInfoEx(UInt32 CodePage, UInt32 dwFlags, out CPINFOEX lpCPInfoEx);


        private const int MAX_DEFAULTCHAR = 2;
        private const int MAX_LEADBYTES = 12;
        private const int MAX_PATH = 260;

        [StructLayout(LayoutKind.Sequential)]
        public struct CPINFOEX
        {
            [MarshalAs(UnmanagedType.U4)] public int MaxCharSize;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DEFAULTCHAR)] public byte[] DefaultChar;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_LEADBYTES)] public byte[] LeadBytes;

            public char UnicodeDefaultChar;

            [MarshalAs(UnmanagedType.U4)] public int CodePage;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)] public string CodePageName;
        }
    }
}