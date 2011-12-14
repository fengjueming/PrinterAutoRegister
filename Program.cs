using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Threading;
using System.ComponentModel;

namespace PrinterAutoRegister
{
    class Program
    {
        static string monoPrinterName = "NUCENet MONO";
        static string colorPrinterName = "NUCENet COLOR";

        static ProcessStartInfo psi = new ProcessStartInfo();
        static Process proc = new Process();

        static void Main(string[] args)
        {
            checkNetWork();
            Application.EnableVisualStyles();
            Application.Run(new Waiting());
            Application.Exit();
        }

        /// <summary>
        /// ネットワークに接続されていることを確認する
        /// </summary>
        private static void checkNetWork()
        {
            if (!TryPing.checkNetwork)
            {
                if (DialogResult.Retry == MessageBox.Show("ネットワークに接続して下さい.", "ネットワークエラー", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning))
                {
                    checkNetWork();
                }
                else
                {
                    Environment.Exit(-1);
                }
            }
        }
    }
}
