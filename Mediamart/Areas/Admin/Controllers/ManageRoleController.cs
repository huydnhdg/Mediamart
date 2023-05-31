using Mediamart.Areas.Admin.Data;
using Mediamart.Models;
using Mediamart.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mediamart.Areas.Admin.Controllers
{
    [Permission(Module = "managerole")]
    public class ManageRoleController : BaseController
    {
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.AspNetRoles
                        select a;

            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Id.Contains(textsearch)
                || a.Name.Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(model.OrderBy(a => a.Id).ToPagedList(pageNumber, pageSize));
        }
        [Permission(Module = "managerole.edit")]
        public ActionResult EditModule(string RoleId)
        {
            //var query = db.AspModuleRoles.Where(a => a.RoleId == RoleId);
            var query = from a in db.AspModuleRoles
                        select new ModuleView()
                        {
                            RoleId = a.RoleId,
                            ModuleId = a.ModuleId,
                            Description = db.AspModules.FirstOrDefault(r => r.Module == a.ModuleId).Description
                        };
            query = query.OrderBy(a => a.Description).Where(a => a.RoleId == RoleId);
            ViewBag.RoleId = RoleId;
            ViewBag.ModuleId = new SelectList(db.AspModules.Where(item => query.FirstOrDefault(r => r.ModuleId == item.Module) == null).ToList(), "Module", "Description");
            return View(query);
        }
        [Permission(Module = "managerole.add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToModule(string RoleId, string[] ModuleId)
        {
            var query = db.AspModuleRoles.Where(a => a.RoleId == RoleId);
            if (ModuleId != null && ModuleId.Count() > 0)
            {
                foreach (string item in ModuleId)
                {
                    var role_module = new AspModuleRole()
                    {
                        RoleId = RoleId,
                        ModuleId = item
                    };
                    db.AspModuleRoles.Add(role_module);
                }
                db.SaveChanges();
            }
            ViewBag.RoleId = RoleId;
            ViewBag.ModuleId = new SelectList(db.AspModules.Where(item => query.FirstOrDefault(r => r.ModuleId == item.Module) == null).ToList(), "Module", "Description");
            return RedirectToAction("EditModule", new { RoleId = RoleId });
        }
        [Permission(Module = "managerole.delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteModuleFromRole(string RoleId, string ModuleId)
        {
            var query = db.AspModuleRoles.Where(a => a.RoleId == RoleId);
            db.AspModuleRoles.Remove(query.FirstOrDefault(a => a.ModuleId == ModuleId));
            db.SaveChanges();
            ViewBag.RoleId = RoleId;
            ViewBag.ModuleId = new SelectList(db.AspModules.Where(item => query.FirstOrDefault(r => r.ModuleId == item.Module) == null).ToList(), "Module", "Description");
            return RedirectToAction("EditModule", new { RoleId = RoleId });
        }
    }
}