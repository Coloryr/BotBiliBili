using BotBiliBili.Config;
using BotBiliBili.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace BotBiliBili.PicGen;

public static class DynamicPicGen
{
    private static Color ColorBack;
    private static Color ColorName;
    private static Color ColorUID;
    private static Color ColorState;
    private static Color ColorText;
    private static Font FontName;
    private static Font FontUID;
    private static Font FontState;
    private static Font FontText;
    private static Color Qback;
    private static Color Qpoint;
    private static DynamicSave Config;
    public static void Init()
    {
        if (!Directory.Exists(Program.RunLocal + "Dynamic"))
            Directory.CreateDirectory(Program.RunLocal + "Dynamic");
        Config = ConfigUtils.DynamicPic;
        ColorBack = Color.Parse(Config.BackGround);
        ColorName = Color.Parse(Config.NameColor);
        ColorUID = Color.Parse(Config.UidColor);
        ColorState = Color.Parse(Config.StateColor);
        ColorText = Color.Parse(Config.TextColor);

        var temp = SystemFonts.Families.Where(a => a.Name == Config.Font).FirstOrDefault();

        FontName = temp.CreateFont(Config.NameSize, FontStyle.Regular);
        FontUID = temp.CreateFont(Config.UidSize, FontStyle.Regular);
        FontState = temp.CreateFont(Config.StateSize, FontStyle.Regular);
        FontText = temp.CreateFont(Config.TextSize, FontStyle.Regular);
        Qback = Color.Parse(Config.QBack);
        Qpoint = Color.Parse(Config.QPoint);
    }

    public static string Gen(JObject obj)
    {
        if (obj == null)
            return null;
        if (obj["data"]?["card"] is not JObject data)
        {
            data = obj["data"]["cards"][0] as JObject;
        }
        string id = data["desc"]["dynamic_id"].ToString();

        JObject desc = data["desc"] as JObject;
        Image<Rgba32> bitmap = new(Config.Width, Config.Height);
        string pic_url = desc["user_profile"]["info"]["face"].ToString();

        using var pic = Tools.GetImgUrl(pic_url);
        using var pic1 = Tools.ZoomImage(pic, (int)Config.HeadPicSize, (int)Config.HeadPicSize);

        using var code = QrCodeBitmap.ToBitmap($"https://t.bilibili.com/{id}", Qback, Qpoint);
        using var code1 = Tools.ZoomImage(code, Config.QSize, Config.QSize);

        DateTime startTime = new(1970, 1, 1, 8, 0, 0);
        DateTime dt = startTime.AddSeconds((long)data["desc"]["timestamp"]);
        string temp = dt.ToString("yyyy/MM/dd HH:mm:ss");

        float NowY = Config.PicStart.Y;

        bitmap.Mutate(m =>
        {
            m.Clear(ColorBack);
            m.DrawImage(pic1, new Point((int)Config.HeadPic.X, (int)Config.HeadPic.Y), 1.0f);
            m.DrawText(desc["user_profile"]["info"]["uname"].ToString(),
                FontName, ColorName, new PointF(Config.NamePos.X, Config.NamePos.Y));
            m.DrawText("UID:" + desc["user_profile"]["info"]["uid"].ToString(),
                FontUID, ColorUID, new PointF(Config.UidPos.X, Config.UidPos.Y));
            m.DrawImage(code1, new Point((int)Config.QPos.X, (int)Config.QPos.Y), 1.0f);
            m.DrawText($"{temp}  观看:{data["desc"]["view"]}  点赞:{data["desc"]["like"]}", FontState, ColorState, new PointF(Config.StatePos.X, Config.StatePos.Y));
        });

        int type = (int)desc["type"];
        try
        {
            switch (type)
            {
                case 1:
                    Type1(JObject.Parse(data["card"].ToString()), ref bitmap, ref NowY);
                    break;
                case 2:
                    Type2(JObject.Parse(data["card"].ToString()), ref bitmap, ref NowY);
                    break;
                case 4:
                    Type4(JObject.Parse(data["card"].ToString()), ref bitmap, ref NowY);
                    break;
                case 8:
                    Type8(JObject.Parse(data["card"].ToString()), ref bitmap, ref NowY);
                    break;
                case 64:
                    Type64(JObject.Parse(data["card"].ToString()), ref bitmap, ref NowY);
                    break;
                case 2048:
                    Type2048(JObject.Parse(data["card"].ToString()), ref bitmap, ref NowY);
                    break;
            }
        }
        catch (Exception e)
        {
            File.WriteAllText($"Dynamic/{id}.json", obj.ToString());
            Program.Error(e);
        }

        if (data["display"] is JObject display)
        {
            if (display["add_on_card_info"] is JArray add)
            {
                foreach (var item in add)
                {
                    int type1 = (int)item["add_on_card_show_type"];
                    if (type1 == 5)
                    {
                        AType5(item["ugc_attach_card"] as JObject, ref bitmap, ref NowY);
                    }
                }
            }
        }

        if (NowY < bitmap.Height)
        {
            DrawImage(NowY + Config.TextDeviation, ref bitmap);
        }

        temp = $"Dynamic/{id}.jpg";

        bitmap.Save(temp, new JpegEncoder()
        {
            Quality = 100
        });
        bitmap.Dispose();
        return Program.RunLocal + temp;
    }

