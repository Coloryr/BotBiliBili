using System;
using System.Runtime.InteropServices;

namespace BotBiliBili.WebP
{
    public partial class LibwebpUtils
    {

        /// Return Type: int
        [DllImport("libwebp", EntryPoint = "WebPGetDecoderVersion", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPGetDecoderVersion();
        /// <summary>
        /// Retrieve basic header information: width, height.
        /// This function will also validate the header and return 0 in
        /// case of formatting error.
        /// Pointers 'width' and 'height' can be passed NULL if deemed irrelevant.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="data_size"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [DllImport("libwebp", EntryPoint = "WebPGetInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPGetInfo([InAttribute()] IntPtr data, UIntPtr data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImport("libwebp", EntryPoint = "WebPDecodeRGBA", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeRGBA([InAttribute()] IntPtr data, UIntPtr data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImport("libwebp", EntryPoint = "WebPDecodeARGB", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeARGB([InAttribute()] IntPtr data, UIntPtr data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImport("libwebp", EntryPoint = "WebPDecodeBGRA", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeBGRA([InAttribute()] IntPtr data, UIntPtr data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImport("libwebp", EntryPoint = "WebPDecodeRGB", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeRGB([InAttribute()] IntPtr data, UIntPtr data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImport("libwebp", EntryPoint = "WebPDecodeBGR", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeBGR([InAttribute()] IntPtr data, UIntPtr data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        ///u: uint8_t**
        ///v: uint8_t**
        ///stride: int*
        ///uv_stride: int*
        [DllImport("libwebp", EntryPoint = "WebPDecodeYUV", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeYUV([InAttribute()] IntPtr data, UIntPtr data_size, ref int width, ref int height, ref IntPtr u, ref IntPtr v, ref int stride, ref int uv_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImport("libwebp", EntryPoint = "WebPDecodeRGBAInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeRGBAInto([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImport("libwebp", EntryPoint = "WebPDecodeARGBInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeARGBInto([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImport("libwebp", EntryPoint = "WebPDecodeBGRAInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeBGRAInto([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImport("libwebp", EntryPoint = "WebPDecodeRGBInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeRGBInto([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImport("libwebp", EntryPoint = "WebPDecodeBGRInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeBGRInto([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///luma: uint8_t*
        ///luma_size: size_t->unsigned int
        ///luma_stride: int
        ///u: uint8_t*
        ///u_size: size_t->unsigned int
        ///u_stride: int
        ///v: uint8_t*
        ///v_size: size_t->unsigned int
        ///v_stride: int
        [DllImport("libwebp", EntryPoint = "WebPDecodeYUVInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeYUVInto([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr luma, UIntPtr luma_size, int luma_stride, IntPtr u, UIntPtr u_size, int u_stride, IntPtr v, UIntPtr v_size, int v_stride);


        /// Return Type: int
        ///param0: WebPDecBuffer*
        ///param1: int
        [DllImport("libwebp", EntryPoint = "WebPInitDecBufferInternal", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPInitDecBufferInternal(ref WebPDecBuffer param0, int param1);


        /// Return Type: void
        ///buffer: WebPDecBuffer*
        [DllImport("libwebp", EntryPoint = "WebPFreeDecBuffer", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WebPFreeDecBuffer(ref WebPDecBuffer buffer);


        /// Return Type: WebPIDecoder*
        ///output_buffer: WebPDecBuffer*
        [DllImport("libwebp", EntryPoint = "WebPINewDecoder", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPINewDecoder(ref WebPDecBuffer output_buffer);


        /// Return Type: WebPIDecoder*
        ///csp: WEBP_CSP_MODE->Anonymous_cb136f5b_1d5d_49a0_aca4_656a79e9d159
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImport("libwebp", EntryPoint = "WebPINewRGB", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPINewRGB(WEBP_CSP_MODE csp, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);


        /// Return Type: WebPIDecoder*
        ///luma: uint8_t*
        ///luma_size: size_t->unsigned int
        ///luma_stride: int
        ///u: uint8_t*
        ///u_size: size_t->unsigned int
        ///u_stride: int
        ///v: uint8_t*
        ///v_size: size_t->unsigned int
        ///v_stride: int
        ///a: uint8_t*
        ///a_size: size_t->unsigned int
        ///a_stride: int
        [DllImport("libwebp", EntryPoint = "WebPINewYUVA", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPINewYUVA(IntPtr luma, UIntPtr luma_size, int luma_stride, IntPtr u, UIntPtr u_size, int u_stride, IntPtr v, UIntPtr v_size, int v_stride, IntPtr a, UIntPtr a_size, int a_stride);


        /// Return Type: WebPIDecoder*
        ///luma: uint8_t*
        ///luma_size: size_t->unsigned int
        ///luma_stride: int
        ///u: uint8_t*
        ///u_size: size_t->unsigned int
        ///u_stride: int
        ///v: uint8_t*
        ///v_size: size_t->unsigned int
        ///v_stride: int
        [DllImport("libwebp", EntryPoint = "WebPINewYUV", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPINewYUV(IntPtr luma, UIntPtr luma_size, int luma_stride, IntPtr u, UIntPtr u_size, int u_stride, IntPtr v, UIntPtr v_size, int v_stride);


        /// Return Type: void
        ///idec: WebPIDecoder*
        [DllImport("libwebp", EntryPoint = "WebPIDelete", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WebPIDelete(ref WebPIDecoder idec);


        /// Return Type: VP8StatusCode->Anonymous_b244cc15_fbc7_4c41_8884_71fe4f515cd6
        ///idec: WebPIDecoder*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        [DllImport("libwebp", EntryPoint = "WebPIAppend", CallingConvention = CallingConvention.Cdecl)]
        public static extern VP8StatusCode WebPIAppend(ref WebPIDecoder idec, [InAttribute()] IntPtr data, UIntPtr data_size);


        /// Return Type: VP8StatusCode->Anonymous_b244cc15_fbc7_4c41_8884_71fe4f515cd6
        ///idec: WebPIDecoder*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        [DllImport("libwebp", EntryPoint = "WebPIUpdate", CallingConvention = CallingConvention.Cdecl)]
        public static extern VP8StatusCode WebPIUpdate(ref WebPIDecoder idec, [InAttribute()] IntPtr data, UIntPtr data_size);


        /// Return Type: uint8_t*
        ///idec: WebPIDecoder*
        ///last_y: int*
        ///width: int*
        ///height: int*
        ///stride: int*
        [DllImport("libwebp", EntryPoint = "WebPIDecGetRGB", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPIDecGetRGB(ref WebPIDecoder idec, ref int last_y, ref int width, ref int height, ref int stride);


        /// Return Type: uint8_t*
        ///idec: WebPIDecoder*
        ///last_y: int*
        ///u: uint8_t**
        ///v: uint8_t**
        ///a: uint8_t**
        ///width: int*
        ///height: int*
        ///stride: int*
        ///uv_stride: int*
        ///a_stride: int*
        [DllImport("libwebp", EntryPoint = "WebPIDecGetYUVA", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPIDecGetYUVA(ref WebPIDecoder idec, ref int last_y, ref IntPtr u, ref IntPtr v, ref IntPtr a, ref int width, ref int height, ref int stride, ref int uv_stride, ref int a_stride);


        /// Return Type: WebPDecBuffer*
        ///idec: WebPIDecoder*
        ///left: int*
        ///top: int*
        ///width: int*
        ///height: int*
        [DllImport("libwebp", EntryPoint = "WebPIDecodedArea", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPIDecodedArea(ref WebPIDecoder idec, ref int left, ref int top, ref int width, ref int height);


        /// Return Type: VP8StatusCode->Anonymous_b244cc15_fbc7_4c41_8884_71fe4f515cd6
        ///param0: uint8_t*
        ///param1: size_t->unsigned int
        ///param2: WebPBitstreamFeatures*
        ///param3: int
        [DllImport("libwebp", EntryPoint = "WebPGetFeaturesInternal", CallingConvention = CallingConvention.Cdecl)]
        public static extern VP8StatusCode WebPGetFeaturesInternal([InAttribute()] IntPtr param0, UIntPtr param1, ref WebPBitstreamFeatures param2, int param3);


        /// Return Type: int
        ///param0: WebPDecoderConfig*
        ///param1: int
        [DllImport("libwebp", EntryPoint = "WebPInitDecoderConfigInternal", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPInitDecoderConfigInternal(ref WebPDecoderConfig param0, int param1);


        /// Return Type: WebPIDecoder*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///config: WebPDecoderConfig*
        [DllImport("libwebp", EntryPoint = "WebPIDecode", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPIDecode([InAttribute()] IntPtr data, UIntPtr data_size, ref WebPDecoderConfig config);


        /// Return Type: VP8StatusCode->Anonymous_b244cc15_fbc7_4c41_8884_71fe4f515cd6
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///config: WebPDecoderConfig*
        [DllImport("libwebp", EntryPoint = "WebPDecode", CallingConvention = CallingConvention.Cdecl)]
        public static extern VP8StatusCode WebPDecode([InAttribute()] IntPtr data, UIntPtr data_size, ref WebPDecoderConfig config);

    }


}