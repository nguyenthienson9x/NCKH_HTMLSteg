using NCKH.HTMLSteg.Common;
using NCKH.HTMLSteg.Models;
using NCKH.HTMLSteg.Models.Dictionary;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NCKH.HTMLSteg.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        HTMLStegDbContext HTMLStegDbContext = new HTMLStegDbContext();
        // GET: Admin/User
        //[AllowAnonymous]
        public ActionResult Index()
        {
            var listUser = HTMLStegDbContext.Users.Where(x => x.Status != "Deleted" && x.UserRole == Constants.UserRole_User).OrderByDescending(x => x.ID).ToList();
            var listXML = HTMLStegDbContext.XMLKeys.Where(x => x.Status != "Deleted").OrderByDescending(x => x.ID).ToList();
            var listHTML = HTMLStegDbContext.ReturnPages.Where(x => x.Status != "Deleted").OrderByDescending(x => x.ID).ToList();

            dynamic model = new ExpandoObject();
            model.listUser = listUser;
            model.listXML = listXML;
            model.listHTML = listHTML;
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetByID(int id)
        {
            var user = HTMLStegDbContext.Users.Where(x => x.ID == id).FirstOrDefault();
            if (user == null)
            {
                return Json(null);
            }
            return Json(user);
        }

        [HttpPost]
        public ActionResult AddUser(User user)
        {
            try
            {
                user.Password = CommonUtils.HashSHA1(user.Password);
                user.Status = Constants.Status_Active;
                user.UserRole = Common.Constants.UserRole_User;
                HTMLStegDbContext.Users.Add(user);
                HTMLStegDbContext.SaveChanges();
            }
            catch
            {
                return Json(new { Result = "Fail" });
            }

            return Json(new { Result = "OK", ID = user.ID });
        }

        [HttpPut]
        public ActionResult UpdateUser(User user)
        {
            try
            {
                var currentUser = HTMLStegDbContext.Users.Where(x => x.ID == user.ID).FirstOrDefault();
                currentUser.FullName = user.FullName;
                currentUser.Message = user.Message;
                currentUser.Username = user.Username;
                currentUser.Status = user.Status;
                currentUser.XMLKeyID = user.XMLKeyID;
                currentUser.ReturnPageID = user.ReturnPageID;
                if (user.Password != currentUser.Password)
                {
                    currentUser.Password = user.Password;
                }

                if(user.Message!="" && Request.Form["HTMLContent"] != null)
                {
                    if(!ValidateHTML(user.Message, Request.Form["HTMLContent"].ToString()))
                    {
                        return Json(new { Result = "Not OK" });
                    }
                }
                else if(user.Message != "" && Request.Form["HTMLContent"] == null)
                {
                    if (!ValidateHTML(user.Message, currentUser.ReturnPage.HTML))
                    {
                        return Json(new { Result = "Not OK" });
                    }
                }

                if (Request.Files.Count > 0)
                {
                    XMLKey xmlKey = new XMLKey();
                    HttpPostedFileBase Xml = Request.Files[0];
                    Xml.SaveAs(Server.MapPath("/File_Upload/XML/") + Xml.FileName);
                    xmlKey.FilePath = Xml.FileName;
                    xmlKey.isPrivate = true;
                    xmlKey.KeyName = "Khóa của " + user.FullName;
                    xmlKey.Status = Constants.Status_Active;
                    HTMLStegDbContext.XMLKeys.Add(xmlKey);
                    HTMLStegDbContext.SaveChanges();
                    currentUser.XMLKeyID = xmlKey.ID;
                }
                if (Request.Form["HTMLContent"] != null)
                {
                    var html = Request.Form["HTMLContent"].ToString();
                    ReturnPage rp = new ReturnPage();
                    rp.HTML = html;
                    rp.isPrivate = true;
                    rp.Name = "HTML của " + user.FullName;
                    rp.Status = Constants.Status_Active;
                    HTMLStegDbContext.ReturnPages.Add(rp);
                    HTMLStegDbContext.SaveChanges();
                    currentUser.ReturnPageID = rp.ID;
                }
                HTMLStegDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Fail" });
            }

            return Json(new { Result = "OK" });
        }

        [HttpDelete]
        public ActionResult DeleteUser(int id)
        {
            try
            {
                var user = HTMLStegDbContext.Users.Where(x => x.ID == id).FirstOrDefault();
                user.Status = Constants.Status_Deleted;
                HTMLStegDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Fail" });
            }

            return Json(new { Result = "OK" });
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
            string username = fc["Username"].Trim();
            string password = fc["Password"].Trim();
            string hashedPassword = CommonUtils.HashSHA1(password);
            var user = HTMLStegDbContext.Users.FirstOrDefault(x => x.UserRole == Constants.UserRole_Admin && x.Username == username && x.Password == hashedPassword);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            var authTicket = new FormsAuthenticationTicket(
    1,                             // version
    username,                      // user name
    DateTime.Now,                  // created
    DateTime.Now.AddMinutes(20),   // expires
    true,                          // persistent?
    Constants.UserRole_Admin       // can be used to store roles
    );

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            Session["AdminName"] = user.FullName;
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
            return RedirectToAction("Index");
        }


        public bool ValidateHTML(string msg, string html)
        {
            int msgLengthBit = msg.Trim().Length * 8;

            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(html);
            htmlDoc.DocumentNode.SelectNodes("html");

            var allTags = htmlDoc.DocumentNode.DescendantNodes().Where(x => x.NodeType == HtmlAgilityPack.HtmlNodeType.Element);
            int bitHtmlCanHide = 0;
            foreach (var tag in allTags)
            {
                bitHtmlCanHide += tag.Attributes.Count / 2;
            }
            if (msgLengthBit > bitHtmlCanHide)
            {
                return false;
            }
            return true;
        }
    }

}