    private static void Type8(JObject data, ref Image<Rgba32> bitmap, ref float NowY)
    {
        string dynamic = data["dynamic"].ToString();
        string desc = data["desc"].ToString();
        if (dynamic.Length == 0)
            dynamic = "发布视频：";
        else
        {
            if (dynamic != desc)
                dynamic = "发布视频：\n" + dynamic;
            else
                dynamic = "发布视频：";
        }

        string temp1;
        DrawStringes(dynamic, ref bitmap, ref NowY);

        NowY += 10;
        float NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            var rect1 = new RectangularPolygon(Config.PicStart.X, NowY1, Config.Width - Config.PicStart.X * 2, 2);
            m.Fill(Color.Black, rect1);
        });
        NowY += 18;

        string title = data["title"].ToString();
        int c = 0;
        while (true)
        {
            int now = 0;
            if (Config.TextLim + c > title.Length)
            {
                temp1 = title;
                break;
            }
            string temp2 = title.Substring(now, Config.TextLim + c);
            var FontNormalOpt = new TextOptions(FontText);
            FontRectangle size = TextMeasurer.Measure(temp2, FontNormalOpt);
            if (size.Width > Config.Width - Config.TextLeft)
            {
                temp1 = string.Concat(title.AsSpan(now, Config.TextLim + c - 2), "...");
                break;
            }
            c++;
        }

        NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            m.DrawText(temp1, FontText, ColorText, new PointF(Config.TextX, NowY1));
        });

        NowY += Config.TextDeviation + 10;

        float xPos = Config.PicStart.X;

        string pic_url = data["pic"].ToString();
        Image pic1 = Tools.GetImgUrl(pic_url);
        pic1 = Tools.ZoomImage(pic1, pic1.Height, Config.PicWidth);
        if (NowY + pic1.Height > bitmap.Height)
        {
            DrawImage(NowY + pic1.Height, ref bitmap);
        }

        NowY1 = NowY;
        bitmap.Mutate(m => 
        {
            m.DrawImage(pic1, new Point((int)xPos, (int)NowY1), 1.0f);
        });
        NowY += pic1.Height + Config.PicPid;

        pic1.Dispose();

        DrawStringes(desc, ref bitmap, ref NowY);
    }

    private static void Type8_1(JObject data, ref Image<Rgba32> bitmap, ref float NowY)
    {
        string dynamic = data["dynamic"].ToString();
        string desc = data["desc"].ToString();
        if (dynamic.Length == 0)
            dynamic = "发布视频：";
        else
        {
            if (dynamic != desc)
                dynamic = "发布视频：\n" + dynamic;
            else
                dynamic = "发布视频：";
        }

        DrawStringes(dynamic, ref bitmap, ref NowY);

        NowY += 10;
        float NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            var rect1 = new RectangularPolygon(Config.PicStart.X, NowY1, Config.Width - Config.PicStart.X * 2, 2);
            m.Fill(Color.Black, rect1);
        });
        NowY += 18;

        string title = data["title"].ToString();
        int c = 0;
        string temp1;
        while (true)
        {
            int now = 0;
            if (Config.TextLim + c > title.Length)
            {
                temp1 = title;
                break;
            }
            string temp2 = title.Substring(now, Config.TextLim + c);
            var FontNormalOpt = new TextOptions(FontText);
            FontRectangle size = TextMeasurer.Measure(temp2, FontNormalOpt);
            if (size.Width > Config.Width - Config.TextLeft)
            {
                temp1 = string.Concat(title.AsSpan(now, Config.TextLim + c - 2), "...");
                break;
            }
            c++;
        }

        NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            m.DrawText(temp1, FontText, ColorText, new PointF(Config.TextX, NowY1));
        });

        NowY += Config.TextDeviation + 10;

        float xPos = Config.PicStart.X;

        string pic_url = data["pic"].ToString();
        Image pic1 = Tools.GetImgUrl(pic_url);
        pic1 = Tools.ZoomImage(pic1, pic1.Height, Config.PicWidth);
        if (NowY + pic1.Height > bitmap.Height)
        {
            DrawImage(NowY + pic1.Height, ref bitmap);
        }

        NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            m.DrawImage(pic1, new Point((int)xPos, (int)NowY1), 1.0f);
        });

        NowY += pic1.Height + Config.PicPid;

        pic1.Dispose();

        DrawStringes(desc, ref bitmap, ref NowY);
    }

    private static void Type1(JObject data, ref Image<Rgba32> bitmap, ref float NowY)
    {
        JObject data1 = data["item"] as JObject;
        string content = data1["content"].ToString();
        content = "转发动态：\n" + content;

        DrawStringes(content, ref bitmap, ref NowY);

        NowY += 10;

        float NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            var rect1 = new RectangularPolygon(Config.PicStart.X, NowY1, Config.Width - Config.PicStart.X * 2, 2);
            m.Fill(Color.Black, rect1);
        });
        NowY += 18;

        var origin_user = data["origin_user"] as JObject;
        var info = origin_user["info"] as JObject;
        if (info.ContainsKey("uname"))
        {
            NowY1 = NowY;
            bitmap.Mutate(m =>
            {
                m.DrawText($"{info["uname"]} UID:{info["uid"]}",
                    FontState, ColorName, new PointF(Config.StatePos.X, NowY1));
            });

            NowY += Config.StateSize + 20;
        }

        var origin_extend_json = JObject.Parse(data["origin_extend_json"].ToString());
        var item_type = data1["orig_type"]?.ToString();
        var origin_type = origin_extend_json["repeat_resource"]?["items"]?[0]?["type"]?.ToString();
        if (item_type != null || origin_type != null)
        {
            if (origin_type == "8")
            {
                Type8_2(JObject.Parse(data["origin"].ToString()), ref bitmap, ref NowY);
            }
            if (item_type == "2")
            {
                Type2(JObject.Parse(data["origin"].ToString()), ref bitmap, ref NowY);
            }
            if (item_type == "64")
            {
                Type64(JObject.Parse(data["origin"].ToString()), ref bitmap, ref NowY);
            }
            if (item_type == "8")
            {
                Type8_1(JObject.Parse(data["origin"].ToString()), ref bitmap, ref NowY);
            }
            if (item_type == "4")
            {
                Type4(JObject.Parse(data["origin"].ToString()), ref bitmap, ref NowY);
            }
        }
        else
        {
            Type2(JObject.Parse(data["origin"].ToString()), ref bitmap, ref NowY);
        }
    }

    private static void Type2(JObject data1, ref Image<Rgba32> bitmap, ref float NowY)
    {
        float xPos = Config.PicStart.X;

        float NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            m.DrawText("发布动态：", FontText, ColorText, new PointF(Config.TextX, NowY1));
        });

        NowY += Config.TextDeviation + 10;
        NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            var rect1 = new RectangularPolygon(Config.PicStart.X, NowY1, Config.Width - Config.PicStart.X * 2, 2);
            m.Fill(Color.Black, rect1);
        });
        NowY += 18;

        if (data1["item"]["pictures"] is JArray array)
        {
            List<Image> piclist = new();
            for (int a = 0; a < array.Count; a++)
            {
                string pic_url1 = array[a]["img_src"].ToString();
                Image pic1 = Tools.GetImgUrl(pic_url1);
                piclist.Add(pic1);
            }

            while (true)
            {
                if (piclist.Count >= 3)
                {
                    var item1 = piclist[0];
                    var item2 = piclist[1];
                    var item3 = piclist[2];
                    if (item1.Width == item1.Height && item2.Width == item2.Height && item3.Width == item3.Height)
                    {
                        if (item1.Width == item2.Width && item2.Width == item3.Width)
                        {
                            using Image pic1 = Tools.ZoomImage(item1, item1.Height, Config.PicWidth / 3);
                            using Image pic2 = Tools.ZoomImage(item2, item2.Height, Config.PicWidth / 3);
                            using Image pic3 = Tools.ZoomImage(item3, item3.Height, Config.PicWidth / 3);
                            if (NowY + pic1.Height > bitmap.Height)
                            {
                                DrawImage(NowY + pic1.Height, ref bitmap);
                            }
                            int temp1 = Config.PicWidth / 3;

                            NowY1 = NowY;
                            bitmap.Mutate(m =>
                            {
                                m.DrawImage(pic1, new Point((int)xPos, (int)NowY1), 1.0f);
                                m.DrawImage(pic2, new Point((int)xPos + temp1, (int)NowY1), 1.0f);

                                m.DrawImage(pic3, new Point((int)xPos + temp1 + temp1, (int)NowY1), 1.0f);
                            });

                            NowY += pic1.Height;
                            piclist.Remove(item1);
                            piclist.Remove(item2);
                            piclist.Remove(item3);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            NowY += Config.PicPid;

            foreach (var item in piclist)
            {
                Image pic1 = Tools.ZoomImage(item, item.Height, Config.PicWidth);
                if (NowY + pic1.Height > bitmap.Height)
                {
                    DrawImage(NowY + pic1.Height, ref bitmap);
                }

                NowY1 = NowY;
                bitmap.Mutate(m =>
                {
                    m.DrawImage(pic1, new Point((int)xPos, (int)NowY1), 1.0f);
                });
                NowY += pic1.Height + Config.PicPid;

                pic1.Dispose();
            }
        }

        string temp;
        var itemObj = data1["item"] as JObject;
        if (itemObj.ContainsKey("description"))
            temp = itemObj["description"].ToString();
        else if (itemObj.ContainsKey("content"))
            temp = itemObj["content"].ToString();
        else if (itemObj.ContainsKey("dynamic"))
            temp = itemObj["dynamic"].ToString();
        else
            temp = "";

        DrawStringes(temp, ref bitmap, ref NowY);
    }

    private static void Type2048(JObject data1, ref Image<Rgba32> bitmap, ref float NowY)
    {
        float xPos = Config.PicStart.X;

        float NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            m.DrawText("哔哩哔哩漫画社区精选：", FontText, ColorText, new PointF(Config.TextX, NowY1));
        });

        NowY += Config.TextDeviation + 10;

        NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            var rect1 = new RectangularPolygon(Config.PicStart.X, NowY1, Config.Width - Config.PicStart.X * 2, 2);
            m.Fill(Color.Black, rect1);
        });
        NowY += 18;

        string pic_url = data1["sketch"]["cover_url"].ToString();
        Image pic1 = Tools.GetImgUrl(pic_url);
        int Width = (int)(Config.Width * 0.2);
        pic1 = Tools.ZoomImage(pic1, pic1.Height, Width);
        if (NowY + pic1.Height > bitmap.Height)
        {
            DrawImage(NowY + pic1.Height, ref bitmap);
        }

        NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            m.DrawImage(pic1, new Point((int)xPos, (int)NowY1), 1.0f);
        });
        NowY += Config.PicPid;

        string temp = data1["sketch"]["desc_text"].ToString();

        DrawStringes(temp, ref bitmap, ref NowY);

        temp = data1["vest"]["content"].ToString();
        NowY += pic1.Height;
        pic1.Dispose();
        DrawStringes(temp, ref bitmap, ref NowY);
    }

    private static void Type8_2(JObject data, ref Image<Rgba32> bitmap, ref float NowY)
    {

        float xPos = Config.PicStart.X;

        float NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            m.DrawText($"电影：{data["apiSeasonInfo"]["title"]}", FontText, ColorText, new PointF(Config.TextX, NowY1));
        });

        NowY += Config.TextDeviation + 10;

        NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            var rect1 = new RectangularPolygon(Config.PicStart.X, NowY1, Config.Width - Config.PicStart.X * 2, 2);
            m.Fill(Color.Black, rect1);
        });
        NowY += 18;

        string pic_url = data["cover"].ToString();
        Image pic1 = Tools.GetImgUrl(pic_url);
        pic1 = Tools.ZoomImage(pic1, pic1.Height, Config.PicWidth);
        if (NowY + pic1.Height > bitmap.Height)
        {
            DrawImage(NowY + pic1.Height + Config.HeadPic.Y, ref bitmap);
        }

        NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            m.DrawImage(pic1, new Point((int)xPos, (int)NowY1), 1.0f);
        });

        NowY += pic1.Height;

        pic1.Dispose();
    }

    private static void Type64(JObject data, ref Image<Rgba32> bitmap, ref float NowY)
    {

        float xPos = Config.PicStart.X;

        float NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            m.DrawText($"发布公告：", FontText, ColorText, new PointF(Config.TextX, NowY1));
        });

        NowY += Config.TextDeviation + 10;

        NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            var rect1 = new RectangularPolygon(Config.PicStart.X, NowY1, Config.Width - Config.PicStart.X * 2, 2);
            m.Fill(Color.Black, rect1);
        });
        NowY += 18;

        if (data["image_urls"] is JArray array)
        {
            for (int a = 0; a < array.Count; a++)
            {
                string pic_url = array[a].ToString();
                Image pic1 = Tools.GetImgUrl(pic_url);
                pic1 = Tools.ZoomImage(pic1, pic1.Height, Config.PicWidth);
                if (NowY + pic1.Height > bitmap.Height)
                {
                    DrawImage(NowY + pic1.Height, ref bitmap);
                }
                NowY1 = NowY;
                bitmap.Mutate(m =>
                {
                    m.DrawImage(pic1, new Point((int)xPos, (int)NowY1), 1.0f);
                });
                NowY += pic1.Height + Config.PicPid;

                pic1.Dispose();
            }
        }

        string temp = data["summary"].ToString() + "...";

        DrawStringes(temp, ref bitmap, ref NowY);
    }

    private static void Type4(JObject data, ref Image<Rgba32> bitmap, ref float NowY)
    {
        float xPos = Config.PicStart.X;

        float NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            m.DrawText($"发布动态：", FontText, ColorText, new PointF(Config.TextX, NowY1));
        });

        NowY += Config.TextDeviation + 10;

        NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            var rect1 = new RectangularPolygon(Config.PicStart.X, NowY1, Config.Width - Config.PicStart.X * 2, 2);
            m.Fill(Color.Black, rect1);
        });

        NowY += 18;

        if (data["image_urls"] is JArray array)
        {
            for (int a = 0; a < array.Count; a++)
            {
                string pic_url = array[a].ToString();
                Image pic1 = Tools.GetImgUrl(pic_url);
                pic1 = Tools.ZoomImage(pic1, pic1.Height, Config.PicWidth);
                if (NowY + pic1.Height > bitmap.Height)
                {
                    DrawImage(NowY + pic1.Height, ref bitmap);
                }

                NowY1 = NowY;
                bitmap.Mutate(m =>
                {
                    m.DrawImage(pic1, new Point((int)xPos, (int)NowY1), 1.0f);
                });
                NowY += pic1.Height + Config.PicPid;

                pic1.Dispose();
            }
        }

        string temp = data["item"]["content"].ToString();
        DrawStringes(temp, ref bitmap, ref NowY);
    }

    private static void AType5(JObject data, ref Image<Rgba32> bitmap, ref float NowY)
    {
        float xPos = Config.PicStart.X;

        float NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            m.DrawText($"关联视频：", FontText, ColorText, new PointF(Config.TextX, NowY1));
        });

        NowY += Config.TextDeviation + 10;

        NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            var rect1 = new RectangularPolygon(Config.PicStart.X, NowY1, Config.Width - Config.PicStart.X * 2, 2);
            m.Fill(Color.Black, rect1);
        });
        NowY += 18;

        string pic_url = data["image_url"].ToString();
        Image pic1 = Tools.GetImgUrl(pic_url);
        pic1 = Tools.ZoomImage(pic1, pic1.Height, Config.PicWidth);
        if (NowY + pic1.Height > bitmap.Height)
        {
            DrawImage(NowY + pic1.Height, ref bitmap);
        }

        NowY1 = NowY;
        bitmap.Mutate(m =>
        {
            m.DrawImage(pic1, new Point((int)xPos, (int)NowY1), 1.0f);
        });

        NowY += pic1.Height + Config.PicPid;

        pic1.Dispose();

        string temp = data["title"].ToString();
        DrawStringes(temp, ref bitmap, ref NowY);
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

    private static void DrawStringes(string draw, ref Image<Rgba32> bitmap, ref float NowY)
    {
        int count = 0;
        string[] list = draw.Split("\n");
        foreach (var item in list)
        {
            int a = item.Length / Config.TextLim;
            count += a == 0 ? 1 : a;
        }
        int AllLength = (count + 2 +
            Tools.SubstringCount(draw, "\n")) * Config.TextDeviation + (int)NowY;

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
                    if (now + Config.TextLim + b > item.Length)
                    {
                        temp1 = item[now..];
                        last = true;
                        break;
                    }
                    string temp2 = item.Substring(now, Config.TextLim + b);
                    var FontNormalOpt = new TextOptions(FontText);
                    FontRectangle size = TextMeasurer.Measure(temp2, FontNormalOpt);
                    if (size.Width > Config.Width - Config.TextLeft)
                    {
                        temp1 = item.Substring(now, Config.TextLim + b - 1);
                        now += temp1.Length;
                        break;
                    }
                    b++;
                }
                float NowY1 = NowY;
                bitmap.Mutate(m =>
                {
                    m.DrawText(temp1, FontText, ColorText, new PointF(Config.TextX, NowY1));
                });
                NowY += Config.TextDeviation;
                a++;
                if (last)
                {
                    break;
                }
            }
        }
    }
}
