using System;
using System.Collections.Generic;

namespace BotBiliBili.Utils
{
    /// <summary>
    /// 帧的集合构成动画
    /// </summary>
    public class WebPAnimation : IDisposable
    {
        private List<WebPAnimationFrame> frames;

        public int FramesCount { get => frames.Count; }
        public List<WebPAnimationFrame> Frames { get => frames; }
        public WebPAnimation()
        {
            frames = new List<WebPAnimationFrame>();
        }

        public void Dispose()
        {
            foreach (var item in frames)
            {
                item.Dispose();
            }
        }
    }
}
