using BotBiliBili.Config;
using BotBiliBili.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            JObject data = obj["data"]["card"] as JObject;
            string id = data["desc"]["dynamic_id"].ToString();
            string temp = data["card"].ToString();
            JObject data1 = JObject.Parse(temp);
            Bitmap bitmap = new(
                Config.Width,
                Config.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.Clear(back);

            string pic_url = data1["user"]["head_url"].ToString();
            using Bitmap pic = Image.FromStream(HttpUtils.GetData(pic_url)) as Bitmap;

            graphics.DrawImage(pic, Config.HeadPic.X,
               Config.HeadPic.Y,
                Config.HeadPicSize,
                Config.HeadPicSize);

            graphics.DrawString(data1["user"]["name"].ToString(), name_font, name_color, Config.NamePos.X, Config.NamePos.Y);

            graphics.DrawString("UID:" + data1["user"]["uid"].ToString(), uid_font, uid_color, Config.UidPos.X, Config.UidPos.Y);
            string sort = $"https://t.bilibili.com/{id}";
            var code = CQode.code(sort, pic, Qback, Qpoint);
            graphics.DrawImage(code, Config.QPos.X, Config.QPos.Y,
                Config.QSize, Config.QSize);

            DateTime startTime = new(1970, 1, 1);
            DateTime dt = startTime.AddSeconds((long)data["desc"]["timestamp"]);
            temp = dt.ToString("yyyy/MM/dd HH:mm:ss");
            graphics.DrawString($"{temp}  观看:{data["desc"]["view"]}  点赞:{data["desc"]["like"]}", state_font, state_color, Config.StatePos.X, Config.StatePos.Y);

            JArray array = data1["item"]["pictures"] as JArray;
            float xPos = Config.PicStart.X, yPos = Config.PicStart.Y;
            for (int a = 0; a < array.Count; a++)
            {
                string pic_url1 = array[a]["img_src"].ToString();
                Bitmap pic1 = Image.FromStream(HttpUtils.GetData(pic_url1)) as Bitmap;
                pic1 = Tools.ZoomImage(pic1, pic1.Height, Config.PicWidth);
                if (yPos + pic1.Height  > bitmap.Height)
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

            yPos += Config.TextPid;

            temp = data1["item"]["description"].ToString();

            int AllLength = (temp.Length / Config.TextLim + 2 +
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
            string[] list = temp.Split("\n");
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

            temp = $"Dynamic/{id}.jpg";

            graphics.Save();
            bitmap.Save(temp);
            graphics.Dispose();
            bitmap.Dispose();
            return Program.RunLocal + temp;
        }
    }
}
