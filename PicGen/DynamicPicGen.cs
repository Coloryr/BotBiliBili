using BotBiliBili.Config;
using BotBiliBili.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;

namespace BotBiliBili.PicGen
{
    class DynamicPicGen
    {
        private static Color back;
        private static Brush name_color;
        private static Brush uid_color;
        private static Brush state_color;
        private static Brush text_color;
        private static Font name_font;
        private static Font uid_font;
        private static Font state_font;
        private static Font text_font;
        private static Color Qback;
        private static Color Qpoint;
        private static DynamicSave Config;
        public static void Init()
        {
            if (!Directory.Exists(Program.RunLocal + "Dynamic"))
                Directory.CreateDirectory(Program.RunLocal + "Dynamic");
            Config = ConfigUtils.DynamicPic;
            back = ColorTranslator.FromHtml(Config.BackGround);
            name_color?.Dispose();
            uid_color?.Dispose();
            state_color?.Dispose();
            text_color?.Dispose();
            name_font?.Dispose();
            uid_font?.Dispose();
            state_font?.Dispose();
            text_font?.Dispose();
            name_color = new SolidBrush(ColorTranslator.FromHtml(Config.NameColor));
            uid_color = new SolidBrush(ColorTranslator.FromHtml(Config.UidColor));
            state_color = new SolidBrush(ColorTranslator.FromHtml(Config.StateColor));
            text_color = new SolidBrush(ColorTranslator.FromHtml(Config.TextColor));
            name_font = new(Config.Font, Config.NameSize, FontStyle.Regular);
            uid_font = new(Config.Font, Config.UidSize, FontStyle.Regular);
            state_font = new(Config.Font, Config.StateSize, FontStyle.Regular);
            text_font = new(Config.Font, Config.TextSize, FontStyle.Regular);
            Qback = ColorTranslator.FromHtml(Config.QBack);
            Qpoint = ColorTranslator.FromHtml(Config.QPoint);
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
            Bitmap bitmap = new(
                Config.Width,
                Config.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.Clear(back);

            string pic_url = desc["user_profile"]["info"]["face"].ToString();
            using Bitmap pic = Image.FromStream(HttpUtils.GetData(pic_url)) as Bitmap;

            graphics.DrawImage(pic, Config.HeadPic.X,
               Config.HeadPic.Y,
                Config.HeadPicSize,
                Config.HeadPicSize);

            graphics.DrawString(desc["user_profile"]["info"]["uname"].ToString(), name_font, name_color, Config.NamePos.X, Config.NamePos.Y);

            graphics.DrawString("UID:" + desc["user_profile"]["info"]["uid"].ToString(), uid_font, uid_color, Config.UidPos.X, Config.UidPos.Y);
            string sort = $"https://t.bilibili.com/{id}";
            var code = CQode.code(sort, pic, Qback, Qpoint);
            graphics.DrawImage(code, Config.QPos.X, Config.QPos.Y,
                Config.QSize, Config.QSize);

            DateTime startTime = new(1970, 1, 1, 8, 0, 0);
            DateTime dt = startTime.AddSeconds((long)data["desc"]["timestamp"]);
            string temp = dt.ToString("yyyy/MM/dd HH:mm:ss");
            graphics.DrawString($"{temp}  观看:{data["desc"]["view"]}  点赞:{data["desc"]["like"]}", state_font, state_color, Config.StatePos.X, Config.StatePos.Y);

            int type = (int)desc["type"];
            try
            {
                switch (type)
                {
                    case 1:
                        Type1(JObject.Parse(data["card"].ToString()), ref bitmap, ref graphics);
                        break;
                    case 2:
                        Type2(JObject.Parse(data["card"].ToString()), ref bitmap, ref graphics);
                        break;
                    case 8:
                        Type8(JObject.Parse(data["card"].ToString()), ref bitmap, ref graphics);
                        break;
                    case 64:
                        Type64(JObject.Parse(data["card"].ToString()), ref bitmap, ref graphics);
                        break;
                    case 2048:
                        Type2048(JObject.Parse(data["card"].ToString()), ref bitmap, ref graphics);
                        break;
                }
            }
            catch (Exception e)
            {
                File.WriteAllText($"Dynamic/{id}.json", obj.ToString());
                Program.Error(e);
            }
            temp = $"Dynamic/{id}.jpg";

            graphics.Save();
            bitmap.Save(temp);
            graphics.Dispose();
            bitmap.Dispose();
            return Program.RunLocal + temp;
        }

        private static void Type8(JObject data, ref Bitmap bitmap, ref Graphics graphics)
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
            string[] list = dynamic.Split("\n");
            int d = 0;
            float NowY = 0;
            foreach (var item in list)
            {
                int a = 0;
                int now = 0;
                while (true)
                {
                    bool last = false;
                    NowY = Config.PicStart.Y + d * Config.TextDeviation;
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
                        SizeF size = graphics.MeasureString(temp2, text_font);
                        if (size.Width > Config.Width - Config.TextLeft)
                        {
                            temp1 = item.Substring(now, Config.TextLim + b - 1);
                            now += temp1.Length;
                            break;
                        }
                        b++;
                    }
                    graphics.DrawString(temp1, text_font, text_color, Config.TextX, NowY);
                    a++;
                    if (last)
                    {
                        break;
                    }
                }
            }
            NowY += Config.TextDeviation + 10;
            graphics.DrawRectangle(new Pen(Brushes.Black, 2), Config.PicStart.X, NowY, Config.Width - Config.PicStart.X * 2, 2);
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
                SizeF size = graphics.MeasureString(temp2, text_font);
                if (size.Width > Config.Width - Config.TextLeft)
                {
                    temp1 = title.Substring(now, Config.TextLim + c - 2) + "...";
                    break;
                }
                c++;
            }

