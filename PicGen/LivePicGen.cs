using BotBiliBili.Config;
using BotBiliBili.Utils;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Collections.Generic;

namespace BotBiliBili.PicGen;

public static class LivePicGen
{
    private static Color ColorBack;
    private static Color ColorName;
    private static Color ColorUID;
    private static Color ColorTitle;
    private static Color ColorState;
    private static Color ColorLive;
    private static Color ColorInfo;
    private static Font FontName;
    private static Font FontUID;
    private static Font FontTitle;
    private static Font FontState;
    private static Font FontLive;
    private static Font FontInfo;
    private static Color Qback;
    private static Color Qpoint;
    private static LiveSave Config;

    private static TextOptions FontNameOpt;
    private static TextOptions FontUIDOpt;
    private static TextOptions FontTitleOpt;
    private static TextOptions FontStateOpt;
    private static TextOptions FontLiveOpt;
    private static TextOptions FontInfoOpt;

    public static void Init()
    {
        if (!Directory.Exists(Program.RunLocal + "Live"))
            Directory.CreateDirectory(Program.RunLocal + "Live");
        Config = ConfigUtils.LivePic;
        ColorBack = Color.Parse(Config.BackGround);
        ColorName = Color.Parse(Config.NameColor);
        ColorUID = Color.Parse(Config.UidColor);
        ColorTitle = Color.Parse(Config.TitleColor);
        ColorState = Color.Parse(Config.StateColor);
        ColorLive = Color.Parse(Config.LiveColor);
        ColorInfo = Color.Parse(Config.InfoColor);

        var temp = SystemFonts.Families.Where(a => a.Name == Config.Font).FirstOrDefault();

        var FontEmoji = SystemFonts.Families.Where(a => a.Name == Config.Font1).FirstOrDefault();

        FontName = temp.CreateFont(Config.NameSize, FontStyle.Regular);
        FontUID = temp.CreateFont(Config.UidSize, FontStyle.Regular);
        FontTitle = temp.CreateFont(Config.TitleSize, FontStyle.Regular);
        FontState = temp.CreateFont(Config.StateSize, FontStyle.Regular);
        FontLive = temp.CreateFont(Config.LiveSize, FontStyle.Regular);
        FontInfo = temp.CreateFont(Config.InfoSize, FontStyle.Regular);

        FontNameOpt = new(FontName)
        {
            FallbackFontFamilies = new List<FontFamily>() { FontEmoji }
        };
        FontUIDOpt = new(FontUID)
        {
            FallbackFontFamilies = new List<FontFamily>() { FontEmoji }
        };
        FontTitleOpt = new(FontTitle)
        {
            FallbackFontFamilies = new List<FontFamily>() { FontEmoji }
        };
        FontStateOpt = new(FontState)
        {
            FallbackFontFamilies = new List<FontFamily>() { FontEmoji }
        };
        FontLiveOpt = new(FontLive)
        {
            FallbackFontFamilies = new List<FontFamily>() { FontEmoji }
        };
        FontInfoOpt = new(FontInfo)
        {
            FallbackFontFamilies = new List<FontFamily>() { FontEmoji }
        };

        Qback = Color.Parse(Config.QBack);
        Qpoint = Color.Parse(Config.QPoint);
    }

