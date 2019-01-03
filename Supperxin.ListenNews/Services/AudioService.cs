using System.Collections.Generic;

namespace Supperxin.ListenNews.Services
{
    public interface IAudioService
    {
        Baidu.Aip.Speech.TtsResponse GeneriteAudio(string text);
    }
    public class BaiduAudioService : IAudioService
    {
        // 设置APPID/AK/SK
        // var APP_ID = "15174756";
        const string API_KEY = "ZixGBp4NKBG1vq5Azec2LbEB";
        const string SECRET_KEY = "FS0wCOyvvIzrAc11Or8uPVzVL2w6zHUe";

        Baidu.Aip.Speech.Tts _ttsClient = new Baidu.Aip.Speech.Tts(API_KEY, SECRET_KEY);
        //_ttsClient.Timeout = 60000;  // 修改超时时间

        // 可选参数
        Dictionary<string, object> option = new Dictionary<string, object>()
        {
            {"spd", 5}, // 语速
            {"vol", 10}, // 音量
            {"per", 1}  // 发音人，4：情感度丫丫童声
        };

        public Baidu.Aip.Speech.TtsResponse GeneriteAudio(string text)
        {
            var result = _ttsClient.Synthesis(text, option);

            return result;
        }
    }
}