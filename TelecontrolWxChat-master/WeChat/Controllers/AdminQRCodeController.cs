using System.Web.Mvc;
using System.Drawing;
using WeChat.Common;
using System.Collections.Generic;

namespace WeChat.Controllers
{
    public class AdminQRCodeController : Controller
    {
        //
        // GET: /AdminQRCode/
        public ActionResult Index() => View();

        //[HttpPost]

        public JsonResult GetORImage(string content)
        {

            var first = content.Substring(0, 1);
            var QRName = "";
            var QRType = "";
            switch (first)
            {
                case "0": QRName = "DX_"; QRType = "智慧电箱模块"; break;
                case "1": QRName = "QJ2_"; QRType = "2位按键情景面板"; break;
                case "2": QRName = "QJ4_"; QRType = "4位按键情景面板"; break;
                case "3": QRName = "KG1_"; QRType = "1位按键开关"; break;
                case "4": QRName = "KG2_"; QRType = "2位按键开关"; break;
                case "5": QRName = "CZ_"; QRType = "插座"; break;
                case "6": QRName = "KG3_"; QRType = "3位按键开关"; break;
                default: break;
            }

            QRName = QRName + content;
            string fileName = Server.MapPath("~") + "Img\\QRImage\\" + QRName + ".jpg";
            Bitmap bitmap = QRCodeHelper.QRCodeEncoderUtil(content);
            bitmap.Save(fileName);//保存位图
            string imageUrl = "/Img/QRImage/" + QRName + ".jpg";//显示图片  
            return Json(new { QRName, QRType, imageUrl }, JsonRequestBehavior.AllowGet); //Content(imageUrl);
        }
        public JsonResult GetORImageList(string[] arr)
        {
            Dictionary<string, JsonResult> JsonList = new Dictionary<string, JsonResult>();
            foreach (var item in arr)
            {
                var result = GetORImage(item);
                JsonList.Add(item, result);
            }
            return Json(new { data = JsonList }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        public ActionResult GetORImageContent(string imageName)
        {
            string fileUrl = Server.MapPath("~") + "Img\\QRImage\\" + imageName;
            Bitmap bitMap = new Bitmap(fileUrl);
            string content = QRCodeHelper.QRCodeDecoderUtil(bitMap);
            return Content(content);
        }
        public ActionResult GetLocalIP()
        {
            ///获取本机IP
            var ip = LocalIPService.GetLocalIP();
            return Json(new { data = ip }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLocalIP2()
        {
            var ip = LocalIPService.GetIP();
            return Json(new { data = ip }, JsonRequestBehavior.AllowGet);
        }
    }
}