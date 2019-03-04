using Face.CLISelf.Common;
using Face.CLISelf.Entity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Face.CLISelf.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage ApiInfo()
        {
            var path = "ApiInfo.html";
            var html = "";
            if (!File.Exists(path))
            {
                html = ShowAPI.ShowWebApi(this);
                byte[] myByte = System.Text.Encoding.UTF8.GetBytes(html);
                using (FileStream fs = File.Create(path))
                {
                    fs.Write(myByte, 0, myByte.Length);
                };
            }
            else {
                html = File.ReadAllText(path);
            }
            return new HttpResponseMessage() {
                Content = new StringContent(html, Encoding.UTF8, "text/html")
            };

        }

        [HttpPost]
        public string FaceRecognitionForIamge([FromBody]string base64)
        {
            return FaceServer.FaceRecognitionForIamge(base64);
        }

        [HttpPost]
        public string FaceEntry([FromBody]JObject item)
        {
            return FaceServer.FaceEntry(item["base64"].ToString(), item["name"].ToString());
        }

        [HttpGet]
        public int ResetFaceEncodings()
        {
            return FaceServer.ResetFaceEncodings();
        }

    }
}
