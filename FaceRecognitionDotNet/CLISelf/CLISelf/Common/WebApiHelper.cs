using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Linq;
using System.Collections.Specialized;

namespace Face.CLISelf.Client
{
    public static class WebApiHelper
    {
        /// <summary>
        /// 全局的头信息
        /// </summary>
        public static Dictionary<string, string> headDic = new Dictionary<string, string>();
        /// <summary>
        /// log记录
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public enum UploadContentType
        {
            /// <summary>
            /// Json的上传方式
            /// </summary>
            [Description("application/json; charset=utf-8")]
            Json = 0,
            /// <summary>
            /// 表单的上传方式
            /// </summary>
            [Description("application/x-www-form-urlencoded; charset=utf-8")]
            Form = 1,
            /// <summary>
            /// 上传文件
            /// </summary>
            [Description("multipart/form-data; boundary={0}")]
            File = 2,
            /// <summary>
            /// Xml提交
            /// </summary>
            [Description("application/xml; charset=UTF-8")]
            Xml = 2,
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="staffId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static T Post<T>(string url, string data, string token="", UploadContentType contentType = UploadContentType.Json)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            Stream reqstream = null;
            StreamReader streamReader = null;
            Stream streamReceive = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Add("token", token);
                }
                AddHeads(request);

                request.Method = "POST";
                request.ContentLength = bytes.Length;
                request.ContentType = contentType.ToDescriptionName();
                
                //读数据
                request.Timeout = 300000;
                request.Headers.Set("Pragma", "no-cache");
                //写数据
                reqstream = request.GetRequestStream();
                reqstream.Write(bytes, 0, bytes.Length);

                response = (HttpWebResponse)request.GetResponse();
                streamReceive = response.GetResponseStream();
                streamReader = new StreamReader(streamReceive, Encoding.UTF8);
                string strResult = streamReader.ReadToEnd();

