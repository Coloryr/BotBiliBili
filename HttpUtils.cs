using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BotBiliBili
{
    class HttpUtils
    {
        private static HttpClient client;
        private static CookieContainer Cookie;
        private static HttpClientHandler HttpClientHandler;
        public static void Init()
        {
            if (client != null)
            {
                client.Dispose();
            }
            Cookie = new();
            Cookie.Add(new Cookie("SESSDATA", 
                ConfigUtils.Config.SESSDATA ?? "","/" ,".bilibili.com"));
            Cookie.Add(new Cookie("bili_jct", 
                ConfigUtils.Config.bili_jct ?? "", "/", ".bilibili.com"));
            HttpClientHandler = new()
            {
                CookieContainer = Cookie,
            };
            client = new HttpClient(HttpClientHandler);
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.72 Safari/537.36 Edg/90.0.818.42");
        }

        public static void Check()
        {
            Program.Log("密匙校验中");
            string url = "https://api.bilibili.com/x/web-interface/archive/like";
            Dictionary<string, string> arg = new();
            arg.Add("bvid", "BV1EW411j7cg");
            arg.Add("like", "1");
            arg.Add("csrf", ConfigUtils.Config.bili_jct);
            var data = Post(url, arg);
            JObject obj = JObject.Parse(data);
            string res = obj["code"].ToString();
            if (res == "-111")
            {
                Program.Error("bili_jct 校验失败");
            }
            else if (res == "-101" || res == "-400")
            {
                Program.Error("SESSDATA 校验失败");
            }
            else
            {
                Program.Log("账户校验成功");
            }
        }

        public static JObject GetVideoA(string aid)
        {
            string url = $"https://api.bilibili.com/x/web-interface/view?aid={aid}";
            var data = Get(url);
            JObject obj = JObject.Parse(data);
            if (obj["code"].ToString() != "0")
            {
                Program.Error($"获取视频信息失败:{obj["message"]}");
                return null;
            }
            return obj;
        }

        public static JObject GetVideoB(string bid)
        {
            string url = $"https://api.bilibili.com/x/web-interface/view?bvid={bid}";
            var data = Get(url);
            JObject obj = JObject.Parse(data);
            if (obj["code"].ToString() != "0")
            {
                Program.Error($"获取视频信息失败:{obj["message"]}");
                return null;
            }
            return obj;
        }

        public static string Post(string url, Dictionary<string, string> arg)
        {
            return client.PostAsync(url, new FormUrlEncodedContent(arg)).Result.Content.ReadAsStringAsync().Result;
        }

        public static string Get(string url)
        {
            return client.GetStringAsync(url).Result;
        }

        public static Stream GetData(string url)
        {
            return client.GetStreamAsync(url).Result;
        }
    }
}
