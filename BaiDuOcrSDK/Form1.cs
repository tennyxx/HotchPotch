using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiDuOcrSDK
{
    public partial class Form1 : Form
    {
        public static string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key].ToString().Trim();
        }
        // APPID
        private static string appId = GetValue("BaiduAI.AppID");
        // 百度云中开通对应服务应用的 API Key 建议开通应用的时候多选服务
        private static String clientId = GetValue("BaiduAI.APIKey");
        // 百度云中开通对应服务应用的 Secret Key
        private static String clientSecret = GetValue("BaiduAI.SecretKey");
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 增值税发票识别
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public JObject VatInvoiceOcr(string path)
        {
            var client = new Baidu.Aip.Ocr.Ocr(clientId, clientSecret);
            client.Timeout = 60000;
            var image = File.ReadAllBytes(path);
            JObject result = new JObject();
            try
            {
                // 调用增值税发票识别，可能会抛出网络等异常，请使用try/catch捕获
                result = client.VatInvoice(image);
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        public JObject TrainTicketDemo(string path)
        {
            var client = new Baidu.Aip.Ocr.Ocr(clientId, clientSecret);
            client.Timeout = 60000;
            var image = File.ReadAllBytes(path);
            JObject result = new JObject();
            try
            {
                // 调用火车票识别，可能会抛出网络等异常，请使用try/catch捕获
                result = client.TrainTicket(image);
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog path = new OpenFileDialog();

            DialogResult dr = path.ShowDialog();
            if (dr == DialogResult.OK)
            {
                var result = VatInvoiceOcr(path.FileName);
                ResultText.Text = result.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog path = new OpenFileDialog();

            DialogResult dr = path.ShowDialog();
            if (dr == DialogResult.OK)
            {
                var result = TrainTicketDemo(path.FileName);
                ResultText.Text = result.ToString();
            }
        }
    }
}
