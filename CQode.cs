using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotBiliBili
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
