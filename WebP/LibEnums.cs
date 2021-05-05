using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotBiliBili.WebP
{
    public enum WebPDemuxState
    {
        WEBP_DEMUX_PARSE_ERROR = -1,  // An error occurred while parsing.
        WEBP_DEMUX_PARSING_HEADER = 0,  // Not enough data to parse full header.
        WEBP_DEMUX_PARSED_HEADER = 1,  // Header parsing complete,
                                       // data may be available.
        WEBP_DEMUX_DONE = 2   // Entire file has been parsed.
    }

    public enum WebPFormatFeature
    {
        WEBP_FF_FORMAT_FLAGS,      // bit-wise combination of WebPFeatureFlags
                                   // corresponding to the 'VP8X' chunk (if present).
        WEBP_FF_CANVAS_WIDTH,
        WEBP_FF_CANVAS_HEIGHT,
        WEBP_FF_LOOP_COUNT,        // only relevant for animated file
        WEBP_FF_BACKGROUND_COLOR,  // idem.
        WEBP_FF_FRAME_COUNT        // Number of frames present in the demux object.
                                   // In case of a partial demux, this is the number
                                   // of frames seen so far, with the last frame
                                   // possibly being partial.
    }
    public enum WebPMuxAnimDispose
    {
        WEBP_MUX_DISPOSE_NONE,       // Do not dispose.
        WEBP_MUX_DISPOSE_BACKGROUND  // Dispose to background color.
    }
    public enum WebPMuxAnimBlend_
    {
        WEBP_MUX_BLEND,              // Blend.
        WEBP_MUX_NO_BLEND            // Do not blend.
    }
    public enum VP8StatusCode
    {

        /// VP8_STATUS_OK -> 0
        VP8_STATUS_OK = 0,

        VP8_STATUS_OUT_OF_MEMORY,

        VP8_STATUS_INVALID_PARAM,

        VP8_STATUS_BITSTREAM_ERROR,

        VP8_STATUS_UNSUPPORTED_FEATURE,

        VP8_STATUS_SUSPENDED,

        VP8_STATUS_USER_ABORT,

        VP8_STATUS_NOT_ENOUGH_DATA,
    }
    public enum WEBP_CSP_MODE
    {

        /// MODE_RGB -> 0
        MODE_RGB = 0,

        /// MODE_RGBA -> 1
        MODE_RGBA = 1,

        /// MODE_BGR -> 2
        MODE_BGR = 2,

        /// MODE_BGRA -> 3
        MODE_BGRA = 3,

        /// MODE_ARGB -> 4
        MODE_ARGB = 4,

        /// MODE_RGBA_4444 -> 5
        MODE_RGBA_4444 = 5,

        /// MODE_RGB_565 -> 6
        MODE_RGB_565 = 6,

        /// MODE_rgbA -> 7
        MODE_rgbA = 7,

        /// MODE_bgrA -> 8
        MODE_bgrA = 8,

        /// MODE_Argb -> 9
        MODE_Argb = 9,

        /// MODE_rgbA_4444 -> 10
        MODE_rgbA_4444 = 10,

        /// MODE_YUV -> 11
        MODE_YUV = 11,

        /// MODE_YUVA -> 12
        MODE_YUVA = 12,

        /// MODE_LAST -> 13
        MODE_LAST = 13,
    }
}
