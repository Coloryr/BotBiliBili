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
    class LivePicGen
    {
        private static Color back;
        private static Brush name_color;
        private static Brush uid_color;
        private static Brush title_color;
        private static Brush state_color;
        private static Brush live_color;
        private static Brush info_color;
        private static Font name_font;
        private static Font uid_font;
        private static Font title_font;
        private static Font state_font;
        private static Font live_font;
        private static Font info_font;
        private static Color Qback;
        private static Color Qpoint;
        private static LiveSave Config;
        public static void Init()
        {
            if (!Directory.Exists(Program.RunLocal + "Live"))
                Directory.CreateDirectory(Program.RunLocal + "Live");
            Config = ConfigUtils.LivePic;
            back = ColorTranslator.FromHtml(Config.BackGround);
            name_color?.Dispose();
            uid_color?.Dispose();
            title_color?.Dispose();
            state_color?.Dispose();
            live_color?.Dispose();
            info_color?.Dispose();
            name_font?.Dispose();
            uid_font?.Dispose();
            title_font?.Dispose();
            state_font?.Dispose();
            live_font?.Dispose();
            info_font?.Dispose();
            name_color = new SolidBrush(ColorTranslator.FromHtml(Config.NameColor));
            uid_color = new SolidBrush(ColorTranslator.FromHtml(Config.UidColor));
            title_color = new SolidBrush(ColorTranslator.FromHtml(Config.TitleColor));
            state_color = new SolidBrush(ColorTranslator.FromHtml(Config.StateColor));
            live_color = new SolidBrush(ColorTranslator.FromHtml(Config.LiveColor));
            info_color = new SolidBrush(ColorTranslator.FromHtml(Config.InfoColor));
            name_font = new(Config.Font, Config.NameSize, FontStyle.Regular);
            uid_font = new(Config.Font, Config.UidSize, FontStyle.Regular);
            title_font = new(Config.Font, Config.TitleSize, FontStyle.Regular);
            state_font = new(Config.Font, Config.StateSize, FontStyle.Regular);
            live_font = new(Config.Font, Config.LiveSize, FontStyle.Regular);
            info_font = new(Config.Font, Config.InfoSize, FontStyle.Regular);
            Qback = ColorTranslator.FromHtml(Config.QBack);
            Qpoint = ColorTranslator.FromHtml(Config.QPoint);
        }
        public static string Gen(JObject obj)
        {
            if (obj == null)
            {
                return null;
            }

            JObject data = obj["data"] as JObject;

            string id = data["room_info"]["room_id"].ToString();

            Bitmap bitmap = new(
                Config.Width,
                Config.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.Clear(back);

            string pic_url = data["anchor_info"]["base_info"]["face"].ToString();
            using Bitmap pic = Image.FromStream(HttpUtils.GetData(pic_url)) as Bitmap;

            graphics.DrawImage(pic, Config.HeadPic.X,
               Config.HeadPic.Y,
                Config.HeadPicSize,
                Config.HeadPicSize);

            graphics.DrawString(data["anchor_info"]["base_info"]["uname"].ToString(), name_font, name_color, Config.NamePos.X, Config.NamePos.Y);

            graphics.DrawString("直播间:" + id, live_font, live_color, Config.LivePos.X, Config.LivePos.Y);

            graphics.DrawString("UID:" + data["room_info"]["uid"].ToString(), uid_font, uid_color, Config.UidPos.X, Config.UidPos.Y);
            string sort = $"https://live.bilibili.com/{id}";
            var code = CQode.code(sort, pic, Qback, Qpoint);
            graphics.DrawImage(code, Config.QPos.X, Config.QPos.Y,
                Config.QSize, Config.QSize);

            DateTime startTime = new(1970, 1, 1, 8, 0, 0);
            DateTime dt = startTime.AddSeconds((long)data["room_info"]["live_start_time"]);
            string temp = dt.ToString("HH:mm:ss");
            graphics.DrawString($"开播时间:{temp}  观看:{data["room_info"]["online"]}  分区:{data["room_info"]["area_name"]}", state_font, state_color, Config.StatePos.X, Config.StatePos.Y);

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
                SizeF size = graphics.MeasureString(temp2, title_font);
                if (size.Width > Config.Width - Config.TextLeft)
                {
                    temp1 = temp.Substring(now, Config.TitleLim + c - 2) + "...";
                    break;
                }
                c++;
            }

            graphics.DrawString(temp1, title_font, title_color, Config.TitlePos.X, Config.TitlePos.Y);

            pic_url = data["room_info"]["cover"].ToString();
            using Bitmap pic1 = Image.FromStream(HttpUtils.GetData(pic_url)) as Bitmap;
            graphics.DrawImage(Tools.ZoomImage(pic1,
                Config.PicHeight, Config.PicWidth), Config.PicPos.X, Config.PicPos.Y);

            temp = data["room_info"]["description"].ToString();
            temp = temp.Replace("<p>", "").Replace("</p>", "");

            int AllLength = (temp.Length / Config.InfoLim + 2 +
                Tools.SubstringCount(temp, "\n")) * Config.InfoDeviation + (int)Config.InfoPos.Y;
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

            string[] list = temp.Split("\n");
            int d = 0;
            foreach (var item in list)
            {
                int a = 0;
                int now = 0;
                while (true)
                {
                    bool last = false;
                    float NowY = Config.InfoPos.Y + d * Config.InfoDeviation;
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
                        SizeF size = graphics.MeasureString(temp2, info_font);
                        if (size.Width > Config.Width - Config.TextLeft)
                        {
                            temp1 = item.Substring(now, Config.InfoLim + b - 1);
                            now += temp1.Length;
                            break;
                        }
                        b++;
                    }
                    graphics.DrawString(temp1, info_font, info_color, Config.InfoPos.X, NowY);
                    a++;
                    if (last)
                        break;
                }
            }

            temp = $"Live/{id}.jpg";

            graphics.Save();
            bitmap.Save(temp);
            graphics.Dispose();
            bitmap.Dispose();
            return Program.RunLocal + temp;
        }
    }
}
