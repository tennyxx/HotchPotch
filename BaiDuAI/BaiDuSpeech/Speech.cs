using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiDuSpeech
{
    class Speech
    {
        // 识别本地文件
        public string AsrData()
        {
            // 设置APPID/AK/SK
            var APP_ID = "15646746";
            var API_KEY = "qv0YYrmbqGZf6oPfpWDQ7OC1";
            var SECRET_KEY = "fLZwuTWNVGq7wKz30MF03T51HIMjNsuD";

            var client = new Baidu.Aip.Speech.Asr(APP_ID, API_KEY, SECRET_KEY);
            var data = File.ReadAllBytes(@"C:\music\16k.wav");
            // 可选参数
            var options = new Dictionary<string, object>
             {
                {"dev_pid", 1536}
             };
            client.Timeout = 120000; // 若语音较长，建议设置更大的超时时间. ms
            var result = client.Recognize(data, "pcm", 16000, options);
            var strInfo = "";
            try
            {
                var arrResult = result["result"].ToArray();
                if (arrResult.Length > 0)
                {
                    strInfo = arrResult[0].ToString();
                }
            }
            catch (Exception ex)
            {

            }
            return strInfo;
        }
    }
}
