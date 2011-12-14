using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace PrinterAutoRegister
{
    /// <summary>
    /// ログを任意の場所に書き出すためのクラス。
    /// ただし、例外処理をしていないので、誰か引継ぎで完成させてくれることを願います。
    /// 2011/04/13 渡部聡
    /// </summary>
    public class LogWriter
    {
        public string logPath { private set; get; }
        private Encoding enc = Encoding.GetEncoding("Shift_JIS");
        private StreamWriter sw;

        /// <summary>
        /// 引数1:デスクトップにLog.logを出力する
        /// </summary>
        /// <param name="append">
        /// true :追記
        /// false:ファイルの初期化
        /// </param>
        public LogWriter(bool append)
        {
            string app_name = Path.GetFileName(Application.ExecutablePath).Replace(".EXE", "").Replace(".exe", "");
            logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), app_name + ".log");
            Initup(append);
        }
        /// <summary>
        /// 引数2:出力される先を指定する
        /// </summary>
        /// <param name="append">
        /// true :追記
        /// false:ファイルの初期化
        /// </param>
        /// <param name="log_path">
        /// 任意の出力先
        /// </param>
        public LogWriter(bool append, string log_path)
        {
            this.logPath = log_path;
            Initup(append);
        }
        /// <summary>
        /// ログを書き出す
        /// </summary>
        /// <param name="mes">
        /// ログの内容
        /// </param>
        public void WriteMes(string mes)
        {
            DateTime dt = DateTime.Now;
            string timestamp = String.Format("[{0:00}:{1:00}:{2:00}]   ", dt.Hour, dt.Minute, dt.Second);
            sw.Write(timestamp);
            sw.WriteLine(mes);
        }
        /// <summary>
        /// CLOSE
        /// </summary>
        public void Close()
        {
            try
            {
                sw.Close();
                sw.Dispose();
            }
            catch { }
        }

        /// <summary>
        /// コンストラクタで呼び出され、日付を出力する
        /// </summary>
        /// <param name="append"></param>
        private void Initup(bool append)
        {
            sw = new StreamWriter(logPath, append, enc);
            DateTime dt = DateTime.Now;
            string timestamp = String.Format("{0:0000}/{1:00}/{2:00}", dt.Year, dt.Month, dt.Day);
            sw.WriteLine(timestamp);
            sw.WriteLine("[HH:MM:SS]");
        }
    }
}
