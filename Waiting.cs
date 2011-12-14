using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Microsoft.Win32;

namespace PrinterAutoRegister
{
    public partial class Waiting : Form
    {
        LogWriter lw;
        string logPath = "log.txt";
        string monoPrinterName = "NUCENet MONO";
        string colorPrinterName = "NUCENet COLOR";

        ProcessStartInfo psi = new ProcessStartInfo();
        Process proc = new Process();

        bool setupResult = false;

        public Waiting()
        {
            InitializeComponent();
            setup();
            backgroundWorker1.RunWorkerAsync();
        }
        /// <summary>
        /// 実行環境を整える
        /// </summary>
        private void setup()
        {
            Directory.SetCurrentDirectory("Data");
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            logPath = Path.Combine(Path.GetTempPath(), "PRINTERLOG.log");
            try
            {
                lw = new LogWriter(false, logPath);
                //lw = new LogWriter(false);
            }
            catch {
                lw = new LogWriter(false);
            }
            lw.WriteMes("ログの記録を開始します.");
        }

        /// <summary>
        /// プリンターを削除する
        /// </summary>
        /// <param name="printerName">削除するプリンターの名前</param>
        private void deletePrinter(string printerName)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            Process proc = new Process();
            try
            {
                psi.FileName = "rundll32.exe";
                psi.Arguments = "";
                psi.Arguments += " printui.dll,PrintUIEntry";
                psi.Arguments += " /q";
                psi.Arguments += " /dl";
                psi.Arguments += " /n";
                psi.Arguments += "  \"" + printerName + "\"";
                proc.StartInfo = psi;
                proc.Start();
                proc.WaitForExit();
            }
            finally
            {
            }
        }

