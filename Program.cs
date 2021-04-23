using BotBiliBili.Config;
using BotBiliBili.PicGen;
using BotBiliBili.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
            while (true)
            {
                string temp = Console.ReadLine();
                string[] arg = temp.Split(' ');
                if (arg[0] == "stop")
                {
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

        private static void SendGroupMessage(string data, long group)
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

        private static void SendGroupImage(string local, long group)
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
                                $"{ConfigUtils.Config.Command.Video} [视频号] 生成视频图片，AV号BV号均可\n" +
                                $"{ConfigUtils.Config.Command.VideoName} [视频名] 生成搜索后的视频图片\n" +
                                $"{ConfigUtils.Config.Command.Dynamic} [动态号] 生成动态图片\n" +
                                $"{ConfigUtils.Config.Command.DynamicUser} [UP的UID] 生成UP主最新动态图片\n" +
                                $"{ConfigUtils.Config.Command.DynamicName} [UP的名字] 生成UP主最新动态图片\n" +
                                $"{ConfigUtils.Config.Command.Live} [房间号] 生成直播间图片\n" +
                                $"{ConfigUtils.Config.Command.LiveName} [UP主名字] 生成UP主的直播间图片\n" +
                                $"{ConfigUtils.Config.Command.LiveUid} [UID] 生成UP主的直播间图片", pack.id);
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
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        var data1 = HttpUtils.GetVideoA(temp[2]);
                                        if (data1 == null)
                                        {
                                            SendGroupMessage($"获取不到视频：{comm}", pack.id);
                                            return;
                                        }
                                        string temp1 = VideoPicGen.Gen(data1);
                                        Log($"已生成{temp1}");
                                        SendGroupImage(temp1, pack.id);
                                    }
                                    catch (Exception e)
                                    {
                                        Error(e);
                                    }
                                });
                            }
                            else if (comm.StartsWith("bv"))
                            {
                                Log($"正在生成视频:{comm}的图片");
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        var data1 = HttpUtils.GetVideoB(temp[2]);
                                        if (data1 == null)
                                        {
                                            SendGroupMessage($"获取不到视频：{comm}", pack.id);
                                            return;
                                        }
                                        string temp1 = VideoPicGen.Gen(data1);
                                        Log($"已生成{temp1}");
                                        SendGroupImage(temp1, pack.id);
                                    }
                                    catch (Exception e)
                                    {
                                        Error(e);
                                    }
                                });
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
                            string comm = temp[2];
                            Log($"正在生成视频:{comm}的图片");
                            Task.Run(() =>
                            {
                                try
                                {
                                    var data1 = HttpUtils.SearchVideo(comm);
                                    if (data1 == null)
                                    {
                                        SendGroupMessage($"搜索不到视频：{comm}", pack.id);
                                        return;
                                    }
                                    var data2 = data1["data"]["result"] as JArray;
                                    if (data2.Count == 0)
                                    {
                                        SendGroupMessage($"搜索：{comm} 没有结果", pack.id);
                                        return;
                                    }
                                    string bid = data2[0]["bvid"].ToString();
                                    data1 = HttpUtils.GetVideoB(bid);
                                    if (data1 == null)
                                    {
                                        SendGroupMessage($"获取视频：{bid}", pack.id);
                                        return;
                                    }
                                    string temp1 = VideoPicGen.Gen(data1);
                                    Log($"已生成{temp1}");
                                    SendGroupImage(temp1, pack.id);
                                }
                                catch (Exception e)
                                {
                                    Error(e);
                                }
                            });
                        }
                        else if (temp[1] == ConfigUtils.Config.Command.Dynamic)
                        {
                            if (temp.Length == 2)
                            {
                                SendGroupMessage("错误的参数", pack.id);
                                break;
                            }
                            string comm = temp[2];
                            if (Tools.IsNumeric(comm))
                            {
                                SendGroupMessage("错误的动态号", pack.id);
                                break;
                            }
                            Log($"正在生成动态:{comm}的图片");
                            Task.Run(() =>
                            {
                                try
                                {
                                    var data1 = HttpUtils.GetDynamic(temp[2]);
                                    if (data1 == null)
                                    {
                                        SendGroupMessage($"获取不到动态：{comm}", pack.id);
                                        return;
                                    }
                                    string temp1 = DynamicPicGen.Gen(data1);
                                    Log($"已生成{temp1}");
                                    SendGroupImage(temp1, pack.id);
                                }
                                catch (Exception e)
                                {
                                    Error(e);
                                }
                            });
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
                                Log($"正在生成动态:{comm}的图片");
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        var data1 = HttpUtils.GetDynamicUid(temp[2]);
                                        if (data1 == null)
                                        {
                                            SendGroupMessage($"获取不到动态：{comm}", pack.id);
                                            return;
                                        }
                                        string temp1 = DynamicPicGen.Gen(data1);
                                        Log($"已生成{temp1}");
                                        SendGroupImage(temp1, pack.id);
                                    }
                                    catch (Exception e)
                                    {
                                        Error(e);
                                    }
                                });
                            }
                            else
                            {
                                SendGroupMessage("错误的UP主号", pack.id);
                                break;
                            }
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
                            Task.Run(() =>
                            {
                                try
                                {
                                    var data1 = HttpUtils.SearchUser(comm);
                                    if (data1 == null)
                                    {
                                        SendGroupMessage($"搜索不到用户：{comm}", pack.id);
                                        return;
                                    }
                                    var data2 = data1["data"]["result"] as JArray;
                                    if (data2.Count == 0)
                                    {
                                        SendGroupMessage($"搜索：{comm} 没有结果", pack.id);
                                        return;
                                    }
                                    string id = data2[0]["mid"].ToString();
                                    data1 = HttpUtils.GetDynamicUid(id);
                                    if (data1 == null)
                                    {
                                        SendGroupMessage($"获取不到动态：{id}", pack.id);
                                        return;
                                    }
                                    string temp1 = DynamicPicGen.Gen(data1);
                                    Log($"已生成{temp1}");
                                    SendGroupImage(temp1, pack.id);
                                }
                                catch (Exception e)
                                {
                                    Error(e);
                                }
                            });
                        }
                        else if (temp[1] == ConfigUtils.Config.Command.Live)
                        {
                            if (temp.Length == 2)
                            {
                                SendGroupMessage("错误的参数", pack.id);
                                break;
                            }
                            string comm = temp[2].ToLower();
                            if (Tools.IsNumeric(comm))
                            {
                                SendGroupMessage("错误的直播号", pack.id);
                                break;
                            }
                            Log($"正在生成直播:{comm}的图片");
                            Task.Run(() =>
                            {
                                try
                                {
                                    var data1 = HttpUtils.GetLive(temp[2]);
                                    if (data1 == null)
                                    {
                                        SendGroupMessage($"获取不到直播间：{comm}", pack.id);
                                        return;
                                    }
                                    string temp1 = LivePicGen.Gen(data1);
                                    Log($"已生成{temp1}");
                                    SendGroupImage(temp1, pack.id);
                                }
                                catch (Exception e)
                                {
                                    Error(e);
                                }
                            });
                        }
                        else if (temp[1] == ConfigUtils.Config.Command.LiveUid)
                        {
                            if (temp.Length == 2)
                            {
                                SendGroupMessage("错误的参数", pack.id);
                                break;
                            }
                            string comm = temp[2];
                            if (Tools.IsNumeric(comm))
                            {
                                SendGroupMessage("错误的UID", pack.id);
                                break;
                            }
                            Log($"正在生成直播:{comm}的图片");
                            Task.Run(() =>
                            {
                                try
                                {
                                    var data1 = HttpUtils.GetLiveUID(temp[2]);
                                    if (data1 == null)
                                    {
                                        SendGroupMessage($"获取不到用户直播间：{comm}", pack.id);
                                        return;
                                    }
                                    string room = data1["data"]["roomid"].ToString();
                                    data1 = HttpUtils.GetLive(room);
                                    if (data1 == null)
                                    {
                                        SendGroupMessage($"获取不到直播间：{room}", pack.id);
                                        return;
                                    }
                                    string temp1 = LivePicGen.Gen(data1);
                                    Log($"已生成{temp1}");
                                    SendGroupImage(temp1, pack.id);
                                }
                                catch (Exception e)
                                {
                                    Error(e);
                                }
                            });
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
                            Task.Run(() =>
                            {
                                try
                                {
                                    var data1 = HttpUtils.SearchLive(temp[2]);
                                    if (data1 == null)
                                    {
                                        SendGroupMessage($"搜索不到直播间：{comm}", pack.id);
                                        return;
                                    }
                                    var data2 = data1["data"]["result"]["live_room"] as JArray;
                                    if (data2.Count == 0)
                                    {
                                        SendGroupMessage($"搜索：{comm} 没有结果", pack.id);
                                        return;
                                    }
                                    string room = data2[0]["roomid"].ToString();
                                    data1 = HttpUtils.GetLive(room);
                                    if (data1 == null)
                                    {
                                        SendGroupMessage($"获取不到直播间：{room}", pack.id);
                                        return;
                                    }
                                    string temp1 = LivePicGen.Gen(data1);
                                    Log($"已生成{temp1}");
                                    SendGroupImage(temp1, pack.id);
                                }
                                catch (Exception e)
                                {
                                    Error(e);
                                }
                            });
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
