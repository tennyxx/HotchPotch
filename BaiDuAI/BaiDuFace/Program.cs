using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiDuFace
{
    class Program
    {
        static void Main(string[] args)
        {
            //人脸图片2张
            var path1 = @"C:\Project\self\C#\base\30\004.png";
            var path2 = @"C:\Project\self\C#\TestBaseImages\liucx\liucx.jpg";
            var result = FaceMatch.match(path1, path2);

            //result格式为：
            //{"error_code":0,"error_msg":"SUCCESS","log_id":304592816892024281,"timestamp":1551689202,"cached":0,"result":{"score":86.09963226,"face_list":[{"face_token":"b740be97ba53cc7dfe8e96b7ac0c4e5f"},{"face_token":"da96f78a85bb0177fdaf8116e04be4f7"}]}}
            //解析
            decimal score = 0;//两张图片相似度
            try
            {
                JObject json = JObject.Parse(result);
                var errorcode = (int)json["error_code"];
                if (errorcode == 0)
                {
                    score = (decimal)json["result"]["score"];
                }
            }
            catch (Exception ex)
            {

            }
            Console.WriteLine("两张图片对比后的相似度为：" + score);
            Console.ReadKey();

        }
    }
}
