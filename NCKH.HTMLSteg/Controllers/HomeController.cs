using NCKH.HTMLSteg.Common;
using NCKH.HTMLSteg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;

namespace NCKH.HTMLSteg.Controllers
{
    public class HomeController : Controller
    {
        HTMLStegDbContext HTMLStegDbContext = new HTMLStegDbContext();
        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            Response.AppendHeader("Return", "ThienSon");
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
            string username = fc["Username"].Trim();
            string password = fc["Password"].Trim();
            string hashedPassword = CommonUtils.HashSHA1(password);
            var user = HTMLStegDbContext.Users.FirstOrDefault(x => x.UserRole == Constants.UserRole_User && x.Username == username && x.Password == hashedPassword);
            if (user == null)
            {
                if (Request.Browser.Browser == "Unknown")
                {
                    return new HttpStatusCodeResult(403);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            var authTicket = new FormsAuthenticationTicket(
    1,                             // version
    username,                      // user name
    DateTime.Now,                  // created
    DateTime.Now.AddMinutes(20),   // expires
    true,                          // persistent?
    Constants.UserRole_User        // can be used to store roles
    );

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            Session["UserName"] = user.FullName;
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
            return RedirectToAction("UserInfo");
        }


        [Authorize(Roles = "User")]
        public ActionResult UserInfo()
        {
            try
            {
                //Get user by username
                Response.AppendHeader("Return", "Home/GetXML");
                var user = HTMLStegDbContext.Users.First(x => x.Username == HttpContext.User.Identity.Name);
                var html = HTMLStegDbContext.ReturnPages.FirstOrDefault(x => x.ID == user.ReturnPageID).HTML;

                //Get XML from File;;;
                var fileName = user.XMLKey.FilePath;
                var filePath = AppDomain.CurrentDomain.BaseDirectory + @"/File_Upload/XML/" + fileName;
                byte[] filedata = System.IO.File.ReadAllBytes(filePath);
                var xml = Encoding.UTF8.GetString(filedata);

                html = EncryptHtml(html, xml);

                ViewBag.Data = html;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "User")]
        public ActionResult GetXML()
        {
            try
            {
                if (Request.Browser.Browser != "Unknown")
                {
                    //throw new AccessViolationException();
                }
                //Get user by username
                var user = HTMLStegDbContext.Users.First(x => x.Username == HttpContext.User.Identity.Name);
                var fileName = user.XMLKey.FilePath;
                var filePath = AppDomain.CurrentDomain.BaseDirectory + @"/File_Upload/XML/"+fileName ;
                byte[] filedata = System.IO.File.ReadAllBytes(filePath);
                string contentType = MimeMapping.GetMimeMapping(filePath);
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = fileName,
                    Inline = true,
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(filedata, contentType);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        public string EncryptHtml(string HTML, string XML)
        {
            try
            {
                List<Tuple<string, string>> listXMLAttr = new List<Tuple<string, string>>();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(XML);
                var allTagsXML = xmlDoc.GetElementsByTagName("*");
                foreach(XmlNode node in allTagsXML)
                {
                    if (node.Attributes.Count > 1)
                    {
                        listXMLAttr.Add(new Tuple<string, string>(node.Attributes[0].Name, node.Attributes[0].Name));
                    }
                }
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(HTML);
                //Lưu các attr vào 1 List. remove all các attr trong node. Swap List rồi add lại
                var allTagsHTML = htmlDoc.DocumentNode.DescendantNodes().Where(x => x.NodeType == HtmlAgilityPack.HtmlNodeType.Element);
                /*foreach(var tag in allTagsHTML)
                {
                    if (tag.Attributes.Count > 0)
                    {
                        tag.Attributes.Remove();
                    }
                }*/
                foreach(var tag in htmlDoc.DocumentNode.DescendantNodes().Where(x => x.NodeType == HtmlAgilityPack.HtmlNodeType.Element))
                {
                    tag.Attributes.RemoveAll();
                }
               

                return htmlDoc.DocumentNode.OuterHtml;
            }
            catch (Exception ex)
            {
                return HTML;
            }
        }
    }
}