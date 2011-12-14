using System;
using System.Collections.Generic;
using System.Text;

namespace PrinterAutoRegister
{
    public class TryPing
    {
        public static Boolean checkNetwork
        {
            get
            {
                try
                {
                    //Pingオブジェクトの作成
                    System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
                    System.Net.NetworkInformation.PingReply reply = p.Send(System.Net.IPAddress.Parse("10.14.0.13"));

                    //結果を取得
                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        //Console.WriteLine("Reply from {0}:bytes={1} time={2}ms TTL={3}", reply.Address, reply.Buffer.Length, reply.RoundtripTime, reply.Options.Ttl);
                        p.Dispose();
                        return (true);
                    }
                    else
                    {
                        //Console.WriteLine("Ping送信に失敗。({0})", reply.Status);
                        p.Dispose();
                        return (false);
                    }
                }
                catch
                {
                    return (false);
                }
            }
        }
    }
}
