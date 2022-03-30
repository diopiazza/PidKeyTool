using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace PidKeyTest
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            richTextBox2.AppendText(value);
            richTextBox2.ScrollToCaret();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string result = "";
            string keys = richTextBox1.Text;
            Thread Thread1 = new Thread(() =>
            {
                MatchCollection matches = Regex.Matches(keys, "\\w{5}-\\w{5}-\\w{5}-\\w{5}-\\w{5}");
                foreach (Match match in matches)
                {
                    result = CheckKey(match.Value);
                    AppendTextBox(result);
                }
            });
            Thread1.Start();
        }
        private string CheckKey(string keys)
        {
            try
            {

                string szReturn = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://pidkey.top/PidKey.aspx?key=" + keys);
                request.KeepAlive = true;
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
                request.Method = "GET";
                try
                {
                    HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
                    using (Stream stream = request.GetResponse().GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            szReturn = reader.ReadToEnd();
                            reader.Close();
                        }
                        stream.Close();
                    }
                    httpWebResponse.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                try
                {
                    dynamic jsons = new JavaScriptSerializer().DeserializeObject(szReturn.Replace("<br />", ""));
                    return jsons["Key"].ToString() + "\r\n" + jsons["Description"].ToString() + "\r\n" + jsons["Remain"].ToString() + "\r\n" + jsons["ErrorCode"].ToString() + "\r\n\r\n";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            catch { }
            return "";
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = "WVNG4-YYJ7J-33HKK-6F62W-2G26M" + "\r\n" + "22TKD-F8XX6-YG69F-9M66D-PMJBM" + "\r\n" + "8G394-KG29Q-K4NVY-DWH6K-XKWKF" + "\r\n" + "P6F97-NQGC2-MX8RR-B9P8G-4M49G" + "\r\n" + "2KKTK-YGJKV-3WMRR-3MDQW-TJP47" + "\r\n" + "6F77B-TN7GY-69H8F-B87KP-D69TY" + "\r\n" + "6DWFN-9DBPB-99W4C-XYWKQ-VXPFM";
        }
    }
}
