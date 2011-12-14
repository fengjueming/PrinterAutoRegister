using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WTCO
{
    public partial class Form1 : Form
    {
        int timeout = 20; //20分

        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.RunWorkerAsync();
            timeoutWorker.RunWorkerAsync();
            subEndWorker.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            bool loop = true;
            //ClientOptionが起動するまで待つ
            while (loop)
            {
                Process[] procs = Process.GetProcesses();
                foreach (Process proc in procs)
                {
                    if (proc.ProcessName.ToLower().IndexOf("ClientOption".ToLower()) > -1)
                    {
                        loop = false;
                        break;
                    }
                }
                Thread.Sleep(50);
            }
            //リポート10
            backgroundWorker1.ReportProgress(10);
            subEndWorker.CancelAsync();
            //ClientOptionが終了するまで待つ
            loop = true;
            while (loop)
            {
                bool hit = false;
                Process[] procs = Process.GetProcesses();
                foreach (Process proc in procs)
                {
                    if (proc.ProcessName.ToLower().IndexOf("ClientOption".ToLower()) > -1)
                    {
                        hit = true;
                        break;
                    }
                }
                System.Threading.Thread.Sleep(50);
                loop = hit;
            }
            //リポート20
            backgroundWorker1.ReportProgress(20);

            //AutoRegisterが終了するまで待つ
            loop = true;
            while (loop)
            {
                bool hit = false;
                Process[] procs = Process.GetProcesses();
                foreach (Process proc in procs)
                {
                    if (proc.ProcessName.ToLower().IndexOf("PrinterAutoRegister.".ToLower()) > -1)
                    {
                        hit = true;
                        break;
                    }
                }
                System.Threading.Thread.Sleep(200);
                loop = hit;
            }
            backgroundWorker1.ReportProgress(100);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (10 == e.ProgressPercentage)
            {
                this.Text = "ClientOptionをインストール中です";
            }
            else if (20 == e.ProgressPercentage)
            {
                this.Text = "終了処理中です.";
            }
            else if (100 == e.ProgressPercentage)
            {
                this.Text = "終了しました.";
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Step = progressBar1.Maximum;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            /*
            ProcessStartInfo psi = new ProcessStartInfo();
            DialogResult result = MessageBox.Show("プリンターのインストールを終了しました。\n再起動して下さい。\n", "プリンターインストール", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (DialogResult.OK == result)
            {

                psi.FileName = "shutdown.exe";
                psi.Arguments = "/r /t 0 /f";
                Process.Start(psi);
                Environment.Exit(0);
            }
            this.Close();
             */
            this.Close();
        }


        private void timeoutWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < timeout; i++)
            {
                Thread.Sleep(60 * 1000); //1分に1回
            }
            try
            {
                backgroundWorker1.CancelAsync();
            }
            catch { }
            MessageBox.Show("タイムアウトしました.\n");
            Environment.Exit(-1);
        }


        private void subEndWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Process[] procs;
            bool loop = true;
            while (loop)
            {
                procs = Process.GetProcesses();
                foreach (Process ps in procs)
                {
                    if (ps.ProcessName.ToLower().IndexOf("PrinterAutoRegister.") > -1)
                    {
                        loop = false;
                        break;
                    }
                }
                Thread.Sleep(1000);
            }
            Thread.Sleep(10000);
            loop = true;
            while (loop)
            {
                procs = Process.GetProcesses();
                loop = false;
                foreach (Process ps in procs)
                {
                    if (ps.ProcessName.ToLower().IndexOf("PrinterAutoRegister.") > -1)
                    {
                        loop = true;
                        break;
                    }
                }
                Thread.Sleep(1000);
            }
            Thread.Sleep(3000);
            try
            {
                backgroundWorker1.CancelAsync();
            }
            catch { }
        }
    }
}
