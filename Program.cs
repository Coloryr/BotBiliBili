using BotBiliBili.Config;
using BotBiliBili.PicGen;
using BotBiliBili.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotBiliBili
{
    class Program
    {
        public static string RunLocal;
        private static Logs logs;
        private static Robot robot;
        private static RobotConfig RobotConfig;
        static void Main(string[] args)
        {
            RunLocal = AppContext.BaseDirectory;
            logs = new Logs(RunLocal);
            Reload();
            if (ConfigUtils.Config.RunQQ == 0 || ConfigUtils.Config.RunGroup.Count == 0)
            {
                Console.WriteLine("运行QQ和运行群未设置，请设置后重启");
                Console.Read();
                return;
            }
            RobotConfig = new()
            {
                IP = ConfigUtils.Config.IP,
                Port = ConfigUtils.Config.Port,
                Name = "BotBiliBili",
                Pack = new() { 49 },
                RunQQ = ConfigUtils.Config.RunQQ,
                Time = 10000,
                CallAction = Message,
                LogAction = Log,
                StateAction = State
            };
            robot = new();
            robot.Set(RobotConfig);
            robot.Start();
            CheckThread.Start();
            while (true)
            {
                string temp = Console.ReadLine();
                string[] arg = temp.Split(' ');
                if (arg[0] == "stop")
                {
                    CheckThread.Stop();
                    HttpUtils.Cancel();
                    robot.Stop();
                    return;
                }
                else if (arg[0] == "test")
                {
                    if (arg.Length < 2)
                    {
                        Error("错误的参数");
                        continue;
                    }
                    if (arg[1] == "video")
                    {
                        if (arg.Length != 3)
                        {
                            Error("错误的参数");
                            continue;
                        }
                        if (arg[2].StartsWith("AV"))
                        {
                            var data = HttpUtils.GetVideoA(arg[2]);
                            VideoPicGen.Gen(data);
                            Log("已生成");
                        }
                        else if (arg[2].StartsWith("BV"))
                        {
                            var data = HttpUtils.GetVideoB(arg[2]);
                            VideoPicGen.Gen(data);
                            Log("已生成");
                        }
                        else
                        {
                            Error("不正确的视频号");
                            continue;
                        }
                    }
                    else if (arg[1] == "dynamic")
                    {
                        if (arg.Length != 3)
                        {
                            Error("错误的参数");
                            continue;
                        }
                        var data = HttpUtils.GetDynamic(arg[2]);
                        DynamicPicGen.Gen(data);
                        Log("已生成");
                    }
                    else if (arg[1] == "duser")
                    {
                        if (arg.Length != 3)
                        {
                            Error("错误的参数");
                            continue;
                        }
                        var data = HttpUtils.GetDynamicUid(arg[2]);
                        DynamicPicGen.Gen(data);
                        Log("已生成");
                    }
                    else if (arg[1] == "live")
                    {
                        if (arg.Length != 3)
                        {
                            Error("错误的参数");
                            continue;
                        }
                        var data = HttpUtils.GetLive(arg[2]);
                        LivePicGen.Gen(data);
                        Log("已生成");
                    }
                }
                else if (arg[0] == "reload")
                {
                    Reload();
                    Log("已重读");
                }
            }
        }

        public static void Reload()
        {
            HttpUtils.Cancel();
            ConfigUtils.LoadAll();
            VideoPicGen.Init();
            DynamicPicGen.Init();
            HttpUtils.Init();
            LivePicGen.Init();
            //HttpUtils.Check();
        }

        public static void Log(string data)
            => logs.LogOut(data);
        public static void Error(string data)
            => logs.LogError(data);
        public static void Error(Exception data)
            => logs.LogError(data);

        public static void SendGroupMessage(string data, long group)
        {
            robot.AddTask(BuildPack.Build(new SendGroupMessagePack
            {
                id = group,
                message = new()
                {
                    data
                }
            }, 52));
        }

        public static void SendGroupImage(string local, long group)
        {
            robot.AddTask(BuildPack.Build(new LoadFileSendToGroupImagePack
            {
                id = group,
                file = local
            }, 75));
        }

        private static void Message(byte type, string data)
        {
            switch (type)
            {
                case 49:
                    var pack = JsonConvert.DeserializeObject<GroupMessageEventPack>(data);
                    if (!ConfigUtils.Config.RunGroup.Contains(pack.id))
                        return;
                    string message = pack.message[^1];
                    if (ConfigUtils.Config.DirectVideo)
                    {
                        string comm = message.ToLower();
                        if (comm.StartsWith("av"))
                        {
                            Log($"正在生成视频:{comm}的图片");
                            BotTask.VideoAV(message, pack.id);
                        }
                        else if (comm.StartsWith("bv"))
                        {
                            Log($"正在生成视频:{comm}的图片");
                            BotTask.VideoBV(message, pack.id);
                        }
                        else
                        {
                            SendGroupMessage("错误的视频号", pack.id);
                        }
                    }
                    if (pack.message[1].StartsWith("[mirai:app:"))
                    {
                        var obj = JObject.Parse(message);
                        string url = obj["meta"]?["news"]?["jumpUrl"]?.ToString();
                        if (string.IsNullOrWhiteSpace(url))
                            break;
                        string tag = obj["meta"]?["news"]?["tag"]?.ToString();
                        if (tag != "哔哩哔哩")
                            break;
                        var code = HttpUtils.GetUrl(url);
                        if (string.IsNullOrWhiteSpace(code))
                            break;
                        if (code.StartsWith("http://t.bilibili.com/"))
                        {
                            code = code.Replace("http://t.bilibili.com/", "");
                            int index = code.IndexOf('?');
                            code = code[..index];
                            BotTask.Dynamic(code, pack.id);
                        }
                        else if(code.StartsWith("https://live.bilibili.com/"))
                        {
                            code = code.Replace("https://live.bilibili.com/", "");
                            int index = code.IndexOf('?');
                            code = code[..index];
                            BotTask.Live(code, pack.id);
                        }
                        else
                        {
                            code = code.Replace("https://www.bilibili.com/", "");
                            int index = code.IndexOf('?');
                            code = code[..index];
                            if (code.StartsWith("video"))
                            {
                                code = code.Replace("video/", "");
                                string comm = code.ToLower();
                                if (comm.StartsWith("av"))
                                {
                                    Log($"正在生成视频:{comm}的图片");
                                    BotTask.VideoAV(code, pack.id);
                                }
                                else if (comm.StartsWith("bv"))
                                {
                                    Log($"正在生成视频:{comm}的图片");
                                    BotTask.VideoBV(code, pack.id);
                                }
                            }
                        }
                    }
                    string[] temp = message.Split(" ");
                    if (temp[0] == ConfigUtils.Config.Command.Head)
                    {
                        if (temp.Length == 1)
                        {
                            SendGroupMessage("错误的参数", pack.id);
                            break;
                        }
                        if (temp[1] == ConfigUtils.Config.Command.Help)
                        {
                            SendGroupMessage("BotBiliBili帮助\n" +
                                $"{ConfigUtils.Config.Command.Head} {ConfigUtils.Config.Command.Video} [视频号] 生成视频图片，AV号BV号均可\n" +
                                $"{ConfigUtils.Config.Command.Head} {ConfigUtils.Config.Command.VideoName} [视频名] 生成搜索后的视频图片\n" +
                                $"{ConfigUtils.Config.Command.Head} {ConfigUtils.Config.Command.Dynamic} [动态号] 生成动态图片\n" +
                                $"{ConfigUtils.Config.Command.Head} {ConfigUtils.Config.Command.DynamicUser} [UP的UID] 生成UP主最新动态图片\n" +
                                $"{ConfigUtils.Config.Command.Head} {ConfigUtils.Config.Command.DynamicName} [UP的名字] 生成UP主最新动态图片\n" +
                                $"{ConfigUtils.Config.Command.Head} {ConfigUtils.Config.Command.Live} [房间号] 生成直播间图片\n" +
                                $"{ConfigUtils.Config.Command.Head} {ConfigUtils.Config.Command.LiveName} [UP主名字] 生成UP主的直播间图片\n" +
                                $"{ConfigUtils.Config.Command.Head} {ConfigUtils.Config.Command.LiveUid} [UID] 生成UP主的直播间图片\n" +
                                $"{ConfigUtils.Config.Command.Head} {ConfigUtils.Config.Command.SubscribeUid} [UID] 订阅UP主的动态\n" +
                                $"{ConfigUtils.Config.Command.Head} {ConfigUtils.Config.Command.SubscribeLive} [UID] 订阅UP主的直播\n" +
                                $"{ConfigUtils.Config.Command.Head} {ConfigUtils.Config.Command.UnSubscribeUid} [UID] 取消订阅UP主的动态\n" +
                                $"{ConfigUtils.Config.Command.Head} {ConfigUtils.Config.Command.UnSubscribeLive} [UID] 取消订阅UP主的直播", pack.id);
                            break;
                        }
                        else if (temp[1] == ConfigUtils.Config.Command.Video)
                        {
                            if (temp.Length == 2)
                            {
                                SendGroupMessage("错误的参数", pack.id);
                                break;
                            }
                            string comm = temp[2].ToLower();
                            if (comm.StartsWith("av"))
                            {
                                Log($"正在生成视频:{comm}的图片");
                                BotTask.VideoAV(temp[2], pack.id);
                            }
                            else if (comm.StartsWith("bv"))
                            {
                                Log($"正在生成视频:{comm}的图片");
                                BotTask.VideoBV(temp[2], pack.id);
                            }
                            else
                            {
                                SendGroupMessage("错误的视频号", pack.id);
                            }
                        }
                        else if (temp[1] == ConfigUtils.Config.Command.VideoName)
                        {
                            if (temp.Length == 2)
                            {
                                SendGroupMessage("错误的参数", pack.id);
                                break;
                            }
                            Log($"正在生成视频:{temp[2]}的图片");
                            BotTask.SearchVideo(temp[2], pack.id);
                        }
                        else if (temp[1] == ConfigUtils.Config.Command.Dynamic)
                        {
                            if (temp.Length == 2)
                            {
                                SendGroupMessage("错误的参数", pack.id);
                                break;
                            }
                            string comm = temp[2];
                            if (!Tools.IsNumeric(comm))
                            {
                                SendGroupMessage("错误的动态号", pack.id);
                                break;
                            }
                            Log($"正在生成动态:{comm}的图片");
                            BotTask.Dynamic(comm, pack.id);
                        }
                        else if (temp[1] == ConfigUtils.Config.Command.DynamicUser)
                        {
                            if (temp.Length == 2)
                            {
                                SendGroupMessage("错误的参数", pack.id);
                                break;
                            }
                            string comm = temp[2];
                            if (!Tools.IsNumeric(comm))
                            {
                                SendGroupMessage("错误的UP主号", pack.id);
                                break;
                            }
                            Log($"正在生成动态:{comm}的图片");
                            BotTask.DynamicUid(comm, pack.id);
                        }
                        else if (temp[1] == ConfigUtils.Config.Command.DynamicName)
                        {
                            if (temp.Length == 2)
                            {
                                SendGroupMessage("错误的参数", pack.id);
                                break;
                            }
                            string comm = temp[2];
                            Log($"正在生成用户:{comm}的动态图片");
                            BotTask.SearchUserDynamic(comm, pack.id);
                        }
                        else if (temp[1] == ConfigUtils.Config.Command.Live)
                        {
                            if (temp.Length == 2)
                            {
                                SendGroupMessage("错误的参数", pack.id);
                                break;
                            }
                            string comm = temp[2].ToLower();
                            if (!Tools.IsNumeric(comm))
                            {
                                SendGroupMessage("错误的直播号", pack.id);
                                break;
                            }
                            Log($"正在生成直播:{comm}的图片");
                            BotTask.Live(comm, pack.id);
                        }
                        else if (temp[1] == ConfigUtils.Config.Command.LiveUid)
                        {
                            if (temp.Length == 2)
                            {
                                SendGroupMessage("错误的参数", pack.id);
                                break;
                            }
                            string comm = temp[2];
                            if (!Tools.IsNumeric(comm))
                            {
                                SendGroupMessage("错误的UID", pack.id);
                                break;
                            }
                            Log($"正在生成直播:{comm}的图片");
                            BotTask.LiveUser(comm, pack.id);
                        }
                        else if (temp[1] == ConfigUtils.Config.Command.LiveName)
                        {
                            if (temp.Length == 2)
                            {
                                SendGroupMessage("错误的参数", pack.id);
                                break;
                            }
                            string comm = temp[2];
                            Log($"正在生成直播:{comm}的图片");
                            BotTask.LiveName(comm, pack.id);
                        }
                        else if (temp[1] == ConfigUtils.Config.Command.SubscribeUid)
                        {
                            if (ConfigUtils.Config.AdminSubscribeOnly)
                            {
                                if (pack.permission == MemberPermission.MEMBER)
                                    break;
                            }
                            if (temp.Length == 2)
                            {
                                SendGroupMessage("错误的参数", pack.id);
                                break;
                            }
                            string comm = temp[2];
                            if (!Tools.IsNumeric(comm))
                            {
                                SendGroupMessage("错误的UID", pack.id);
                                break;
                            }
                            Log($"群:{pack.id} 订阅UP主:{comm} 的动态");
                            if (ConfigUtils.Subscribes.Uids.ContainsKey(comm))
                            {
                                var list = ConfigUtils.Subscribes.Uids[comm];
                                if (list.Contains(pack.id))
                                {
                                    SendGroupMessage("已经订阅过了", pack.id);
                                    break;
                                }
                                list.Add(pack.id);
                            }
                            else
                            {
                                List<long> list = new();
                                list.Add(pack.id);
                                ConfigUtils.Subscribes.Uids.TryAdd(comm, list);
                            }
                            SendGroupMessage("订阅成功", pack.id);
                            ConfigUtils.SaveSubscribe();
                        }
                        else if (temp[1] == ConfigUtils.Config.Command.SubscribeLive)
                        {
                            if (ConfigUtils.Config.AdminSubscribeOnly)
                            {
                                if (pack.permission == MemberPermission.MEMBER)
                                    break;
                            }
                            if (temp.Length == 2)
                            {
                                SendGroupMessage("错误的参数", pack.id);
                                break;
                            }
                            string comm = temp[2];
                            if (!Tools.IsNumeric(comm))
                            {
                                SendGroupMessage("错误的UID", pack.id);
                                break;
                            }
                            Log($"群:{pack.id} 订阅UP主:{comm} 的直播");
                            if (ConfigUtils.Subscribes.Lives.ContainsKey(comm))
                            {
                                var list = ConfigUtils.Subscribes.Lives[comm];
                                if (list.Contains(pack.id))
                                {
                                    SendGroupMessage("已经订阅过了", pack.id);
                                    break;
                                }
                                list.Add(pack.id);
                            }
                            else
                            {
                                List<long> list = new();
                                list.Add(pack.id);
                                ConfigUtils.Subscribes.Lives.TryAdd(comm, list);
                            }
                            SendGroupMessage("订阅成功", pack.id);
                            ConfigUtils.SaveSubscribe();
                        }
                        else if (temp[1] == ConfigUtils.Config.Command.UnSubscribeUid)
                        {
                            if (ConfigUtils.Config.AdminSubscribeOnly)
                            {
                                if (pack.permission == MemberPermission.MEMBER)
                                    break;
                            }
                            if (temp.Length == 2)
                            {
                                SendGroupMessage("错误的参数", pack.id);
                                break;
                            }
                            string comm = temp[2];
                            if (!Tools.IsNumeric(comm))
                            {
                                SendGroupMessage("错误的UID", pack.id);
                                break;
                            }
                            Log($"群:{pack.id} 取消订阅UP主:{comm} 的动态");
                            if (ConfigUtils.Subscribes.Uids.ContainsKey(comm))
                            {
                                var list = ConfigUtils.Subscribes.Uids[comm];
                                if (list.Contains(pack.id))
                                {
                                    list.Remove(pack.id);
                                    if (list.Count == 0)
                                    {
                                        ConfigUtils.Subscribes.Uids.TryRemove(comm, out var a);
                                    }
                                    SendGroupMessage("取消订阅成功", pack.id);
                                    ConfigUtils.SaveSubscribe();
                                    break;
                                }
                                else
                                {
                                    SendGroupMessage("没有订阅过", pack.id);
                                    break;
                                }
                            }
                            else
                            {
                                SendGroupMessage("没有订阅过", pack.id);
                            }
                        }
                        else if (temp[1] == ConfigUtils.Config.Command.UnSubscribeLive)
                        {
                            if (ConfigUtils.Config.AdminSubscribeOnly)
                            {
                                if (pack.permission == MemberPermission.MEMBER)
                                    break;
                            }
                            if (temp.Length == 2)
                            {
                                SendGroupMessage("错误的参数", pack.id);
                                break;
                            }
                            string comm = temp[2];
                            if (!Tools.IsNumeric(comm))
                            {
                                SendGroupMessage("错误的UID", pack.id);
                                break;
                            }
                            Log($"群:{pack.id} 取消订阅UP主:{comm} 的直播");
                            if (ConfigUtils.Subscribes.Lives.ContainsKey(comm))
                            {
                                var list = ConfigUtils.Subscribes.Lives[comm];
                                if (list.Contains(pack.id))
                                {
                                    list.Remove(pack.id);
                                    if (list.Count == 0)
                                    {
                                        ConfigUtils.Subscribes.Lives.TryRemove(comm, out var a);
                                    }
                                    SendGroupMessage("取消订阅成功", pack.id);
                                    ConfigUtils.SaveSubscribe();
                                    break;
                                }
                                else
                                {
                                    SendGroupMessage("没有订阅过", pack.id);
                                    break;
                                }
                            }
                            else
                            {
                                SendGroupMessage("没有订阅过", pack.id);
                            }
                        }
                    }
                    break;
            }
        }

        private static void Log(LogType type, string data)
        {
            Log($"{type} {data}");
        }

        private static void State(StateType type)
        {
            Log($"{type}");
        }
    }
}