            graphics.DrawString(temp1, text_font, text_color, Config.TextX, NowY);

            NowY += Config.TextDeviation + 10;

            float xPos = Config.PicStart.X;

            string pic_url = data["pic"].ToString();
            Bitmap pic1 = Image.FromStream(HttpUtils.GetData(pic_url)) as Bitmap;
            pic1 = Tools.ZoomImage(pic1, pic1.Height, Config.PicWidth);
            if (NowY + pic1.Height > bitmap.Height)
            {
                graphics.Save();
                Bitmap bitmap1 = new(Config.Width, (int)(NowY + pic1.Height));
                graphics = Graphics.FromImage(bitmap1);
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                graphics.Clear(back);
                graphics.DrawImage(bitmap, 0, 0);
                bitmap.Dispose();
                bitmap = bitmap1;
            }
            graphics.DrawImage(pic1, xPos,
               NowY, pic1.Width, pic1.Height);
            NowY += pic1.Height + Config.PicPid;

            pic1.Dispose();

            int count = 0;
            list = desc.Split("\n");
            foreach (var item in list)
            {
                int a = item.Length / Config.TextLim;
                count += a == 0 ? 1 : a;
            }
            int AllLength = (count +
                Tools.SubstringCount(desc, "\n")) * Config.TextDeviation + (int)NowY;

            if (AllLength > bitmap.Height)
            {
                Bitmap bitmap1 = new(Config.Width, AllLength);
                graphics.Save();
                graphics.Dispose();
                graphics = Graphics.FromImage(bitmap1);
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                graphics.Clear(back);
                graphics.DrawImage(bitmap, 0, 0);
                bitmap.Dispose();
                bitmap = bitmap1;
            }

