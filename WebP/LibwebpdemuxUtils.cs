using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BotBiliBili.WebP
{
    class LibwebpdemuxUtils
    {
        //[DllImport("libwebpdemux_x86.dll", CallingConvention = CallingConvention.Cdecl)]
        //public extern static int WebPGetDemuxVersion_x86();
        //[DllImport("libwebpdemux_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        //public extern static int WebPGetDemuxVersion_x64();

        //public static int WebPGetDemuxVersion()
        //{
        //    switch (IntPtr.Size)
        //    {
        //        case 4:
        //            return WebPGetDemuxVersion_x86();
        //        case 8:
        //            return WebPGetDemuxVersion_x64();
        //        default:
        //            throw new InvalidOperationException("Invalid platform. Can not find proper function");
        //    }
        //}

        [DllImport("libwebpdemux.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int WebPGetDemuxVersion();
        [DllImport("libwebpdemux.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr WebPDemuxInternal(IntPtr data, int allow_partial,
                             IntPtr state, int version);
        [DllImport("libwebpdemux.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void WebPDemuxDelete(IntPtr dmux);
        [DllImport("libwebpdemux.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int WebPDemuxGetI(IntPtr dmux, WebPFormatFeature feature);
        [DllImport("libwebpdemux.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int WebPDemuxGetFrame(IntPtr dmux, int frame, IntPtr iter);
        [DllImport("libwebpdemux.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int WebPDemuxNextFrame(IntPtr iter);
        [DllImport("libwebpdemux.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int WebPDemuxPrevFrame(IntPtr iter);
        [DllImport("libwebpdemux.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void WebPDemuxReleaseIterator(IntPtr iter);
        [DllImport("libwebpdemux.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int WebPDemuxGetChunk(IntPtr dmux,
                     [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] char[] fourcc, int chunk_num, IntPtr iter);
        [DllImport("libwebpdemux.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int WebPDemuxNextChunk(IntPtr iter);
        [DllImport("libwebpdemux.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int WebPDemuxPrevChunk(IntPtr iter);
        [DllImport("libwebpdemux.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void WebPDemuxReleaseChunkIterator(IntPtr iter);


        [DllImport("libwebpdemux.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int WebPAnimDecoderOptionsInitInternal(IntPtr dec_options, int abi_version);
        [DllImport("libwebpdemux.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr WebPAnimDecoderNewInternal(IntPtr webp_data, IntPtr dec_options, int abi_version);
        [DllImport("libwebpdemux.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int WebPAnimDecoderGetInfo(IntPtr dec, IntPtr info);
        [DllImport("libwebpdemux.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int WebPAnimDecoderGetNext(IntPtr dec, ref byte[] buf_ptr, IntPtr timestamp_ptr);
    }
}
