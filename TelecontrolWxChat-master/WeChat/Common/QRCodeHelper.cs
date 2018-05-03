using System.Text;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using System.Drawing;


namespace WeChat.Common
{
    public class QRCodeHelper
    {
        /// <summary> 
        /// 生成二维码 
        /// </summary> 
        /// <param name="qrCodeContent">要编码的内容</param> 
        /// <returns>返回二维码位图</returns> 
        public static Bitmap QRCodeEncoderUtil(string qrCodeContent)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder
            {
                QRCodeVersion = 0
            };
            Bitmap img = qrCodeEncoder.Encode(qrCodeContent, Encoding.UTF8);//指定utf-8编码， 支持中文 
            return img;
        }

        /// <summary> 
        /// 解析二维码 
        /// </summary> 
        /// <param name="bitmap">要解析的二维码位图</param> 
        /// <returns>解析后的字符串</returns> 
        public static string QRCodeDecoderUtil(Bitmap bitmap)
        {
            QRCodeDecoder decoder = new QRCodeDecoder();
            string decodedString = decoder.decode(new QRCodeBitmapImage(bitmap), Encoding.UTF8);//指定utf-8编码， 支持中文 
            return decodedString;
        }
    }
}
