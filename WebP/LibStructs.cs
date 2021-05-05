using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BotBiliBili.WebP
{
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPIDecoder { }
    [StructLayout(LayoutKind.Sequential)]
    public struct WebPData : IDisposable
    {
        /// <summary>
        /// uint8_t*
        /// </summary>
        public IntPtr bytes;
        /// <summary>
        /// size_t-&gt;unsigned int
        /// 参考https://blog.csdn.net/natahew/article/details/72901682
        /// </summary>
        public UIntPtr size;
        /// <summary>
        /// 使用字节数组初始化WebP对象的实例
        /// </summary>
        /// <param name="data">webP图片文件原始数据</param>
        public WebPData(byte[] data)
        {
            this.bytes = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
            this.size = new UIntPtr((ulong)data.Length);
        }
        /// <summary>
        /// 使用指向原始webp图像的指针初始化webpData的实例
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        public WebPData(IntPtr data, long size)
        {
            this.bytes = data;
            this.size = new UIntPtr((ulong)size);
        }
        public void Dispose()
        {
            bytes = IntPtr.Zero;
            size = UIntPtr.Zero;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WebPIterator
    {
        public int frame_num;
        public int num_frames;          // equivalent to WEBP_FF_FRAME_COUNT.
        public int x_offset, y_offset;  // offset relative to the canvas.
        public int width, height;       // dimensions of this frame.
        public int duration;            // display duration in milliseconds.
        public WebPMuxAnimDispose dispose_method;  // dispose method for the frame.
        public int complete;   // true if 'fragment' contains a full frame. partial images
                               // may still be decoded with the WebP incremental decoder.
        public WebPData fragment;  // The frame given by 'frame_num'. Note for historical
                                   // reasons this is called a fragment.
        public int has_alpha;      // True if the frame contains transparency.
        public WebPMuxAnimBlend_ blend_method;  // Blend operation for the frame.

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] pad;         // padding for later use.
        public IntPtr private_;          // for internal use only.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WebPAnimDecoderOptions
    {
        // Output colorspace. Only the following modes are supported:
        // MODE_RGBA, MODE_BGRA, MODE_rgbA and MODE_bgrA.
        WEBP_CSP_MODE color_mode;
        int use_threads;           // If true, use multi-threaded decoding.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        uint[] padding;       // Padding for later use.
    }
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPRGBABuffer
    {

        /// uint8_t*
        public IntPtr rgba;

        /// int
        public int stride;

        /// size_t->unsigned int
        public UIntPtr size;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPYUVABuffer
    {

        /// uint8_t*
        public IntPtr y;

        /// uint8_t*
        public IntPtr u;

        /// uint8_t*
        public IntPtr v;

        /// uint8_t*
        public IntPtr a;

        /// int
        public int y_stride;

        /// int
        public int u_stride;

        /// int
        public int v_stride;

        /// int
        public int a_stride;

        /// size_t->unsigned int
        public UIntPtr y_size;

        /// size_t->unsigned int
        public UIntPtr u_size;

        /// size_t->unsigned int
        public UIntPtr v_size;

        /// size_t->unsigned int
        public UIntPtr a_size;
    }

    [StructLayoutAttribute(LayoutKind.Explicit)]
    public struct Anonymous_690ed5ec_4c3d_40c6_9bd0_0747b5a28b54
    {

        /// WebPRGBABuffer->Anonymous_47cdec86_3c1a_4b39_ab93_76bc7499076a
        [FieldOffsetAttribute(0)]
        public WebPRGBABuffer RGBA;

        /// WebPYUVABuffer->Anonymous_70de6e8e_c3ae_4506_bef0_c17f17a7e678
        [FieldOffsetAttribute(0)]
        public WebPYUVABuffer YUVA;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPDecBuffer
    {

        /// WEBP_CSP_MODE->Anonymous_cb136f5b_1d5d_49a0_aca4_656a79e9d159
        public WEBP_CSP_MODE colorspace;

        /// int
        public int width;

        /// int
        public int height;

        /// int
        public int is_external_memory;

        /// Anonymous_690ed5ec_4c3d_40c6_9bd0_0747b5a28b54
        public Anonymous_690ed5ec_4c3d_40c6_9bd0_0747b5a28b54 u;

        /// uint32_t[4]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
        public uint[] pad;

        /// uint8_t*
        public IntPtr private_memory;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPBitstreamFeatures
    {

        /// <summary>
        /// Width in pixels, as read from the bitstream
        /// </summary>
        public int width;

        /// <summary>
        /// Height in pixels, as read from the bitstream.
        /// </summary>
        public int height;

        /// <summary>
        /// // True if the bitstream contains an alpha channel.
        /// </summary>
        public int has_alpha;

        /// <summary>
        /// True if the bitstream contains an animation
        /// </summary>
        public int has_animation;

        /// <summary>
        /// 0 = undefined (/mixed), 1 = lossy, 2 = lossless
        /// </summary>
        public int format;


        /// <summary>
        /// Padding for later use
        /// </summary>
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = UnmanagedType.U4)]
        public uint[] pad;
    }




    // Decoding options
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPDecoderOptions
    {
        public int bypass_filtering;               // if true, skip the in-loop filtering
        public int no_fancy_upsampling;            // if true, use faster pointwise upsampler
        public int use_cropping;                   // if true, cropping is applied _first_
        public int crop_left, crop_top;            // top-left position for cropping.
        // Will be snapped to even values.
        public int crop_width, crop_height;        // dimension of the cropping area
        public int use_scaling;                    // if true, scaling is applied _afterward_
        public int scaled_width, scaled_height;    // final resolution
        public int use_threads;                    // if true, use multi-threaded decoding
        public int dithering_strength;             // dithering strength (0=Off, 100=full)

        public int flip;                           // flip output vertically
        public int alpha_dithering_strength;       // alpha dithering strength in [0..100]

        /// uint32_t[5]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = UnmanagedType.U4)]
        public uint[] pad;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct WebPDecoderConfig
    {

        /// WebPBitstreamFeatures->Anonymous_c6b01f0b_3e38_4731_b2d6_9c0e3bdb71aa
        public WebPBitstreamFeatures input;

        /// WebPDecBuffer->Anonymous_5c438b36_7de6_498e_934a_d3613b37f5fc
        public WebPDecBuffer output;

        /// WebPDecoderOptions->Anonymous_78066979_3e1e_4d74_aee5_f316b20e3385
        public WebPDecoderOptions options;
    }
}
