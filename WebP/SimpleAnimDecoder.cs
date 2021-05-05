using BotBiliBili.WebP;
using System;
using System.Runtime.InteropServices;

namespace BotBiliBili.Utils
{
    public class SimpleAnimDecoder
    {
        private const int Version = 0x0107;
        /// <summary>
        /// 从WebPData结构体解码动画
        /// </summary>
        /// <param name="webPData">WebPData结构体</param>
        /// <returns>WebP动画</returns>
        public WebPAnimation DecodeFromWebPData(WebPData webPData)
        {
            //从非托管内存空间创建指向webPData结构体的指针
            IntPtr ptrWebPData = Marshal.AllocHGlobal(Marshal.SizeOf(webPData));
            //将webPData复制到非托管内存空间
            Marshal.StructureToPtr(webPData, ptrWebPData, true);
            //解码
            WebPAnimation animation = DecodeFromWebPDataPointer(ptrWebPData);
            //释放开辟的内存
            Marshal.FreeHGlobal(ptrWebPData);
            //将指针置为空
            ptrWebPData = IntPtr.Zero;
            //返回解码后的动画
            return animation;
        }
        /// <summary>
        /// 从指向非托管内存空间的WebPData指针解码
        /// </summary>
        /// <param name="webPDataPointer"></param>
        /// <returns></returns>
        public WebPAnimation DecodeFromWebPDataPointer(IntPtr webPDataPointer)
        {
            //解调WebP数据以提取所有帧、ICC配置文件和EXIF / XMP元数据
            IntPtr demux = LibwebpdemuxUtils.WebPDemuxInternal(webPDataPointer, 0, IntPtr.Zero, Version);

            //创建迭代器，用来遍历动画的每一帧
            WebPIterator iter = new WebPIterator();
            //创建指向迭代器的指针
            IntPtr ptrIter = Marshal.AllocHGlobal(Marshal.SizeOf(iter));
            //给迭代器指针赋初值
            Marshal.StructureToPtr(iter, ptrIter, true);
            //初始化WebP动画结构体，这是本函数要返回的结果
            WebPAnimation animation = new WebPAnimation();

            //遍历所有帧
            if (LibwebpdemuxUtils.WebPDemuxGetFrame(demux, 1, ptrIter) != 0)
            {
                //如果成功获取了第一帧，就创建一个简单解码器
                SimpleDecoder simpleDecoder = new SimpleDecoder();
                do
                {
                    //解引用迭代器指针，恢复出迭代器对象
                    iter = Marshal.PtrToStructure<WebPIterator>(ptrIter);
                    //创建一个动画帧对象
                    WebPAnimationFrame frame = new WebPAnimationFrame();
                    //将迭代器中获得的数据存入动画帧对象中
                    frame.Complete = Convert.ToBoolean(iter.complete);
                    frame.Duration = iter.duration;
                    frame.HasAlpha = Convert.ToBoolean(iter.has_alpha);
                    frame.Height = iter.height;
                    frame.Width = iter.width;
                    frame.XOffset = iter.x_offset;
                    frame.YOffset = iter.y_offset;
                    frame.Image = simpleDecoder.DecodeFromPointer(iter.fragment.bytes, (long)iter.fragment.size);
                    //将动画帧添加到动画对象中
                    animation.Frames.Add(frame);
                } while (LibwebpdemuxUtils.WebPDemuxNextFrame(ptrIter) != 0);
                //释放迭代器
                LibwebpdemuxUtils.WebPDemuxReleaseIterator(ptrIter);
            }
            //释放之前申请的非托管内存空间
            Marshal.FreeHGlobal(ptrIter);
            //指针置为0
            ptrIter = IntPtr.Zero;
            //返回动画对象
            return animation;
        }
        /// <summary>
        /// 从字节数组解码webP动画
        /// </summary>
        /// <param name="data">原始的webp图片</param>
        /// <returns>webP动画</returns>
        public WebPAnimation DecodeFromBytes(byte[] data)
        {
            //将原始数据封装成webPData结构体
            WebPData struWebPData = new WebPData(data);
            //调用其它方法来解码
            WebPAnimation animation = DecodeFromWebPData(struWebPData);
            //释放结构体空间
            struWebPData.Dispose();
            return animation;
        }

    }
}
