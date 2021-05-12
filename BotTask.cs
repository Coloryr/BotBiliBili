using BotBiliBili.PicGen;
using BotBiliBili.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotBiliBili
{
    class BotTask
    {
        public static void VideoAV(string video, long group)
        {
            Task.Run(() =>
            {
                try
                {
                    var data1 = HttpUtils.GetVideoA(video);
                    if (data1 == null)
                    {
                        Program.SendGroupMessage($"获取不到视频：{video}", group);
                        return;
                    }
                    string temp1 = VideoPicGen.Gen(data1);
                    Program.Log($"已生成{temp1}");
                    Program.SendGroupImage(temp1, group);
                }
                catch (Exception e)
                {
                    Program.Error(e);
                }
            });
        }

        public static void VideoBV(string video, long group)
        {
            Task.Run(() =>
            {
                try
                {
                    var data1 = HttpUtils.GetVideoB(video);
                    if (data1 == null)
                    {
                        Program.SendGroupMessage($"获取不到视频：{video}", group);
                        return;
                    }
                    string temp1 = VideoPicGen.Gen(data1);
                    Program.Log($"已生成{temp1}");
                    Program.SendGroupImage(temp1, group);
                }
                catch (Exception e)
                {
                    Program.Error(e);
                }
            });
        }

        public static void SearchVideo(string name, long group)
        {
            Task.Run(() =>
            {
                try
                {
                    var data1 = HttpUtils.SearchVideo(name);
                    if (data1 == null)
                    {
                        Program.SendGroupMessage($"搜索不到视频：{name}", group);
                        return;
                    }
                    var data2 = data1["data"]["result"] as JArray;
                    if (data2.Count == 0)
                    {
                        Program.SendGroupMessage($"搜索：{name} 没有结果", group);
                        return;
                    }
                    string bid = data2[0]["bvid"].ToString();
                    data1 = HttpUtils.GetVideoB(bid);
                    if (data1 == null)
                    {
                        Program.SendGroupMessage($"获取视频：{bid}", group);
                        return;
                    }
                    string temp1 = VideoPicGen.Gen(data1);
                    Program.Log($"已生成{temp1}");
                    Program.SendGroupImage(temp1, group);
                }
                catch (Exception e)
                {
                    Program.Error(e);
                }
            });
        }

        public static void Dynamic(string id, long group)
        {
            Task.Run(() =>
            {
                try
                {
                    var data1 = HttpUtils.GetDynamic(id);
                    if (data1 == null)
                    {
                        Program.SendGroupMessage($"获取不到动态：{id}", group);
                        return;
                    }
                    string temp1 = DynamicPicGen.Gen(data1);
                    Program.Log($"已生成{temp1}");
                    Program.SendGroupImage(temp1, group);
                }
                catch (Exception e)
                {
                    Program.Error(e);
                }
            });
        }

        public static void DynamicUid(string uid, long group)
        {
            Task.Run(() =>
            {
                try
                {
                    var data1 = HttpUtils.GetDynamic(uid);
                    if (data1 == null)
                    {
                        Program.SendGroupMessage($"获取不到动态：{uid}", group);
                        return;
                    }
                    string temp1 = DynamicPicGen.Gen(data1);
                    Program.Log($"已生成{temp1}");
                    Program.SendGroupImage(temp1, group);
                }
                catch (Exception e)
                {
                    Program.Error(e);
                }
            });
        }

        public static void SearchUserDynamic(string comm, long group)
        {
            Task.Run(() =>
            {
                try
                {
                    var data1 = HttpUtils.SearchUser(comm);
                    if (data1 == null)
                    {
                        Program.SendGroupMessage($"搜索不到用户：{comm}", group);
                        return;
                    }
                    var data2 = data1["data"]["result"] as JArray;
                    if (data2.Count == 0)
                    {
                        Program.SendGroupMessage($"搜索：{comm} 没有结果", group);
                        return;
                    }
                    string id = data2[0]["mid"].ToString();
                    data1 = HttpUtils.GetDynamicUid(id);
                    if (data1 == null)
                    {
                        Program.SendGroupMessage($"获取不到动态：{id}", group);
                        return;
                    }
                    string temp1 = DynamicPicGen.Gen(data1);
                    Program.Log($"已生成{temp1}");
                    Program.SendGroupImage(temp1, group);
                }
                catch (Exception e)
                {
                    Program.Error(e);
                }
            });
        }

        public static void Live(string comm, long group)
        {
            Task.Run(() =>
            {
                try
                {
                    var data1 = HttpUtils.GetLive(comm);
                    if (data1 == null)
                    {
                        Program.SendGroupMessage($"获取不到直播间：{comm}", group);
                        return;
                    }
                    string temp1 = LivePicGen.Gen(data1);
                    Program.Log($"已生成{temp1}");
                    Program.SendGroupImage(temp1, group);
                }
                catch (Exception e)
                {
                    Program.Error(e);
                }
            });
        }
        public static void LiveUser(string comm, long group)
        {
            Task.Run(() =>
            {
                try
                {
                    var data1 = HttpUtils.GetLiveUID(comm);
                    if (data1 == null)
                    {
                        Program.SendGroupMessage($"获取不到用户直播间：{comm}", group);
                        return;
                    }
                    string room = data1["data"]["roomid"].ToString();
                    data1 = HttpUtils.GetLive(room);
                    if (data1 == null)
                    {
                        Program.SendGroupMessage($"获取不到直播间：{room}", group);
                        return;
                    }
                    string temp1 = LivePicGen.Gen(data1);
                    Program.Log($"已生成{temp1}");
                    Program.SendGroupImage(temp1, group);
                }
                catch (Exception e)
                {
                    Program.Error(e);
                }
            });
        }

        public static void LiveName(string comm, long group)
        {
            Task.Run(() =>
            {
                try
                {
                    var data1 = HttpUtils.SearchLive(comm);
                    if (data1 == null)
                    {
                        Program.SendGroupMessage($"搜索不到直播间：{comm}", group);
                        return;
                    }
                    var data2 = data1["data"]["result"]["live_room"] as JArray;
                    if (data2.Count == 0)
                    {
                        Program.SendGroupMessage($"搜索：{comm} 没有结果", group);
                        return;
                    }
                    string room = data2[0]["roomid"].ToString();
                    data1 = HttpUtils.GetLive(room);
                    if (data1 == null)
                    {
                        Program.SendGroupMessage($"获取不到直播间：{room}", group);
                        return;
                    }
                    string temp1 = LivePicGen.Gen(data1);
                    Program.Log($"已生成{temp1}");
                    Program.SendGroupImage(temp1, group);
                }
                catch (Exception e)
                {
                    Program.Error(e);
                }
            });
        }
    }
}
