using QRCoder;
using System.Drawing;

namespace BotBiliBili.Utils
{
    class CQode
    {
        public static Bitmap code(string msg, Bitmap bitmap, Color point, Color back)
        {
            QRCodeGenerator code_generator = new QRCodeGenerator();

            QRCodeData code_data = code_generator.CreateQrCode(msg, QRCodeGenerator.ECCLevel.Q);
            QRCode code = new(code_data);
            Bitmap bmp = code.GetGraphic(80, back, point, bitmap, 20, 2, true);

            return bmp;

        }
    }
}
