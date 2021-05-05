using System;
using System.Drawing;

namespace BotBiliBili.Utils
{
    /// <summary>
    /// 帧
    /// </summary>
    public class WebPAnimationFrame : IDisposable
    {
        private int width;
        private int height;
        private int xOffset;
        private int yOffset;
        private int duration;
        private bool hasAlpha;
        private bool complete;
        private Image image;

        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
        public int XOffset { get => xOffset; set => xOffset = value; }
        public int YOffset { get => yOffset; set => yOffset = value; }
        public int Duration { get => duration; set => duration = value; }
        public bool HasAlpha { get => hasAlpha; set => hasAlpha = value; }
        public bool Complete { get => complete; set => complete = value; }
        public Image Image { get => image; set => image = value; }

        public void Dispose()
        {
            image.Dispose();
        }
    }
}