    public static string Gen(JObject obj)
    {
        if (obj == null)
        {
            return null;
        }

        JObject data = obj["data"] as JObject;

        string id = data["room_info"]["room_id"].ToString();

        Image<Rgba32> bitmap = new(Config.Width, Config.Height);
        bitmap.Mutate(m =>
        {
            m.Clear(ColorBack);
        });

        string pic_url = data["anchor_info"]["base_info"]["face"].ToString();
        using var pic = Tools.GetImgUrl(pic_url);
        using var pic1 = Tools.ZoomImage(pic, (int)Config.HeadPicSize, (int)Config.HeadPicSize);

        using var code = QrCodeBitmap.ToBitmap($"https://live.bilibili.com/{id}", Qback, Qpoint);
        using var code1 = Tools.ZoomImage(code, Config.QSize, Config.QSize);

        bitmap.Mutate(m =>
        {
            m.DrawImage(pic1, new Point((int)Config.HeadPic.X, (int)Config.HeadPic.Y), 1.0f);
            m.DrawText(new TextOptions(FontNameOpt)
            {
                Origin = new PointF(Config.NamePos.X, Config.NamePos.Y)
            }, data["anchor_info"]["base_info"]["uname"].ToString(), ColorName);
            m.DrawText(new TextOptions(FontLiveOpt)
            {
                Origin = new PointF(Config.LivePos.X, Config.LivePos.Y)
            }, $"直播间:{id}", ColorLive);
            m.DrawText(new TextOptions(FontUIDOpt)
            {
                Origin = new PointF(Config.UidPos.X, Config.UidPos.Y)
            }, $"UID:{data["room_info"]["uid"]}", ColorUID);
            m.DrawImage(code1, new Point((int)Config.QPos.X, (int)Config.QPos.Y), 1.0f);
        });

        DateTime startTime = new(1970, 1, 1, 8, 0, 0);
        DateTime dt = startTime.AddSeconds((long)data["room_info"]["live_start_time"]);
        string temp = dt.ToString("HH:mm:ss");
        bitmap.Mutate(m =>
        {
            m.DrawText(new TextOptions(FontStateOpt)
            {
                Origin = new PointF(Config.StatePos.X, Config.StatePos.Y)
            }, $"开播时间:{temp} 观看:{data["room_info"]["online"]} 分区:{data["room_info"]["area_name"]}", ColorState);
        });

        temp = data["room_info"]["title"].ToString();

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
            string temp2 = temp.Substring(now, Config.TitleLim + c);
            FontRectangle size = TextMeasurer.Measure(temp2, FontTitleOpt);
            if (size.Width > Config.Width - Config.TextLeft)
            {
                temp1 = string.Concat(temp.AsSpan(now, Config.TitleLim + c - 2), "...");
                break;
            }
            c++;
        }

        bitmap.Mutate(m =>
        {
            m.DrawText(temp1, FontTitle, ColorTitle, new PointF(Config.TitlePos.X, Config.TitlePos.Y));
        });

        pic_url = data["room_info"]["cover"].ToString();
        using var pic2 = Tools.GetImgUrl(pic_url);
        using var pic3 = Tools.ZoomImage(pic2, Config.PicHeight, Config.PicWidth);

        bitmap.Mutate(m =>
        {
            m.DrawImage(pic3, new Point((int)Config.PicPos.X, (int)Config.PicPos.Y), 1.0f);
        });

        temp = data["room_info"]["description"].ToString();
        var doc = new HtmlDocument();
        doc.LoadHtml(temp);

        float NowY = Config.InfoPos.Y;
        DrawStringes(doc.DocumentNode.InnerText, ref bitmap, ref NowY);

        if (NowY < bitmap.Height)
        {
            DrawImage(NowY + Config.InfoDeviation, ref bitmap);
        }

        temp = $"Live/{id}.jpg";

        bitmap.Save(temp, new JpegEncoder()
        {
            Quality = 100
        });
        bitmap.Dispose();
        return Program.RunLocal + temp;
    }

    private static void DrawStringes(string draw, ref Image<Rgba32> bitmap, ref float NowY)
    {
        int count = 0;
        string[] list = draw.Split("\n");
        foreach (var item in list)
        {
            int a = item.Length / Config.InfoLim;
            count += a == 0 ? 1 : a;
        }
        int AllLength = (count + 2 + list.Length) * Config.InfoDeviation + (int)NowY;

        if (AllLength > bitmap.Height)
        {
            DrawImage(AllLength, ref bitmap);
        }
        string temp1;
        int d = 0;
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
                    FontRectangle size = TextMeasurer.Measure(temp2, FontInfoOpt);
                    if (size.Width > Config.Width - Config.TextLeft)
                    {
                        temp1 = item.Substring(now, Config.InfoLim + b - 1);
                        now += temp1.Length;
                        break;
                    }
                    b++;
                }

                float NowY1 = NowY;
                bitmap.Mutate(m =>
                {
                    m.DrawText(new TextOptions(FontInfoOpt)
                    {
                        Origin = new PointF(Config.InfoPos.X, NowY1)
                    }, temp1, ColorInfo);
                });
                NowY += Config.InfoDeviation;
                a++;
                if (last)
                {
                    break;
                }
            }
        }
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
