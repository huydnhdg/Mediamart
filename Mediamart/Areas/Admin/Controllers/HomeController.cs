using Mediamart.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Mediamart.Areas.Admin.Controllers
{
    [Permission(Module = "home")]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Warranti");
        }

        [HttpPost]
        public ActionResult Index(string type)
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

        public PartialViewResult _LoadNotification()
        {
            var query = db.Notifications.OrderByDescending(a => a.Createdate).Where(a => a.SentTo == User.Identity.Name && a.IsRead == null);

            return PartialView(query);
        }
        public PartialViewResult _LoadProfile()
        {
            var query = db.AspNetUsers.FirstOrDefault(a => a.UserName == User.Identity.Name);

            return PartialView(query);
        }
        [HttpPost]
        public JsonResult ReadNotif(long Id)
        {
            var model = db.Notifications.Find(Id);
            model.IsRead = true;
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult loadDataNotif()
        {
            var query = db.Notifications.OrderByDescending(a=>a.Createdate).Where(a => a.SentTo == User.Identity.Name).Take(10);
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult loadModalNotif()
        {
            var query = db.Notifications.OrderByDescending(a => a.Createdate).FirstOrDefault(a=>a.SentTo == User.Identity.Name);
            if(query.IsRead == null)
            {
                return PartialView("~/Areas/Admin/Views/Home/_loadModalNotif.cshtml", query);
            }
            else
            {
                return PartialView("~/Areas/Admin/Views/Home/_loadModalNotif.cshtml", null);
            }
        }
    }
}