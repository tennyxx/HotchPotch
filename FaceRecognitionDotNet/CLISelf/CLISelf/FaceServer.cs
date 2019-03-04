using DlibDotNet;
using DlibDotNet.Extensions;
using FaceRecognitionDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Face.CLISelf
{
    public static class FaceServer
    {
        //
        private static FaceRecognition _FaceRecognition;
        //模型对应的标记名
        private static Dictionary<FaceEncoding, string> _faceDictionary;
        /// <summary>
        /// 容忍差值
        /// </summary>
        public static double _tolerance = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["tolerance"].ToString());
        /// <summary>
        /// 初始化
        /// </summary>
        public static int Init()
        {
            _FaceRecognition = FaceRecognition.Create("models");//当前目录
            _faceDictionary = GetFaceEncodesList();
            return _faceDictionary.Count;
        }
        /// <summary>
        /// 识别
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static string FaceRecognitionForIamge(string base64)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            return FaceRecognitionForIamge(imageBytes);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <returns></returns>
        public static string FaceRecognitionForIamge(byte[] imageBytes)
        {
            if (imageBytes.Length > 0)
            {
                var bitmap = ToFormat24bpprgb(imageBytes);
                var array2d = bitmap.ToArray2D<RgbPixel>();
                bitmap.Dispose();
                var bytes = array2d.ToBytes();
                var image1 = FaceRecognition.LoadImage(bytes, array2d.Rows, array2d.Columns, 3);
                array2d.Dispose();
                var endodings1 = _FaceRecognition.FaceEncodings(image1).ToArray();
                image1.Dispose();
                if (endodings1.Length > 0)
                {
                    var list = FaceRecognition.CompareFaces(_faceDictionary.Keys.ToList(), endodings1[0], _tolerance).ToList<FaceEncodingData>();
                    foreach (var endoding in endodings1) {
                        endoding.Dispose();
                    }
                    if (list.Count > 0)
                    {
                        //Dictionary<string, double> dic = new Dictionary<string, double>();
                        Dictionary<double, string> dic = new Dictionary<double, string>();
                        for (int i = 0; i < list.Count; i++)
                        {
                            string temp = "";
                            if (!dic.TryGetValue(list[i].distance, out temp))
                            {
                                dic.Add(list[i].distance, _faceDictionary[list[i].faceEncoding]);
                            }
                        }
                        var item = dic.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value).First();
                        return item.Value + "|" + (1 - item.Key) * 100;
                    }
                    else
                    {
                        return "No Suitable Data";
                    }
                }
                else
                {
                    return "No Recognition";
                }
            }
            else { return "Error"; }
        }
        /// <summary>
        /// 入录
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string FaceEntry(string base64,string name) {
            base64 = HttpUtility.HtmlDecode(base64);
            byte[] imageBytes = Convert.FromBase64String(base64);
            return FaceEntry(imageBytes, name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string FaceEntry(byte[] imageBytes, string name)
        {
            try
            {
                var bitmap = ToFormat24bpprgb(imageBytes);
                var array2d = bitmap.ToArray2D<RgbPixel>();
                var bytes = array2d.ToBytes();
                using (var image1 = FaceRecognition.LoadImage(bytes, array2d.Rows, array2d.Columns, 3))
                {
                    array2d.Dispose();
                    var encoding2 = _FaceRecognition.FaceEncodingTop1(image1);
                    if (encoding2!=null)
                    {
                        SaveFaceEncodes(imageBytes,encoding2, name);
                        //同步字典
                        _faceDictionary.Add(encoding2, name);
                        //encoding2.Dispose();
                    }
                    else
                    {
                        return "NO Face";
                    }
                }
                return "OK";
            }
            catch { return "Error"; }
        }

        /// <summary>
        /// 检测人脸
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static int FaceCheck(string base64) {
            if (string.IsNullOrWhiteSpace(base64))
                return 0;
            byte[] imageBytes = Convert.FromBase64String(base64);

            using (var faceDetector = DlibDotNet.Dlib.GetFrontalFaceDetector())
            using (var ms = new MemoryStream(imageBytes))
            using (var bitmap = (Bitmap)System.Drawing.Image.FromStream(ms))
            {
                using (var image = bitmap.ToArray2D<RgbPixel>())
                {
                    var dets = faceDetector.Operator(image);
                    //foreach (var r in dets)
                    //    DlibDotNet.Dlib.DrawRectangle(image, r, new RgbPixel { Green = 255 });
                    return dets.Length;
                }
            }
        }

        /// <summary>
        /// 重新加载人脸模型
        /// </summary>
        /// <returns></returns>
        public static int ResetFaceEncodings() {
            _faceDictionary.Clear();
            _faceDictionary = GetFaceEncodesList();
            return _faceDictionary.Count;
        }
        /// <summary>
        /// 释放
        /// </summary>
        public static void Dispose() {
            _FaceRecognition.Dispose();
        }
        /// <summary>
        /// 保存人脸模型
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="name"></param>
        private static void SaveFaceEncodes(byte[] imageBytes,FaceEncoding encoding, string name)
        {
            var dest = $"faces/{name}/";
            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
                File.Create(Path.Combine(dest, name+".txt"));
            }
            string[] files = Directory.GetFiles(dest);
            //imageBytes
            string ts = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + encoding.Size;
            //格式 日期加上特征数
            string datFilePath = $"{dest}/{ts}.dat";
            string jpgFilePath = $"{dest}/{ts}.jpg";

            var bf = new BinaryFormatter();
            using (var fs = new FileStream(datFilePath, FileMode.OpenOrCreate))
            {
                bf.Serialize(fs, encoding);
            }
            File.WriteAllBytes(jpgFilePath, imageBytes);
        }
        /// <summary>
        /// 载入已入录人脸模型
        /// </summary>
        /// <returns></returns>
        private static Dictionary<FaceEncoding, string> GetFaceEncodesList()
        {
            Dictionary<FaceEncoding, string> dictionary = new Dictionary<FaceEncoding, string>();
            string path = @"faces";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string[] files = Directory.GetFiles(path, "*.txt", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);//取特征的人名
                var dirPath=Path.GetDirectoryName(file);
                string[] dats=Directory.GetFiles(dirPath, "*.dat", SearchOption.AllDirectories);
                foreach (var dat in dats){
                    var bf = new BinaryFormatter();
                    using (var fs = new FileStream(dat, FileMode.Open))
                    {
                        FaceEncoding face = bf.Deserialize(fs) as FaceEncoding;
                        dictionary.Add(face, name);
                    }
                }
            }
            return dictionary;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        private static Bitmap ToFormat24bpprgb(byte[] bs)
        {
            MemoryStream ms = new MemoryStream(bs);
            System.Drawing.Image imgr = System.Drawing.Image.FromStream(ms);
            Bitmap bmp = new Bitmap(imgr.Width, imgr.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(imgr, new System.Drawing.Point(0, 0));
            g.Dispose();
            imgr.Dispose();
            ms.Dispose();
            return bmp;
        }
    }
}
