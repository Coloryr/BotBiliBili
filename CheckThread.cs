using BotBiliBili.Config;
using BotBiliBili.PicGen;
using BotBiliBili.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BotBiliBili
{
    class CheckThread
    {
        private static Thread thread;
        private static bool IsRun;
        private static bool save;
        public static void Start()
        {
            thread = new(Task);
            IsRun = true;
            thread.Start();
        }

        private static void GetDynamic(KeyValuePair<string, List<long>> item)
        {
            var item1 = item.Key;
            Program.Log($"检查用户{item1}动态");
            var obj1 = HttpUtils.GetDynamicUid(item1);
            Thread.Sleep(ConfigUtils.Config.CheckDelay);
            if (!IsRun)
                return;
            if (obj1 == null)
                return;
            var obj2 = obj1["data"]["cards"] as JArray;
            if (obj2.Count == 0)
                return;
            string first = obj2[0]["desc"]["dynamic_id"].ToString();
            long time = (long)obj2[0]["desc"]["timestamp"];
            if (!ConfigUtils.UidLast.Dynamic.ContainsKey(item1))
            {
                var data1 = HttpUtils.GetDynamic(first);
                if (data1 == null)
                {
                    return;
                }
                DynamicObj obj = new()
                {
                    ID = first,
                    Time = time
                };
                ConfigUtils.UidLast.Dynamic.Add(item1, obj);
                save = true;
                string temp1 = DynamicPicGen.Gen(data1);
                Program.Log($"已生成{temp1}");
                foreach (var item3 in item.Value)
                {
                    Program.SendGroupImage(temp1, item3);
                }
            }
            else
            {
                foreach (var item2 in obj2)
                {
                    var obj3 = item2["desc"]["dynamic_id"].ToString();
                    var time1 = (long)item2["desc"]["timestamp"];
                    if (ConfigUtils.UidLast.Dynamic[item1].ID == obj3)
                        break;
                    if (ConfigUtils.UidLast.Dynamic[item1].Time > time1)
                        break;
                    var data1 = HttpUtils.GetDynamic(obj3);
                    if (data1 == null)
                    {
                        continue;
                    }
                    string temp1 = DynamicPicGen.Gen(data1);
                    Program.Log($"已生成{temp1}");
                    foreach (var item3 in item.Value)
                    {
                        Program.SendGroupImage(temp1, item3);
                    }
                }
                ConfigUtils.UidLast.Dynamic[item1] = new()
                {
                    Time = time,
                    ID = first
                };
                save = true;
            }
        }

        private static void GetLive(KeyValuePair<string, List<long>> item)
        {
            var item1 = item.Key;
            Program.Log($"检查用户{item1}直播");
            var obj1 = HttpUtils.GetLiveUID(item1);
            Thread.Sleep(ConfigUtils.Config.CheckDelay);
            if (!IsRun)
                return;
            if (obj1 == null)
                return;
            var obj2 = (int)obj1["data"]["roomStatus"];
            if (obj2 == 0)
                return;
            bool obj3 = (int)obj1["data"]["liveStatus"] == 1;
            if (ConfigUtils.UidLast.Live.ContainsKey(item1))
            {
                if (ConfigUtils.UidLast.Live[item1] == obj3)
                    return;
                else
                    ConfigUtils.UidLast.Live[item1] = obj3;
                save = true;
            }
            else
            {
                ConfigUtils.UidLast.Live.Add(item1, obj3);
                save = true;
            }
            if (!obj3)
                return;

            var data1 = HttpUtils.GetLive(obj1["data"]["roomid"].ToString());
            if (data1 == null)
            {
                return;
            }
            string temp1 = LivePicGen.Gen(data1);
            Program.Log($"已生成{temp1}");
            foreach (var item2 in item.Value)
            {
                Program.SendGroupImage(temp1, item2);
            }
        }

        private static void Task()
        {
            while (IsRun)
            {
                try
                {
                    save = false;
                    foreach (var item in ConfigUtils.Subscribes.Uids)
                    {
                        if (!IsRun)
                            return;
                        GetDynamic(item);
                    }
                    foreach (var item in ConfigUtils.Subscribes.Lives)
                    {
                        if (!IsRun)
                            return;
                        GetLive(item);
                    }
                    if (save)
                    {
                        ConfigUtils.SaveTemp();
                    }
                    Program.Log("检查暂停");
                    for (int a = 0; a < ConfigUtils.Config.WaitTime; a++)
                    {
                        Thread.Sleep(1000);
                        if (!IsRun)
                            return;
                    }
                }
                catch (Exception e)
                {
                    Program.Error(e);
                }
            }
        }

        public static void Stop()
        {
            IsRun = false;
        }
    }
}
