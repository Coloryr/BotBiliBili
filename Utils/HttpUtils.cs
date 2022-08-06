using BotBiliBili.Config;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace BotBiliBili.Utils;

public static class HttpUtils
{
    private static HttpClient client;
    private static CookieContainer Cookie;
    private static HttpClientHandler HttpClientHandler;
    private static CancellationTokenSource cancellation;
    public static void Cancel()
    {
        cancellation?.Cancel(false);
    }
    public static void Init()
    {
        if (client != null)
        {
            client.Dispose();
        }
        Cookie = new();
        Cookie.Add(new Cookie("SESSDATA",
            ConfigUtils.Config.SESSDATA ?? "", "/", ".bilibili.com"));
        Cookie.Add(new Cookie("bili_jct",
            ConfigUtils.Config.bili_jct ?? "", "/", ".bilibili.com"));
        HttpClientHandler = new()
        {
            CookieContainer = Cookie,
        };
        client = new HttpClient(HttpClientHandler)
        {
            Timeout = TimeSpan.FromSeconds(ConfigUtils.Config.TimeOut)
        };
        foreach (var item in ConfigUtils.Config.RequestHeaders)
        {
            client.DefaultRequestHeaders.Add(item.Key, item.Value);
        }
        cancellation = new();
    }

    public static string GetUrl(string url)
    {
        try
        {
            var data = client.GetAsync(url).Result;
            if (data.StatusCode == HttpStatusCode.Found)
                return data.Headers.Location.AbsoluteUri;
            else
                return data.RequestMessage.RequestUri.AbsoluteUri;
        }
        catch (Exception e)
        {
            Program.Error(e);
            return null;
        }
    }
    public static void Check()
    {
        Program.Log("密匙校验中");
        string url = "https://api.bilibili.com/x/web-interface/archive/like";
        Dictionary<string, string> arg = new()
        {
            { "bvid", "BV1EW411j7cg" },
            { "like", "1" },
            { "csrf", ConfigUtils.Config.bili_jct }
        };
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
        try
        {
            string url = $"https://api.bilibili.com/x/web-interface/view?aid={aid.ToLower().Replace("av", "")}";
            var data = Get(url);
            JObject obj = JObject.Parse(data);
            if (obj["code"].ToString() != "0")
            {
                Program.Error($"获取视频信息失败:{obj["message"]}");
                return null;
            }
            return obj;
        }
        catch (Exception e)
        {
            Program.Error(e);
            return null;
        }
    }

    public static JObject GetVideoB(string bid)
    {
        try
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
        catch (Exception e)
        {
            Program.Error(e);
            return null;
        }
    }

    public static JObject GetDynamic(string did)
    {
        try
        {
            string url = $"https://api.vc.bilibili.com/dynamic_svr/v1/dynamic_svr/get_dynamic_detail?dynamic_id={did}";
            var data = Get(url);
            JObject obj = JObject.Parse(data);
            if (obj["code"].ToString() != "0")
            {
                Program.Error($"获取动态信息失败:{obj["message"]}");
                return null;
            }
            else if (obj["data"]["card"] == null && obj["data"]["cards"] == null)
            {
                Program.Error($"获取动态信息失败:动态已删除");
                return null;
            }
            return obj;
        }
        catch (Exception e)
        {
            Program.Error(e);
            return null;
        }
    }
    public static JObject GetLive(string room)
    {
        try
        {
            string url = $"https://api.live.bilibili.com/xlive/web-room/v1/index/getInfoByRoom?room_id={room}";
            var data = Get(url);
            JObject obj = JObject.Parse(data);
            if (obj["code"].ToString() != "0")
            {
                Program.Error($"获取直播间信息失败:{obj["message"]}");
                return null;
            }
            return obj;
        }
        catch (Exception e)
        {
            Program.Error(e);
            return null;
        }
    }
    public static JObject GetLiveUID(string uid)
    {
        try
        {
            string url = $"https://api.live.bilibili.com/room/v1/Room/getRoomInfoOld?mid={uid}";
            var data = Get(url);
            JObject obj = JObject.Parse(data);
            if (obj["code"].ToString() != "0")
            {
                Program.Error($"获取用户直播间信息失败:{obj["message"]}");
                return null;
            }
            return obj;
        }
        catch (Exception e)
        {
            Program.Error(e);
            return null;
        }
    }

    public static JObject GetDynamicUid(string uid)
    {
        try
        {
            string url = $"https://api.vc.bilibili.com/dynamic_svr/v1/dynamic_svr/space_history?host_uid={uid}";
            var data = Get(url);
            JObject obj = JObject.Parse(data);
            if (obj["code"].ToString() != "0")
            {
                Program.Error($"获取动态信息失败:{obj["message"]}");
                return null;
            }
            return obj;
        }
        catch (Exception e)
        {
            Program.Error(e);
            return null;
        }
    }
    public static JObject SearchUser(string name)
    {
        try
        {
            string url = $"https://api.bilibili.com/x/web-interface/search/type?keyword={name}&search_type=bili_user";
            var data = Get(url);
            JObject obj = JObject.Parse(data);
            if (obj["code"].ToString() != "0")
            {
                Program.Error($"搜索用户失败:{obj["message"]}");
                return null;
            }
            return obj;
        }
        catch (Exception e)
        {
            Program.Error(e);
            return null;
        }
    }
    public static JObject SearchVideo(string name)
    {
        try
        {
            string url = $"https://api.bilibili.com/x/web-interface/search/type?keyword={name}&search_type=video";
            var data = Get(url);
            JObject obj = JObject.Parse(data);
            if (obj["code"].ToString() != "0")
            {
                Program.Error($"搜索视频失败:{obj["message"]}");
                return null;
            }
            return obj;
        }
        catch (Exception e)
        {
            Program.Error(e);
            return null;
        }
    }
    public static JObject SearchLive(string name)
    {
        try
        {
            string url = $"https://api.bilibili.com/x/web-interface/search/type?keyword={name}&search_type=live";
            var data = Get(url);
            JObject obj = JObject.Parse(data);
            if (obj["code"].ToString() != "0")
            {
                Program.Error($"搜索视频失败:{obj["message"]}");
                return null;
            }
            return obj;
        }
        catch (Exception e)
        {
            Program.Error(e);
            return null;
        }
    }

    public static string Post(string url, Dictionary<string, string> arg)
    {
        return client.PostAsync(url, new FormUrlEncodedContent(arg), cancellation.Token).Result.Content.ReadAsStringAsync().Result;
    }

    public static string Get(string url)
    {
        return client.GetStringAsync(url, cancellation.Token).Result;
    }

    public static Stream GetData(string url)
    {
        return client.GetStreamAsync(url, cancellation.Token).Result;
    }

    public static byte[] GetByte(string url)
    {
        return client.GetByteArrayAsync(url, cancellation.Token).Result;
    }
}
