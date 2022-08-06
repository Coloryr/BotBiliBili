using BotBiliBili.Config;
using BotBiliBili.Utils;
using Newtonsoft.Json.Linq;
using System;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using static Net.Codecrete.QrCodeGenerator.QrSegment;

namespace BotBiliBili.PicGen;

public static class VideoPicGen
{
    private static Color ColorBack;
    private static Color ColorName;
    private static Color ColorUID;
    private static Color ColorTitle;
    private static Color ColorInfo;
    private static Color ColorState;
    private static Font FontName;
    private static Font FontUID;
    private static Font FontTitle;
    private static Font FontInfo;
    private static Font FontState;
    private static Color Qback;
    private static Color Qpoint;
    private static VideoSave Config;
    public static void Init()
    {
        if (!Directory.Exists(Program.RunLocal + "Video"))
            Directory.CreateDirectory(Program.RunLocal + "Video");
        Config = ConfigUtils.VideoPic;
        ColorBack = Color.Parse(Config.BackGround);
        ColorName = Color.Parse(Config.NameColor);
        ColorUID = Color.Parse(Config.UidColor);
        ColorTitle = Color.Parse(Config.TitleColor);
        ColorInfo = Color.Parse(Config.InfoColor);
        ColorState = Color.Parse(Config.StateColor);

        var temp = SystemFonts.Families.Where(a => a.Name == Config.Font).FirstOrDefault();

        FontName = temp.CreateFont(Config.NameSize, FontStyle.Regular);
        FontUID = temp.CreateFont(Config.UidSize, FontStyle.Regular);
        FontTitle = temp.CreateFont(Config.TitleSize, FontStyle.Regular);
        FontInfo = temp.CreateFont(Config.InfoSize, FontStyle.Regular);
        FontState = temp.CreateFont(Config.StateSize, FontStyle.Regular);
        Qback = Color.Parse(Config.QBack);
        Qpoint = Color.Parse(Config.QPoint);
    }
    public static string Gen(JObject obj)
    {
        if (obj == null)
        {
            return null;
        }

        Image<Rgba32> bitmap = new(Config.Width, Config.Height);
        bitmap.Mutate(m =>
        {
            m.Clear(ColorBack);
        });
        JObject data = obj["data"] as JObject;
        string pic_url = data["owner"]["face"].ToString();
        using var pic = Tools.GetImgUrl(pic_url);
        using var pic1 = Tools.ZoomImage(pic, (int)Config.HeadPicSize, (int)Config.HeadPicSize);

        using var code = QrCodeBitmap.ToBitmap($"https://b23.tv/{data["bvid"]}", Qback, Qpoint);
        using var code1 = Tools.ZoomImage(code, Config.QSize, Config.QSize);

        bitmap.Mutate(m =>
        {
            m.DrawImage(pic1, new Point((int)Config.HeadPic.X, (int)Config.HeadPic.Y), 1.0f);
            m.DrawText(data["owner"]["name"].ToString(), FontName, ColorName, 
                new PointF(Config.NamePos.X, Config.NamePos.Y));
            m.DrawText("UID:" + data["owner"]["mid"].ToString(), FontUID, ColorUID, 
                new PointF(Config.UidPos.X, Config.UidPos.Y));
            m.DrawImage(code1, new Point((int)Config.QPos.X, (int)Config.QPos.Y), 1.0f);
        });

        string temp = data["title"].ToString();

        string temp1;
        int c = 0;
        while (true)
        {
            int now = 0;
            if (Config.TitleLim + c > temp.Length)
            {
                temp1 = temp;
                break;
            }
            string temp2 = temp.Substring(now, Config.InfoLim + c);
            var FontNormalOpt = new TextOptions(FontInfo);
            FontRectangle size = TextMeasurer.Measure(temp2, FontNormalOpt);
            if (size.Width > Config.Width - Config.InfoLeft)
            {
                temp1 = string.Concat(temp.AsSpan(now, Config.TitleLim + c - 2), "...");
                break;
            }
            c++;
        }

        DateTime startTime = new(1970, 1, 1, 8, 0, 0);
        DateTime dt = startTime.AddSeconds((long)data["pubdate"]);
        temp = dt.ToString("yyyy/MM/dd HH:mm:ss");

        pic_url = data["pic"].ToString();
        using var pic2 = Tools.GetImgUrl(pic_url);
        using var pic3 = Tools.ZoomImage(pic2, Config.PicHeight, Config.PicWidth);

        bitmap.Mutate(m =>
        {
            m.DrawText(temp1, FontTitle, ColorTitle,
                new PointF(Config.TitlePos.X, Config.TitlePos.Y));
            m.DrawText($"{temp}  观看:{data["stat"]["view"]}  点赞:{data["stat"]["like"]}", 
                FontState, ColorState, new PointF(Config.StatePos.X, Config.StatePos.Y));
            m.DrawImage(pic3,  new Point((int)Config.PicPos.X, (int)Config.PicPos.Y), 1.0f);
        });

        temp = data["desc"].ToString();

        int AllLength = (temp.Length / Config.InfoLim + 2 +
            Tools.SubstringCount(temp, "\n")) * Config.InfoDeviation + (int)Config.InfoPos.Y;
        if (AllLength > bitmap.Height)
        {
            DrawImage(AllLength, ref bitmap);
        }

        string[] list = temp.Split("\n");
        int d = 0;
        float NowY = Config.InfoPos.Y;
        foreach (var item in list)
        {
            int a = 0;
            int now = 0;
            while (true)
            {
                bool last = false;
                d++;
                int b = 0;
                while (true)
                {
                    if (now + Config.InfoLim + b > item.Length)
                    {
                        temp1 = item[now..];
                        last = true;
                        break;
                    }
                    string temp2 = item.Substring(now, Config.InfoLim + b);
                    var FontNormalOpt = new TextOptions(FontInfo);
                    FontRectangle size = TextMeasurer.Measure(temp2, FontNormalOpt);
                    if (size.Width > Config.Width - Config.InfoLeft)
                    {
                        temp1 = item.Substring(now, Config.InfoLim + b - 1);
                        now += temp1.Length;
                        break;
                    }
                    b++;
                }
                bitmap.Mutate(m =>
                {
                    m.DrawText(temp1, FontInfo, ColorInfo, new PointF(Config.InfoPos.X, NowY));
                });
                NowY += Config.InfoDeviation;
                a++;
                if (last)
                    break;
            }
        }
        NowY += Config.InfoDeviation;
        if (NowY < bitmap.Height)
        {
            DrawImage(NowY + Config.InfoDeviation, ref bitmap);
        }

        temp = $"Video/{data["bvid"]}.png";

        bitmap.Save(temp, new PngEncoder()
        { 
            
        });
        bitmap.Dispose();
        return Program.RunLocal + temp;
    }
    private static void DrawImage(float NowY, ref Image<Rgba32> bitmap)
    {
        Image<Rgba32> bitmap1 = new(Config.Width, (int)NowY);
        var bitmap2 = bitmap;
        bitmap1.Mutate(m =>
        {
            m.Clear(ColorBack);
            m.DrawImage(bitmap2, new Point(0, 0), 1.0f);
        });
        bitmap.Dispose();
        bitmap = bitmap1;
    }
}
