using Mediamart.Areas.Admin.Data;
using Mediamart.Areas.Admin.Data.Accessary;
using Mediamart.Models;
using Mediamart.Utils;
using NLog;
using PagedList;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using static Mediamart.Utils.Common;

namespace Mediamart.Areas.Admin.Controllers
{
    [Permission(Module = "export")]
    public class A_ExportController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public ActionResult Index(int? page, string textsearch, string station, string chanel, string status, string start_date, string end_date)
        {

            var model = from a in db.A_Export
                        select new A_Export_ListView()
                        {
                            Id = a.Id,
                            Code = db.Warrantis.FirstOrDefault(x => x.Id == a.WarrantiId).Code,
                            WarrantiId = a.WarrantiId,
                            Export_Warehouse = a.Export_Warehouse,
                            CusName = a.CusName,
                            Hub = a.Hub,
                            Import_Warehouse = a.Import_Warehouse,
                            Address = a.Address,
                            Note = a.Note,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Boss = a.Boss,
                            Quantity = a.Quantity,
                            Amount = a.Amount,
                            Total = a.Total,
                            Discount = a.Discount,
                            Status = a.Status,
                        };

            string user = User.Identity.Name;
            if (User.IsInModule("export.user"))
            {
                model = model.Where(a => a.Import_Warehouse == user || a.Createby == user);
            }

            var countstatus = new countstatus()
            {
                all = model.Count(),
                s0 = model.Where(a => a.Status == 0).Count(),
                s1 = model.Where(a => a.Status == 1).Count(),
                s2 = model.Where(a => a.Status == 2).Count(),
                s3 = model.Where(a => a.Status == 3).Count(),
                s4 = model.Where(a => a.Status == 4).Count(),
                s5 = model.Where(a => a.Status == 5).Count()
            };
            ViewBag.countstatus = countstatus;

