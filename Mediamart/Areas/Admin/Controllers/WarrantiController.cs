using NLog;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mediamart.Areas.Admin.Data;
using Mediamart.Models;
using Mediamart.Utils;
using ImageResizer;
using static Mediamart.Utils.Common;

namespace Mediamart.Areas.Admin.Controllers
{
    [Permission(Module = "warranti")]
    public class WarrantiController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        //static IEnumerable<WarrantiDetail> listexc = null;
        string domain = ConfigControl.DOMAIN;
        public ActionResult Index(int? page, string phone, string textsearch, string cate, string station, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            //var data = db.Warrantis;
            //foreach(var item in data)
            //{
            //    var text = db.Warrantis.FirstOrDefault(a => a.Id == item.Id);
            //    var user = db.AspNetUsers.FirstOrDefault(a => a.UserName == text.Createby);

            //    text.Zone = user.Zone;
            //    text.Hub = user.Hub;
            //    db.Entry(text).State = EntityState.Modified;
            //}
            //db.SaveChanges();
            var user = db.AspNetUsers.FirstOrDefault(a => a.UserName == User.Identity.Name);
            var model = from a in db.Warrantis
                        select new WarrantiDetail()
                        {
                            Id = a.Id,
                            Status = a.Status,
                            Code = a.Code,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Chanel = a.Chanel,
                            Phone = a.Phone,
                            PhoneExtra = a.PhoneExtra,
                            Address = a.Address + " " + a.Ward + " " + a.District + " " + a.Province,
                            Ward = a.Ward,
                            District = a.District,
                            Province = a.Province,
                            Cate = a.Cate,
                            Warranti_Cate = a.Warranti_Cate,
                            Note = a.Note,
                            Extra = a.Extra,
                            Deadline = a.Deadline,
                            Station_Warranti = a.Station_Warranti,
                            Station = db.AspNetUsers.FirstOrDefault(x => x.UserName == a.Station_Warranti).FullName,
                            KTV_Warranti = a.KTV_Warranti,
                            KTV = db.AspNetUsers.FirstOrDefault(x => x.UserName == a.KTV_Warranti).FullName,
                            Successdate = a.Successdate,
                            Successnote = a.Successnote,
                            SuccessExtra = a.SuccessExtra,
                            Lat = a.Lat,
                            Long = a.Long,
                            KM = a.KM,
                            Price_Move = a.Price_Move,
                            Price_Accessary = a.Price_Accessary,
                            Price_Cate = a.Price_Cate,
                            Price = a.Price,
                            Cate_Home = a.Cate_Home,
                            Price_Home = a.Price_Home,
                            Amount = a.Amount,
                            Sign = a.Sign,
                            ProductCode = a.ProductCode,
                            Model = a.Model,
                            Serial = a.Serial,
                            Zone = a.Zone,
                            Hub = a.Hub,
                            Zone_Station = db.AspNetUsers.FirstOrDefault(r => r.UserName == a.Station_Warranti).Zone,
                            Zone_KTV = db.AspNetUsers.FirstOrDefault(r => r.UserName == a.KTV_Warranti).Zone,
                            ProductCate = a.ProductCate,

                            CusName = db.Customers.FirstOrDefault(x => x.Phone == a.Phone).Name,

                            ProductName = a.ProductName,
                            BuyAdr = a.BuyAdr,
                            Buydate = a.Buydate,

                            Warranti_Accessaries = db.Warranti_Accessary.Where(w => w.WarrantiId == a.Id).ToList(),
                            Image = db.Warranti_Image.Where(w => w.IdWarranti == a.Id).ToList(),

                            FirstChange = db.Warranti_Station.FirstOrDefault(w => w.WarrantiId == a.Id).Station,
                            ChangeStation = db.Warranti_Station.OrderByDescending(w => w.Createdate).FirstOrDefault(w => w.WarrantiId == a.Id).ChangeStation,
                            WarrantiStation = db.Warranti_Station.OrderByDescending(x => x.Createdate).Where(x => x.WarrantiId == a.Id)
                        };
            var count_model = model;
            if (User.IsInModule("warranti.user"))
            {
                count_model = count_model.Where(a => a.Zone == user.Zone || a.Zone_Station == user.Zone || a.Zone_KTV == user.Zone);
            }
            DateTime today = DateTime.Now.AddDays(3);
            var countstatus = new countstatus()
            {
                all = count_model.Count(),
                s0 = count_model.Where(a => a.Status == Common.Status.News).Count(),
                s1 = count_model.Where(a => a.Status == Common.Status.Success).Count(),
                s2 = count_model.Where(a => a.Status == Common.Status.Station).Count(),
                s3 = count_model.Where(a => a.Status == Common.Status.Feedback).Count(),
                s4 = count_model.Where(a => a.Status == Common.Status.Refuse).Count(),
                s5 = count_model.Where(a => a.Status == Common.Status.Processing).Count(),
                s6 = count_model.Where(a => a.Status == Common.Status.Back).Count(),
                s7 = count_model.Where(a => a.Status == Common.Status.Accessary).Count(),
                s8 = count_model.Where(a => a.Status == Common.Status.Fixed).Count(),
                s9 = count_model.Where(a => a.Status == Common.Status.Cancel).Count(),
                outdate = count_model.Where(a => a.Status == Common.Status.Processing && a.Deadline < today).Count(),
                chgstation1 = count_model.Where(a => a.Status == Common.Status.Send).Count(),
                chgstation2 = count_model.Where(a => a.Status == Common.Status.Recevice).Count(),
            };
            ViewBag.countstatus = countstatus;
            bool isSearch = false;
            if (!string.IsNullOrEmpty(phone))
            {
                model = model.Where(a => a.Phone.Contains(phone) || a.PhoneExtra.Contains(phone));
                ViewBag.phone = phone;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(cate))
            {
                model = model.Where(a => a.ProductCate.Contains(cate));
                ViewBag.cate = cate;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Code.Contains(textsearch)
                || a.Createby.Contains(textsearch)
                || a.Note.Contains(textsearch)
                || a.Phone.Contains(textsearch)
                || a.PhoneExtra.Contains(textsearch)
                || a.Address.Contains(textsearch)
                || a.ProductCode.Contains(textsearch)
                || a.Model.Contains(textsearch)
                || a.Serial.Contains(textsearch)
                );
                ViewBag.textsearch = textsearch;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(station))
            {
                model = model.Where(a => a.Station_Warranti.Contains(station)
                || a.KTV_Warranti.Contains(station));
                ViewBag.station = station;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(chanel))
            {
                model = model.Where(a => a.Chanel.Contains(chanel));
                ViewBag.chanel = chanel;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(start_date))
            {
                DateTime s = DateTime.ParseExact(start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model = model.Where(a => a.Createdate >= s);
                ViewBag.start_date = start_date;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                DateTime s = DateTime.ParseExact(end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                model = model.Where(a => a.Createdate <= s);
                ViewBag.end_date = end_date;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(im_start_date))
            {
                DateTime s = DateTime.ParseExact(im_start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model = model.Where(a => a.Successdate >= s);
                ViewBag.im_start_date = im_start_date;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(im_end_date))
            {
                DateTime s = DateTime.ParseExact(im_end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                model = model.Where(a => a.Successdate <= s);
                ViewBag.im_end_date = im_end_date;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "-99")
                {
                    count_model = count_model.Where(a => a.Status == Common.Status.Processing && a.Deadline < today);
                }
                else if (status == "-1")
                {
                    count_model = count_model.Where(a => a.Status == Common.Status.Send);
                }
                else if (status == "-2")
                {
                    count_model = count_model.Where(a => a.Status == Common.Status.Recevice);
                }
                else
                {
                    count_model = count_model.Where(a => a.Status.ToString().Contains(status));
                }
                ViewBag.status = status;
                //isSearch = true;
            }

            if (!isSearch)
            {
                //nếu không search thì hiển thị theo zone
                model = count_model;
            }
            ViewBag.cate = db.ProductCates.OrderBy(a => a.Name).ToList();
            //listexc = model as IEnumerable<WarrantiDetail>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(model.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult ViewBillSend(long Id)
        {
            var model = new ChangeStationModel();
            var _warr = db.Warrantis.Find(Id);
            model.Bill = _warr;
            model.Status = Common.getStatusstring(_warr.Status);
            if (_warr != null)
            {
                var send_item = db.Warranti_Station.OrderByDescending(a => a.Createdate).FirstOrDefault(a => a.WarrantiId == Id);

                if (send_item != null)
                {
                    var send = db.AspNetUsers.FirstOrDefault(a => a.UserName == send_item.Station);
                    model.Send_Station = send.UserName;
                    model.Send_Name = send.FullName;
                    model.Send_Address = send.Address + " " + send.Ward + " " + send.District + " " + send.Province;

                    var recevice = db.AspNetUsers.FirstOrDefault(a => a.UserName == send_item.ChangeStation);
                    model.Recevice_Station = recevice.UserName;
                    model.Recevice_Name = recevice.FullName;
                    model.Recevice_Address = recevice.Address + " " + recevice.Ward + " " + recevice.District + " " + recevice.Province;

                    model.Status = send_item.Note;
                }
            }

            return PartialView("~/Areas/Admin/Views/Warranti/_ViewBillSend.cshtml", model);
        }
        [HttpPost]
        public ActionResult ViewBillRecevice(long Id)
        {
            var model = new ChangeStationModel();
            var _warr = db.Warrantis.Find(Id);
            model.Bill = _warr;
            model.Status = Common.getStatusstring(_warr.Status);
            if (_warr != null)
            {
                var send_item = db.Warranti_Station.OrderByDescending(a => a.Createdate).FirstOrDefault(a => a.WarrantiId == Id);

                if (send_item != null)
                {
                    var send = db.AspNetUsers.FirstOrDefault(a => a.UserName == send_item.ChangeStation);
                    model.Send_Station = send.UserName;
                    model.Send_Name = send.FullName;
                    model.Send_Address = send.Address + " " + send.Ward + " " + send.District + " " + send.Province;

                    var recevice = db.AspNetUsers.FirstOrDefault(a => a.UserName == send_item.Station);
                    model.Recevice_Station = recevice.UserName;
                    model.Recevice_Name = recevice.FullName;
                    model.Recevice_Address = recevice.Address + " " + recevice.Ward + " " + recevice.District + " " + recevice.Province;

                    model.Status = send_item.Note;
                }
            }

            return PartialView("~/Areas/Admin/Views/Warranti/_ViewBillRecevice.cshtml", model);
        }
        [Permission(Module = "warranti.sents")]
        [HttpPost]
        public ActionResult SendBill(long Id)
        {
            var model = new ChangeStationModel();
            var _warr = db.Warrantis.Find(Id);
            model.Bill = _warr;
            model.Status = Common.getStatusstring(_warr.Status);
            if (_warr != null)
            {
                var send_item = db.Warranti_Station.OrderByDescending(a => a.Createdate).FirstOrDefault(a => a.WarrantiId == Id);

                var send = db.AspNetUsers.FirstOrDefault(a => a.UserName == _warr.Station_Warranti);
                if (send != null)
                {
                    //trạm chuyển
                    model.Send_Station = send.UserName;
                    model.Send_Name = send.FullName;
                    model.Send_Address = send.Address + " " + send.Ward + " " + send.District + " " + send.Province;
                }
                else
                {
                    //người chuyển 
                    send = db.AspNetUsers.FirstOrDefault(a => a.UserName == User.Identity.Name);
                    model.Send_Station = send.UserName;
                    model.Send_Name = send.FullName;
                    model.Send_Address = send.Address + " " + send.Ward + " " + send.District + " " + send.Province;
                }

                if (send_item != null)
                {
                    var recevice = db.AspNetUsers.FirstOrDefault(a => a.UserName == send_item.Station);
                    model.Recevice_Station = recevice.UserName;
                    model.Recevice_Name = recevice.FullName;
                    model.Recevice_Address = recevice.Address + " " + recevice.Ward + " " + recevice.District + " " + recevice.Province;
                }
            }

            return PartialView("~/Areas/Admin/Views/Warranti/_SendBill.cshtml", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendBillConfirm([Bind(Include = "")] ChangeStationModel model)
        {
            if (ModelState.IsValid)
            {
                var warranti = db.Warrantis.Find(model.Bill.Id);
                if (!string.IsNullOrEmpty(model.Recevice_Station) && !string.IsNullOrEmpty(model.Send_Station))
                {
                    var check_user = db.AspNetUsers.FirstOrDefault(a => a.UserName == model.Recevice_Station);
                    var check_user1 = db.AspNetUsers.FirstOrDefault(a => a.UserName == model.Send_Station);
                    if (check_user == null || check_user1 == null)
                    {
                        SetAlert("Trạm bàn giao không tồn tại trong hệ thống.", "danger");
                        return RedirectToAction("Detail", "Warranti", new { Id = model.Bill.Id });
                    }
                    var change_station = new Warranti_Station()
                    {
                        Station = model.Send_Station,
                        ChangeStation = model.Recevice_Station,
                        Createdate = DateTime.Now,
                        WarrantiId = warranti.Id,
                        Status = 0,
                        Note = Common.getStatusstring(warranti.Status)
                    };
                    db.Warranti_Station.Add(change_station);

                    warranti.KTV_Warranti = "";
                    warranti.Station_Warranti = change_station.ChangeStation;
                    //chuyển trạng thái chuyển trạm đi trạng thái bàn giao đi
                    warranti.Status = -1;
                    db.Entry(warranti).State = EntityState.Modified;

                    var log = new Warranti_Log()
                    {
                        Id_Warranti = warranti.Id,
                        Createdate = DateTime.Now,
                        Description = "Chuyển phiếu đến trạm" + model.Recevice_Station
                    };
                    db.Warranti_Log.Add(log);
                    //tao thong bao
                    Signal.SendSignal signal = new Signal.SendSignal();
                    Notification notif = new Notification()
                    {
                        SentTo = warranti.Station_Warranti,
                        Createdate = DateTime.Now,
                        Title = String.Format(TitleNotif.sent_station, warranti.Code),
                        DetailsURL = String.Format(TitleNotif.url_warranti, warranti.Id)
                    };
                    signal.Save(notif);
                    db.SaveChanges();
                    SetAlert("Đã lưu thông tin thành công.", "success");
                    return RedirectToAction("Detail", "Warranti", new { Id = model.Bill.Id });
                }
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Detail", "Warranti", new { Id = model.Bill.Id });

        }
        [Permission(Module = "warranti.add")]
        [HttpPost]
        public ActionResult Create(long Id = 0, long Crack_Id = 0)
        {
            string cusname = "";
            var warranti = new Warranti();
            if (Crack_Id > 0)
            {
                var product_crack = db.ProductCracks.FirstOrDefault(a => a.ID == Crack_Id);
                var customer = db.AspNetUsers.FirstOrDefault(a => a.UserName == product_crack.Station);
                if (customer != null)
                {
                    cusname = customer.FullName;
                    warranti.Phone = customer.PhoneNumber;
                    warranti.Address = customer.Address;
                    warranti.Ward = customer.Ward;
                    warranti.District = customer.District;
                    warranti.Province = customer.Province;
                }

                warranti.ProductCode = product_crack.Code;
                warranti.Serial = product_crack.Serial;
                warranti.Model = product_crack.Model;
                warranti.ProductName = product_crack.Name;
                warranti.Warranti_Cate = "Khắc phục mới";
                ViewBag.Crack_Id = product_crack.ID;
                ViewBag.CusName = cusname;
                return PartialView("~/Areas/Admin/Views/Warranti/_Create.cshtml", warranti);
            }
            if (Id > 0)
            {
                var product = db.Products.Find(Id);
                var customer = db.Customers.FirstOrDefault(a => a.Phone == product.Active_phone);
                if (customer != null)
                {
                    warranti.Phone = customer.Phone;

                    warranti.Address = customer.Address;
                    warranti.Ward = customer.Ward;
                    warranti.District = customer.District;
                    warranti.Province = customer.Province;
                }

                warranti.ProductCode = product.Code;
                warranti.Serial = product.Serial;
                warranti.Model = product.Model;
                warranti.ProductName = product.Name;
                warranti.ProductCate = product.ProductCate;
                warranti.Buydate = product.Buydate;
                warranti.BuyAdr = product.BuyAdr;

                return PartialView("~/Areas/Admin/Views/Warranti/_Create.cshtml", warranti);
            }
            return PartialView("~/Areas/Admin/Views/Warranti/_Create.cshtml", warranti);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm([Bind(Include = "")] Warranti warranti, string CusName, long Crack_Id = 0)
        {
            if (ModelState.IsValid)
            {
                if (Crack_Id > 0)
                {
                    var crack = db.ProductCracks.FirstOrDefault(a => a.ID == Crack_Id);
                    crack.Status = -1;
                    db.Entry(crack).State = EntityState.Modified;
                }
                var _customer = db.Customers.SingleOrDefault(a => a.Phone == warranti.Phone);
                if (_customer == null)
                {
                    var customer = new Customer()
                    {
                        Name = CusName,
                        Phone = warranti.Phone,
                        Address = warranti.Address,
                        Ward = warranti.Ward,
                        District = warranti.District,
                        Province = warranti.Province,
                        Createdate = DateTime.Now,
                    };
                    db.Customers.Add(customer);
                }
                else
                {
                    _customer.Name = CusName;
                    _customer.Address = warranti.Address;
                    _customer.Ward = warranti.Ward;
                    _customer.District = warranti.District;
                    _customer.Province = warranti.Province;
                    db.Entry(_customer).State = EntityState.Modified;
                }
                var user = db.AspNetUsers.FirstOrDefault(a => a.UserName == User.Identity.Name);
                warranti.Zone = user.Zone;
                warranti.Hub = user.Hub;
                warranti.Createby = User.Identity.Name;
                warranti.Createdate = DateTime.Now;
                warranti.Status = 0;
                warranti.Chanel = "CMS";
                //nếu được trạm tạo thì chuyển luôn cho trạm 
                if (User.IsInRole(Roles.KEY))
                {
                    warranti.Status = 2;
                    warranti.Station_Warranti = User.Identity.Name;
                }
                db.Warrantis.Add(warranti);
                db.SaveChanges();

                warranti.Code = Utils.Control.CreateCode(warranti.Id);
                db.Entry(warranti).State = EntityState.Modified;

                var log = new Warranti_Log()
                {
                    Id_Warranti = warranti.Id,
                    Createdate = DateTime.Now,
                    Description = User.Identity.Name + " Tạo mới phiếu bảo hành."
                };
                db.Warranti_Log.Add(log);
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult Views(long Id)
        {
            var model = db.Warrantis.Find(Id);
            var station = db.AspNetUsers.SingleOrDefault(a => a.UserName == model.Station_Warranti);
            var ktv = db.AspNetUsers.SingleOrDefault(a => a.UserName == model.KTV_Warranti);
            var customer = db.Customers.SingleOrDefault(a => a.Phone == model.Phone);
            var product = db.Products.SingleOrDefault(a => a.Code == model.ProductCode);
            var list = from a in db.Warranti_Accessary
                       where a.WarrantiId == model.Id
                       join b in db.Accessary_Key on a.AccessaryId equals b.Id
                       select new PhuTungPrint()
                       {
                           Name = a.ProductName,
                           Code = b.Code
                       };
            PrintModel print = new PrintModel();
            print.Code = model.Code;
            if (station != null)
            {
                print.Station = station.FullName;
                print.StationAddress = station.Address + " " + station.Ward + " " + station.District + " " + station.Province;
                print.StationPhone = station.PhoneNumber;
                print.StationName = station.FullName;
            }
            if (ktv != null)
            {
                print.KTV = ktv.UserName;
                print.KTVName = ktv.FullName;
            }

            if (customer != null)
            {
                print.CusName = customer.Name;
            }

            print.CusPhone = model.Phone;
            print.CusAddress = model.Address + " " + model.Ward + " " + model.District + " " + model.Province;
            if (product != null)
            {
                print.ProdName = product.Name;
            }
            print.ProdCode = model.ProductCode;
            print.Serial = model.Serial;
            print.Model = model.Model;
            if (model.Buydate != null)
            {
                print.Buydate = model.Buydate.Value.ToString("dd/MM/yyyy");
            }
            print.BuyAdr = model.BuyAdr;
            print.CusPhoneExtra = model.PhoneExtra;
            print.Note = model.Note;
            print.Cate = model.Cate;
            print.Warranti_Cate = model.Warranti_Cate;
            print.PhuTung_Recevice = "";
            print.Createdate = model.Createdate.Value.ToString("dd/MM/yyyy");
            print.Extra = model.Extra;
            print.Successnote = model.Successnote;
            print.Amount = model.Amount.ToString();
            print.PhuTung_Export = list.ToList();

            return PartialView("~/Areas/Admin/Views/Warranti/_Views.cshtml", print);
        }
        [Permission(Module = "warranti.edit")]
        [HttpPost]
        public ActionResult Edit(long Id)
        {
            var model = db.Warrantis.Find(Id);
            return PartialView("~/Areas/Admin/Views/Warranti/_Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] Warranti warranti, string CusName)
        {
            if (ModelState.IsValid)
            {
                //if (Utils.Control.getMobileOperator(warranti.Phone) == "UNKNOWN")
                //{
                //    SetAlert("Số điện thoại không đúng", "danger");
                //    return RedirectToAction("Index");
                //}
                var _warranti = db.Warrantis.Find(warranti.Id);
                var log = new Warranti_Log()
                {
                    Id_Warranti = warranti.Id,
                    Createdate = DateTime.Now,
                    Description = User.Identity.Name + " Chỉnh sửa thông tin phiếu."
                };
                db.Warranti_Log.Add(log);
                var customer = db.Customers.SingleOrDefault(a => a.Phone == warranti.Phone);
                if (customer != null)
                {
                    customer.Name = CusName;
                    db.Entry(customer).State = EntityState.Modified;
                }

                _warranti.Phone = warranti.Phone;
                _warranti.PhoneExtra = warranti.PhoneExtra;
                _warranti.Province = warranti.Province;
                _warranti.District = warranti.District;
                _warranti.Ward = warranti.Ward;
                _warranti.Address = warranti.Address;

                _warranti.ProductCode = warranti.ProductCode;
                _warranti.Model = warranti.Model;
                _warranti.Serial = warranti.Serial;
                _warranti.ProductName = warranti.ProductName;

                _warranti.Extra = warranti.Extra;
                _warranti.Note = warranti.Note;
                _warranti.Cate = warranti.Cate;
                _warranti.Warranti_Cate = warranti.Warranti_Cate;
                db.Entry(_warranti).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }
        public ActionResult Detail(long Id)
        {
            var model = from a in db.Warrantis
                        join cus in db.Customers on a.Phone equals cus.Phone into tempc
                        from c in tempc.DefaultIfEmpty()
                        join prod in db.Products on a.ProductId equals prod.Id into tempp
                        from p in tempp.DefaultIfEmpty()

                        select new WarrantiDetail()
                        {
                            Id = a.Id,
                            Status = a.Status,
                            Code = a.Code,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Chanel = a.Chanel,
                            Phone = a.Phone,
                            PhoneExtra = a.PhoneExtra,
                            Address = a.Address,
                            Ward = a.Ward,
                            District = a.District,
                            Province = a.Province,
                            Cate = a.Cate,
                            Warranti_Cate = a.Warranti_Cate,
                            Note = a.Note,
                            Extra = a.Extra,
                            Deadline = a.Deadline,
                            Station_Warranti = a.Station_Warranti,
                            Station = db.AspNetUsers.FirstOrDefault(s => s.UserName == a.Station_Warranti).FullName,
                            KTV_Warranti = a.KTV_Warranti,
                            KTV = db.AspNetUsers.FirstOrDefault(s => s.UserName == a.KTV_Warranti).FullName,
                            Successdate = a.Successdate,
                            Successnote = a.Successnote,
                            SuccessExtra = a.SuccessExtra,
                            Lat = a.Lat,
                            Long = a.Long,
                            KM = a.KM,
                            Price_Move = a.Price_Move,
                            Price_Accessary = a.Price_Accessary,
                            Price_Cate = a.Price_Cate,
                            Price = a.Price,
                            Cate_Home = a.Cate_Home,
                            Price_Home = a.Price_Home,
                            Amount = a.Amount,
                            Sign = a.Sign,
                            ProductCode = a.ProductCode,
                            Model = a.Model,
                            Serial = a.Serial,

                            ProductCate = a.ProductCate,
                            ProductId = p.Id,
                            GroupName = a.GroupName,

                            CusName = c.Name,
                            Limited = p.Limited,
                            Enddate = p.End_date,
                            Activedate = p.Active_date,

                            ProductName = a.ProductName,
                            Buydate = a.Buydate,
                            BuyAdr = a.BuyAdr,

                            Warranti_Accessaries = db.Warranti_Accessary.Where(a => a.WarrantiId == Id).ToList(),
                            Image = db.Warranti_Image.Where(w => w.IdWarranti == a.Id).ToList(),
                            Type_Errors = db.Warranti_TypeError.Where(w => w.WarrantiId == a.Id).ToList(),

                            FirstChange = db.Warranti_Station.FirstOrDefault(w => w.WarrantiId == a.Id).Station,
                            ChangeStation = db.Warranti_Station.OrderByDescending(a => a.Createdate).FirstOrDefault(w => w.WarrantiId == a.Id).ChangeStation,
                            WarrantiStation = db.Warranti_Station.OrderByDescending(x => x.Createdate).Where(x => x.WarrantiId == a.Id)
                        };
            WarrantiDetail warranti = model.FirstOrDefault(a => a.Id == Id);
            return View(warranti);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DetailConfirm([Bind(Include = "")] WarrantiDetail warranti, IEnumerable<HttpPostedFileBase> Image, List<Warranti_TypeError> Type_Errors, int StatusStation = -99)
        {
            try
            {
                var _waranti = db.Warrantis.Find(warranti.Id);
                //phiếu trống thì gán cho người xử lý đầu tiền luôn
                if (string.IsNullOrEmpty(_waranti.Createby))
                {
                    var user = db.AspNetUsers.FirstOrDefault(a => a.UserName == User.Identity.Name);
                    _waranti.Zone = user.Zone;
                    _waranti.Hub = user.Hub;
                    _waranti.Createby = User.Identity.Name;
                }
                //them moi nhom loi
                if (Type_Errors != null && Type_Errors.Count() > 0)
                {
                    foreach (var item in Type_Errors)
                    {
                        if (item.Id == 0)
                        {
                            var type_error = new Warranti_TypeError()
                            {
                                Title = item.Title,
                                WarrantiId = _waranti.Id
                            };
                            db.Warranti_TypeError.Add(type_error);
                        }
                    }
                }
                //luu anh vao phieu neu co
                var listimage = new List<Warranti_Image>();
                foreach (var item in Image)
                {
                    if (item != null)
                    {
                        string strrand = Guid.NewGuid().ToString();
                        var fileName = Path.GetFileName(item.FileName);
                        var path = Path.Combine(Server.MapPath("~/Data/ImageWarr/"), strrand + "_" + fileName);
                        item.SaveAs(path);
                        ResizeSettings resizeSetting = new ResizeSettings
                        {
                            MaxWidth = 800,
                            MaxHeight = 800,
                        };
                        ImageBuilder.Current.Build(path, path, resizeSetting);
                        string link = "/Data/ImageWarr/" + strrand + "_" + fileName;

                        var image = new Warranti_Image()
                        {
                            Image = link,
                            IdWarranti = _waranti.Id,
                        };
                        listimage.Add(image);
                    }

                }
                db.Warranti_Image.AddRange(listimage);

                //lưu danh sách linh kiện dùng
                var items_acc = db.Warranti_Accessary.Where(a => a.WarrantiId == _waranti.Id);
                int lkcu = items_acc.Count();
                int lkmoi = (warranti.Warranti_Accessaries != null) ? warranti.Warranti_Accessaries.Count() : 0;

                if (lkmoi != lkcu || lkmoi != 0)
                {
                    //duyet lkmoi bo qua linh kien cu
                    //đã có linh kiện cũ thì bắt đầu từ lkmoi, chưa có thì bắt đầu từ 0

                    int lkbatdau = (lkmoi > lkcu) ? (lkmoi - lkcu) : 0;
                    for (int i = lkcu; i < lkmoi; i++)
                    {
                        if (warranti.Warranti_Accessaries[i].ProductName == null)
                        {
                            continue;
                        }
                        var _lkthem = warranti.Warranti_Accessaries[i];
                        //trừ đi linh kiện
                        var a_key = db.A_Station.Find(_lkthem.AccessaryId);
                        if (a_key != null && (a_key.CountImport - a_key.CountExport) > 0)
                        {
                            a_key.CountExport = a_key.CountExport + 1;
                            db.Entry(a_key).State = EntityState.Modified;

                            var acc = new Warranti_Accessary()
                            {
                                WarrantiId = warranti.Id,
                                ProductName = _lkthem.ProductName,
                                Price = _lkthem.Price,
                                Quantity = 1,
                                Amount = warranti.Price,
                                AccessaryId = _lkthem.AccessaryId
                            };
                            db.Warranti_Accessary.Add(acc);
                            //ok hết thì mới cộng tổng tiền vào

                            //add log xuat linh kien
                            var log_akey = new Acessary_Log_Key()
                            {
                                Accessary = acc.ProductName,
                                Code = warranti.Code,
                                Count = 1,
                                Createdate = DateTime.Now,
                                Id_Akey = a_key.Id,
                                Type = 2//xuat
                            };
                            db.Acessary_Log_Key.Add(log_akey);
                            //tong tien
                            _waranti.Price_Accessary = warranti.Price_Accessary;
                        }
                        else
                        {
                            SetAlert("Linh kiện không còn ở trạm", "danger");
                            return RedirectToAction("Detail", "Warranti", new { Id = warranti.Id });
                        }

                    }

                }

                //lưu log chi tiet
                //
                if (_waranti.Status != warranti.Status)
                {
                    var log = new Warranti_Log()
                    {
                        Id_Warranti = warranti.Id,
                        Createdate = DateTime.Now,
                        Description = User.Identity.Name + " Cập nhật trạng thái " + Common.getStatusstring(warranti.Status)
                    };
                    db.Warranti_Log.Add(log);
                    _waranti.Status = warranti.Status;
                }
                else
                {
                    var log = new Warranti_Log()
                    {
                        Id_Warranti = warranti.Id,
                        Createdate = DateTime.Now,
                        Description = User.Identity.Name + " Cập nhật chi tiết phiếu."
                    };
                    db.Warranti_Log.Add(log);
                }

                //chuyển trạm
                if (warranti.Station_Warranti != _waranti.Station_Warranti)
                {
                    if (_waranti.Station_Warranti == null)
                    {
                        var log = new Warranti_Log()
                        {
                            Id_Warranti = warranti.Id,
                            Createdate = DateTime.Now,
                            Description = User.Identity.Name + " Chuyển phiếu cho trạm " + warranti.Station_Warranti
                        };
                        db.Warranti_Log.Add(log);

                        //tao thong bao
                        Signal.SendSignal signal = new Signal.SendSignal();
                        Notification notif = new Notification()
                        {
                            SentTo = warranti.Station_Warranti,
                            Createdate = DateTime.Now,
                            Title = String.Format(TitleNotif.sent_station, _waranti.Code),
                            DetailsURL = String.Format(TitleNotif.url_warranti, _waranti.Id)
                        };
                        signal.Save(notif);

                        //xóa thợ ở phiếu cũ đi
                        _waranti.KTV_Warranti = null;
                        _waranti.Station_Warranti = warranti.Station_Warranti;
                        _waranti.Status = 2;
                    }
                    else
                    {
                        var log = new Warranti_Log()
                        {
                            Id_Warranti = warranti.Id,
                            Createdate = DateTime.Now,
                            Description = User.Identity.Name + " Chuyển từ trạm " + _waranti.Station_Warranti + " sang " + warranti.Station_Warranti
                        };
                        db.Warranti_Log.Add(log);

                        //tao thong bao
                        Signal.SendSignal signal = new Signal.SendSignal();
                        Notification notif = new Notification()
                        {
                            SentTo = warranti.Station_Warranti,
                            Createdate = DateTime.Now,
                            Title = String.Format(TitleNotif.sent_station, _waranti.Code),
                            DetailsURL = String.Format(TitleNotif.url_warranti, _waranti.Id)
                        };
                        signal.Save(notif);

                        //xóa thợ ở phiếu cũ đi
                        _waranti.KTV_Warranti = null;
                        _waranti.Station_Warranti = warranti.Station_Warranti;
                        _waranti.Status = 2;
                    }
                }
                //chuyển thợ
                if (warranti.KTV_Warranti != _waranti.KTV_Warranti)
                {
                    if (_waranti.KTV_Warranti == null)
                    {
                        var log = new Warranti_Log()
                        {
                            Id_Warranti = warranti.Id,
                            Createdate = DateTime.Now,
                            Description = User.Identity.Name + " Chuyển phiếu đến " + warranti.KTV_Warranti
                        };
                        db.Warranti_Log.Add(log);

                        //tao thong bao
                        Signal.SendSignal signal = new Signal.SendSignal();
                        Notification notif = new Notification()
                        {
                            SentTo = warranti.KTV_Warranti,
                            Createdate = DateTime.Now,
                            Title = String.Format(TitleNotif.sent_station, _waranti.Code),
                            DetailsURL = String.Format(TitleNotif.url_warranti, _waranti.Id)
                        };
                        signal.Save(notif);

                        _waranti.KTV_Warranti = warranti.KTV_Warranti;
                        _waranti.Status = 5;
                    }
                    else
                    {
                        var log = new Warranti_Log()
                        {
                            Id_Warranti = warranti.Id,
                            Createdate = DateTime.Now,
                            Description = User.Identity.Name + " Chuyển phiếu của " + _waranti.KTV_Warranti + " sang " + warranti.KTV_Warranti
                        };
                        db.Warranti_Log.Add(log);

                        //tao thong bao
                        Signal.SendSignal signal = new Signal.SendSignal();
                        Notification notif = new Notification()
                        {
                            SentTo = warranti.KTV_Warranti,
                            Createdate = DateTime.Now,
                            Title = String.Format(TitleNotif.sent_station, _waranti.Code),
                            DetailsURL = String.Format(TitleNotif.url_warranti, _waranti.Id)
                        };
                        signal.Save(notif);

                        _waranti.KTV_Warranti = warranti.KTV_Warranti;
                        _waranti.Status = 5;
                    }
                }
                //tự động chuyển trạng thái phiếu bảo hành
                //_waranti.Status = SetStatus(_waranti.Status, warranti, warranti.Price_Accessary > 0);

                //log bàn giao nếu có
                var status_station = db.Warranti_Station.OrderByDescending(a => a.Createdate).FirstOrDefault(a => a.WarrantiId == _waranti.Id); ;
                if (status_station != null)
                {
                    if (StatusStation == 0)
                    {
                        _waranti.Status = 2;
                        // nhận bàn giao phiếu chuyển sang chuyển trạm
                        status_station.Status = 0;
                        status_station.Note = "dừng bàn giao đi";
                        var log = new Warranti_Log()
                        {
                            Id_Warranti = warranti.Id,
                            Createdate = DateTime.Now,
                            Description = User.Identity.Name + " nhận bàn giao phiếu."
                        };
                        db.Warranti_Log.Add(log);

                        //tao thong bao
                        Signal.SendSignal signal = new Signal.SendSignal();
                        Notification notif = new Notification()
                        {
                            SentTo = status_station.Station,
                            Createdate = DateTime.Now,
                            Title = String.Format(TitleNotif.hand_ss_over, User.Identity.Name, _waranti.Code),
                            DetailsURL = String.Format(TitleNotif.url_warranti, _waranti.Id)
                        };
                        signal.Save(notif);
                    }
                    else if (StatusStation == -1)
                    {
                        _waranti.Status = -2;
                        //bàn giao về
                        status_station.Status = -1;
                        status_station.Note = "bàn giao về";
                        var log = new Warranti_Log()
                        {
                            Id_Warranti = warranti.Id,
                            Createdate = DateTime.Now,
                            Description = User.Identity.Name + " bàn giao phiếu."
                        };
                        db.Warranti_Log.Add(log);

                        //tao thong bao
                        Signal.SendSignal signal = new Signal.SendSignal();
                        Notification notif = new Notification()
                        {
                            SentTo = status_station.Station,
                            Createdate = DateTime.Now,
                            Title = String.Format(TitleNotif.hand_over, User.Identity.Name, _waranti.Code),
                            DetailsURL = String.Format(TitleNotif.url_warranti, _waranti.Id)
                        };
                        signal.Save(notif);
                    }
                    else if (StatusStation == -2)
                    {
                        //kết thúc bàn giao 
                        status_station.Status = -2;
                        status_station.Note = "bàn giao thứ cấp";
                        var log = new Warranti_Log()
                        {
                            Id_Warranti = warranti.Id,
                            Createdate = DateTime.Now,
                            Description = User.Identity.Name + " xác nhận bàn giao phiếu."
                        };
                        db.Warranti_Log.Add(log);
                        //tao thong bao
                        Signal.SendSignal signal = new Signal.SendSignal();
                        Notification notif = new Notification()
                        {
                            SentTo = warranti.FirstChange,//tram đầu tiên nhận phiếu
                            Createdate = DateTime.Now,
                            Title = String.Format(TitleNotif.hand_ss_over, User.Identity.Name, _waranti.Code),
                            DetailsURL = String.Format(TitleNotif.url_warranti, _waranti.Id)
                        };
                        signal.Save(notif);
                    }
                    else if (StatusStation == -3)
                    {
                        _waranti.Status = 8;
                        //kết thúc bàn giao chuyển sang chờ phản hồi
                        status_station.Status = 1;
                        status_station.Note = "kết thúc bàn giao";
                        var log = new Warranti_Log()
                        {
                            Id_Warranti = warranti.Id,
                            Createdate = DateTime.Now,
                            Description = User.Identity.Name + " xác nhận bàn giao phiếu."
                        };
                        db.Warranti_Log.Add(log);

                        //tao thong bao
                        Signal.SendSignal signal = new Signal.SendSignal();
                        Notification notif = new Notification()
                        {
                            SentTo = _waranti.Station_Warranti,// tram dang xu ly
                            Createdate = DateTime.Now,
                            Title = String.Format(TitleNotif.hand_ss_over, User.Identity.Name, _waranti.Code),
                            DetailsURL = String.Format(TitleNotif.url_warranti, _waranti.Id)
                        };
                        signal.Save(notif);
                    }
                    db.Entry(status_station).State = EntityState.Modified;
                }
                //lưu thông tin khách hàng vào phiếu bảo hành
                var customer = db.Customers.FirstOrDefault(a => a.Phone == warranti.Phone);
                if (customer != null)
                {
                    customer.Name = warranti.CusName;
                    db.Entry(customer).State = EntityState.Modified;
                }

                _waranti.Phone = warranti.Phone;
                _waranti.PhoneExtra = warranti.PhoneExtra;
                _waranti.Province = warranti.Province;
                _waranti.District = warranti.District;
                _waranti.Ward = warranti.Ward;
                _waranti.Address = warranti.Address;

                //lưu thông tin sản phẩm vào phiếu
                _waranti.ProductCode = warranti.ProductCode;
                _waranti.Model = warranti.Model;
                _waranti.Serial = warranti.Serial;
                _waranti.Buydate = warranti.Buydate;
                _waranti.BuyAdr = warranti.BuyAdr;
                _waranti.ProductName = warranti.ProductName;
                _waranti.ProductCate = warranti.ProductCate;
                _waranti.GroupName = warranti.GroupName;

                //lưu thông tin chi tiết phiếu
                _waranti.Deadline = warranti.Deadline;
                _waranti.Successdate = warranti.Successdate;
                _waranti.Cate = warranti.Cate;
                _waranti.Warranti_Cate = warranti.Warranti_Cate;
                _waranti.Note = warranti.Note;
                _waranti.Extra = warranti.Extra;
                _waranti.Successnote = warranti.Successnote;
                _waranti.SuccessExtra = warranti.SuccessExtra;
                _waranti.KM = warranti.KM;
                _waranti.Price_Move = warranti.Price_Move;
                _waranti.Price_Cate = warranti.Price_Cate;
                _waranti.Price = warranti.Price;
                _waranti.Cate_Home = warranti.Cate_Home;
                _waranti.Price_Home = warranti.Price_Home;
                _waranti.Amount = warranti.Amount;

                db.Entry(_waranti).State = EntityState.Modified;

                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Detail", "Warranti", new { Id = warranti.Id });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Detail", "Warranti", new { Id = warranti.Id });

        }

        [HttpPost]
        public ActionResult ViewLog(long Id)
        {
            var log = db.Warranti_Log.Where(a => a.Id_Warranti == Id);
            ViewBag.Code = db.Warrantis.Find(Id).Code;
            return PartialView("~/Areas/Admin/Views/Warranti/_Viewlog.cshtml", log.ToList());
        }
        [Permission(Module = "warranti.back")]

        public ActionResult BackLK(long Id, string LK)
        {
            try
            {
                var warranti = db.Warrantis.Find(Id);
                var log_key = db.Acessary_Log_Key.FirstOrDefault(a => a.Code == warranti.Code && a.Accessary == LK);
                var item = db.Warranti_Accessary.FirstOrDefault(a => a.WarrantiId == Id && a.ProductName == LK);
                var tram = db.A_Station.FirstOrDefault(a => a.Id == log_key.Id_Akey);
                //update lại số lk ở trạm
                tram.CountExport = tram.CountExport - 1;
                db.Entry(tram).State = EntityState.Modified;
                //xoá linh kiện trong phiếu
                db.Warranti_Accessary.Remove(item);
                //xoá log xuất lk
                db.Acessary_Log_Key.Remove(log_key);
                //nếu tính phí thì phải trừ tiền đi
                if (warranti.Price_Accessary > 0)
                {
                    warranti.Price_Accessary = warranti.Price_Accessary - item.Price;
                    warranti.Amount = warranti.Amount - item.Price;
                    db.Entry(warranti).State = EntityState.Modified;
                }
                db.SaveChanges();
                SetAlert("Đã trả linh kiện thành công.", "success");
            }
            catch (Exception ex)
            {
                SetAlert("Trả linh kiện không thành công. Hãy kiểm tra lại.", "danger");
            }

            return RedirectToAction("Detail", "Warranti", new { Id = Id });
        }
        [Permission(Module = "warranti.deleteimage")]
        public ActionResult DeleteImage(long Id)
        {
            var image = db.Warranti_Image.Find(Id);
            db.Warranti_Image.Remove(image);
            db.SaveChanges();
            return RedirectToAction("Detail", "Warranti", new { Id = image.IdWarranti });
        }
        [Permission(Module = "warranti.delete")]
        public ActionResult Delete(long Id)
        {
            var bill = db.Warrantis.Find(Id);
            db.Warrantis.Remove(bill);
            db.SaveChanges();
            return RedirectToAction("Index", "Warranti");
        }
        [Permission(Module = "warranti.deletetype")]
        public ActionResult DeleteTypeError(long Id)
        {
            var image = db.Warranti_TypeError.Find(Id);
            db.Warranti_TypeError.Remove(image);
            db.SaveChanges();
            return RedirectToAction("Detail", "Warranti", new { Id = image.WarrantiId });
        }

        [HttpPost]
        public JsonResult GetAccessary(string text, string station)
        {
            var cate = (from a in db.A_Station
                        where a.StationId == station
                        where (a.CountImport - a.CountExport >= 0)
                        where a.Name.Contains(text) || a.Code.Contains(text)
                        select new { a.Name, a.Id, a.Code, a.Price, a.CountImport, a.CountExport });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetCate(string text)
        {
            var cate = (from a in db.Type_Err
                        where a.Name.Contains(text) || a.Description.Contains(text)
                        select new { a.Name, a.Description, a.Overcome, a.Reason });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProductCate(string text)
        {
            var cate = (from a in db.ProductCates
                        where a.Name.Contains(text) || a.Code.Contains(text)
                        select new { a.Name, a.Code });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetZone(string text)
        {
            var cate = (from a in db.AspNetUsers
                        where a.UserName == text
                        select new { a.UserName, a.FullName, a.Zone });
            var c = cate.ToList();
            return Json(cate.FirstOrDefault(), JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult GetStation(string text)
        {
            var cr_user = db.AspNetUsers.FirstOrDefault(a => a.UserName == User.Identity.Name);
            if (cr_user.AspNetRoles.FirstOrDefault(r => r.Id == Roles.ADMIN) != null || cr_user.Zone == "HO")
            {
                var cate = (from a in db.AspNetUsers
                            from b in a.AspNetRoles
                            where b.Id == Roles.KEY
                            where a.UserName.Contains(text) || a.FullName.Contains(text)
                            select new { a.UserName, a.FullName, a.Zone });
                return Json(cate, JsonRequestBehavior.AllowGet);
            }
            else if (cr_user.HO == cr_user.Zone)
            {
                //cr_user.HO : S3
                //cr_user.Zone = "HO" S3 = "HO"
                var cate = (from a in db.AspNetUsers
                            from b in a.AspNetRoles
                            where b.Id == Roles.KEY && a.HO == cr_user.HO
                            where a.UserName.Contains(text) || a.FullName.Contains(text)
                            select new { a.UserName, a.FullName, a.Zone });
                return Json(cate, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var cate = (from a in db.AspNetUsers
                            from b in a.AspNetRoles
                            where b.Id == Roles.KEY && a.Hub == cr_user.Hub
                            where a.UserName.Contains(text) || a.FullName.Contains(text)
                            select new { a.UserName, a.FullName, a.Zone });
                return Json(cate, JsonRequestBehavior.AllowGet);
            }


        }
        [HttpPost]
        public JsonResult GetStationfromZone(string text)
        {
            var cr_user = db.AspNetUsers.FirstOrDefault(a => a.UserName == User.Identity.Name);
            if (cr_user.AspNetRoles.FirstOrDefault(r => r.Id == Roles.ADMIN) != null || cr_user.Zone == "HO")
            {
                var cate = (from a in db.AspNetUsers
                            from b in a.AspNetRoles
                            where b.Id == Roles.KEY
                            where a.Zone.Contains(text)
                            select new { a.UserName, a.FullName, a.Zone });
                return Json(cate, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var cate = (from a in db.AspNetUsers
                            from b in a.AspNetRoles
                            where b.Id == Roles.KEY && cr_user.Hub == a.Hub
                            where a.Zone.Contains(text)
                            select new { a.UserName, a.FullName, a.Zone });
                return Json(cate, JsonRequestBehavior.AllowGet);
            }



        }
        
        [HttpPost]
        public JsonResult GetStationParent(string text, string station)
        {
            var cr_station = db.AspNetUsers.FirstOrDefault(a => a.UserName == station);

            var cate = from a in db.AspNetUsers
                       from b in a.AspNetRoles
                       where b.Id == Roles.KEY && a.Zone == cr_station.HO
                       where a.UserName.Contains(text.ToUpper()) || a.FullName.Contains(text)
                       select new { a.UserName, a.FullName, a.Zone };
            return Json(cate, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult GetStationParentwithzone(string text, string station)
        {
            var cr_station = db.AspNetUsers.FirstOrDefault(a => a.UserName == station);

            var cate = from a in db.AspNetUsers
                       from b in a.AspNetRoles
                       where b.Id == Roles.KEY && a.Zone == cr_station.HO
                       where a.Zone.Contains(text)
                       select new { a.UserName, a.FullName, a.Zone };
            return Json(cate, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult GetKTV(string text, string station)
        {
            var cr_station = db.AspNetUsers.FirstOrDefault(a => a.UserName == station);
            var cate = (from a in db.AspNetUsers
                        from b in a.AspNetRoles
                        where b.Id == Roles.KTV
                        where a.Zone == cr_station.Zone
                        where a.UserName.Contains(text) || a.FullName.Contains(text)
                        select new { a.UserName, a.FullName, a.Zone });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetKM(string text)
        {
            var cate = (from a in db.Move_Price
                        where a.Name.Contains(text)
                        select new { a.Name });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPrice(string text)
        {
            var cate = (from a in db.Repair_Price
                        where a.Name.Contains(text) && a.Status == 0
                        select new { a.Name });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPriceHome(string text)
        {
            var cate = (from a in db.Repair_Price
                        where a.Name.Contains(text) && a.Status == 1
                        select new { a.Name });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetPriceMove(string text)
        {
            var product = db.Move_Price.Where(a => a.Name == text).SingleOrDefault();
            return Json(product.Price, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetPriceAccess(string Id)
        {
            var product = db.A_Station.Find(Id);
            return Json(product.Price, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetPriceCate(string text)
        {
            var product = db.Repair_Price.Where(a => a.Name == text).SingleOrDefault();
            return Json(product.Price, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetProductFromModel(string text, string phone)
        {
            var product = db.Products.Where(a => a.Model == text && a.Active_phone == phone).FirstOrDefault();
            if (product != null)
            {
                var result = new ProductResult()
                {
                    Name = product.Name,
                    Limited = product.Limited.ToString(),
                    Activedate = product.Active_date.Value.ToString("yyyy/MM/dd"),
                    Enddate = product.End_date.Value.ToString("yyyy/MM/dd"),
                    Model = product.Model,
                    Serial = product.Serial,
                    Code = product.Code,
                    ProductCate = product.ProductCate,
                };
                var query = from a in db.Warrantis
                            where a.ProductCate == product.ProductCate && a.Phone == product.Active_phone
                            select a;
                result.count_warranti = query.Count() + 1;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetProduct(string text)
        {
            var product = db.Products.Where(a => a.Code == text || a.Serial == text).FirstOrDefault();
            if (product != null)
            {
                var result = new ProductResult()
                {
                    Name = product.Name,
                    Limited = product.Limited.ToString(),
                    Activedate = product.Active_date.Value.ToString("yyyy/MM/dd"),
                    Enddate = product.End_date.Value.ToString("yyyy/MM/dd"),
                    Model = product.Model,
                    Serial = product.Serial,
                    Code = product.Code
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult LoadCustomer(string text)
        {
            var customer = db.Customers.Where(a => a.Phone == text).SingleOrDefault();
            var result = new CustomerResult()
            {
                Name = customer.Name,
                Province = customer.Province,
                District = customer.District,
                Ward = customer.Ward,
                Address = customer.Address
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetCustomer(string phone)
        {
            var customer = db.Customers.Where(a => a.Phone == phone).FirstOrDefault();
            if (customer == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var product = db.Products.Where(a => a.Active_phone == phone).OrderByDescending(a => a.Active_date);
            var result = new CustomerResult()
            {
                Name = customer.Name,
                Province = customer.Province,
                District = customer.District,
                Ward = customer.Ward,
                Address = customer.Address,
                ListProduct = product.ToList()
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetCode(string text)
        {
            var cate = (from a in db.Products
                        where a.Code.Contains(text)
                        select new { a.Code, a.Serial, a.Model, a.Name });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetSerial(string text)
        {
            var cate = (from a in db.Products
                        where a.Serial.Contains(text)
                        select new { a.Code, a.Serial, a.Model, a.Name });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetProductModel(string text)
        {
            var cate = (from a in db.Products
                        where a.Model.Contains(text)
                        select a);
            return Json(cate.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetCountWarrantibyPhone(string product_cate, string phone)
        {
            if (!string.IsNullOrEmpty(product_cate) && !string.IsNullOrEmpty(phone))
            {
                var query = from a in db.Warrantis
                            where a.ProductCate == product_cate && a.Phone == phone
                            select a;
                return Json(query.Count() + 1, JsonRequestBehavior.AllowGet);
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetModel(string text)
        {
            var cate = (from a in db.Products
                        where a.Model.Contains(text)
                        group a by a.Model into g
                        select new { Model = g.FirstOrDefault().Model, Name = g.FirstOrDefault().Name, ProductCate = g.FirstOrDefault().ProductCate });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CheckOrder(long Id)
        {
            var warranti = db.Warrantis.FirstOrDefault(a => a.Id == Id);
            var check_export = db.A_Export.FirstOrDefault(a => a.WarrantiId == Id);
            if (check_export != null && !string.IsNullOrEmpty(warranti.Warranti_Cate))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public int SetStatus(int status, Warranti warranti, bool isAccessary)
        {
            if (warranti.Status == 1)
            {
                warranti.Status = 10;
            }
            int result = status;
            //chuyen tram
            if (warranti.Station_Warranti != null && warranti.Status < 2)
            {
                result = 2;
            }
            //dang xu ly
            if (warranti.KTV_Warranti != null && warranti.Status < 5)
            {
                result = 5;
            }
            //chua co linh kien ma da chon hoan thanh
            if ((!isAccessary) && (warranti.Status == 10 || warranti.Successdate != null))
            {
                result = 7;
            }
            //cho phan hoi
            if ((isAccessary) && warranti.Status < 8)
            {
                result = 8;
            }

            if (User.IsInRole("Trạm - Trưởng trạm") || User.IsInRole("Admin"))
            {
                if (warranti.Status > result)
                {
                    result = warranti.Status;
                    if (result == 10)
                    {
                        result = 1;//tra về trạng thái đúng
                    }
                }
            }
            return result;
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
            public int s6 { get; set; }
            public int s7 { get; set; }
            public int s8 { get; set; }
            public int s9 { get; set; }
            public int outdate { get; set; }
            public int chgstation1 { get; set; }
            public int chgstation2 { get; set; }
        }

        public void Mediamart_Phieubaohanh(int? page, string phone, string textsearch, string cate, string station, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {

            var user = db.AspNetUsers.FirstOrDefault(a => a.UserName == User.Identity.Name);
            var model = from a in db.Warrantis
                        select new WarrantiDetail()
                        {
                            Id = a.Id,
                            Status = a.Status,
                            Code = a.Code,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Chanel = a.Chanel,
                            Phone = a.Phone,
                            PhoneExtra = a.PhoneExtra,
                            Address = a.Address + " " + a.Ward + " " + a.District + " " + a.Province,
                            Ward = a.Ward,
                            District = a.District,
                            Province = a.Province,
                            Cate = a.Cate,
                            Warranti_Cate = a.Warranti_Cate,
                            Note = a.Note,
                            Extra = a.Extra,
                            Deadline = a.Deadline,
                            Station_Warranti = a.Station_Warranti,
                            Station = db.AspNetUsers.FirstOrDefault(x => x.UserName == a.Station_Warranti).FullName,
                            KTV_Warranti = a.KTV_Warranti,
                            KTV = db.AspNetUsers.FirstOrDefault(x => x.UserName == a.KTV_Warranti).FullName,
                            Successdate = a.Successdate,
                            Successnote = a.Successnote,
                            SuccessExtra = a.SuccessExtra,
                            Lat = a.Lat,
                            Long = a.Long,
                            KM = a.KM,
                            Price_Move = a.Price_Move,
                            Price_Accessary = a.Price_Accessary,
                            Price_Cate = a.Price_Cate,
                            Price = a.Price,
                            Cate_Home = a.Cate_Home,
                            Price_Home = a.Price_Home,
                            Amount = a.Amount,
                            Sign = a.Sign,
                            ProductCode = a.ProductCode,
                            Model = a.Model,
                            Serial = a.Serial,
                            Zone = a.Zone,
                            Hub = a.Hub,
                            Zone_Station = db.AspNetUsers.FirstOrDefault(r => r.UserName == a.Station_Warranti).Zone,
                            Zone_KTV = db.AspNetUsers.FirstOrDefault(r => r.UserName == a.KTV_Warranti).Zone,
                            ProductCate = a.ProductCate,

                            CusName = db.Customers.FirstOrDefault(x => x.Phone == a.Phone).Name,

                            ProductName = a.ProductName,
                            BuyAdr = a.BuyAdr,
                            Buydate = a.Buydate,

                            Warranti_Accessaries = db.Warranti_Accessary.Where(w => w.WarrantiId == a.Id).ToList(),
                            Image = db.Warranti_Image.Where(w => w.IdWarranti == a.Id).ToList(),

                            FirstChange = db.Warranti_Station.FirstOrDefault(w => w.WarrantiId == a.Id).Station,
                            ChangeStation = db.Warranti_Station.OrderByDescending(w => w.Createdate).FirstOrDefault(w => w.WarrantiId == a.Id).ChangeStation,
                            WarrantiStation = db.Warranti_Station.OrderByDescending(x => x.Createdate).Where(x => x.WarrantiId == a.Id)
                        };
            var count_model = model;
            if (User.IsInModule("warranti.user"))
            {
                count_model = count_model.Where(a => a.Zone == user.Zone || a.Zone_Station == user.Zone || a.Zone_KTV == user.Zone);
            }
            DateTime today = DateTime.Now.AddDays(3);
            var countstatus = new countstatus()
            {
                all = count_model.Count(),
                s0 = count_model.Where(a => a.Status == Common.Status.News).Count(),
                s1 = count_model.Where(a => a.Status == Common.Status.Success).Count(),
                s2 = count_model.Where(a => a.Status == Common.Status.Station).Count(),
                s3 = count_model.Where(a => a.Status == Common.Status.Feedback).Count(),
                s4 = count_model.Where(a => a.Status == Common.Status.Refuse).Count(),
                s5 = count_model.Where(a => a.Status == Common.Status.Processing).Count(),
                s6 = count_model.Where(a => a.Status == Common.Status.Back).Count(),
                s7 = count_model.Where(a => a.Status == Common.Status.Accessary).Count(),
                s8 = count_model.Where(a => a.Status == Common.Status.Fixed).Count(),
                s9 = count_model.Where(a => a.Status == Common.Status.Cancel).Count(),
                outdate = count_model.Where(a => a.Status == Common.Status.Processing && a.Deadline < today).Count(),
                chgstation1 = count_model.Where(a => a.Status == Common.Status.Send).Count(),
                chgstation2 = count_model.Where(a => a.Status == Common.Status.Recevice).Count(),
            };
            ViewBag.countstatus = countstatus;
            bool isSearch = false;
            if (!string.IsNullOrEmpty(phone))
            {
                model = model.Where(a => a.Phone.Contains(phone) || a.PhoneExtra.Contains(phone));
                ViewBag.phone = phone;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(cate))
            {
                model = model.Where(a => a.ProductCate.Contains(cate));
                ViewBag.cate = cate;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Code.Contains(textsearch)
                || a.Createby.Contains(textsearch)
                || a.Note.Contains(textsearch)
                || a.Phone.Contains(textsearch)
                || a.PhoneExtra.Contains(textsearch)
                || a.Address.Contains(textsearch)
                || a.ProductCode.Contains(textsearch)
                || a.Model.Contains(textsearch)
                || a.Serial.Contains(textsearch)
                );
                ViewBag.textsearch = textsearch;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(station))
            {
                model = model.Where(a => a.Station_Warranti.Contains(station)
                || a.KTV_Warranti.Contains(station));
                ViewBag.station = station;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(chanel))
            {
                model = model.Where(a => a.Chanel.Contains(chanel));
                ViewBag.chanel = chanel;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "-99")
                {
                    model = model.Where(a => a.Status == Common.Status.Processing && a.Deadline < today);
                }
                else if (status == "-1")
                {
                    model = model.Where(a => a.Status == Common.Status.Send);
                }
                else if (status == "-2")
                {
                    model = model.Where(a => a.Status == Common.Status.Recevice);
                }
                else
                {
                    model = model.Where(a => a.Status.ToString().Contains(status));
                }
                ViewBag.status = status;
                //isSearch = true;
            }
            if (!string.IsNullOrEmpty(start_date))
            {
                DateTime s = DateTime.ParseExact(start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model = model.Where(a => a.Createdate >= s);
                ViewBag.start_date = start_date;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                DateTime s = DateTime.ParseExact(end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                model = model.Where(a => a.Createdate <= s);
                ViewBag.end_date = end_date;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(im_start_date))
            {
                DateTime s = DateTime.ParseExact(im_start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model = model.Where(a => a.Successdate >= s);
                ViewBag.im_start_date = im_start_date;
                isSearch = true;
            }
            if (!string.IsNullOrEmpty(im_end_date))
            {
                DateTime s = DateTime.ParseExact(im_end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                model = model.Where(a => a.Successdate <= s);
                ViewBag.im_end_date = im_end_date;
                isSearch = true;
            }

            if (!isSearch)
            {
                //nếu không search thì hiển thị theo zone
                model = count_model;
            }
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

            Sheet.DefaultColWidth = 20;
            //Sheet.Cells.Style.WrapText = true;

            Sheet.Cells["A1"].Value = "STT";
            Sheet.Cells["B1"].Value = "Trạm BH";
            Sheet.Cells["C1"].Value = "NGÀY NHẬN";
            Sheet.Cells["D1"].Value = "NGÀY TRẢ";
            Sheet.Cells["E1"].Value = "TÊN SẢN PHẨM";
            Sheet.Cells["F1"].Value = "MODEL";
            Sheet.Cells["G1"].Value = "SERIAL";
            Sheet.Cells["H1"].Value = "NGÀY MUA";
            Sheet.Cells["I1"].Value = "HIỆN TƯỢNG HƯ HỎNG";
            Sheet.Cells["J1"].Value = "HÌNH THỨC SỬA CHỮA";
            Sheet.Cells["K1"].Value = "LINH KIỆN THAY THẾ";
            Sheet.Cells["L1"].Value = "MÃ LINH KIỆN";
            Sheet.Cells["M1"].Value = "SỐ LƯỢNG";
            Sheet.Cells["N1"].Value = "MÃ BIÊN NHẬN SỬA CHỮA";
            Sheet.Cells["O1"].Value = "TÊN KHÁCH HÀNG";
            Sheet.Cells["P1"].Value = "SỐ ĐIỆN THOẠI";
            Sheet.Cells["Q1"].Value = "ĐỊA CHỈ";
            Sheet.Cells["R1"].Value = "Thành tiền";
            Sheet.Cells["S1"].Value = "BẢO HÀNH";
            Sheet.Cells["T1"].Value = "HÌNH ẢNH";

            int index = 1;
            int row = 2;
            foreach (var item in model)
            {
                string acessary = "";
                string amount = "";
                string code = "";
                string count = "";
                string image = "";

                foreach (var jtem in item.Warranti_Accessaries)
                {
                    count += "1" + "\n";
                    acessary += jtem.ProductName + "\n";
                    amount += jtem.Amount.ToString() + "\n";
                    try
                    {
                        var ass = db.A_Station.FirstOrDefault(a => a.Id == jtem.AccessaryId);
                        code += ass.Code + "\n";
                    }
                    catch (Exception ex)
                    {
                        code += "-" + "\n";
                    }

                }
                foreach (var jtem in item.Image)
                {
                    image += domain + jtem.Image + "\n";
                }

                Sheet.Cells[string.Format("A{0}", row)].Value = index++;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Station_Warranti;
                Sheet.Cells[string.Format("C{0}", row)].Value = (item.Createdate != null) ? item.Createdate.Value.ToString("dd/MM/yyyy") : "";
                Sheet.Cells[string.Format("D{0}", row)].Value = (item.Successdate != null) ? item.Successdate.Value.ToString("dd/MM/yyyy") : "";
                Sheet.Cells[string.Format("E{0}", row)].Value = item.ProductName;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Model;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Serial;
                Sheet.Cells[string.Format("H{0}", row)].Value = (item.Buydate != null) ? item.Buydate.Value.ToString("dd/MM/yyyy") : "";
                Sheet.Cells[string.Format("I{0}", row)].Value = item.Note;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.Successnote;
                Sheet.Cells[string.Format("K{0}", row)].Value = acessary;
                Sheet.Cells[string.Format("L{0}", row)].Value = code;
                Sheet.Cells[string.Format("M{0}", row)].Value = count;
                Sheet.Cells[string.Format("N{0}", row)].Value = item.Code;
                Sheet.Cells[string.Format("O{0}", row)].Value = item.CusName;
                Sheet.Cells[string.Format("P{0}", row)].Value = item.Phone;
                Sheet.Cells[string.Format("Q{0}", row)].Value = item.Address;
                Sheet.Cells[string.Format("R{0}", row)].Value = item.Amount;
                Sheet.Cells[string.Format("S{0}", row)].Value = item.Warranti_Cate;
                Sheet.Cells[string.Format("T{0}", row)].Value = image;

                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}