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
        public static void Start()
        {
            thread = new(Task);
            IsRun = true;
            thread.Start();
        }

        private static bool GetDynamic(KeyValuePair<string, List<long>> item)
        {
            var item1 = item.Key;
            Program.Log($"检查用户{item1}动态");
            var obj1 = HttpUtils.GetDynamicUid(item1);
            Thread.Sleep(ConfigUtils.Config.CheckDelay);
            if (!IsRun)
                return false;
            if (obj1 == null)
                return false;
            var obj2 = obj1["data"]["cards"] as JArray;
            if (obj2.Count == 0)
                return false;
            string first = obj2[0]["desc"]["dynamic_id"].ToString();
            bool save = false;
            if (!ConfigUtils.UidLast.Dynamic.ContainsKey(item1))
            {
                var data1 = HttpUtils.GetDynamic(first);
                if (data1 == null)
                {
                    return save;
                }
                ConfigUtils.UidLast.Dynamic.Add(item1, first);
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
                    if (ConfigUtils.UidLast.Dynamic[item1] == obj3)
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
                ConfigUtils.UidLast.Dynamic[item1] = first;
                save = true;
            }
            return save;
        }

        private static bool GetLive(KeyValuePair<string, List<long>> item)
        {
            var item1 = item.Key;
            Program.Log($"检查用户{item1}直播");
            var obj1 = HttpUtils.GetLiveUID(item1);
            Thread.Sleep(ConfigUtils.Config.CheckDelay);
            if (!IsRun)
                return false;
            if (obj1 == null)
                return false;
            var obj2 = (int)obj1["data"]["roomStatus"];
            if (obj2 == 0)
                return false;
            bool obj3 = (int)obj1["data"]["liveStatus"] == 1;
            bool save;
            if (ConfigUtils.UidLast.Live.ContainsKey(item1))
            {
                if (ConfigUtils.UidLast.Live[item1] == obj3)
                    return false;
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
                return save;

            var data1 = HttpUtils.GetLive(obj1["data"]["roomid"].ToString());
            if (data1 == null)
            {
                return save;
            }
            string temp1 = LivePicGen.Gen(data1);
            Program.Log($"已生成{temp1}");
            foreach (var item2 in item.Value)
            {
                Program.SendGroupImage(temp1, item2);
            }
            return save;
        }

        private static void Task()
        {
            while (IsRun)
            {
                try
                {
                    bool save = false;
                    foreach (var item in ConfigUtils.Subscribes.Uids)
                    {
                        try
                        {
                            if (!IsRun)
                                return;
                            save = GetDynamic(item);
                        }
                        catch
                        {
                            Program.Error($"获取{item.Key}动态错误，进行重试");
                            if (!IsRun)
                                return;
                            try
                            {
                                save = GetDynamic(item);
                            }
                            catch (Exception e)
                            {
                                Program.Error($"获取{item.Key}动态失败");
                                Program.Error(e);
                            }
                        }
                    }
                    foreach (var item in ConfigUtils.Subscribes.Lives)
                    {
                        try
                        {
                            if (!IsRun)
                                return;
                            save = GetLive(item);
                        }
                        catch
                        {
                            Program.Error($"获取{item.Key}直播错误，进行重试");
                            if (!IsRun)
                                return;
                            try
                            {
                                save = GetLive(item);
                            }
                            catch (Exception e)
                            {
                                Program.Error($"获取{item.Key}直播失败");
                                Program.Error(e);
                            }
                        }
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
