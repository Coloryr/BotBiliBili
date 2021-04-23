using BotBiliBili.PicGen;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace BotBiliBili.Config
{
    class ConfigUtils
    {
        private static object Locker = new object();
        public static ConfigObj Config;
        public static VideoSave VideoPic;
        public static DynamicSave DynamicPic;
        public static LiveSave LivePic;
        public static SubscribeObj Subscribes;
        public static UidLastSave UidLast;

        public static T Load<T>(T obj1, string FilePath) where T : new()
        {
            FileInfo file = new FileInfo(FilePath);
            T obj;
            if (!file.Exists)
            {
                if (obj1 == null)
                    obj = new T();
                else
                    obj = obj1;
                Save(obj, FilePath);
            }
            else
            {
                lock (Locker)
                {
                    obj = JsonConvert.DeserializeObject<T>(File.ReadAllText(FilePath));
                }
            }
            return obj;
        }
        /// <summary>
        /// 保存配置文件
        /// </summary>
        public static void Save(object obj, string FilePath)
        {
            lock (Locker)
            {
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(obj, Formatting.Indented));
            }
        }

        public static void LoadAll()
        {
            Config = Load(new ConfigObj()
            {
                IP = "127.0.0.1",
                Port = 23333,
                RunQQ = 0,
                RunGroup = new(),
                SESSDATA = "",
                bili_jct = "",
                Command = new()
                {
                    Head = "#bili",
                    Help = "help",
                    Video = "video",
                    VideoName = "nvideo",
                    Dynamic = "dynamic",
                    DynamicUser = "duser",
                    DynamicName = "nuser",
                    Live = "live",
                    LiveName = "nlive",
                    LiveUid = "ulive",
                    SubscribeUid = "suid",
                    SubscribeLive = "slive"
                },
                CheckDelay = 1000,
                TimeOut = 10,
                AdminSubscribeOnly = true,
                RequestHeaders = new()
                {
                    { "user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.72 Safari/537.36 Edg/90.0.818.42" }
                }
            }, Program.RunLocal + "config.json");

            VideoPic = Load(new VideoSave()
            {
                BackGround = "#F5F5F5",
                QBack = "#F8F8FF",
                QPoint = "#0000CD",
                QPos = new()
                {
                    X = 460,
                    Y = 20
                },
                QSize = 120,
                Font = "微软雅黑",
                Width = 600,
                Height = 800,
                HeadPic = new()
                {
                    X = 20,
                    Y = 20
                },
                HeadPicSize = 120,
                NamePos = new()
                {
                    X = 155,
                    Y = 20
                },
                NameSize = 20,
                NameColor = "#FF6A6A",
                UidPos = new()
                {
                    X = 160,
                    Y = 75
                },
                UidSize = 15,
                UidColor = "#363636",
                TitlePos = new()
                {
                    X = 20,
                    Y = 155
                },
                TitleSize = 20,
                TitleColor = "#000000",
                TitleLim = 20,
                StatePos = new()
                {
                    X = 30,
                    Y = 195
                },
                StateSize = 15,
                StateColor = "#000000",
                PicPos = new()
                {
                    X = 20,
                    Y = 230
                },
                PicWidth = 560,
                PicHeight = 560,
                InfoPos = new()
                {
                    X = 20,
                    Y = 600
                },
                InfoSize = 20,
                InfoColor = "#000000",
                InfoLim = 20,
                InfoDeviation = 40,
                InfoLeft = 20
            }, Program.RunLocal + "video.json");

            DynamicPic = Load(new DynamicSave()
            {
                BackGround = "#F5F5F5",
                QBack = "#F8F8FF",
                QPoint = "#0000CD",
                QPos = new()
                {
                    X = 460,
                    Y = 20
                },
                QSize = 120,
                Font = "微软雅黑",
                Width = 600,
                Height = 800,
                HeadPic = new()
                {
                    X = 20,
                    Y = 20
                },
                HeadPicSize = 120,
                NamePos = new()
                {
                    X = 155,
                    Y = 20
                },
                NameSize = 20,
                NameColor = "#FF6A6A",
                UidPos = new()
                {
                    X = 160,
                    Y = 75
                },
                UidSize = 15,
                UidColor = "#363636",
                StatePos = new()
                {
                    X = 30,
                    Y = 150
                },
                StateSize = 15,
                StateColor = "#000000",
                PicStart = new()
                {
                    X = 20,
                    Y = 180
                },
                PicWidth = 560,
                PicPid = 10,
                TextX = 20,
                TextPid = 20,
                TextSize = 20,
                TextColor = "#000000",
                TextDeviation = 40,
                TextLeft = 30,
                TextLim = 20
            }, Program.RunLocal + "dynamic.json");

            LivePic = Load(new LiveSave()
            {
                BackGround = "#F5F5F5",
                QBack = "#F8F8FF",
                QPoint = "#0000CD",
                QPos = new()
                {
                    X = 460,
                    Y = 20
                },
                QSize = 120,
                Font = "微软雅黑",
                Width = 600,
                Height = 800,
                HeadPic = new()
                {
                    X = 20,
                    Y = 20
                },
                HeadPicSize = 120,
                NamePos = new()
                {
                    X = 155,
                    Y = 20
                },
                NameSize = 20,
                NameColor = "#FF6A6A",
                UidPos = new()
                {
                    X = 160,
                    Y = 75
                },
                UidSize = 15,
                UidColor = "#363636",
                StatePos = new()
                {
                    X = 30,
                    Y = 190
                },
                StateSize = 15,
                StateColor = "#000000",
                TitlePos = new()
                {
                    X = 20,
                    Y = 150
                },
                TitleSize = 20,
                TitleColor = "#000000",
                TitleLim = 20,
                LivePos = new()
                {
                    X = 160,
                    Y = 120
                },
                LiveSize = 15,
                LiveColor = "#000000",
                TextLeft = 30,
                PicPos = new()
                {
                    X = 20,
                    Y = 220
                },
                PicWidth = 560,
                PicHeight = 560,
                InfoPos = new()
                {
                    X = 20,
                    Y = 540
                },
                InfoSize = 20,
                InfoColor = "#000000",
                InfoLim = 20,
                InfoDeviation = 40,
            }, Program.RunLocal + "live.json");

            Subscribes = Load(new SubscribeObj()
            {
                Lives = new(),
                Uids = new()
            }, Program.RunLocal + "subscribes.json");

            UidLast = Load(new UidLastSave()
            {
                Dynamic = new(),
                Live = new()
            }, Program.RunLocal + "temp.json");
        }
        public static void SaveTemp()
        {
            Save(UidLast, Program.RunLocal + "temp.json");
        }
        public static void SaveSubscribe()
        {
            Save(Subscribes, Program.RunLocal + "subscribes.json");
        }
    }
}
