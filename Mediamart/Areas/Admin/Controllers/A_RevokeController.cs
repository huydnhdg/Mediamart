using Mediamart.Areas.Admin.Data.Accessary;
using Mediamart.Models;
using Mediamart.Utils;
using NLog;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mediamart.Areas.Admin.Controllers
{
    [Permission(Module = "revoke")]
    public class A_RevokeController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public ActionResult Index(int? page, string textsearch, string station, string chanel, string status, string start_date, string end_date)
        {
            var model = from a in db.A_Revoke
                        select a;

            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Createby.Contains(textsearch)
                || a.Successby.Contains(textsearch)
                || a.Accessary.Contains(textsearch)
                || a.Accessary_Change.Contains(textsearch)
                || a.StationId.Contains(textsearch)
                || a.WarrantiCode.Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }
            if (!string.IsNullOrEmpty(station))
            {
                model = model.Where(a => a.Createby.Contains(station));
                ViewBag.station = station;
            }
            if (!string.IsNullOrEmpty(status))
            {
                int st = int.Parse(status);
                model = model.Where(a => a.Status == st);
                ViewBag.status = status;
            }
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
            if (User.IsInModule("revoke.user"))
            {
                model = model.Where(a => a.Createby == User.Identity.Name || a.StationId == User.Identity.Name);
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(model.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Report()
        {
            var query = new Model_Export_Report();
            var list_station = from a in db.A_Revoke
                               group a by a.Createby
                               into gr
                               select new Export_Report()
                               {
                                   Name = gr.Key,
                                   all = db.A_Revoke.Where(z => z.Createby == gr.Key).Count(),
                                   news = db.A_Revoke.Where(z => z.Createby == gr.Key && z.Status == 0).Count(),
                                   cancel = db.A_Revoke.Where(z => z.Createby == gr.Key && z.Status == 2).Count(),
                                   success = db.A_Revoke.Where(z => z.Createby == gr.Key && z.Status == 1).Count(),
                               };
            query.Station = list_station.ToList();

            var list_model = from a in db.A_Revoke_Items
                             group a by a.ProductCode into gr
                             select new Export_Report()
                             {
                                 Name = gr.Key,
                                 all = db.A_Revoke_Items.Where(z => z.ProductCode == gr.Key).Count(),
                                 news = db.A_Revoke_Items.Where(z => z.ProductCode == gr.Key && z.Status == 0).Count(),
                                 cancel = db.A_Revoke_Items.Where(z => z.ProductCode == gr.Key && z.Status == 2).Count(),
                                 success = db.A_Revoke_Items.Where(z => z.ProductCode == gr.Key && z.Status == 1).Count(),
                             };
            query.Model = list_model.ToList();
            return View(query);
        }

        [HttpPost]
        public ActionResult ViewConfirm(long Id)
        {
            var query = from a in db.A_Revoke
                        select new A_Revoke_Model()
                        {
                            Id = a.Id,
                            WarrantiId = a.WarrantiId,
                            WarrantiCode = a.WarrantiCode,
                            StationId = a.StationId,
                            Status = a.Status,
                            Hub = a.Hub,
                            Createby = a.Createby,
                            Createdate = a.Createdate,
                            Successby = a.Successby,
                            Successdate = a.Successdate,
                            Note = a.Note,
                            Items = db.A_Revoke_Items.Where(i => i.RevokeId == a.Id).ToList()
                        };
            var model = query.FirstOrDefault(a => a.Id == Id);
            ViewBag.Warranti = db.Warrantis.Find(model.WarrantiId);
            return PartialView("~/Areas/Admin/Views/A_Revoke/_Confirm.cshtml", model);
        }

        [Permission(Module = "revoke.action")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActionConfirm([Bind(Include = "")] A_Revoke_Model _model, int Status)
        {
            if (ModelState.IsValid)
            {
                var revoke = db.A_Revoke.Find(_model.Id);
                string status = "";
                if (Status == 0)
                {
                    //admin hủy xác nhận
                    revoke.Status = 0;
                    revoke.Successdate = null;
                    revoke.Successby = null;
                    revoke.Note = null;

                    status = "Tạo mới";
                }
                else if (Status == 2)
                {
                    revoke.Status = 2;
                    revoke.Successdate = DateTime.Now;
                    revoke.Successby = User.Identity.Name;
                    revoke.Note = _model.Note;
                    status = "Huỷ xác";
                }
                else
                {
                    revoke.Status = 1;
                    revoke.Successdate = DateTime.Now;
                    revoke.Successby = User.Identity.Name;
                    revoke.Note = _model.Note;
                    status = "Hoàn thành";
                }

                db.Entry(revoke).State = EntityState.Modified;

                //ghi log
                var log = new A_Revoke_Log()
                {
                    ActionId = revoke.Id,
                    Createdate = DateTime.Now,
                    Description = User.Identity.Name + " thay đổi trạng thái thành " + status
                };
                db.A_Revoke_Log.Add(log);

                // thay đổi trạng thái cho chi tiết phiếu luôn

                var ex_item = db.A_Revoke_Items.Where(a => a.RevokeId == revoke.Id);
                foreach (var item in ex_item)
                {
                    var _ex = db.A_Revoke_Items.Find(item.Id);
                    _ex.Status = revoke.Status;
                    db.Entry(_ex).State = EntityState.Modified;
                }

                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult ViewLog(long Id)
        {
            var log = db.A_Revoke_Log.Where(a => a.ActionId == Id);
            ViewBag.Code = db.A_Revoke.Find(Id).WarrantiCode;
            return PartialView("~/Areas/Admin/Views/A_Revoke/_Viewlog.cshtml", log.ToList());
        }
    }
}