            if (!string.IsNullOrEmpty(chanel))
            {
                int st = int.Parse(chanel);
                model = model.Where(a => a.Status == st);
                ViewBag.chanel = chanel;
            }
            if (!string.IsNullOrEmpty(station))
            {
                model = model.Where(a => a.Createby == station);
                ViewBag.station = station;
            }

            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Createby.Contains(textsearch)
                || a.CusName.Contains(textsearch)
                || a.Center.Contains(textsearch));
                ViewBag.textsearch = textsearch;
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
            ViewBag.ls_station = db.AspNetUsers.Where(a => a.AspNetRoles.FirstOrDefault(r => r.Id == Roles.KEY) != null).ToList();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(model.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Report()
        {
            var query = new Model_Export_Report();
            var list_station = from a in db.A_Export
                               group a by a.Import_Warehouse
                               into gr
                               select new Export_Report()
                               {
                                   Name = gr.Key,
                                   all = db.A_Export.Where(z => z.Import_Warehouse == gr.Key).Count(),
                                   news = db.A_Export.Where(z => z.Import_Warehouse == gr.Key && z.Status == 0).Count(),
                                   stocker = db.A_Export.Where(z => z.Import_Warehouse == gr.Key && z.Status == 1).Count(),
                                   accountant = db.A_Export.Where(z => z.Import_Warehouse == gr.Key && z.Status == 2).Count(),
                                   admin = db.A_Export.Where(z => z.Import_Warehouse == gr.Key && z.Status == 3).Count(),
                                   success = db.A_Export.Where(z => z.Import_Warehouse == gr.Key && z.Status == 4).Count(),
                               };
            query.Station = list_station.ToList();

            var list_model = from a in db.A_Export_Items
                             group a by a.ProductModel
                               into gr
                             select new Export_Report()
                             {
                                 Name = gr.Key,
                                 all = db.A_Export_Items.Where(z => z.ProductModel == gr.Key).Count(),
                                 news = db.A_Export_Items.Where(z => z.ProductModel == gr.Key && z.Status == 0).Count(),
                                 admin = db.A_Export_Items.Where(z => z.ProductModel == gr.Key && z.Status == 1).Count(),
                                 stocker = db.A_Export_Items.Where(z => z.ProductModel == gr.Key && z.Status == 2).Count(),
                                 accountant = db.A_Export_Items.Where(z => z.ProductModel == gr.Key && z.Status == 3).Count(),
                                 success = db.A_Export_Items.Where(z => z.ProductModel == gr.Key && z.Status == 4).Count(),
                             };
            query.Model = list_model.ToList();
            var list_code = from a in db.A_Export_Items
                            group a by a.ProductCode
                               into gr
                            select new Export_Report()
                            {
                                Name = gr.Key,
                                all = db.A_Export_Items.Where(z => z.ProductCode == gr.Key).Count(),
                                news = db.A_Export_Items.Where(z => z.ProductCode == gr.Key && z.Status == 0).Count(),
                                admin = db.A_Export_Items.Where(z => z.ProductCode == gr.Key && z.Status == 1).Count(),
                                stocker = db.A_Export_Items.Where(z => z.ProductCode == gr.Key && z.Status == 2).Count(),
                                accountant = db.A_Export_Items.Where(z => z.ProductCode == gr.Key && z.Status == 3).Count(),
                                success = db.A_Export_Items.Where(z => z.ProductCode == gr.Key && z.Status == 4).Count(),
                            };
            query.Accessary = list_code.ToList();
            return View(query);
        }
        public class countstatus
        {
            public int all { get; set; }
            public int s0 { get; set; }
            public int s1 { get; set; }
            public int s2 { get; set; }
            public int s3 { get; set; }
            public int s4 { get; set; }
            public int s5 { get; set; }
        }

        [HttpPost]
        public ActionResult ViewConfirm(long Id)
        {
            var model = from a in db.A_Export
                        select new A_Export_Model()
                        {
                            Id = a.Id,
                            CusName = a.CusName,
                            Hub = a.Hub,
                            Center = a.Center,
                            Import_Warehouse = a.Import_Warehouse,
                            Address = a.Address,
                            Export_Warehouse = a.Export_Warehouse,
                            Note = a.Note,
                            Createby = a.Createby,
                            Createdate = a.Createdate,
                            Boss = a.Boss,
                            Quantity = a.Quantity,
                            Amount = a.Amount,
                            Total = a.Total,
                            Discount = a.Discount,
                            Status = a.Status,
                            Items = db.A_Export_Items.Where(i => i.ExportId == a.Id).ToList()

                        };
            return PartialView("~/Areas/Admin/Views/A_Export/_Confirm.cshtml", model.FirstOrDefault(a => a.Id == Id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActionConfirm([Bind(Include = "")] A_Export_Model _model)
        {
            if (ModelState.IsValid)
            {
                var export = db.A_Export.Find(_model.Id);
                if (User.IsInRole("Thủ kho"))
                {
                    //tao thong bao cho ke toan
                    var staff = db.AspNetUsers.Where(a => a.AspNetRoles.FirstOrDefault().Id == "Accountant");
                    foreach (var item in staff)
                    {
                        Signal.SendSignal signal = new Signal.SendSignal();
                        Notification notif = new Notification()
                        {
                            SentTo = item.UserName,
                            Createdate = DateTime.Now,
                            Title = String.Format(TitleNotif.accessary_ktv_use, db.Warrantis.Find(export.WarrantiId).Code),
                            DetailsURL = String.Format(TitleNotif.url_accessary, export.Id)
                        };
                        signal.Save(notif);
                    }
                    if (export.Status == 0)
                    {
                        export.Status = 1;
                    }
                    else
                    {
                        //đang vận chuyển
                        Signal.SendSignal signal = new Signal.SendSignal();
                        Notification notif = new Notification()
                        {
                            SentTo = export.Import_Warehouse,
                            Createdate = DateTime.Now,
                            Title = String.Format(TitleNotif.accessary_ktv_use, db.Warrantis.Find(export.WarrantiId).Code),
                            DetailsURL = String.Format(TitleNotif.url_accessary, export.Id)
                        };
                        signal.Save(notif);
                        export.Status = 4;
                    }
                }
                else if (User.IsInRole("Kế toán"))
                {
                    //tao thong bao cho admin
                    var staff = db.AspNetUsers.Where(a => a.AspNetRoles.FirstOrDefault().Id == "Admin");
                    foreach (var item in staff)
                    {
                        Signal.SendSignal signal = new Signal.SendSignal();
                        Notification notif = new Notification()
                        {
                            SentTo = item.UserName,
                            Createdate = DateTime.Now,
                            Title = String.Format(TitleNotif.accessary_ktv_use, db.Warrantis.Find(export.WarrantiId).Code),
                            DetailsURL = String.Format(TitleNotif.url_accessary, export.Id)
                        };
                        signal.Save(notif);
                    }
                    export.Status = 2;
                }
                else if (User.IsInRole("Admin"))
                {
                    //tao thong bao cho thủ kho
                    var staff = db.AspNetUsers.Where(a => a.AspNetRoles.FirstOrDefault().Id == "Stocker");
                    foreach (var item in staff)
                    {
                        Signal.SendSignal signal = new Signal.SendSignal();
                        Notification notif = new Notification()
                        {
                            SentTo = item.UserName,
                            Createdate = DateTime.Now,
                            Title = String.Format(TitleNotif.accessary_ktv_use, db.Warrantis.Find(export.WarrantiId).Code),
                            DetailsURL = String.Format(TitleNotif.url_accessary, export.Id)
                        };
                        signal.Save(notif);
                    }
                    export.Status = 3;
                }
                else
                {
                    //phần này của trạm
                    if (export.Status == -1)
                    {
                        //trạm đề xuất phiếu được tạo từ KTV
                        export.Status = 0;
                        //tao thong bao cho thu kho trong khu vuc
                        var staff = db.AspNetUsers.Where(a => a.AspNetRoles.FirstOrDefault().Id == "Stocker");
                        foreach (var item in staff)
                        {
                            Signal.SendSignal signal = new Signal.SendSignal();
                            Notification notif = new Notification()
                            {
                                SentTo = item.UserName,
                                Createdate = DateTime.Now,
                                Title = String.Format(TitleNotif.accessary_ktv_use, db.Warrantis.Find(export.WarrantiId).Code),
                                DetailsURL = String.Format(TitleNotif.url_accessary, export.Id)
                            };
                            signal.Save(notif);
                        }
                    }
                    else if (export.Status == -2)
                    {
                        //trạm xác nhận phiếu từ KTV tạo phiếu nội bộ
                        export.Status = 5;

                        //tao thong bao
                        Signal.SendSignal signal = new Signal.SendSignal();
                        Notification notif = new Notification()
                        {
                            SentTo = export.Createby,
                            Createdate = DateTime.Now,
                            Title = String.Format(TitleNotif.acc_ss_ktv_use, db.Warrantis.Find(export.Id).Code),
                            DetailsURL = String.Format(TitleNotif.url_warranti, export.WarrantiId)
                        };
                        signal.Save(notif);

                        string _user = User.Identity.Name;
                        var Items = db.A_Export_Items.Where(a => a.ExportId == export.Id);
                        if (Items.Count() > 0)
                        {
                            var _warranti = db.Warrantis.Find(export.WarrantiId);
                            int pricewarranti = _warranti.Price_Accessary;
                            foreach (var item in Items)
                            {
                                int amout = item.Quantity * item.Price;

                                //công xuất kho linh kiện trạm
                                string _stationId = Guid.NewGuid().ToString();

                                var station = db.A_Station.FirstOrDefault(a => a.Code == item.ProductCode && a.StationId == _user);

                                _stationId = station.Id;
                                station.CountExport = station.CountExport + item.Quantity;
                                station.CountImport = station.CountImport + item.Quantity;
                                db.Entry(station).State = EntityState.Modified;
                                //cộng linh kiện cho phiếu bảo hành và xuất linh kiện ở trạm luôn
                                var acc = new Warranti_Accessary()
                                {
                                    WarrantiId = export.WarrantiId,
                                    ProductName = item.ProductName,
                                    Price = item.Price,
                                    Quantity = item.Quantity,
                                    Amount = amout,
                                    AccessaryId = _stationId
                                };
                                db.Warranti_Accessary.Add(acc);
                                //ghi log linh kiện cho phiếu
                                var log_akey = new Acessary_Log_Key()
                                {
                                    Accessary = acc.ProductName,
                                    Code = item.WarrantiCode,
                                    Count = item.Quantity,
                                    Createdate = DateTime.Now,
                                    Id_Akey = _stationId,
                                    Type = 2
                                };
                                db.Acessary_Log_Key.Add(log_akey);
                                //update tiền vào phiếu
                                pricewarranti = pricewarranti + amout;
                                _warranti.Price_Accessary = pricewarranti;
                                _warranti.Amount = _warranti.Price_Accessary + _warranti.Price_Move + _warranti.Price;
                                db.Entry(_warranti).State = EntityState.Modified;
                            }
                            db.SaveChanges();

                        }
                    }
                    else
                    {
                        //trạm xác nhận đã nhận linh kiện
                        export.Status = 5;
                        //cộng vào ds trạm | cộng vào phiếu bảo hành
                        string _user = User.Identity.Name;
                        if (export.WarrantiId > 0)
                        {
                            //trong phiếu bảo hành
                            var Items = db.A_Export_Items.Where(a => a.ExportId == export.Id);
                            if (Items.Count() > 0)
                            {
                                var _warranti = db.Warrantis.Find(export.WarrantiId);
                                int pricewarranti = _warranti.Price_Accessary;
                                foreach (var item in Items)
                                {
                                    int amout = item.Quantity * item.Price;
                                    //cộng xuất kho linh kiện tổng
                                    var center = db.A_Access_Center.FirstOrDefault(a => a.Code == item.ProductCode);
                                    center.CountExport = center.CountExport + item.Quantity;
                                    db.Entry(center).State = EntityState.Modified;
                                    //công xuất kho linh kiện trạm
                                    string _stationId = Guid.NewGuid().ToString();

                                    var station = db.A_Station.FirstOrDefault(a => a.Code == item.ProductCode && a.StationId == _user);
                                    if(item.Status == 0)
                                    {
                                        //lấy từ trung tâm : cộng xuất, cộng nhập
                                        if (station != null)
                                        {
                                            _stationId = station.Id;
                                            station.CountExport = station.CountExport + item.Quantity;
                                            station.CountImport = station.CountImport + item.Quantity;
                                            db.Entry(station).State = EntityState.Modified;
                                        }
                                        else
                                        {
                                            var _station = new A_Station()
                                            {
                                                Id = _stationId,
                                                Code = item.ProductCode,
                                                Name = item.ProductName,
                                                Model = item.ProductModel,
                                                Price = item.Price,
                                                CountImport = item.Quantity,
                                                CountExport = item.Quantity,
                                                StationId = User.Identity.Name
                                            };
                                            db.A_Station.Add(_station);
                                        }
                                    }
                                    else
                                    {
                                        //lấy ngay tại trạm: cộng xuất
                                        if (station != null)
                                        {
                                            _stationId = station.Id;
                                            station.CountExport = station.CountExport + item.Quantity;
                                            db.Entry(station).State = EntityState.Modified;
                                        }
                                    }
                                    
                                    //cộng linh kiện cho phiếu bảo hành và xuất linh kiện ở trạm luôn
                                    var acc = new Warranti_Accessary()
                                    {
                                        WarrantiId = export.WarrantiId,
                                        ProductName = item.ProductName,
                                        Price = item.Price,
                                        Quantity = item.Quantity,
                                        Amount = amout,
                                        AccessaryId = _stationId
                                    };
                                    db.Warranti_Accessary.Add(acc);
                                    //ghi log linh kiện cho phiếu
                                    var log_akey = new Acessary_Log_Key()
                                    {
                                        Accessary = acc.ProductName,
                                        Code = item.WarrantiCode,
                                        Count = item.Quantity,
                                        Createdate = DateTime.Now,
                                        Id_Akey = _stationId,
                                        Type = 2
                                    };
                                    db.Acessary_Log_Key.Add(log_akey);
                                    //update tiền vào phiếu
                                    pricewarranti = pricewarranti + amout;
                                    _warranti.Price_Accessary = pricewarranti;
                                    _warranti.Amount = _warranti.Price_Accessary + _warranti.Price_Move + _warranti.Price;
                                    db.Entry(_warranti).State = EntityState.Modified;
                                }
                                db.SaveChanges();

                            }
                        }
                        else
                        {
                            //ngoài phiếu bảo hành
                            var Items = db.A_Export_Items.Where(a => a.ExportId == export.Id);
                            if (Items.Count() > 0)
                            {
                                foreach (var item in Items)
                                {
                                    //cộng xuất kho linh kiện tổng
                                    var center = db.A_Access_Center.FirstOrDefault(a => a.Code == item.ProductCode);
                                    center.CountExport = center.CountExport + item.Quantity;
                                    db.Entry(center).State = EntityState.Modified;

                                    //công nhập kho linh kiện trạm
                                    var station = db.A_Station.FirstOrDefault(a => a.Code == item.ProductCode && a.StationId == _user);
                                    if (station != null)
                                    {
                                        station.CountImport = station.CountImport + item.Quantity;
                                        db.Entry(station).State = EntityState.Modified;
                                        //db.SaveChanges();
                                    }
                                    else
                                    {
                                        var _station = new A_Station()
                                        {
                                            Id = Guid.NewGuid().ToString(),
                                            Code = item.ProductCode,
                                            Name = item.ProductName,
                                            Model = item.ProductModel,
                                            Price = item.Price,
                                            CountImport = item.Quantity,
                                            StationId = User.Identity.Name
                                        };
                                        db.A_Station.Add(_station);
                                        //db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }

                }
                db.Entry(export).State = EntityState.Modified;

                // thay đổi trạng thái cho chi tiết phiếu luôn

                var ex_item = db.A_Export_Items.Where(a => a.ExportId == export.Id);
                foreach (var item in ex_item)
                {
                    var _ex = db.A_Export_Items.Find(item.Id);
                    _ex.Status = export.Status;
                    db.Entry(_ex).State = EntityState.Modified;
                }

                var log = new A_Export_Log()
                {
                    Createdate = DateTime.Now,
                    Createby = User.Identity.Name,
                    ExportId = export.Id,
                    Description = "Duyệt phiếu đề xuất linh kiện"
                };
                db.A_Export_Log.Add(log);

                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }

        [Permission(Module = "export.changestatus")]
        [HttpPost]
        public ActionResult ChangeStatus(long Id)
        {
            var model = db.A_Export.Find(Id);
            return PartialView("~/Areas/Admin/Views/A_Export/_ChangeStatus.cshtml", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActionChangeStatus([Bind(Include = "")] A_Export _model)
        {
            if (ModelState.IsValid)
            {
                var query = db.A_Export.Find(_model.Id);
                query.Status = _model.Status;
                db.Entry(query).State = EntityState.Modified;

                var log = new A_Export_Log()
                {
                    Createdate = DateTime.Now,
                    Createby = User.Identity.Name,
                    ExportId = _model.Id,
                    Description = "Thay đổi trạng thái"
                };
                db.A_Export_Log.Add(log);

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
            var log = db.A_Export_Log.Where(a => a.ExportId == Id);
            return PartialView("~/Areas/Admin/Views/A_Export/_Viewlog.cshtml", log.ToList());
        }
    }
}