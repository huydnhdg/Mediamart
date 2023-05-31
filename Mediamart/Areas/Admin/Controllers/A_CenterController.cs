using Mediamart.Areas.Admin.Data.Accessary;
using Mediamart.Models;
using Mediamart.Utils;
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

namespace Mediamart.Areas.Admin.Controllers
{
    [Permission(Module = "center")]
    public class A_CenterController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date)
        {
            var model = from a in db.A_Access_Center
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
            return View(model.OrderBy(a => a.Name).ToPagedList(pageNumber, pageSize));
        }
        [Permission(Module = "center.edit")]
        [HttpPost]
        public ActionResult Edit(long Id)
        {
            var model = db.A_Access_Center.Find(Id);
            return PartialView("~/Areas/Admin/Views/A_Center/_Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] A_Access_Center center)
        {
            if (ModelState.IsValid)
            {
                var model = db.A_Access_Center.Find(center.Id);
                model.Model = center.Model;
                model.Code = center.Code;
                model.Name = center.Name;
                model.Price = center.Price;

                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }
        [Permission(Module = "center.upload")]
        public ActionResult UploadFile()
        {
            var model = new A_Import_Model()
            {
                Items = new List<Models.A_Import_Items>()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult UploadFile(FormCollection collection)
        {
            A_Import_Model list_product = new A_Import_Model();
            List<A_Import_Items> list_item = new List<A_Import_Items>();
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
                        long ID = 0;
                        string code = "";
                        int quantity = 0;
                        int amount = 0;
                        int total = 0;
                        string discount;
                        string note = "";
                        var import = new A_Import();
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {


                            string item_code;
                            string item_name;
                            string item_model;
                            string item_quantity;
                            string item_price;
                            //lấy mã phiếu nhập trong file
                            if (rowIterator == 2)
                            {
                                try { code = workSheet.Cells[2, 1].Value.ToString(); } catch (Exception) { code = ""; }
                                try { note = workSheet.Cells[2, 2].Value.ToString(); } catch (Exception) { note = ""; }
                                if (string.IsNullOrEmpty(code))
                                {
                                    DateTime now_date = DateTime.Now;
                                    var count_bill = db.A_Import;

                                    int index = 0;
                                    if (count_bill != null)
                                    {
                                        index = count_bill.Count();
                                    }
                                    string date = DateTime.Now.ToString("yyMMdd");
                                    code = "PN" + date + index++;
                                }
                                import.Code = code;
                                import.Note = note;
                                import.Createdate = DateTime.Now;
                                import.Createby = User.Identity.Name;

                                db.A_Import.Add(import);
                                db.SaveChanges();
                                ID = import.Id;
                            }
                            //lấy linh kiện nhập vào
                            if (rowIterator > 3)
                            {
                                try { item_code = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { item_code = ""; }
                                try { item_name = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { item_name = ""; }
                                try { item_model = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception) { item_model = ""; }
                                try { item_quantity = workSheet.Cells[rowIterator, 4].Value.ToString(); } catch (Exception) { item_quantity = ""; }
                                try { item_price = workSheet.Cells[rowIterator, 5].Value.ToString(); } catch (Exception) { item_price = ""; }
                                int pri = 0;
                                int qty = 0;
                                try
                                {
                                    pri = int.Parse(item_price);
                                }
                                catch (Exception ex) { }
                                try
                                {
                                    qty = int.Parse(item_quantity);
                                }
                                catch (Exception ex) { }


                                int tot = pri * qty;
                                int amo = pri * qty;
                                var items = new A_Import_Items()
                                {
                                    ProductCode = item_code,
                                    ProductName = item_name,
                                    ProductModel = item_model,
                                    Price = pri,
                                    Quantity = qty,
                                    Total = tot,
                                    Amount = amo,
                                    ImportId = ID
                                };
                                db.A_Import_Items.Add(items);
                                db.SaveChanges();
                                list_item.Add(items);
                                quantity = quantity + qty;
                                amount = amount + amo;
                                total = total + tot;
                                //add linh kiện vào ds linh kiện của trung tâm
                                var center = db.A_Access_Center.FirstOrDefault(a => a.Code == items.ProductCode);
                                if (center != null)
                                {
                                    //đã tồn tại mã linh kiện
                                    center.CountImport = center.CountImport + qty;
                                    center.Createdate = DateTime.Now;
                                    center.Createby = User.Identity.Name;
                                    db.Entry(center).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    //nhập linh kiện mới
                                    var _center = new A_Access_Center()
                                    {
                                        Code = item_code,
                                        Name = item_name,
                                        Model = item_model,
                                        Createdate = DateTime.Now,
                                        Createby = User.Identity.Name,
                                        Price = pri,
                                        ListedPrice = pri,
                                        CountImport = qty
                                    };
                                    db.A_Access_Center.Add(_center);
                                    db.SaveChanges();
                                }
                                import.Quantity = quantity;
                                import.Total = total;
                                import.Amount = amount;
                                db.Entry(import).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        list_product.Code = code;
                        list_product.Quantity = quantity;
                        list_product.Amount = total;
                        list_product.Total = amount;
                        list_product.Note = note;
                        list_product.Items = list_item;
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
        [Permission(Module = "center.upload")]
        public ActionResult ExportUploadFile()
        {
            A_Export_Model list_product = new A_Export_Model();
            return View(list_product);
        }
        [HttpPost]
        public ActionResult ExportUploadFile(FormCollection collection)
        {
            A_Export_Model list_product = new A_Export_Model();
            list_product.Items = new List<A_Export_Items>();
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

                        string center = "";
                        string hub = "";
                        string people = "";
                        string import_house = "";
                        string address = "";
                        string export_house = "";
                        string note = "";
                        try { center = workSheet.Cells[1, 2].Value.ToString(); } catch (Exception) { center = ""; }
                        try { hub = workSheet.Cells[2, 2].Value.ToString(); } catch (Exception) { hub = ""; }
                        try { people = workSheet.Cells[3, 2].Value.ToString(); } catch (Exception) { people = ""; }
                        try { import_house = workSheet.Cells[4, 2].Value.ToString(); } catch (Exception) { import_house = ""; }
                        try { address = workSheet.Cells[5, 2].Value.ToString(); } catch (Exception) { address = ""; }
                        try { export_house = workSheet.Cells[6, 2].Value.ToString(); } catch (Exception) { export_house = ""; }
                        try { note = workSheet.Cells[7, 2].Value.ToString(); } catch (Exception) { note = ""; }

                        var export = new A_Export();
                        export.CusName = people;
                        export.Hub = hub;
                        export.Center = center;
                        export.Import_Warehouse = import_house;
                        export.Address = address;
                        export.Export_Warehouse = export_house;
                        export.Note = note;
                        export.Createdate = DateTime.Now;
                        export.Createby = User.Identity.Name;
                        //export.WarrantiId = null;
                        db.A_Export.Add(export);
                        db.SaveChanges();
                        int quantity = 0;
                        int total = 0;
                        int amount = 0;
                        for (int rowIterator = 10; rowIterator <= noOfRow; rowIterator++)
                        {
                            string model = "";
                            string code = "";
                            string qty = "";
                            string _note = "";
                            try { model = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { model = ""; }
                            try { code = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { code = ""; }
                            try { qty = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception) { qty = ""; }
                            try { _note = workSheet.Cells[rowIterator, 4].Value.ToString(); } catch (Exception) { _note = ""; }

                            var item = db.A_Access_Center.FirstOrDefault(a => a.Code == code);
                            int _qty = 1;
                            if (!string.IsNullOrEmpty(qty))
                            {
                                _qty = int.Parse(qty);
                            }
                            int tot = item.Price * _qty;
                            var _item = new A_Export_Items()
                            {
                                ProductCode = item.Code,
                                ProductName = item.Name,
                                ProductModel = item.Model,
                                Quantity = _qty,
                                Price = item.Price,
                                Amount = tot,
                                Total = tot,
                                //WarrantiCode = item.WarrantiCode,
                                Note = _note,
                                ExportId = export.Id
                            };
                            db.A_Export_Items.Add(_item);
                            quantity = quantity + _qty;
                            total = total + tot;
                            amount = amount + tot;

                            list_product.Items.Add(_item);
                        }
                        export.Quantity = quantity;
                        export.Total = total;
                        export.Amount = amount;
                        db.Entry(export).State = EntityState.Modified;
                        db.SaveChanges();

                        list_product.Center = center;
                        list_product.Hub = hub;
                        list_product.Import_Warehouse = import_house;
                        list_product.CusName = people;
                        list_product.Export_Warehouse = export_house;
                        list_product.Address = address;
                        list_product.Note = note;
                        list_product.Quantity = quantity;
                        list_product.Total = total;
                        list_product.Amount = amount;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);

                list_product.Note = ex.Message;
            }
            return View(list_product);
        }
    }
}