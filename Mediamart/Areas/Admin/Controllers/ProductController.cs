using Newtonsoft.Json;
using NLog;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mediamart.Areas.Admin.Data;
using Mediamart.Models;
using Mediamart.Utils;

namespace Mediamart.Areas.Admin.Controllers
{
    [Permission(Module = "product")]
    public class ProductController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        static IEnumerable<ProductView> listexc = null;
        public ActionResult Index(int? page, string phone, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.Products
                        join b in db.Customers on a.Active_phone equals b.Phone into temp
                        from c in temp.DefaultIfEmpty()

                        select new ProductView()
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Serial = a.Serial,
                            Code = a.Code,
                            Model = a.Model,
                            Trademark = a.Trademark,
                            Exportdate = a.Exportdate,
                            ProductBase = a.ProductBase,

                            AgentC1 = a.AgentC1,
                            Limited = a.Limited,
                            Createdate = a.Createdate,

                            Importdate = a.Importdate,
                            AgentC2 = a.AgentC2,
                            Importchanel = a.Importchanel,

                            CusName = c.Name,
                            Active_phone = a.Active_phone,
                            Address = c.Address,
                            Ward = c.Ward,
                            District = c.District,
                            Province = c.Province,
                            Buydate = a.Buydate,

                            Active_date = a.Active_date,
                            End_date = a.End_date,
                            Active_chanel = a.Active_chanel,
                            Active_by = a.Active_by,
                            AgentPhone = db.AspNetUsers.FirstOrDefault(x => x.UserName == a.Active_by).PhoneNumber,
                        };
            //hiển thị theo quyền
            if (User.IsInModule("product.user"))
            {
                model = model.Where(a => a.Active_by == User.Identity.Name || a.AgentC1 == User.Identity.Name || a.AgentC2 == User.Identity.Name);
            }
            //lọc theo thông tin
            if (!string.IsNullOrEmpty(phone))
            {
                model = model.Where(a => a.Active_phone == phone);
                ViewBag.phone = phone;
            }
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Name.Contains(textsearch)
                || a.Serial.Contains(textsearch)
                || a.Code.Contains(textsearch)
                || a.Model.Contains(textsearch)
                || a.ProductBase.Contains(textsearch)

                || a.AgentC1.Contains(textsearch)
                || a.AgentC2.Contains(textsearch)

                || a.CusName.Contains(textsearch)
                || a.Active_phone.Contains(textsearch)
                || a.Address.Contains(textsearch)
                || a.Ward.Contains(textsearch)
                || a.District.Contains(textsearch)
                || a.Province.Contains(textsearch)

