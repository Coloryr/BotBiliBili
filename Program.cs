using BotBiliBili.Config;
using BotBiliBili.PicGen;
using BotBiliBili.Utils;
using Newtonsoft.Json;
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
            HttpUtils.Check();
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
                        if (temp[1] == ConfigUtils.Config.Command.Video)
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
                                    var data1 = HttpUtils.GetVideoA(temp[2]);
                                    if (data1 == null)
                                    {
                                        SendGroupMessage($"获取不到视频：{comm}", pack.id);
                                        return;
                                    }
                                    string temp1 = VideoPicGen.Gen(data1);
                                    Log($"已生成{temp1}");
                                    SendGroupImage(temp1, pack.id);
                                });
                            }
                            else if (comm.StartsWith("bv"))
                            {
                                Log($"正在生成视频:{comm}的图片");
                                Task.Run(() =>
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
                                });
                            }
                            else
                            {
                                SendGroupMessage("错误的视频号", pack.id);
                            }
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
                                Log($"正在生成动态:{comm}的图片");
                                Task.Run(() =>
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
                                });
                            }
                            else
                            {
                                SendGroupMessage("错误的动态号", pack.id);
                                break;
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
