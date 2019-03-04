using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Face.CLISelf
{
    class Program
    {
        public static IDisposable m_webapi = null;
        public static NLog.Logger m_logger = LogManager.GetCurrentClassLogger();
        public static string m_webApiHost = System.Configuration.ConfigurationManager.AppSettings["webApiHost"].ToString();


        static void Main(string[] args)
        {
            Console.Title = "Face Recognition";
            m_logger.Info("Face Recognition Start!");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            WebHost();
            int count = FaceServer.Init();//初始化人脸识别引擎
            m_logger.Info("Face Recognition Init OK! Face Model Count {0}", count);

            string input = "";
            if (args.Length > 0)
            {
                input = args[0];
            }
            else
            {
                Console.WriteLine("Please Enter Command!");
                input = Console.ReadLine();
            }
            do
            {
                if (input.ToLower() == "exit")
                {
                    WriteColorLine("Y/N ?", ConsoleColor.Red);
                    input = Console.ReadLine();
                    if (input.ToLower() == "y")
                    {
                        try
                        {
                            m_webapi.Dispose();
                            FaceServer.Dispose();
                        }
                        catch { }
                        finally
                        {
                            Environment.Exit(0);
                        }
                    }
                }
                else if (input.ToLower() == "face init")
                {
                    string[] strs = Directory.GetFiles(@"C:\Project\self\C#\TestBaseImages", "*.*", SearchOption.AllDirectories);
                    m_logger.Debug("初始化图片：{0}张", strs.Length);
                    foreach (var path in strs)
                    {
                        byte[] bys = File.ReadAllBytes(path);
                        DirectoryInfo info = new DirectoryInfo(Path.GetDirectoryName(path));
                        string name = info.Name;
                        string result = FaceServer.FaceEntry(bys, name);
                        m_logger.Debug("{0}，{1}", name, result);
                    }
                    m_logger.Debug("人脸模型初始化成功");
                }
                else if (input.ToLower() == "face add")
                {
                    m_logger.Debug("从add目录开始入录");
                    string addPath = @"C:\Project\self\C#\TestBaseImages";
                    if (Directory.Exists(addPath))
                    {

                        string[] addDir = Directory.GetDirectories(addPath, "*", SearchOption.TopDirectoryOnly);
                        m_logger.Debug("入录{0}个人", addDir.Length);
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        int a = 0, b = 0, c = 0, d = 0;
                        List<string> list = new List<string>();
                        foreach (var imagesPath in addDir)
                        {
                            string[] images = Directory.GetFiles(imagesPath, "*.*", SearchOption.AllDirectories);
                            foreach (var image in images)
                            {
                                d++;
                                string result = "";
                                try
                                {
                                    result = FaceServer.FaceRecognitionForIamge(File.ReadAllBytes(image));
                                    if (result == "No Suitable Data")
                                    {
                                        a++;
                                    }
                                    else if (result == "No Recognition")
                                    {
                                        b++;
                                    }
                                    else if (result == "Error")
                                    {
                                        c++;
                                    }
                                    else
                                    {
                                        string[] arr = result.Split('|');
                                        if (!list.Contains(arr[0]))
                                        {
                                            list.Add(arr[0]);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //m_logger.Debug("{0},识别为{1}", image, result);
                                }
                                m_logger.Debug("{0},识别为{1}", image, result);
                            }
                        }
                        stopwatch.Stop();
                        m_logger.Info("Image Count ：{0}", d);
                        m_logger.Info("No Suitable Data：{0}", a);
                        m_logger.Info("No Recognition：{0}", b);
                        m_logger.Info("Error：{0}", c);
                        m_logger.Info("Recognition：{0}", list.Count);
                        m_logger.Info("耗时：{0}s", stopwatch.ElapsedMilliseconds / 1000);
                    }
                    else
                    {
                        m_logger.Error("目录不存在或出错");
                    }
                }
                else if (input.ToLower() == "face test")
                {
                    WriteColorLine("请输入目录 ?", ConsoleColor.Yellow);
                    // input = Console.ReadLine();
                    input = @"C:\Project\self\C#\base\30";
                    if (!string.IsNullOrEmpty(input) && Directory.Exists(input))
                    {
                        m_logger.Debug("开始性能与识别测试");
                        string[] images = Directory.GetFiles(input, "*.*", SearchOption.AllDirectories);
                        m_logger.Debug("测试图片{0}张", images.Length);
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        int a = 0, b = 0, c = 0;
                        List<string> list = new List<string>();
                        foreach (var image in images)
                        {

                            string result = "";
                            try
                            {
                                result = FaceServer.FaceRecognitionForIamge(File.ReadAllBytes(image));
                                if (result == "No Suitable Data")
                                {
                                    a++;
                                }
                                else if (result == "No Recognition")
                                {
                                    b++;
                                }
                                else if (result == "Error")
                                {
                                    c++;
                                }
                                else
                                {
                                    string[] arr = result.Split('|');
                                    if (!list.Contains(arr[0]))
                                    {
                                        list.Add(arr[0]);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //m_logger.Debug("{0},识别为{1}", image, result);
                            }
                            m_logger.Debug("{0},识别为{1}", image, result);

                        }
                        stopwatch.Stop();
                        m_logger.Info("No Suitable Data：{0}", a);
                        m_logger.Info("No Recognition：{0}", b);
                        m_logger.Info("Error：{0}", c);
                        m_logger.Info("Recognition：{0}", list.Count);
                        m_logger.Info("耗时：{0}s", stopwatch.ElapsedMilliseconds / 1000);
                    }
                    else
                    {
                        m_logger.Error("目录不存在或出错");
                    }
                }

                input = Console.ReadLine();
            } while (true);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = e.ExceptionObject as Exception;
                m_logger.Error(ex, "CurrentDomain_UnhandledException");
            }
            catch { }
        }

        public static void WebHost()
        {
            try
            {
                m_webapi = Microsoft.Owin.Hosting.WebApp.Start<Startup>(url: m_webApiHost);
                m_logger.Info("WebApi Host:{0} OK!", m_webApiHost);
            }
            catch (Exception ex)
            {
                m_logger.Error(ex, "WebHost");
            }
        }

        public static void WriteColorLine(string str, ConsoleColor color)
        {
            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(str);
            Console.ForegroundColor = currentForeColor;
        }

    }
}