        /// <summary>
        /// 対象のプリンターが存在するか調べる
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        private bool exist(string printerName)
        {
            //インストールされている全てのプリンターを確認する
            foreach (string installedPrinterName in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                if (installedPrinterName.Equals(printerName))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// windowNameで指定した文字列を名前に含むウインドウが存在するかを調べる
        /// </summary>
        /// <param name="windowName"></param>
        /// <returns></returns>
        private bool isWindow(string windowName)
        {
            bool hit = false;
            Process[] windows = Process.GetProcesses();
            foreach (Process window in windows)
            {
                if (window.MainWindowHandle != IntPtr.Zero)
                {
                    if (window.MainWindowTitle.IndexOf(windowName) > -1)
                    {
                        hit = true;
                    }
                }
            }
            return hit;
        }

        /// <summary>
        /// 指定した名前を持つプロセスが存在するか調べるメソッド
        /// </summary>
        private bool busy(string psName)
        {
            Process[] procs = Process.GetProcesses();
            foreach (Process ps in procs)
            {
                if (ps.ProcessName.ToLower().IndexOf(psName.ToLower()) > -1)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ドライバーインストーラーを呼び出す
        /// </summary>
        /// <returns></returns>
        private bool driverInstall()
        {
            lw.WriteMes("ドライバーのインストールセクションに入りました.");
            bool res = true;
            string windowName = "EpsonPrintersVer";
            if (!(exist(monoPrinterName) & exist(colorPrinterName)))
            {
                deletePrinter(monoPrinterName);
                deletePrinter(colorPrinterName);
                if (4 == IntPtr.Size)
                {
                    psi.FileName = "EpsonPrinters_32bit.exe";
                }
                else
                {
                    psi.FileName = "EpsonPrinters_64bit.exe";
                }
                if (!File.Exists(psi.FileName))
                {
                    MessageBox.Show(psi.FileName + "が見つかりませんでした.");
                    return false;
                }
                proc.StartInfo = psi;
                proc.Start();
                lw.WriteMes(psi.FileName + " を開始しました.");
                while (!isWindow(windowName)) Thread.Sleep(500);
                lw.WriteMes(windowName + "...が開かれました.");
                proc.WaitForExit();
                while (busy(psi.FileName)) Thread.Sleep(200);
                lw.WriteMes(psi.FileName + " が終了しました.  ExitCode:" + proc.ExitCode);
                while (!exist(monoPrinterName)) Thread.Sleep(200);
                lw.WriteMes(monoPrinterName + " のインストールに成功しました.");
                while (!exist(colorPrinterName)) Thread.Sleep(200);
                lw.WriteMes(colorPrinterName + " のインストールに成功しました.");
                while (isWindow(windowName)) Thread.Sleep(200);
                lw.WriteMes(windowName + "...が閉じられました.");
                if (0 > proc.ExitCode)
                {
                    res = false;
                }
            }
            else
            {
                lw.WriteMes("すでにインストールされています.");
            }
            lw.WriteMes("ドライバーのインストールセクションが終了しました.");
            return res;
        }

        /// <summary>
        /// ポートモニターを呼び出す
        /// </summary>
        /// <returns></returns>
        private bool sPSPortMonitorRun()
        {
            lw.WriteMes("SPSPortMonitorのインストールセクションに入りました.");
            RegistryKey uninstallKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true);
            bool res = true;
            string[] installedKey = uninstallKey.GetSubKeyNames();
            foreach (string s in installedKey)
            {
                if (s.Equals("{202D5DBC-CC82-45B7-9350-1B07CA553C5D}"))
                {
                    lw.WriteMes("すでにインストールされています. キーを削除します.");
                    uninstallKey.DeleteSubKeyTree(s);
                }
            }
            uninstallKey.Close();
            psi.FileName = "SPSPortMonitor.exe";
            psi.Arguments = "/s";
            if (!File.Exists(psi.FileName))
            {
                MessageBox.Show(psi.FileName + "が見つかりませんでした.");
                return false;
            }
            proc.StartInfo = psi;
            lw.WriteMes(psi.FileName + " を開始しました.");
            proc.Start();
            proc.WaitForExit();
            while (busy(psi.FileName)) Thread.Sleep(200);
            lw.WriteMes(psi.FileName + " が終了しました.  ExitCode:" + proc.ExitCode);
            if (0 > proc.ExitCode)
            {
                res = false;
            }
            psi.Arguments = "";
            lw.WriteMes("SPSPortMonitorのインストールセクションを終了します.");
            return res;
        }

        /// <summary>
        /// ClientOptionImageOnlySetupを呼び出す
        /// </summary>
        private bool clientOptionImageOnlySetupRun()
        {
            lw.WriteMes("ClientOptionのインストールセクションに入りました.");
            RegistryKey uninstallKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true);
            bool res = true;
            string[] installedKey = uninstallKey.GetSubKeyNames();
            foreach (string s in installedKey)
            {
                if (s.Equals("{FD9A1AE3-F4C7-4911-BADE-293888A0A89B}"))
                {
                    uninstallKey.DeleteSubKeyTree(s);
                }
            }
            uninstallKey.Close();
            psi.FileName = "ClientOptionImageOnlySetup.exe";
            psi.Arguments = "/s";
            if (!File.Exists(psi.FileName))
            {
                MessageBox.Show(psi.FileName + "が見つかりませんでした.");
                return false;
            }
            proc.StartInfo = psi;
            proc.Start();
            lw.WriteMes(psi.FileName + " を開始しました.");
            proc.WaitForExit();
            while (busy(psi.FileName)) Thread.Sleep(100);
            lw.WriteMes(psi.FileName + " が終了しました.  ExitCode:" + proc.ExitCode);
            lw.WriteMes("ClientOptionのインストールセクションを終了します.");
            return res;
        }

        /// <summary>
        /// ユーザーIDがレジストリに書き込まれていることを確認する
        /// </summary>
        private void checkUserName()
        {
            lw.WriteMes("ユーザーIDを確認します.");
            RegistryKey open;
            string targetKeyName = "UserName";
            try
            {
                lw.WriteMes("Optionキーを取得します.");
                open = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Control\Print\Monitors\SPS TCP/IP Port\Option", true);
                if (null != open.GetValue(targetKeyName))
                {
                    lw.WriteMes(targetKeyName + "キーが見つかりました.");
                    if (!open.GetValue(targetKeyName).Equals(""))
                    {
                        lw.WriteMes("現在の" + targetKeyName + "キー:" + open.GetValue(targetKeyName));
                        return;
                    }
                }
                lw.WriteMes(targetKeyName + "キーに正しい値が存在しませんでした.");
                lw.WriteMes("入力フォームを表示します.");
                UserNameForm unf = new UserNameForm();
                unf.TopMost = true;
                unf.ShowDialog();
                lw.WriteMes("入力されたユーザーID:" + unf.userName);
                if (unf.flug)
                {
                    open.SetValue(targetKeyName, unf.userName);
                    lw.WriteMes("ユーザーIDを設定しました.");
                }
                else
                {
                    lw.WriteMes("ユーザーID入力をキャンセルしました.");
                    lw.WriteMes("「印刷時にユーザーIDを入力する」を有効化します.");
                    open.SetValue("AccMode", 1);
                    lw.WriteMes(open.Name + "のAccModeを1にしました.");
                }
                unf.Dispose();
            }
            catch (Exception ex)
            {
                lw.WriteMes("" + ex);
            }
            lw.WriteMes("ユーザーIDの確認を終了しました.");
        }

        /// <summary>
        /// シャットダウンメッセージを出力
        /// </summary>
        ///
        private void shutdown()
        {
            lw.WriteMes("シャットダウンセクションに入りました.");
            if (!setupResult)
            {
                lw.WriteMes("完了していないため、シャットダウンは行いません.");
                lw.Close();
                MessageBox.Show("完了していない処理があります." + lw.logPath + "\nを確認して下さい.");
                return;
            }
            DialogResult result = MessageBox.Show("プリンターのインストールを終了しました。\nネットワークからログアウト後に再起動して下さい。\n", "プリンターインストール", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //while (TryPing.checkNetwork) MessageBox.Show("再起動を実行する前にログアウトを行ってください。", "プリンターインストール", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //if (DialogResult.OK == result)
            //{
            lw.WriteMes("再起動を実行しました.");
            psi.FileName = "shutdown.exe";
            psi.Arguments = "/r /t 1 /f";
            proc.StartInfo = psi;
            proc.Start();
            //}
        }

        /// <summary>
        /// その他・ユーザー名とか
        /// </summary>
        private void end()
        {
            //通常使うプリンターを設定する
            lw.WriteMes("通常使うプリンターを設定します.");
            SetDefaultPrinter sdp = new SetDefaultPrinter();
            lw.WriteMes("通常使うプリンターを設定しました.");
            //ユーザーIDを確認する
            checkUserName();
        }

        /// <summary>
        /// 終了時にログをHDDに書き出して、シャットダウンメッセージを出力する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Waiting_FormClosed(object sender, FormClosedEventArgs e)
        {
            shutdown();
            lw.Close();
        }


        /// <summary>
        /// 各メソッドを制御するバックグラウンドワーカー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker1.ReportProgress(5);
            //古いプリンターの削除
            deletePrinter("EPSON LP-S4200");
            //ここから新プリンターの設定
            bool driverRes = false;
            bool spsRes = false;
            bool coRes = false;
            //リポート  10
            backgroundWorker1.ReportProgress(10);
            driverRes = driverInstall();
            if (driverRes)
            {
                //リポート  20
                backgroundWorker1.ReportProgress(20);
                spsRes = sPSPortMonitorRun();
                if (spsRes)
                {
                    //リポート  30
                    backgroundWorker1.ReportProgress(30);
                    coRes = clientOptionImageOnlySetupRun();
                }
            }
            //リポート 40
            backgroundWorker1.ReportProgress(40);
            if (driverRes & spsRes & coRes)
            {
                setupResult = true;
                end();
            }
            lw.WriteMes("ドライバーのインストール結果　　：" + driverRes);
            lw.WriteMes("SPSPortMonitorのインストール結果：" + spsRes);
            lw.WriteMes("ClientOptionのインストール結果　：" + coRes);
        }

        /// <summary>
        /// ワーカーがフォームに対して操作するためのメソッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 5:
                    ShowInTaskbar = false;
                    break;
                case 10:
                    ShowInTaskbar = true;
                    this.Text = "ドライバーをインストールしています.";
                    break;
                case 20:
                    this.Text = "SPSPortMonitorをインストールしています.";
                    break;
                case 30:
                    this.Text = "ClientOptionをインストールしています.";
                    break;
                case 40:
                    //WindowState = FormWindowState.Minimized;
                    //this.Hide();
                    this.Text = "終了しました.";
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    progressBar1.Value = progressBar1.Maximum;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// バックグラウンドワーカーの終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }

    }
}