            list = desc.Split("\n");
            d = 0;
            float yPos = NowY;
            foreach (var item in list)
            {
                int a = 0;
                int now = 0;
                while (true)
                {
                    bool last = false;
                    NowY = yPos + d * Config.TextDeviation;
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
                        SizeF size = graphics.MeasureString(temp2, text_font);
                        if (size.Width > Config.Width - Config.TextLeft)
                        {
                            temp1 = item.Substring(now, Config.TextLim + b - 1);
                            now += temp1.Length;
                            break;
                        }
                        b++;
                    }
                    graphics.DrawString(temp1, text_font, text_color, Config.TextX, NowY);
                    a++;
                    if (last)
                        break;
                }
            }
        }

        private static void Type8_1(JObject data, ref Bitmap bitmap, ref Graphics graphics, float y = 0)
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
            string[] list = dynamic.Split("\n");
            int d = 0;
            float NowY = y;

            int count = 0;
            foreach (var item in list)
            {
                int a = item.Length / Config.TextLim;
                count += a == 0 ? 1 : a;
            }
            int AllLength = (count +
                Tools.SubstringCount(dynamic, "\n")) * Config.TextDeviation + (int)NowY;

            if (AllLength > bitmap.Height)
            {
                Bitmap bitmap1 = new(Config.Width, AllLength);
                graphics.Save();
                graphics.Dispose();
                graphics = Graphics.FromImage(bitmap1);
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                graphics.Clear(back);
                graphics.DrawImage(bitmap, 0, 0);
                bitmap.Dispose();
                bitmap = bitmap1;
            }

            foreach (var item in list)
            {
                int a = 0;
                int now = 0;
                while (true)
                {
                    bool last = false;
                    NowY = y + d * Config.TextDeviation;
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
                        SizeF size = graphics.MeasureString(temp2, text_font);
                        if (size.Width > Config.Width - Config.TextLeft)
                        {
                            temp1 = item.Substring(now, Config.TextLim + b - 1);
                            now += temp1.Length;
                            break;
                        }
                        b++;
                    }
                    graphics.DrawString(temp1, text_font, text_color, Config.TextX, NowY);
                    a++;
                    if (last)
                    {
                        break;
                    }
                }
            }

            bitmap.Save("test.jpg");

            NowY += Config.TextDeviation + 10;
            graphics.DrawRectangle(new Pen(Brushes.Black, 2), Config.PicStart.X, NowY, Config.Width - Config.PicStart.X * 2, 2);
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
                SizeF size = graphics.MeasureString(temp2, text_font);
                if (size.Width > Config.Width - Config.TextLeft)
                {
                    temp1 = title.Substring(now, Config.TextLim + c - 2) + "...";
                    break;
                }
                c++;
            }

            graphics.DrawString(temp1, text_font, text_color, Config.TextX, NowY);

            NowY += Config.TextDeviation + 10;

            float xPos = Config.PicStart.X;

            string pic_url = data["pic"].ToString();
            Bitmap pic1 = Image.FromStream(HttpUtils.GetData(pic_url)) as Bitmap;
            pic1 = Tools.ZoomImage(pic1, pic1.Height, Config.PicWidth);
            if (NowY + pic1.Height > bitmap.Height)
            {
                graphics.Save();
                Bitmap bitmap1 = new(Config.Width, (int)(NowY + pic1.Height));
                graphics = Graphics.FromImage(bitmap1);
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                graphics.Clear(back);
                graphics.DrawImage(bitmap, 0, 0);
                bitmap.Dispose();
                bitmap = bitmap1;
            }
            graphics.DrawImage(pic1, xPos,
               NowY, pic1.Width, pic1.Height);
            NowY += pic1.Height + Config.PicPid;

            pic1.Dispose();

            count = 0;
            list = desc.Split("\n");
            foreach (var item in list)
            {
                int a = item.Length / Config.TextLim;
                count += a == 0 ? 1 : a;
            }
            AllLength = (count +
                Tools.SubstringCount(desc, "\n")) * Config.TextDeviation + (int)NowY;

            if (AllLength > bitmap.Height)
            {
                Bitmap bitmap1 = new(Config.Width, AllLength);
                graphics.Save();
                graphics.Dispose();
                graphics = Graphics.FromImage(bitmap1);
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                graphics.Clear(back);
                graphics.DrawImage(bitmap, 0, 0);
                bitmap.Dispose();
                bitmap = bitmap1;
            }

            d = 0;
            float yPos = NowY;
            foreach (var item in list)
            {
                int a = 0;
                int now = 0;
                while (true)
                {
                    bool last = false;
                    NowY = yPos + d * Config.TextDeviation;
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
                        SizeF size = graphics.MeasureString(temp2, text_font);
                        if (size.Width > Config.Width - Config.TextLeft)
                        {
                            temp1 = item.Substring(now, Config.TextLim + b - 1);
                            now += temp1.Length;
                            break;
                        }
                        b++;
                    }
                    graphics.DrawString(temp1, text_font, text_color, Config.TextX, NowY);
                    a++;
                    if (last)
                        break;
                }
            }
        }

        private static void Type1(JObject data, ref Bitmap bitmap, ref Graphics graphics)
        {
            JObject data1 = data["item"] as JObject;
            string content = data1["content"].ToString();
            content = "转发动态：\n" + content;
            string temp1;
            float yPos = Config.PicStart.Y;
            string[] list = content.Split("\n");
            int d = 0;
            float NowY = 0;
            foreach (var item in list)
            {
                int a = 0;
                int now = 0;
                while (true)
                {
                    bool last = false;
                    NowY = yPos + d * Config.TextDeviation;
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
                        SizeF size = graphics.MeasureString(temp2, text_font);
                        if (size.Width > Config.Width - Config.TextLeft)
                        {
                            temp1 = item.Substring(now, Config.TextLim + b - 1);
                            now += temp1.Length;
                            break;
                        }
                        b++;
                    }
                    graphics.DrawString(temp1, text_font, text_color, Config.TextX, NowY);
                    a++;
                    if (last)
                    {
                        break;
                    }
                }
            }
            NowY += Config.TextDeviation + 10;
            graphics.DrawRectangle(new Pen(Brushes.Black, 2), Config.PicStart.X, NowY, Config.Width - Config.PicStart.X * 2, 2);
            NowY += 18;

            var origin_user = data["origin_user"] as JObject;
            var info = origin_user["info"] as JObject;
            if (info.ContainsKey("uname"))
            {
                graphics.DrawString(info["uname"].ToString() + " UID:" + info["uid"].ToString(), state_font, name_color, Config.StatePos.X, NowY);

                NowY += Config.StateSize + 20;
            }

            var origin_extend_json = JObject.Parse(data["origin_extend_json"].ToString());
            var item_type = data1["orig_type"]?.ToString();
            var origin_type = origin_extend_json["repeat_resource"]?["items"]?[0]?["type"]?.ToString();
            if (item_type != null || origin_type != null)
            {
                if (origin_type == "8")
                {
                    Type8(JObject.Parse(data["origin"].ToString()), ref bitmap, ref graphics, NowY);
                }
                if (item_type == "2")
                {
                    Type2(JObject.Parse(data["origin"].ToString()), ref bitmap, ref graphics, NowY);
                }
                if (item_type == "8")
                {
                    Type8_1(JObject.Parse(data["origin"].ToString()), ref bitmap, ref graphics, NowY);
                }
            }
            else
            {
                Type2(JObject.Parse(data["origin"].ToString()), ref bitmap, ref graphics, NowY);
            }
        }

        private static void Type2(JObject data1, ref Bitmap bitmap, ref Graphics graphics, float y = 0)
        {

            float xPos = Config.PicStart.X, yPos = y == 0 ? Config.PicStart.Y : y;

            graphics.DrawString("发布动态：", text_font, text_color, Config.TextX, yPos);

            yPos += Config.TextDeviation + 10;
            graphics.DrawRectangle(new Pen(Brushes.Black, 2), Config.PicStart.X, yPos, Config.Width - Config.PicStart.X * 2, 2);
            yPos += 18;

            if (data1["item"]["pictures"] is JArray array)
            {
                for (int a = 0; a < array.Count; a++)
                {
                    string pic_url1 = array[a]["img_src"].ToString();
                    Bitmap pic1 = Image.FromStream(HttpUtils.GetData(pic_url1)) as Bitmap;
                    pic1 = Tools.ZoomImage(pic1, pic1.Height, Config.PicWidth);
                    if (yPos + pic1.Height > bitmap.Height)
                    {
                        graphics.Save();
                        Bitmap bitmap1 = new(Config.Width, (int)(yPos + pic1.Height));
                        graphics = Graphics.FromImage(bitmap1);
                        graphics.InterpolationMode = InterpolationMode.High;
                        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                        graphics.Clear(back);
                        graphics.DrawImage(bitmap, 0, 0);
                        bitmap.Dispose();
                        bitmap = bitmap1;
                    }
                    graphics.DrawImage(pic1, xPos,
                       yPos, pic1.Width, pic1.Height);
                    yPos += pic1.Height + Config.PicPid;

                    pic1.Dispose();
                }
            }

            yPos += Config.TextPid;

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

            int count =0 ;
            string[] list = temp.Split("\n");
            foreach (var item in list)
            {
                if (string.IsNullOrWhiteSpace(item))
                    continue;
                int a = item.Length / Config.TextLim;
                count += a == 0 ? 1 : a;
            }

            int AllLength = (count +
                Tools.SubstringCount(temp, "\n")) * Config.TextDeviation + (int)yPos;
            if (AllLength > bitmap.Height)
            {
                Bitmap bitmap1 = new(Config.Width, AllLength);
                graphics.Save();
                graphics.Dispose();
                graphics = Graphics.FromImage(bitmap1);
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                graphics.Clear(back);
                graphics.DrawImage(bitmap, 0, 0);
                bitmap.Dispose();
                bitmap = bitmap1;
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
                    float NowY = yPos + d * Config.TextDeviation;
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
                        SizeF size = graphics.MeasureString(temp2, text_font);
                        if (size.Width > Config.Width - Config.TextLeft)
                        {
                            temp1 = item.Substring(now, Config.TextLim + b - 1);
                            now += temp1.Length;
                            break;
                        }
                        b++;
                    }
                    graphics.DrawString(temp1, text_font, text_color, Config.TextX, NowY);
                    a++;
                    if (last)
                        break;
                }
            }
        }

        private static void Type2048(JObject data1, ref Bitmap bitmap, ref Graphics graphics, float y = 0)
        {

            float xPos = Config.PicStart.X, yPos = y == 0 ? Config.PicStart.Y : y;

            graphics.DrawString("哔哩哔哩漫画社区精选：", text_font, text_color, Config.TextX, yPos);

            yPos += Config.TextDeviation + 10;
            graphics.DrawRectangle(new Pen(Brushes.Black, 2), Config.PicStart.X, yPos, Config.Width - Config.PicStart.X * 2, 2);
            yPos += 18;

            string pic_url1 = data1["sketch"]["cover_url"].ToString();
            Bitmap pic1 = Image.FromStream(HttpUtils.GetData(pic_url1)) as Bitmap;
            int Width = (int)(Config.Width * 0.2);
            pic1 = Tools.ZoomImage(pic1, pic1.Height, Width);
            if (yPos + pic1.Height > bitmap.Height)
            {
                graphics.Save();
                Bitmap bitmap1 = new(Config.Width, (int)(yPos + pic1.Height));
                graphics = Graphics.FromImage(bitmap1);
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                graphics.Clear(back);
                graphics.DrawImage(bitmap, 0, 0);
                bitmap.Dispose();
                bitmap = bitmap1;
            }
            graphics.DrawImage(pic1, xPos,
               yPos, pic1.Width, pic1.Height);
            yPos += Config.PicPid;

            string temp = data1["sketch"]["desc_text"].ToString();

            int count = 0;
            string[] list = temp.Split("\n");
            foreach (var item in list)
            {
                int a = item.Length / Config.TextLim;
                count += a == 0 ? 1 : a;
            }
            int AllLength = (count +
                Tools.SubstringCount(temp, "\n")) * Config.TextDeviation + (int)yPos;
            if (AllLength > bitmap.Height)
            {
                Bitmap bitmap1 = new(Config.Width, AllLength);
                graphics.Save();
                graphics.Dispose();
                graphics = Graphics.FromImage(bitmap1);
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                graphics.Clear(back);
                graphics.DrawImage(bitmap, 0, 0);
                bitmap.Dispose();
                bitmap = bitmap1;
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
                    float NowY = yPos + d * Config.TextDeviation;
                    d++;
                    int b = 0;
                    while (true)
                    {
                        if (now + Config.TextLim - 15 + b > item.Length)
                        {
                            temp1 = item[now..];
                            last = true;
                            break;
                        }
                        string temp2 = item.Substring(now, Config.TextLim - 15 + b);
                        SizeF size = graphics.MeasureString(temp2, text_font);
                        if (size.Width > Config.Width - Config.TextLeft - Width - Config.TextPid)
                        {
                            temp1 = item.Substring(now, Config.TextLim - 15 + b - 1);
                            now += temp1.Length;
                            break;
                        }
                        b++;
                    }
                    graphics.DrawString(temp1, text_font, text_color, xPos + Width + Config.TextPid, NowY);
                    a++;
                    if (last)
                        break;
                }
            }

            temp = data1["vest"]["content"].ToString();
            list = temp.Split("\n");
            d = 0;
            yPos += pic1.Height;
            pic1.Dispose();
            foreach (var item in list)
            {
                int a = 0;
                int now = 0;
                while (true)
                {
                    bool last = false;
                    float NowY = yPos + d * Config.TextDeviation;
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
                        SizeF size = graphics.MeasureString(temp2, text_font);
                        if (size.Width > Config.Width - Config.TextLeft)
                        {
                            temp1 = item.Substring(now, Config.TextLim + b - 1);
                            now += temp1.Length;
                            break;
                        }
                        b++;
                    }
                    graphics.DrawString(temp1, text_font, text_color, Config.TextX, NowY);
                    a++;
                    if (last)
                        break;
                }
            }
        }

        private static void Type8(JObject data, ref Bitmap bitmap, ref Graphics graphics, float y = 0)
        {

            float xPos = Config.PicStart.X, yPos = y == 0 ? Config.PicStart.Y : y;

            graphics.DrawString($"电影：{data["apiSeasonInfo"]["title"]}", text_font, text_color, Config.TextX, yPos);

            yPos += Config.TextDeviation;
            graphics.DrawRectangle(new Pen(Brushes.Black, 2), Config.PicStart.X, yPos, Config.Width - Config.PicStart.X * 2, 2);
            yPos += 18;

            yPos += Config.TextDeviation;

            string pic_url = data["cover"].ToString();
            Bitmap pic1 = Image.FromStream(HttpUtils.GetData(pic_url)) as Bitmap;
            pic1 = Tools.ZoomImage(pic1, pic1.Height, Config.PicWidth);
            if (yPos + pic1.Height > bitmap.Height)
            {
                graphics.Save();
                Bitmap bitmap1 = new(Config.Width, (int)(yPos + pic1.Height));
                graphics = Graphics.FromImage(bitmap1);
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                graphics.Clear(back);
                graphics.DrawImage(bitmap, 0, 0);
                bitmap.Dispose();
                bitmap = bitmap1;
            }
            graphics.DrawImage(pic1, xPos,
               yPos, pic1.Width, pic1.Height);

            pic1.Dispose();
        }

        private static void Type64(JObject data, ref Bitmap bitmap, ref Graphics graphics, float y = 0)
        {

            float xPos = Config.PicStart.X, yPos = y == 0 ? Config.PicStart.Y : y;

            graphics.DrawString($"游戏公告：", text_font, text_color, Config.TextX, yPos);

            yPos += Config.TextDeviation;
            graphics.DrawRectangle(new Pen(Brushes.Black, 2), Config.PicStart.X, yPos, Config.Width - Config.PicStart.X * 2, 2);
            yPos += 18;

            if (data["image_urls"] is JArray array)
            {
                for (int a = 0; a < array.Count; a++)
                {
                    string pic_url1 = array[a].ToString();
                    Bitmap pic1 = Image.FromStream(HttpUtils.GetData(pic_url1)) as Bitmap;
                    pic1 = Tools.ZoomImage(pic1, pic1.Height, Config.PicWidth);
                    if (yPos + pic1.Height > bitmap.Height)
                    {
                        graphics.Save();
                        Bitmap bitmap1 = new(Config.Width, (int)(yPos + pic1.Height));
                        graphics = Graphics.FromImage(bitmap1);
                        graphics.InterpolationMode = InterpolationMode.High;
                        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                        graphics.Clear(back);
                        graphics.DrawImage(bitmap, 0, 0);
                        bitmap.Dispose();
                        bitmap = bitmap1;
                    }
                    graphics.DrawImage(pic1, xPos,
                       yPos, pic1.Width, pic1.Height);
                    yPos += pic1.Height + Config.PicPid;

                    pic1.Dispose();
                }
            }

            string temp = data["summary"].ToString() + "...";

            int count = 0;
            string[] list = temp.Split("\n");
            foreach (var item in list)
            {
                int a = item.Length / Config.TextLim;
                count += a == 0 ? 1 : a;
            }
            int AllLength = (count +
                Tools.SubstringCount(temp, "\n")) * Config.TextDeviation + (int)yPos;
            if (AllLength > bitmap.Height)
            {
                Bitmap bitmap1 = new(Config.Width, AllLength);
                graphics.Save();
                graphics.Dispose();
                graphics = Graphics.FromImage(bitmap1);
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                graphics.Clear(back);
                graphics.DrawImage(bitmap, 0, 0);
                bitmap.Dispose();
                bitmap = bitmap1;
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
                    float NowY = yPos + d * Config.TextDeviation;
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
                        SizeF size = graphics.MeasureString(temp2, text_font);
                        if (size.Width > Config.Width - Config.TextLeft)
                        {
                            temp1 = item.Substring(now, Config.TextLim + b - 1);
                            now += temp1.Length;
                            break;
                        }
                        b++;
                    }
                    graphics.DrawString(temp1, text_font, text_color, Config.TextX, NowY);
                    a++;
                    if (last)
                        break;
                }
            }
        }
    }
}
