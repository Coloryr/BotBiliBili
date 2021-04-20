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

namespace BotBiliBili
{
    class VideoPicGen
    {
        private static Color back;
        private static Brush name_color;
        private static Brush uid_color;
        private static Brush title_color;
        private static Brush info_color;
        private static Brush state_color;
        private static Font name_font;
        private static Font uid_font;
        private static Font title_font;
        private static Font info_font;
        private static Font state_font;
        private static Color Qback;
        private static Color Qpoint;
        private static VideoSave Config;
        public static void Init()
        {
            if (!Directory.Exists(Program.RunLocal + "Video"))
                Directory.CreateDirectory(Program.RunLocal + "Video");
            Config = ConfigUtils.Config.VideoPic;
            back = ColorTranslator.FromHtml(Config.BackGround);
            name_color?.Dispose();
            uid_color?.Dispose();
            title_color?.Dispose();
            info_color?.Dispose();
            state_color?.Dispose();
            name_font?.Dispose();
            uid_font?.Dispose();
            title_font?.Dispose();
            info_font?.Dispose();
            state_font?.Dispose();
            name_color = new SolidBrush(ColorTranslator.FromHtml(Config.NameColor));
            uid_color = new SolidBrush(ColorTranslator.FromHtml(Config.UidColor));
            title_color = new SolidBrush(ColorTranslator.FromHtml(Config.TitleColor));
            info_color = new SolidBrush(ColorTranslator.FromHtml(Config.InfoColor));
            state_color = new SolidBrush(ColorTranslator.FromHtml(Config.StateColor));
            name_font = new(Config.Font, Config.NameSize, FontStyle.Regular);
            uid_font = new(Config.Font, Config.UidSize, FontStyle.Regular);
            title_font = new(Config.Font, Config.TitleSize, FontStyle.Regular);
            info_font = new(Config.Font, Config.InfoSize, FontStyle.Regular);
            state_font = new(Config.Font, Config.StateSize, FontStyle.Regular);
            Qback = ColorTranslator.FromHtml(Config.QBack);
            Qpoint = ColorTranslator.FromHtml(Config.QPoint);
        }
        public static string Gen(JObject obj)
        {
            if (obj == null)
            {
                return null;
            }

            Bitmap bitmap = new(
                Config.Width,
                Config.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.Clear(back);
            JObject data = obj["data"] as JObject;
            string pic_url = data["owner"]["face"].ToString();
            using Bitmap pic = Image.FromStream(HttpUtils.GetData(pic_url)) as Bitmap;

            graphics.DrawImage(pic, Config.HeadPic.X,
               Config.HeadPic.Y,
                Config.HeadPicSize,
                Config.HeadPicSize);

            graphics.DrawString(data["owner"]["name"].ToString(), name_font, name_color, Config.NamePos.X, Config.NamePos.Y);

            graphics.DrawString("UID:" + data["owner"]["mid"].ToString(), uid_font, uid_color, Config.UidPos.X, Config.UidPos.Y);
            string sort = $"https://b23.tv/{data["bvid"]}";
            var code = CQode.code(sort, pic, Qback, Qpoint);
            graphics.DrawImage(code, Config.QPos.X, Config.QPos.Y,
                Config.QSize, Config.QSize);

            string temp = data["title"].ToString();
            if (temp.Length > Config.TitleLim)
                temp = temp.Substring(0, Config.TitleLim - 3) + "...";
            graphics.DrawString(temp, title_font, title_color, Config.TitlePos.X, Config.TitlePos.Y);

            DateTime startTime = new(1970, 1, 1);
            DateTime dt = startTime.AddMilliseconds((long)data["pubdate"]);
            temp = dt.ToString("yyyy/MM/dd HH:mm:ss:ffff");
            graphics.DrawString($"{temp} 预览:{data["stat"]["view"]} 点赞:{data["stat"]["like"]}", state_font, state_color, Config.StatePos.X, Config.StatePos.Y);

            pic_url = data["pic"].ToString();
            using Bitmap pic1 = Image.FromStream(HttpUtils.GetData(pic_url)) as Bitmap;
            graphics.DrawImage(Utils.ZoomImage(pic1, Config.PicWidth,
                Config.PicHeight), Config.PicPos.X, Config.PicPos.Y);

            temp = data["desc"].ToString();

            int AllLength = ((temp.Length / Config.InfoLim + 1 + 
                Utils.SubstringCount(temp, "\n")) * Config.InfoDeviation) + (int)Config.InfoPos.Y;
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

            string[] temp2 = temp.Split("\n");
            int d = 0;
            foreach (var item in temp2)
            {
                int a = 0;
                while (true)
                {
                    float NowY = Config.InfoPos.Y + d * Config.InfoDeviation;
                    d++;
                    if (item.Length - a * Config.InfoLim < Config.InfoLim)
                    {
                        string temp1 = item[(a * Config.InfoLim)..];
                        graphics.DrawString(temp1, info_font, info_color, Config.InfoPos.X, NowY);
                        break;
                    }
                    else
                    {
                        string temp1 = item.Substring(a * Config.InfoLim, Config.InfoLim);
                        graphics.DrawString(temp1, info_font, info_color, Config.InfoPos.X, NowY);
                    }
                    a++;
                }
            }

            graphics.Save();

            bitmap.Save($"Video/{data["bvid"]}.jpg");
            graphics.Dispose();
            bitmap.Dispose();
            return Program.RunLocal + $"Video/{data["bvid"]}.jpg";
        }
    }
}
