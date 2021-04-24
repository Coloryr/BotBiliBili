using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BotBiliBili.Config
{
    public class ConfigObj
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public long RunQQ { get; set; }
        public List<long> RunGroup { get; set; }
        public string SESSDATA { get; set; }
        public string bili_jct { get; set; }
        public CommandObj Command { get; set; }
        public int CheckDelay { get; set; }
        public int TimeOut { get; set; }
        public bool AdminSubscribeOnly { get; set; }
        public int WaitTime { get; set; }
        public Dictionary<string, string> RequestHeaders { get; set; }
    }
    public class UidLastSave
    {
        public Dictionary<string, string> Dynamic { get; set; }
        public Dictionary<string, bool> Live { get; set; }
    }
    public class SubscribeObj
    { 
        public ConcurrentDictionary<string, List<long>> Uids { get; set; }
        public ConcurrentDictionary<string, List<long>> Lives { get; set; }
    }
    public class CommandObj
    {
        public string Head { get; set; }
        public string Help { get; set; }
        public string Video { get; set; }
        public string VideoName { get; set; }
        public string Dynamic { get; set; }
        public string DynamicUser { get; set; }
        public string DynamicName { get; set; }
        public string Live { get; set; }
        public string LiveName { get; set; }
        public string LiveUid { get; set; }
        public string SubscribeUid { get; set; }
        public string SubscribeLive { get; set; }
    }

    public class GroupSave
    {
        public long Group { get; set; }
        public List<long> Uid { get; set; }
        public List<long> Fid { get; set; }
        public List<long> Zid { get; set; }
    }
    public abstract class PicSave
    {
        public string BackGround { get; set; }
        public string Font { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string QBack { get; set; }
        public string QPoint { get; set; }
        public Pos QPos { get; set; }
        public int QSize { get; set; }
        public Pos HeadPic { get; set; }
        public float HeadPicSize { get; set; }
        public Pos NamePos { get; set; }
        public float NameSize { get; set; }
        public string NameColor { get; set; }
        public Pos UidPos { get; set; }
        public float UidSize { get; set; }
        public string UidColor { get; set; }
    }

    public class Pos
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
    public class DynamicSave : PicSave
    {
        public Pos StatePos { get; set; }
        public float StateSize { get; set; }
        public string StateColor { get; set; }
        public Pos PicStart { get; set; }
        public int PicWidth { get; set; }
        public int PicPid { get; set; }
        public int TextX { get; set; }
        public int TextPid { get; set; }
        public int TextSize { get; set; }
        public int TextLim { get; set; }
        public int TextDeviation { get; set; }
        public int TextLeft { get; set; }
        public string TextColor { get; set; }
    }

    public class VideoSave : PicSave
    {
        public Pos TitlePos { get; set; }
        public float TitleSize { get; set; }
        public string TitleColor { get; set; }
        public int TitleLim { get; set; }
        public Pos StatePos { get; set; }
        public float StateSize { get; set; }
        public string StateColor { get; set; }
        public Pos PicPos { get; set; }
        public int PicWidth { get; set; }
        public int PicHeight { get; set; }
        public Pos InfoPos { get; set; }
        public float InfoSize { get; set; }
        public string InfoColor { get; set; }
        public int InfoLim { get; set; }
        public int InfoDeviation { get; set; }
        public int InfoLeft { get; set; }
    }
    public class LiveSave : PicSave
    {
        public Pos StatePos { get; set; }
        public float StateSize { get; set; }
        public string StateColor { get; set; }
        public Pos TitlePos { get; set; }
        public float TitleSize { get; set; }
        public string TitleColor { get; set; }
        public int TitleLim { get; set; }
        public Pos LivePos { get; set; }
        public float LiveSize { get; set; }
        public string LiveColor { get; set; }
        public int TextLeft { get; set; }
        public Pos PicPos { get; set; }
        public int PicWidth { get; set; }
        public int PicHeight { get; set; }
        public Pos InfoPos { get; set; }
        public float InfoSize { get; set; }
        public string InfoColor { get; set; }
        public int InfoLim { get; set; }
        public int InfoDeviation { get; set; }
    }
}
