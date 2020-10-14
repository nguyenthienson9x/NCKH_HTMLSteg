using NCKH.HTMLSteg.Common;
using NCKH.HTMLSteg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NCKH.HTMLSteg.Areas.Admin.Controllers
{
    public class MenuController : Controller
    {
        HTMLStegDbContext HTMLStegDbContext = new HTMLStegDbContext();

        // GET: Admin/Menu
        public ActionResult Index()
        {
            var listLeftMenu = (from menus in HTMLStegDbContext.Menus where (menus.Status==Constants.Status_Active && menus.Type=="Left") select menus).ToList();
            return PartialView(listLeftMenu);
        }
    }
}