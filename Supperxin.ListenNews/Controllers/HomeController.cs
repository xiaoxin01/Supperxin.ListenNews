using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Supperxin.ListenNews.Models;

namespace Supperxin.ListenNews.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy([FromQuery]string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Error();
            }
            // 设置APPID/AK/SK
            var APP_ID = "15174756";
            var API_KEY = "ZixGBp4NKBG1vq5Azec2LbEB";
            var SECRET_KEY = "FS0wCOyvvIzrAc11Or8uPVzVL2w6zHUe";

            var _ttsClient = new Baidu.Aip.Speech.Tts(API_KEY, SECRET_KEY);
            _ttsClient.Timeout = 60000;  // 修改超时时间

            // 可选参数
            var option = new Dictionary<string, object>()
            {
                {"spd", 5}, // 语速
                {"vol", 7}, // 音量
                {"per", 4}  // 发音人，4：情感度丫丫童声
            };
            var result = _ttsClient.Synthesis(text, option);

            if (result.ErrorCode == 0)  // 或 result.Success
            {
                //System.IO.File.WriteAllBytes("合成的语音文件本地存储地址.mp3", result.Data);
                return File(result.Data, "application/octet-stream", $"{Guid.NewGuid().ToString()}.mp3");
            }

            return Error();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
