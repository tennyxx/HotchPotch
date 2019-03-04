using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace BaiDuOCR
{
    public class IdentityCardOCR
    {
        // 调用getAccessToken()获取的 access_token建议根据expires_in 时间 设置缓存
        // 返回token示例
        public static String TOKEN = "24.adda70c11b9786206253ddb70affdc46.2592000.1493524354.282335-1234567";
        // 百度云中开通对应服务应用的 API Key 建议开通应用的时候多选服务
        private static String clientId = "qv0YYrmbqGZf6oPfpWDQ7OC1";
        // 百度云中开通对应服务应用的 Secret Key
        private static String clientSecret = "fLZwuTWNVGq7wKz30MF03T51HIMjNsuD";
        /// <summary>
        /// 身份证识别
        /// </summary>
        /// <param name="type">0为照片正面  1为国徽背面</param>
        /// <returns></returns>
        public static string idcard(string path, string type)
        {
            string token = getAccessToken();
            string strbaser64 = ImageHelper.ImgToBase64String(path); // 图片的base64编码
            string host = "https://aip.baidubce.com/rest/2.0/ocr/v1/idcard?access_token=" + token;
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = true;
            var side = type == "0" ? "front" : "back";
            String str = "id_card_side=" + side + "&image=" + UrlEncode(strbaser64);
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string result = reader.ReadToEnd();
            Console.WriteLine("身份证识别:");
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
        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }
    }

}
