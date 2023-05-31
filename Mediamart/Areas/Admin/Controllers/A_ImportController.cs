using Mediamart.Areas.Admin.Data.Accessary;
using Mediamart.Utils;
using NLog;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mediamart.Areas.Admin.Controllers
{
    [Permission(Module = "import")]
    public class A_ImportController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date)
        {
            var model = from a in db.A_Import
                        select a;

            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Createby.Contains(textsearch)
                || a.Code.Contains(textsearch)
                || a.Note.Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }
            //if (!string.IsNullOrEmpty(status))
            //{
            //    int st = int.Parse(status);
            //    model = model.Where(a => a.Status == st);
            //    ViewBag.status = status;
            //}
            if (!string.IsNullOrEmpty(start_date))
            {
                DateTime s = DateTime.ParseExact(start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model = model.Where(a => a.Createdate >= s);
                ViewBag.start_date = start_date;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                DateTime s = DateTime.ParseExact(end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                model = model.Where(a => a.Createdate <= s);
                ViewBag.end_date = end_date;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(model.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Views(long Id)
        {
            var model = from a in db.A_Import
                        select new A_Import_Model()
                        {
                            Id = a.Id,
                            Code = a.Code,
                            Quantity = a.Quantity,
                            Discount = a.Discount,
                            Amount = a.Amount,
                            Total = a.Total,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Note = a.Note,
                            Items = db.A_Import_Items.Where(i => i.ImportId == Id).ToList()
                        };
            var list = db.A_Import_Items.Where(i => i.ImportId == Id).ToList();
            return PartialView("~/Areas/Admin/Views/A_Import/_Views.cshtml", model.FirstOrDefault(a => a.Id == Id));
        }

    }
}