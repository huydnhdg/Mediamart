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
using static Mediamart.Utils.Common;

namespace Mediamart.Areas.Admin.Controllers
{
    [Permission(Module = "station")]
    public class A_StationController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date)
        {
            var model = from a in db.A_Station
                        select a;

            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Name.Contains(textsearch)
                || a.Model.Contains(textsearch)
                || a.Code.Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }
            if (!string.IsNullOrEmpty(status))
            {
                if (status.Equals("0"))
                {
                    model = model.Where(a => a.CountImport - a.CountExport > 0);
                    ViewBag.status = status;
                }
                else
                {
                    model = model.Where(a => a.CountImport - a.CountExport == 0);
                    ViewBag.status = status;
                }
            }
            if (!string.IsNullOrEmpty(chanel))
            {
                model = model.Where(a => a.StationId == chanel);
                ViewBag.chanel = chanel;
            }
            ViewBag.station = db.AspNetUsers.Where(a => a.AspNetRoles.FirstOrDefault(r => r.Id == Roles.KEY) != null).ToList();
            if (User.IsInModule("station.user"))
            {
                model = model.Where(a => a.StationId == User.Identity.Name);
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(model.OrderBy(a => a.Name).ToPagedList(pageNumber, pageSize));
        }
        [Permission(Module = "station.add")]
        public ActionResult Create()
        {
            var user = db.AspNetUsers.FirstOrDefault(a => a.UserName == User.Identity.Name);
            var model = new A_Export_Model()
            {
                Items = new List<Models.A_Export_Items>(),
                Hub = user.Hub,
                CusName = user.FullName,
                Address = user.Address,


            };
            return View(model);
        }
        [HttpPost]
        public ActionResult Create([Bind(Include = "")] A_Export_Model model, string WarrantiCode)
        {
            string msg = "Có lỗi xảy ra trong quá trình lưu phiếu.";
            try
            {
                string station;
                //tài khoản trạm thì lấy luôn, tài khoản thợ thì chuyển về trạm
                var cr_user = db.AspNetUsers.FirstOrDefault(a => a.UserName == User.Identity.Name);
                if (User.IsInRole(Roles.KTV))
                {
                    //station = cr_user.Createby;
                    var s = db.AspNetUsers.FirstOrDefault(a => a.Zone == cr_user.Zone && a.AspNetRoles.FirstOrDefault(r => r.Id == Roles.KEY) != null);
                    station = s.UserName;
                }
                else
                {
                    station = cr_user.UserName;
                }
                //đề xuất trong phiếu bảo hành sẽ phải tạo phiếu hoàn xác
                long rId = 0;
                if (model.WarrantiId > 0)
                {
                    //tạo phiếu hoàn xác
                    var revoke = new A_Revoke();

                    revoke.Createdate = DateTime.Now;
                    revoke.Createby = User.Identity.Name;
                    revoke.WarrantiId = model.WarrantiId;
                    revoke.WarrantiCode = WarrantiCode;
                    revoke.Hub = model.Hub;
                    revoke.StationId = station;
                    db.A_Revoke.Add(revoke);
                    db.SaveChanges();
                    rId = revoke.Id;
                }

                //tạo phiếu đề xuất
                var export = new A_Export();

                export.CusName = model.CusName;
                export.Hub = model.Hub;
                export.Center = model.Center;
                export.Import_Warehouse = station;
                export.Address = model.Address;
                export.Export_Warehouse = model.Export_Warehouse;
                export.Note = model.Note;
                export.Createdate = DateTime.Now;
                export.Createby = User.Identity.Name;
                export.WarrantiId = model.WarrantiId;
                db.A_Export.Add(export);
                db.SaveChanges();

                int quantity = 0;
                int total = 0;
                int amount = 0;
                if (model.Items.Count() > 0)
                {
                    foreach (var item in model.Items)
                    {
                        var check_station = db.A_Station.FirstOrDefault(a => a.StationId == station && a.Code == item.ProductCode);
                        int isHas = check_station.CountImport - check_station.CountExport;
                        int tot = item.Price * item.Quantity;
                        var _item = new A_Export_Items()
                        {
                            ProductCode = item.ProductCode,
                            ProductName = item.ProductName,
                            ProductModel = item.ProductModel,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            Amount = tot,
                            Total = tot,
                            WarrantiCode = item.WarrantiCode,
                            Note = item.Note,
                            ExportId = export.Id,
                            Status = (isHas > 0) ? 1 : 0,

                        };
                        db.A_Export_Items.Add(_item);
                        //
                        //đề xuất trong phiếu bảo hành sẽ phải tạo phiếu hoàn xác
                        if (model.WarrantiId > 0)
                        {
                            var _rItem = new A_Revoke_Items()
                            {
                                ProductCode = item.ProductCode,
                                ProductName = item.ProductName,
                                ProductModel = item.ProductModel,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                Amount = tot,
                                Total = tot,
                                WarrantiCode = item.WarrantiCode,
                                Note = item.Note,
                                RevokeId = rId
                            };
                            db.A_Revoke_Items.Add(_rItem);
                        }
                        quantity = quantity + item.Quantity;
                        total = total + tot;
                        amount = amount + tot;

                    }
                    //add linh kiện vào phiếu xuất luôn
                    db.SaveChanges();
                }
                export.Quantity = quantity;
                export.Total = total;
                export.Amount = amount;
                // nếu kỹ thuật viên tạo phiếu thì phải đợi trạm xác nhận mới chuyển lên trên
                // tạo trạng thái cho trạm xác nhận
                // nếu linh kiện còn ở trạm thì tạo phiếu nội bộ trạm xác nhận KTV luôn
                var check_phieu = db.A_Export_Items.Where(a => a.ExportId == export.Id && a.Status == 0);
                if (check_phieu.Count() > 0)
                {
                    //phiếu đề xuất lên trên
                    export.Status = Common.Create_Accessary.KTV_CREATE_ORDER;
                }
                else
                {
                    //phiếu đề xuất nội bộ
                    export.Status = Common.Create_Accessary.KTV_CREATE_USE;
                }
                if (station != User.Identity.Name && check_phieu != null)
                {
                    //tao thong bao
                    Signal.SendSignal signal = new Signal.SendSignal();
                    Notification notif = new Notification()
                    {
                        SentTo = station,
                        Createdate = DateTime.Now,
                        Title = String.Format(TitleNotif.accessary_ktv_use, WarrantiCode),
                        DetailsURL = String.Format(TitleNotif.url_accessary, export.Id)
                    };
                    signal.Save(notif);
                }
                db.Entry(export).State = EntityState.Modified;

                var log = new A_Export_Log()
                {
                    Createdate = DateTime.Now,
                    Createby = User.Identity.Name,
                    ExportId = export.Id,
                    Description = " Tạo phiếu đề xuất linh kiện"
                };
                db.A_Export_Log.Add(log);

                db.SaveChanges();

                SetAlert("Phiếu đề xuất được tạo thành công.", "success");
                return RedirectToAction("Index", "A_Export");
            }
            catch (Exception ex)
            {
                SetAlert(msg, "danger");
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult KTVCreate([Bind(Include = "")] A_Export_Model model, string WarrantiCode)
        {
            string msg = "Có lỗi xảy ra trong quá trình lưu phiếu.";
            try
            {
                string station;
                //tài khoản trạm thì lấy luôn, tài khoản thợ thì chuyển về trạm
                var cr_user = db.AspNetUsers.FirstOrDefault(a => a.UserName == User.Identity.Name);
                if (User.IsInRole(Roles.KTV))
                {
                    var s = db.AspNetUsers.FirstOrDefault(a => a.Zone == cr_user.Zone && a.AspNetRoles.FirstOrDefault(r => r.Id == Roles.KEY) != null);
                    station = cr_user.Createby;
                }
                else
                {
                    station = cr_user.UserName;
                }

                //tạo phiếu đề xuất
                var export = new A_Export();

                export.CusName = model.CusName;
                export.Hub = model.Hub;
                export.Center = model.Center;
                export.Import_Warehouse = station;
                export.Address = model.Address;
                export.Export_Warehouse = model.Export_Warehouse;
                export.Note = model.Note;
                export.Createdate = DateTime.Now;
                export.Createby = User.Identity.Name;
                export.WarrantiId = model.WarrantiId;
                db.A_Export.Add(export);
                db.SaveChanges();

                int quantity = 0;
                int total = 0;
                int amount = 0;
                if (model.Items.Count() > 0)
                {
                    foreach (var item in model.Items)
                    {
                        //check còn linh kiện không đã
                        int tot = item.Price * item.Quantity;
                        var _item = new A_Export_Items()
                        {
                            ProductCode = item.ProductCode,
                            ProductName = item.ProductName,
                            ProductModel = item.ProductModel,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            Amount = tot,
                            Total = tot,
                            WarrantiCode = item.WarrantiCode,
                            Note = item.Note,
                            ExportId = export.Id,
                        };
                        db.A_Export_Items.Add(_item);
                        quantity = quantity + item.Quantity;
                        total = total + tot;
                        amount = amount + tot;

                    }
                }
                export.Quantity = quantity;
                export.Total = total;
                export.Amount = amount;
                // đề xuất nội bộ
                if (station != User.Identity.Name)
                {
                    export.Status = Common.Create_Accessary.KTV_CREATE_USE;
                    //tao thong bao
                    Signal.SendSignal signal = new Signal.SendSignal();
                    Notification notif = new Notification()
                    {
                        SentTo = station,
                        Createdate = DateTime.Now,
                        Title = String.Format(TitleNotif.accessary_ktv_use, WarrantiCode),
                        DetailsURL = String.Format(TitleNotif.url_accessary, export.Id)
                    };
                    signal.Save(notif);
                }
                db.Entry(export).State = EntityState.Modified;

                var log = new A_Export_Log()
                {
                    Createdate = DateTime.Now,
                    Createby = User.Identity.Name,
                    ExportId = export.Id,
                    Description = " Tạo phiếu đề xuất linh kiện"
                };
                db.A_Export_Log.Add(log);

                db.SaveChanges();

                SetAlert("Phiếu đề xuất được tạo thành công.", "success");
                return RedirectToAction("Index", "A_Export");
            }
            catch (Exception ex)
            {
                SetAlert(msg, "danger");
                return View(model);
            }
        }
        [HttpPost]
        public JsonResult GetPriceByCode(string text)
        {
            var cate = db.A_Access_Center.FirstOrDefault(a => a.Code == text);
            return Json(cate, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAccessaryByCode(string text, string model)
        {
            var cate = (from a in db.A_Access_Center
                        where (a.CountImport - a.CountExport > 0)
                        where a.Code.Contains(text) || a.Name.Contains(text)
                        select new { a.Code, a.Name, a.Model, a.Price });
            if (!string.IsNullOrEmpty(model))
            {
                cate = cate.Where(a => a.Model == model);
            }
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetAccessaryByModel(string text)
        {
            var cate = (from a in db.A_Access_Center
                        where a.Model == text
                        select new { a.Code, a.Name, a.Model, a.Price });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetModel(string text)
        {
            var cate = (from a in db.A_Access_Center
                        group a by a.Model into g
                        where g.Key.Contains(text)
                        select new { g.Key });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ViewConfirm(long Id)
        {
            var user = db.AspNetUsers.FirstOrDefault(a => a.UserName == User.Identity.Name);
            var model = new A_Export_Model()
            {
                Items = new List<Models.A_Export_Items>(),
                Hub = user.Hub,
                CusName = user.FullName,
                Address = user.Address,
            };
            ViewBag.Warranti = db.Warrantis.Find(Id);
            return PartialView("~/Areas/Admin/Views/A_Station/_Confirm.cshtml", model);
        }

        [HttpPost]
        public ActionResult KTVViewConfirm(long Id)
        {

            var model = new A_Export_Model()
            {
                Items = new List<Models.A_Export_Items>()
            };
            ViewBag.Warranti = db.Warrantis.Find(Id);
            return PartialView("~/Areas/Admin/Views/A_Station/_KTVConfirm.cshtml", model);
        }

        [HttpPost]
        public JsonResult KTVGetAccessaryByCode(string text, string model)
        {
            // lấy linh kiện còn tồn ở trạm
            //var cate = (from a in db.A_Station
            //            where (a.CountImport - a.CountExport > 0)
            //            where a.Code.Contains(text) || a.Name.Contains(text)
            //            select new { a.Code, a.Name, a.Model, a.Price, Count = 0, a.CountImport });

            //lấy linh kiện ở tổng
            var cate = (from a in db.A_Access_Center
                        where a.CountImport - a.CountExport > 0
                        where a.Code.Contains(text) || a.Name.Contains(text)
                        select new { a.Code, a.Name, a.Model, a.Price }
                       );
            if (!string.IsNullOrEmpty(model))
            {
                cate = cate.Where(a => a.Model == model);
            }
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult KTVGetModel(string text)
        {
            var cate = (from a in db.A_Station
                        group a by a.Model into g
                        where g.Key.Contains(text)
                        select new { g.Key });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult KTVGetPriceByCode(string text)
        {
            var cate = db.A_Station.FirstOrDefault(a => a.Code == text);
            return Json(cate, JsonRequestBehavior.AllowGet);
        }

    }
}