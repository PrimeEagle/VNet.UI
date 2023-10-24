namespace VNet.UI
{
    using System;
    using System.Runtime.InteropServices;

    public static class WindowsSystemFontInfo
    {
        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetStockObject(int fnObject);


        // These values are from the Windows API documentation for the LOGFONT structure and DeviceCaps.
        private const int LOGPIXELSY = 90;
        private const int DEFAULT_GUI_FONT = 17;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName; // This specifies the name of the font face.
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetObject(IntPtr hgdiobj, int cbBuffer, ref LOGFONT lpvObject);

        public static (string fontName, double fontSize) GetSystemFontInfo()
        {
            IntPtr hDC = IntPtr.Zero;
            try
            {
                hDC = GetDC(IntPtr.Zero);
                int logpixelsy = GetDeviceCaps(hDC, LOGPIXELSY);

                LOGFONT logFont = new LOGFONT();
                IntPtr hFont = GetStockObject(DEFAULT_GUI_FONT);

                if (GetObject(hFont, Marshal.SizeOf(logFont), ref logFont))
                {
                    // Convert the lfHeight, which is in logical units, to points
                    double points = logFont.lfHeight * 72.0 / logpixelsy;

                    return (logFont.lfFaceName, Math.Abs(points));  // Abs to ensure the value is non-negative
                }

                return (string.Empty, 0);  // You might want to handle this case differently
            }
            finally
            {
                if (hDC != IntPtr.Zero)
                {
                    ReleaseDC(IntPtr.Zero, hDC);
                }
            }
        }
    }
}