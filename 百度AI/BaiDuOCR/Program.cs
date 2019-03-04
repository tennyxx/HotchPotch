using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiDuOCR
{
    class Program
    {
        /// <summary>
        /// 百度AI OCR身份证识别Demo实例
        /// @author liucx
        /// @date 2019年3月1日 16:22:16
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var path = @"C:\Project\self\C#\base\30\id1.jpg";
            var cardType = "0";//0正面1反面
            var strResult = IdentityCardOCR.idcard(path, cardType);
            //转实体
            var model = ResultToObject(strResult, cardType);
            model.ImgageName = GetFileName(path);
        }
        /// <summary>
        ///  OCR识别结果字符串转model
        /// </summary>
        /// <param name="resultStr">接口返回结果字符串</param>
        /// <param name="type">类别 0正面1反面</param>
        /// <returns></returns>
        public static IdcardOCRModel ResultToObject(string resultStr, string type)
        {
            var model = new IdcardOCRModel();
            //转json失败则直接返回空对象
            try
            {
                JObject json = JObject.Parse(resultStr);
                var words_result = json["words_result"];
                if (type == "0")
                {
                    model.Address = (string)words_result["住址"]["words"];
                    model.Birth = (string)words_result["出生"]["words"];
                    model.Name = (string)words_result["姓名"]["words"];
                    model.Id = (string)words_result["公民身份号码"]["words"];
                    model.Nation = (string)words_result["民族"]["words"];
                    model.Sex = (string)words_result["性别"]["words"];
                }
                else
                {
                    model.Authority = (string)words_result["签发机关"]["words"];
                    model.ValidDate = (string)words_result["失效日期"]["words"];
                }
            }
            catch (Exception ex)
            {
            }
            return model;
        }
        /// <summary>
        /// 获取文件名称
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        protected static string GetFileName(string fullFileName)
        {
            int idx = fullFileName.LastIndexOf("\\");
            return fullFileName.Substring(idx + 1);
        }
    }
}
