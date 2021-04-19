using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BotBiliBili
{
    public class Logs
    {
        public string log { get; set; }
        private string RunLocal;
        private readonly object lockobject = new object();

        public Logs(string RunLocal)
        {
            this.RunLocal = RunLocal;
            if (string.IsNullOrWhiteSpace(log))
                log = "logs.log";
            if (!File.Exists(RunLocal + log))
                File.Create(RunLocal + log);
        }

        private void LogWrite(string a)
        {
            lock (lockobject)
            {
                try
                {
                    var date = DateTime.Now;
                    string year = date.ToShortDateString().ToString();
                    string time = date.ToLongTimeString().ToString();
                    string write = "[" + year + "]" + "[" + time + "]" + a;
                    File.AppendAllText(RunLocal + log, write + Environment.NewLine, Encoding.UTF8);
                    Console.WriteLine(write);
                }
                catch
                { }
            }
        }
        public void LogError(Exception e)
        {
            Task.Factory.StartNew(() =>
            {
                string a = "[Error]" + e.Message + ":" + e.Source + "\n" + e.StackTrace;
                LogWrite(a);
            });
        }
        public void LogError(string a)
        {
            Task.Factory.StartNew(() =>
            {
                a = "[Error]" + a;
                LogWrite(a);
            });
        }

        public void LogOut(string a)
        {
            Task.Factory.StartNew(() =>
            {
                a = "[Info]" + a;
                LogWrite(a);
            });
        }
    }
}