                || a.Active_by.Contains(textsearch)
                || a.AgentPhone.Contains(textsearch)
                );

                ViewBag.textsearch = textsearch;
            }
            if (!string.IsNullOrEmpty(chanel))
            {
                model = model.Where(a => a.Active_chanel.Contains(chanel));
                ViewBag.chanel = chanel;
            }
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "0")
                {
                    model = model.Where(a => a.Active_phone == null);
                }
                else
                {
                    model = model.Where(a => a.Active_phone != null);
                }
                ViewBag.status = status;
            }
            if (!string.IsNullOrEmpty(start_date))
            {
                DateTime s = DateTime.ParseExact(start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model = model.Where(a => a.Active_date >= s);
                ViewBag.start_date = start_date;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                DateTime s = DateTime.ParseExact(end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                model = model.Where(a => a.Active_date <= s);
                ViewBag.end_date = end_date;
            }
            if (!string.IsNullOrEmpty(im_start_date))
            {
                DateTime s = DateTime.ParseExact(im_start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model = model.Where(a => a.Active_date >= s);
                ViewBag.im_start_date = im_start_date;
            }
            if (!string.IsNullOrEmpty(im_end_date))
            {
                DateTime s = DateTime.ParseExact(im_end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                model = model.Where(a => a.Active_date <= s);
                ViewBag.im_end_date = im_end_date;
            }
            listexc = model as IEnumerable<ProductView>;

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(model.OrderByDescending(a => a.Active_date).ToPagedList(pageNumber, pageSize));
        }
        [Permission(Module = "product.upload")]
        public ActionResult UploadFile()
        {
            List<Product> list_product = new List<Product>();
            return View(list_product);
        }

        [HttpPost]
        public ActionResult UploadFile(FormCollection collection)
        {
            List<Product> list_product = new List<Product>();
            try
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            string serial;
                            string name;
                            string code;
                            string cate;
                            string model;
                            string trademark;
                            string limited;
                            string exportdate;
                            string agent1;
                            string productbase;

                            try { name = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { name = ""; }

                            try { code = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { code = ""; }
                            try { serial = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception) { serial = ""; }

                            try { cate = workSheet.Cells[rowIterator, 4].Value.ToString(); } catch (Exception) { cate = ""; }
                            try { model = workSheet.Cells[rowIterator, 5].Value.ToString(); } catch (Exception) { model = ""; }
                            try { trademark = workSheet.Cells[rowIterator, 6].Value.ToString(); } catch (Exception) { trademark = ""; }
                            try { limited = workSheet.Cells[rowIterator, 7].Value.ToString(); } catch (Exception) { limited = ""; }
                            try { exportdate = workSheet.Cells[rowIterator, 8].Value.ToString(); } catch (Exception) { exportdate = null; }
                            try { agent1 = workSheet.Cells[rowIterator, 9].Value.ToString(); } catch (Exception) { agent1 = ""; }
                            try { productbase = workSheet.Cells[rowIterator, 10].Value.ToString(); } catch (Exception) { productbase = ""; }


                            int limit = int.Parse(limited);
                            DateTime? date = null;
                            if (!string.IsNullOrEmpty(exportdate))
                            {
                                date = DateTime.ParseExact(exportdate, "dd/MM/yyyy", null);
                            }
                            //add thong tin rows vao product
                            var product = new Product()
                            {
                                Serial = serial.ToUpper(),
                                Name = name,
                                Code = code.ToUpper(),
                                ProductCate = cate,
                                Model = model.ToUpper(),
                                Trademark = trademark,
                                Limited = limit,
                                Exportdate = date,
                                AgentC1 = agent1,
                                Createdate = DateTime.Now,
                                Createby = User.Identity.Name,
                                ProductBase = productbase
                            };

                            if (!string.IsNullOrEmpty(agent1))
                            {
                                var check_agent = db.AspNetUsers.FirstOrDefault(a => a.UserName == agent1);
                                if (check_agent == null)
                                {
                                    product.AgentC1 = "###";//đánh dấu lỗi ở đấy
                                }
                            }

                            //check trung serial code
                            if (!string.IsNullOrEmpty(serial) && !string.IsNullOrEmpty(code) && product.AgentC1 != "###")
                            {

                                var old_product = db.Products.Where(a => a.Serial == serial || a.Code == code);
                                if (old_product.Count() == 0)
                                {
                                    db.Products.Add(product);
                                    db.SaveChanges();
                                }

                            }
                            list_product.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }
            return View(list_product);
        }
        [Permission(Module = "product.upload")]
        public ActionResult UploadExtra()
        {
            List<Product> list_product = new List<Product>();
            return View(list_product);
        }

        [HttpPost]
        public ActionResult UploadExtra(FormCollection collection)
        {
            List<Product> list_product = new List<Product>();
            HttpPostedFileBase file = Request.Files["UploadedFile"];
            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
            {
                string fileName = file.FileName;
                string fileContentType = file.ContentType;
                byte[] fileBytes = new byte[file.ContentLength];
                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        string serial;
                        string name;
                        string code;
                        string cate;
                        string model;
                        string trademark;
                        string limited;
                        string exportdate;
                        string agent1;
                        string productbase;

                        try { name = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { name = ""; }

                        try { code = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { code = ""; }
                        try { serial = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception) { serial = ""; }
                        try { cate = workSheet.Cells[rowIterator, 4].Value.ToString(); } catch (Exception) { cate = ""; }
                        try { model = workSheet.Cells[rowIterator, 5].Value.ToString(); } catch (Exception) { model = ""; }
                        try { trademark = workSheet.Cells[rowIterator, 6].Value.ToString(); } catch (Exception) { trademark = ""; }
                        try { limited = workSheet.Cells[rowIterator, 7].Value.ToString(); } catch (Exception) { limited = ""; }
                        try { exportdate = workSheet.Cells[rowIterator, 8].Value.ToString(); } catch (Exception) { exportdate = null; }
                        try { agent1 = workSheet.Cells[rowIterator, 9].Value.ToString(); } catch (Exception) { agent1 = ""; }
                        try { productbase = workSheet.Cells[rowIterator, 10].Value.ToString(); } catch (Exception) { productbase = ""; }

                        DateTime? date = null;
                        if (!string.IsNullOrEmpty(exportdate))
                        {
                            date = DateTime.ParseExact(exportdate, "dd/MM/yyyy", null);
                        }
                        int limit = int.Parse(limited);

                        var check_agent = db.AspNetUsers.FirstOrDefault(a => a.UserName == agent1);
                        var update_product = db.Products.SingleOrDefault(a => a.Code == code);
                        //chưa kích hoạt thì mới bổ sung thông tin được
                        if (update_product != null && update_product.Status == 0)
                        {
                            //update thong tin vao san pham
                            if (!string.IsNullOrEmpty(name))
                            {
                                update_product.Name = name;
                            }
                            if (!string.IsNullOrEmpty(cate))
                            {
                                update_product.ProductCate = cate;
                            }
                            if (!string.IsNullOrEmpty(model))
                            {
                                update_product.Model = model.ToUpper();
                            }
                            if (!string.IsNullOrEmpty(trademark))
                            {
                                update_product.Trademark = trademark;
                            }
                            if (!string.IsNullOrEmpty(limited))
                            {
                                update_product.Limited = limit;
                            }
                            if (!string.IsNullOrEmpty(exportdate))
                            {
                                update_product.Exportdate = date;
                            }
                            if (!string.IsNullOrEmpty(agent1) && check_agent != null)
                            {
                                update_product.AgentC1 = agent1;
                            }
                            if (!string.IsNullOrEmpty(productbase))
                            {
                                update_product.ProductBase = productbase;
                            }
                            db.Entry(update_product).State = EntityState.Modified;
                            db.SaveChanges();
                            list_product.Add(update_product);
                        }
                        else
                        {
                            //xuat ra loi
                            var product = new Product()
                            {
                                Serial = serial.ToUpper(),
                                Name = name,
                                Code = code.ToUpper(),
                                ProductCate = cate,
                                Model = model.ToUpper(),
                                Trademark = trademark,
                                Limited = limit,
                                Exportdate = date,
                                AgentC1 = agent1,
                                Createdate = DateTime.Now,
                                Createby = User.Identity.Name,
                                ProductBase = productbase
                            };
                            list_product.Add(product);
                        }

                    }
                }
            }
            return View(list_product);
        }

        [Permission(Module = "product.upload")]
        public ActionResult UploadActive()
        {
            List<ProductView> list_product = new List<ProductView>();
            return View(list_product);
        }

        [HttpPost]
        public ActionResult UploadActive(FormCollection collection)
        {
            List<ProductView> list_product = new List<ProductView>();
            try
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            string model;
                            string item_no;
                            string product_name;
                            string group_code;
                            string serial_no;
                            string code;
                            string customer_name;
                            string phone;
                            string mobile_phone;
                            string address;
                            string limited;
                            string start_date;
                            string end_date;
                            string sale_date;
                            string buy_adr;
                            string channel;
                            string active_by;

                            try { model = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { model = ""; }
                            try { item_no = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception) { item_no = ""; }
                            try { product_name = workSheet.Cells[rowIterator, 4].Value.ToString(); } catch (Exception) { product_name = ""; }
                            try { group_code = workSheet.Cells[rowIterator, 5].Value.ToString(); } catch (Exception) { group_code = ""; }
                            try { serial_no = workSheet.Cells[rowIterator, 6].Value.ToString(); } catch (Exception) { serial_no = ""; }
                            try { code = workSheet.Cells[rowIterator, 7].Value.ToString(); } catch (Exception) { code = ""; }
                            try { customer_name = workSheet.Cells[rowIterator, 8].Value.ToString(); } catch (Exception) { customer_name = ""; }
                            try { phone = workSheet.Cells[rowIterator, 9].Value.ToString(); } catch (Exception) { phone = ""; }
                            try { mobile_phone = workSheet.Cells[rowIterator, 10].Value.ToString(); } catch (Exception) { mobile_phone = ""; }
                            try { address = workSheet.Cells[rowIterator, 11].Value.ToString(); } catch (Exception) { address = ""; }
                            try { limited = workSheet.Cells[rowIterator, 12].Value.ToString(); } catch (Exception) { limited = ""; }
                            try { start_date = workSheet.Cells[rowIterator, 13].Value.ToString(); } catch (Exception) { start_date = ""; }
                            try { end_date = workSheet.Cells[rowIterator, 14].Value.ToString(); } catch (Exception) { end_date = ""; }
                            try { sale_date = workSheet.Cells[rowIterator, 15].Value.ToString(); } catch (Exception) { sale_date = ""; }
                            try { buy_adr = workSheet.Cells[rowIterator, 16].Value.ToString(); } catch (Exception) { buy_adr = ""; }
                            try { channel = workSheet.Cells[rowIterator, 17].Value.ToString(); } catch (Exception) { channel = ""; }
                            try { active_by = workSheet.Cells[rowIterator, 18].Value.ToString(); } catch (Exception) { active_by = ""; }

                            if (!string.IsNullOrEmpty(mobile_phone))
                            {
                                string check_phone = Utils.Control.getMobileOperator(mobile_phone);
                                if (check_phone == "UNKNOWN")
                                {
                                    mobile_phone = "sdt khong dung";
                                }
                            }
                            int _limited = 0;
                            if (!string.IsNullOrEmpty(limited))
                            {
                                try
                                {
                                    _limited = int.Parse(limited);
                                }
                                catch (Exception ex) { 
                                    
                                }
                            }
                            DateTime? _start_date = null;
                            if (!string.IsNullOrEmpty(start_date))
                            {
                                try
                                {
                                    _start_date = DateTime.ParseExact(start_date, "dd/MM/yyyy", null);
                                }
                                catch (Exception ex) {
                                    _start_date = Convert.ToDateTime(start_date);
                                }
                            }
                            DateTime? _end_date = null;
                            if (!string.IsNullOrEmpty(end_date))
                            {
                                try
                                {
                                    _end_date = DateTime.ParseExact(end_date, "dd/MM/yyyy", null);
                                }
                                catch (Exception ex) {
                                    _end_date = Convert.ToDateTime(end_date);
                                }
                            }
                            DateTime? _sale_date = null;
                            if (!string.IsNullOrEmpty(sale_date))
                            {
                                try
                                {
                                    _sale_date = DateTime.ParseExact(sale_date, "dd/MM/yyyy", null);
                                }
                                catch (Exception ex) {
                                    _sale_date = Convert.ToDateTime(sale_date);
                                }
                            }
                            ProductView view = new ProductView()
                            {
                                GroupName = product_name,
                                Name = model,
                                ProductBase = item_no,
                                ProductCate = group_code,
                                Model = model,
                                Serial = serial_no,
                                Code = code,
                                Active_phone = mobile_phone,
                                Active_code = phone,
                                Address = address,
                                Limited = _limited,
                                Active_date = _start_date,
                                End_date = _end_date,
                                Buydate = _sale_date,
                                BuyAdr = buy_adr,
                                Active_chanel = channel,
                                Active_by = active_by
                            };
                            var check_customer = db.Customers.FirstOrDefault(a => a.Phone == mobile_phone);
                            if (check_customer == null)
                            {
                                var customer = new Customer
                                {
                                    Phone = mobile_phone,
                                    Mobile = phone,
                                    Name = customer_name,
                                    Address = address,
                                };
                                db.Customers.Add(customer);
                            }

                            var product = new Product()
                            {
                                GroupName = product_name,
                                Name = model,
                                Serial = serial_no,
                                Code = code,
                                ProductCate = group_code,
                                Limited = _limited,
                                Active_date = _start_date,
                                End_date = _end_date,
                                Buydate = _sale_date,
                                BuyAdr = buy_adr,
                                Active_chanel = channel,
                                Active_by = active_by,
                                Active_phone = mobile_phone,
                                Active_code = phone,
                                Model = model,
                                ProductBase = item_no,
                                Createdate = DateTime.Now,
                                Createby = User.Identity.Name,
                                Status = 1
                            };
                            db.Products.Add(product);
                            db.SaveChanges();

                            view.Id = product.Id;
                            list_product.Add(view);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }
            return View(list_product);
        }

        [Permission(Module = "product.add")]
        [HttpPost]
        public ActionResult Create()
        {
            return PartialView("~/Areas/Admin/Views/Product/_Create.cshtml", null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm([Bind(Include = "")] Product product)
        {
            if (ModelState.IsValid)
            {
                var _prod = db.Products.SingleOrDefault(a => a.Code == product.Code || a.Serial == product.Serial);
                if (_prod != null)
                {
                    SetAlert("Mã cào hoặc Serial đã tồn tại trong hệ thống.", "danger");
                    return RedirectToAction("Index");
                }
                product.Code = product.Code.ToUpper();
                product.Serial = product.Serial.ToUpper();
                product.Model = product.Model.ToUpper();
                product.Createby = User.Identity.Name;
                product.Createdate = DateTime.Now;
                product.ProductBase = product.ProductBase;
                db.Products.Add(product);
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }
        [Permission(Module = "edit")]
        [HttpPost]
        public ActionResult Edit(long Id)
        {
            var model = db.Products.Find(Id);
            return PartialView("~/Areas/Admin/Views/Product/_Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] Product product)
        {
            if (ModelState.IsValid)
            {
                var model = db.Products.Find(product.Id);
                string json = JsonConvert.SerializeObject(model);
                logger.Info(string.Format("[Edit] @UserName={0} @Product={1}", User.Identity.Name, json));

                model.Code = product.Code;
                model.Name = product.Name;
                model.Serial = product.Serial;
                model.Trademark = product.Trademark;
                model.Limited = product.Limited;
                model.AgentC1 = product.AgentC1;
                model.Exportdate = product.Exportdate;
                model.ProductBase = product.ProductBase;

                db.Entry(model).State = EntityState.Modified;
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }
        [Permission(Module = "product.delete")]
        public ActionResult Delete(long Id)
        {
            try
            {
                var model = db.Products.Find(Id);
                if (model.Status != 0)
                {
                    SetAlert("Sản phẩm này đã được kích hoạt.", "danger");
                    return RedirectToAction("Index");
                }
                string json = JsonConvert.SerializeObject(model);
                logger.Info(string.Format("[Delete] @UserName={0} @Product={1}", User.Identity.Name, json));
                db.Products.Remove(model);
                db.SaveChanges();
                SetAlert("Đã xóa thành công.", "success");
            }
            catch (Exception ex)
            {
                SetAlert(ex.Message, "danger");
            }

            return RedirectToAction("Index");
        }
        [Permission(Module = "product.reactive")]
        public ActionResult RemoveActive(long Id)
        {
            try
            {
                var model = db.Products.Find(Id);
                if (model.Status == 0)
                {
                    SetAlert("Sản phẩm này chưa được kích hoạt.", "danger");
                    return RedirectToAction("Index");
                }
                string json = JsonConvert.SerializeObject(model);
                logger.Info(string.Format("[Remove Active] @UserName={0} @Product={1}", User.Identity.Name, json));

                model.Status = 0;
                model.Active_phone = null;
                model.Active_code = null;
                model.Active_date = null;
                model.Active_by = null;
                model.Active_chanel = null;
                model.Buydate = null;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Đã hoàn kích hoạt thành công.", "success");
            }
            catch (Exception ex)
            {
                SetAlert(ex.Message, "danger");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult GetCate(string text)
        {
            var cate = (from a in db.ProductCates
                        where a.Name.Contains(text)
                        select new { a.Name });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetAgent(string text)
        {
            var cate = (from a in db.AspNetUsers
                        where a.UserName.Contains(text)
                        select new { a.UserName });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }


        public void Mediamart_Sanpham()
        {
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "stt";
            Sheet.Cells["B1"].Value = "tên sản phẩm - mã";
            Sheet.Cells["C1"].Value = "serial";
            Sheet.Cells["D1"].Value = "mã cào";
            Sheet.Cells["E1"].Value = "model";
            Sheet.Cells["F1"].Value = "thương hiệu";
            Sheet.Cells["G1"].Value = "thời hạn";
            Sheet.Cells["H1"].Value = "ngày tạo";
            Sheet.Cells["I1"].Value = "đại lý";
            Sheet.Cells["K1"].Value = "ngày xuất";
            Sheet.Cells["L1"].Value = "sđt kích hoạt";
            Sheet.Cells["M1"].Value = "họ tên";
            Sheet.Cells["N1"].Value = "địa chỉ";
            Sheet.Cells["O1"].Value = "ngày kích hoạt";
            Sheet.Cells["P1"].Value = "ngày hết hạn";
            Sheet.Cells["Q1"].Value = "kênh";
            Sheet.Cells["R1"].Value = "kích hoạt bởi";

            int index = 1;
            int row = 2;
            foreach (var item in listexc)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = index++;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Name + " " + item.ProductBase;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Serial;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Code;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Model;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Trademark;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Limited;
                Sheet.Cells[string.Format("H{0}", row)].Value = (item.Createdate != null) ? item.Createdate.Value.ToString("dd/MM/yyyy") : "";
                Sheet.Cells[string.Format("I{0}", row)].Value = item.AgentC1;
                Sheet.Cells[string.Format("K{0}", row)].Value = (item.Exportdate != null) ? item.Exportdate.Value.ToString("dd/MM/yyyy") : "";
                Sheet.Cells[string.Format("L{0}", row)].Value = item.Active_phone;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.CusName;
                Sheet.Cells[string.Format("N{0}", row)].Value = item.Address;
                Sheet.Cells[string.Format("O{0}", row)].Value = (item.Active_date != null) ? item.Active_date.Value.ToString("dd/MM/yyyy") : "";
                Sheet.Cells[string.Format("P{0}", row)].Value = (item.End_date != null) ? item.End_date.Value.ToString("dd/MM/yyyy") : "";
                Sheet.Cells[string.Format("Q{0}", row)].Value = item.Active_chanel;
                Sheet.Cells[string.Format("R{0}", row)].Value = item.Active_by;
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