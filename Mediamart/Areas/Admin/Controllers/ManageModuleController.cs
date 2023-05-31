using Mediamart.Models;
using Mediamart.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mediamart.Areas.Admin.Controllers
{
    [Permission(Module = "managerole")]
    public class ManageModuleController : BaseController
    {
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.AspModules
                        select a;

            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Module.Contains(textsearch)
                || a.Description.Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(model.OrderBy(a => a.Module).ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Edit(string module)
        {
            var model = db.AspModules.FirstOrDefault(a=>a.Module == module);
            return PartialView("~/Areas/Admin/Views/ManageModule/_Edit.cshtml", model);
        }

        [HttpPost]
        public ActionResult EditConfirm([Bind(Include = "")] AspModule _user)
        {
            if (ModelState.IsValid)
            {
                var model = db.AspModules.FirstOrDefault(a => a.Module == _user.Module);
                model.Description = _user.Description;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");
        }
    }
}