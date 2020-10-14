using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace NCKH.HTML.AppClient
{
    public partial class Form1 : Form
    {
        string linkToXml = "";
        string domain = "";
        public Form1()
        {
            InitializeComponent();
#if DEBUG
            txtbLink.Text = @"http://localhost:56768/Home/Login";
            txtbPassword.Text = "1";
            txtbUsername.Text = "ntson1";
#endif
        }

        private async void btnLogin_ClickAsync(object sender, EventArgs e)
        {
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            handler.UseCookies = true;
            Uri uri = new Uri(txtbLink.Text);
            domain = uri.Scheme + "://" + uri.Authority;
            domain = domain[domain.Length - 1] == '/' ? domain : domain + "/";
            HttpClient httpClient = new HttpClient(handler);
            var formData = new FormUrlEncodedContent(new[] {
                 new KeyValuePair<string, string>("Username", txtbUsername.Text),
                    new KeyValuePair<string, string>("Password", txtbPassword.Text)
            });
            var response = await httpClient.PostAsync(uri, formData);
            var a = cookies.Count;

            var responseStream = response.Content.ReadAsStreamAsync();
            StreamReader readStream = new StreamReader(responseStream.Result, Encoding.UTF8);
            //txtbResult.Text = response.StatusCode.ToString();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                linkToXml = response.Headers.GetValues("Return").First().ToString();
                var strHTML = readStream.ReadToEnd();
                //Get XML
                uri = new Uri(domain + linkToXml);
                var btyeXML = await httpClient.GetByteArrayAsync(uri);
                var strXML = System.Text.Encoding.UTF8.GetString(btyeXML);
                Decrypt(strHTML, strXML);
            }
        }

        public string Decrypt(string HTML, string XML)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(XML);

            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(HTML);
            
            return "";
        }
    }
}
