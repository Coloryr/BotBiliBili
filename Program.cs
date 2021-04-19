using System;

namespace BotBiliBili
{
    class Program
    {
        public static string RunLocal;
        public static ConfigObj Config;
        static void Main(string[] args)
        {
            RunLocal = AppContext.BaseDirectory;
            Config = ConfigUtils.Config(new ConfigObj()
            {

            }, RunLocal + "config.json");

            while (true)
            {
                string temp = Console.ReadLine();
                if (temp == "stop")
                { 
                    
                }
            }
        }
    }
}
