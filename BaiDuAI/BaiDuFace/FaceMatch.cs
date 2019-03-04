using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace BaiDuFace
{
    /// <summary>
    ///  人脸图像对比Demo，
    ///  ApiKey和SecretKey皆为个人申请的测试Key，生产环境中在webconfig配置到实际项目申请的key
    /// </summary>
    public class FaceMatch
    {
        // 调用getAccessToken()获取的 access_token建议根据expires_in 时间 设置缓存
        // 返回token示例 没用，仅是格式示例
        public static String TOKEN = "24.adda70c11b9786206253ddb70affdc46.2592000.1493524354.282335-1234567";
        // 百度云中开通对应服务应用的 API Key 建议开通应用的时候多选服务
        private static String clientId = "qv0YYrmbqGZf6oPfpWDQ7OC1";
        // 百度云中开通对应服务应用的 Secret Key
        private static String clientSecret = "fLZwuTWNVGq7wKz30MF03T51HIMjNsuD";
        // 人脸对比
        public static string match(string imgPath1, string imgPath2)
        {
            string token = getAccessToken();
            string host = "https://aip.baidubce.com/rest/2.0/face/v3/match?access_token=" + token;
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            var img1 = ImageHelper.ImgToBase64String(imgPath1);
            var img2 = ImageHelper.ImgToBase64String(imgPath2);
            var param1 = new FaceMatchParam()
            {
                image = img1,
                image_type = "BASE64",
                liveness_control = "NONE",
                quality_control = "LOW",
                face_type = "LIVE"
            };
            var param2 = new FaceMatchParam()
            {
                image = img2,
                image_type = "BASE64",
                liveness_control = "NONE",
                quality_control = "NONE",
                face_type = "LIVE"
            };
            var list = new List<FaceMatchParam>();
            list.Add(param1);
            list.Add(param2);
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            //String str = "[{\"image\":\"sfasq35sadvsvqwr5q...\",\"image_type\":\"BASE64\",\"face_type\":\"LIVE\",\"quality_control\":\"LOW\",\"liveness_control\":\"HIGH\"},{\"image\":\"sfasq35sadvsvqwr5q...\",\"image_type\":\"BASE64\",\"face_type\":\"IDCARD\",\"quality_control\":\"LOW\",\"liveness_control\":\"HIGH\"}]";
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string result = reader.ReadToEnd();
            Console.WriteLine("人脸对比:");
            Console.WriteLine(result);
            return result;
        }



        public static String getAccessToken()
        {
            String authHost = "https://aip.baidubce.com/oauth/2.0/token";
            HttpClient client = new HttpClient();
            List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
            paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            paraList.Add(new KeyValuePair<string, string>("client_id", clientId));
            paraList.Add(new KeyValuePair<string, string>("client_secret", clientSecret));

            HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
            String result = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(result);
            JObject json = JObject.Parse(result);
            var accessToken = (string)json["access_token"];
            return accessToken;
        }
    }
}