                return JsonConvert.DeserializeObject<T>(strResult);
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                return default(T);
            }
            finally {
                //关闭流
                if (reqstream != null) reqstream.Close();
                if (streamReader != null) streamReader.Close();
                if (streamReceive != null) streamReceive.Close();
                if (request != null) request.Abort();
                if (response != null) response.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="files"></param>
        /// <param name="token"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static T Post<T>(string url, NameValueCollection data, List<Tuple<string, string, byte[]>> files, string token = "", UploadContentType contentType = UploadContentType.Json)
        {
            Stream reqstream = null;
            StreamReader streamReader = null;
            Stream streamReceive = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Add("token", token);
                }
                AddHeads(request);
                //request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                //request.ProtocolVersion = HttpVersion.Version11;
                //request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";
                request.Method = "POST";

                //读数据
                request.Timeout = 300000;
                request.Headers.Set("Pragma", "no-cache");

                var postBytes = new List<byte[]>();
                var boundary = "------------" + DateTime.Now.Ticks.ToString("x");
                request.ContentType = string.Format(contentType.ToDescriptionName(), boundary);
                var boundarybytes = Encoding.UTF8.GetBytes("\r\n--"  + boundary + "\r\n");

                foreach (var _data in data.AllKeys.ToDictionary(c => c, c => data.GetValues(c)))
                {
                    foreach (var value in _data.Value)
                    {
                        postBytes.Add(boundarybytes);
                        postBytes.Add(Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"" + _data.Key + "\"\r\n\r\n" + value + "\r\n"));
                    }
                }
                if (files.Count > 0) {
                    //写上传文件数据
                    foreach (var file in files)
                    {
                        postBytes.Add(boundarybytes);
                        postBytes.Add(Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"" + file.Item1 + "\"; filename=\"" + file.Item2 + "\"\r\nContent-Type: application/octet-stream\r\n\r\n"));
                        postBytes.Add(file.Item3);
                    }
                }
                
                var endbytes = Encoding.UTF8.GetBytes("\r\n--"  + boundary + "--\r\n");
                postBytes.Add(endbytes);

                request.ContentLength = postBytes.Sum(c => c.Length);
                //写数据
                reqstream = request.GetRequestStream();
                foreach (var postByte in postBytes) { reqstream.Write(postByte, 0, postByte.Length); } 

                response = (HttpWebResponse)request.GetResponse();
                streamReceive = response.GetResponseStream();
                streamReader = new StreamReader(streamReceive, Encoding.UTF8);
                string strResult = streamReader.ReadToEnd();

                return JsonConvert.DeserializeObject<T>(strResult);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return default(T);
            }
            finally
            {
                //关闭流
                if (reqstream != null) reqstream.Close();
                if (streamReader != null) streamReader.Close();
                if (streamReceive != null) streamReceive.Close();
                if (request != null) request.Abort();
                if (response != null) response.Close();
            }
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webApi">api地址</param>
        /// <param name="queryStr"></param>
        /// <param name="staffId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static T Get<T>(string webApi, string token="", UploadContentType contentType = UploadContentType.Json)
        {
            StreamReader streamReader = null;
            Stream streamReceive = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(webApi);
                if (!string.IsNullOrEmpty(token)) {
                    request.Headers.Add("token", token);
                }
                AddHeads(request);
                //提交方式
                request.Method = "GET";
                request.ContentType = contentType.ToDescriptionName();
                request.Timeout = 90000;
                request.Headers.Set("Pragma", "no-cache");

                response = (HttpWebResponse)request.GetResponse();
                streamReceive = response.GetResponseStream();
                streamReader = new StreamReader(streamReceive, Encoding.UTF8);
                string strResult = streamReader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(strResult);
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                return default(T);
            }
            finally {
                //关闭流
                if (streamReader != null) streamReader.Close();
                if (streamReceive != null) streamReceive.Close();
                if (request != null) request.Abort();
                if (response != null) response.Close();
            }
        }

        private static HttpWebRequest PostPack(HttpWebRequest myRequest,NameValueCollection data, List<Tuple<string, string, byte[]>> files,UploadContentType contentType = UploadContentType.Json)
        {
            Stream stream = null;
            try
            {
                myRequest.ContentType = contentType.ToDescriptionName();
                var postBytes = new List<byte[]>();
                #region 组装GetRequestStream
                switch (contentType)
                {
                    //case UploadContentType.Default:
                    //    var defaultData = ((NameValueCollection)head.Data).ToData();
                    //    var defaultBytes = Encoding.UTF8.GetBytes(defaultData);

                    //    postBytes.Add(defaultBytes);
                    //    break;
                    //case UploadContentType.Json:
                    //    var jsonData = head.Data.ToJson();
                    //    var jsonBytes = Encoding.UTF8.GetBytes(jsonData);

                    //    postBytes.Add(jsonBytes);
                    //    break;
                    //case UploadContentType.Xml:
                    //    var xmlData = head.ToXml();
                    //    var xmlBytes = Encoding.UTF8.GetBytes(xmlData);

                    //    postBytes.Add(xmlBytes);
                    //    break;
                    case UploadContentType.File:
                        #region 组装PostBytes
                        var boundary = DateTime.Now.Ticks.ToString("x");
                        var sp = "------------";
                        myRequest.ContentType = string.Format(myRequest.ContentType, sp+boundary);
                        var boundarybytes = Encoding.UTF8.GetBytes("\r\n"+sp+boundary+"\r\n");
                        var endbytes = Encoding.UTF8.GetBytes("\r\n"+sp+boundary+"--\r\n");

                        foreach (var _data in (data).AllKeys.ToDictionary(c => c, c => (data).GetValues(c)))
                        {
                            foreach (var value in _data.Value)
                            {
                                postBytes.Add(boundarybytes);
                                postBytes.Add(Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\""+_data.Key+"\"\r\nContent-Type: text/plain\r\n\r\n"+value));
                            }
                        }

                        foreach (var file in files)
                        {
                            postBytes.Add(boundarybytes);
                            postBytes.Add(Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\""+file.Item1+"\"; filename=\""+file.Item2+"\"\r\nContent-Type: application/octet-stream\r\n\r\n"));
                            postBytes.Add(file.Item3);
                        }

                        postBytes.Add(endbytes);
                        #endregion

                        break;
                }
                #endregion
                myRequest.ContentLength = postBytes.Sum(c => c.Length);
                stream = myRequest.GetRequestStream();
                foreach (var postByte in postBytes) stream.Write(postByte, 0, postByte.Length);
                return myRequest;
            }
            finally
            {
                if(stream!=null)stream.Close();
                if(stream!=null)stream.Dispose();
            }
        }


        /// <summary>
        /// 全局添加头信息
        /// </summary>
        /// <param name="request"></param>
        private static void AddHeads(HttpWebRequest request) {
            foreach (var dic in headDic)
            {
                request.Headers.Add(dic.Key, dic.Value);
            }
        }
        /// <summary>
        /// 获取详细
        /// </summary>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        private static string ToDescriptionName(this Enum enumeration)
        {
            var type = enumeration.GetType();
            var memInfo = type.GetMember(enumeration.ToString());
            if (memInfo.Length <= 0) return enumeration.ToString();
            var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attrs.Length > 0 ? ((DescriptionAttribute)attrs[0]).Description : enumeration.ToString();
        }

        /// <summary>
        /// 将文件转换成byte[] 数组
        /// </summary>
        /// <param name="fileUrl">文件路径文件名称</param>
        /// <returns>byte[]</returns>
        public static byte[] GetFileData(string fileFullPath)
        {
            FileStream fs = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read);
            try
            {
                byte[] buffur = new byte[fs.Length];
                fs.Read(buffur, 0, (int)fs.Length);

                return buffur;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (fs != null)
                {

                    //关闭资源
                    fs.Close();
                }
            }
        }
    }

}
