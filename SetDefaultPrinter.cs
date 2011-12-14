using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PrinterAutoRegister
{
    class SetDefaultPrinter
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int LoadLibraryA([MarshalAs(UnmanagedType.LPStr)] 
		string DllName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int FreeLibrary(int hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetProcAddress(int hModule,
            [MarshalAs(UnmanagedType.LPStr)] 
		string lpProcName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetDesktopWindow();

        public delegate void PrintUIEntryW(
            Int32 hwnd,
            Int32 hinst,
            [MarshalAs(UnmanagedType.LPWStr)] string text,
            Int32 nCmdShow);

        public SetDefaultPrinter()
        {
            int hModule = LoadLibraryA("printui.dll");
            IntPtr ptr;
            ptr = (IntPtr)GetProcAddress(hModule, "PrintUIEntryW");
            if (ptr != IntPtr.Zero)
            {
                PrintUIEntryW func1 =
                    (PrintUIEntryW)Marshal.GetDelegateForFunctionPointer(ptr, typeof(PrintUIEntryW));

                if (System.Environment.OSVersion.Version.Major >= 6)
                    // Windows Vistaまたは7（つまりOSメジャーバージョン6以上）の場合のみ、通常使うプリンタの設定を行う
                    // この判定部分は2010/02/01に追加
                    func1(GetDesktopWindow(), hModule, " /y /n \"Microsoft XPS Document Writer\"", 5);
                FreeLibrary(hModule);
            }
        }
    }
}
