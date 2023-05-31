using Mediamart.Areas.Admin.Data;
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
    [Permission(Module = "productcrack")]
    public class ProductCrackController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date)
        {
            var model = from a in db.ProductCracks
                        where a.Status != -1
                        select a;

            var countstatus = new countstatus()
            {
                all = model.Count(),
                s0 = model.Where(a => a.Status == 0).Count(),
                s1 = model.Where(a => a.Status == 1).Count(),
            };
            ViewBag.countstatus = countstatus;

            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Name.Contains(textsearch)
                || a.Model.Contains(textsearch)
                || a.CodeWarranti.Contains(textsearch)
                || a.SNNo.Contains(textsearch)
                || a.Station.Contains(textsearch)
                || a.Serial.Contains(textsearch)
                || a.Note.Contains(textsearch)
                || a.Code.Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }
            if (!string.IsNullOrEmpty(status))
            {
                int _status = int.Parse(status);
                model = model.Where(a => a.Status == _status);
                ViewBag.status = status;
            }
            if (!string.IsNullOrEmpty(chanel))
            {
                model = model.Where(a => a.Model == chanel);
                ViewBag.chanel = chanel;
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
            if (User.IsInModule("productcrack.user"))
            {
                model = model.Where(a => a.Createby == User.Identity.Name);
            }
                ViewBag.model = db.ProductCracks.OrderBy(a => a.Model).GroupBy(a => a.Model).ToList();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(model.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }
        public class countstatus
        {
            public int all { get; set; }
            public int s0 { get; set; }
            public int s1 { get; set; }
        }
        [HttpPost]
        public JsonResult GetAccessary(string text)
        {
            var cate = (from a in db.A_Access_Center
                        where a.Name.Contains(text) || a.Code.Contains(text)
                        select new { a.Name, a.Id, a.Code });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [Permission(Module = "productcrack.add")]
        public ActionResult Create()
        {
            var model = new ProductCrackModel()
            {
                Items = new List<ProductCrack_Item>()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm([Bind(Include = "")] ProductCrack type, List<ProductCrack_Item> Items)
        {
            if (ModelState.IsValid)
            {
                var _product = db.ProductCracks.FirstOrDefault(a => a.Serial == type.Serial);
                if (_product != null)
                {
                    SetAlert("Lưu thông tin không thành công. Sản phẩm đã tồn tại trong hệ thống.", "danger");
                    return RedirectToAction("Index");
                }
                type.Createdate = DateTime.Now;
                type.Createby = User.Identity.Name;
                db.ProductCracks.Add(type);
                db.SaveChanges();

                if (Items != null && Items.Count() > 0)
                {
                    foreach (var item in Items)
                    {
                        var _item = new ProductCrack_Item()
                        {
                            Code = item.Code,
                            Name = item.Name,
                            ProductId = type.ID,
                            Note = item.Note,
                            Quantity = 1
                        };
                        db.ProductCrack_Item.Add(_item);
                    }
                    db.SaveChanges();
                }

                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }
        [Permission(Module = "productcrack.edit")]
        public ActionResult Edit(long ID)
        {
            var model = from a in db.ProductCracks
                        select new ProductCrackModel()
                        {
                            ID = a.ID,
                            Code = a.Code,
                            CodeWarranti = a.CodeWarranti,
                            SNNo = a.SNNo,
                            Station = a.Station,
                            Name = a.Name,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Model = a.Model,
                            Serial = a.Serial,
                            Status = a.Status,
                            Note = a.Note,
                            Items = db.ProductCrack_Item.Where(z => z.ProductId == a.ID).ToList()
                        };

            return View(model.FirstOrDefault(a=>a.ID == ID));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] ProductCrack type, List<ProductCrack_Item> Items)
        {
            if (ModelState.IsValid)
            {
                var _product = db.ProductCracks.FirstOrDefault(a => a.Serial == type.Serial || a.Code == type.Code);                

                if (Items != null && Items.Count() > 0)
                {
                    foreach (var item in Items)
                    {
                        if (item.Id >0)
                        {
                            continue;
                        }
                        else
                        {
                            var _item = new ProductCrack_Item()
                            {
                                Code = item.Code,
                                Name = item.Name,
                                ProductId = type.ID,
                                Note = item.Note,
                                Quantity = 1
                            };
                            db.ProductCrack_Item.Add(_item);
                        }

                    }
                    db.SaveChanges();
                }

                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Edit", "ProductCrack", new { Id = type.ID });
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Edit", "ProductCrack", new { Id = type.ID });

        }
        [Permission(Module = "productcrack.delete")]
        public ActionResult DeleteItem(long Id)
        {
            var image = db.ProductCrack_Item.Find(Id);
            db.ProductCrack_Item.Remove(image);
            db.SaveChanges();
            return RedirectToAction("Edit", "ProductCrack", new { Id = image.ProductId });
        }
        [Permission(Module = "productcrack.upload")]
        public ActionResult UploadFile()
        {
            List<ProductCrack> list_product = new List<ProductCrack>();
            return View(list_product);
        }

        [HttpPost]
        public ActionResult UploadFile(FormCollection collection)
        {
            List<ProductCrack> list_product = new List<ProductCrack>();
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

                            string name;
                            string serial;
                            string code;
                            string model;
                            string status;
                            string accessary;
                            string note;

                            try { name = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { name = ""; }
                            try { serial = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { serial = ""; }
                            try { code = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception) { code = ""; }

                            try { model = workSheet.Cells[rowIterator, 4].Value.ToString(); } catch (Exception) { model = ""; }
                            try { status = workSheet.Cells[rowIterator, 5].Value.ToString(); } catch (Exception) { status = ""; }
                            try { accessary = workSheet.Cells[rowIterator, 6].Value.ToString(); } catch (Exception) { accessary = ""; }
                            try { note = workSheet.Cells[rowIterator, 7].Value.ToString(); } catch (Exception) { note = ""; }

                            int _status = 0;
                            if (!string.IsNullOrEmpty(status))
                            {
                                _status = int.Parse(status);
                            }
                            var product = new ProductCrack()
                            {
                                Name = name,
                                Serial = serial,
                                Code = code,
                                Model = model,
                                Status = _status,
                                Note = note,
                                Createdate = DateTime.Now,
                                Createby = User.Identity.Name
                            };

                            //check trung serial code
                            if (!string.IsNullOrEmpty(serial) && !string.IsNullOrEmpty(accessary))
                            {
                                var old_product = db.ProductCracks.FirstOrDefault(a => a.Code == code|| a.Serial == serial);
                                if (old_product == null)
                                {
                                    db.ProductCracks.Add(product);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    product.ID = old_product.ID;

                                }
                                var check_item = db.A_Access_Center.FirstOrDefault(a => a.Code == accessary);
                                if (check_item != null)
                                {
                                    var _item = new ProductCrack_Item()
                                    {
                                        Code = check_item.Code,
                                        Name = check_item.Name,
                                        ProductId = product.ID,
                                        Quantity = 1
                                    };
                                    db.ProductCrack_Item.Add(_item);
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

        [Permission(Module = "productcrack.changestatus")]
        [HttpPost]
        public ActionResult ChangeStatus(long Id)
        {
            var model = db.ProductCracks.Find(Id);
            return PartialView("~/Areas/Admin/Views/ProductCrack/_ChangeStatus.cshtml", model);
        }
        public JsonResult GetStation(string text)
        {
            var cate = (from a in db.AspNetUsers
                        from b in a.AspNetRoles
                        where b.Id == "Key"
                        where a.UserName.Contains(text) || a.FullName.Contains(text)
                        select new { a.UserName, a.FullName });
            return Json(cate, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetProduct(string text)
        {
            var cate = (from a in db.Products
                        where a.Name.Contains(text)
                        select new { a.Name });
            return Json(cate, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeStatusConfirm([Bind(Include = "")] ProductCrack model)
        {
            if (ModelState.IsValid)
            {
                var product = db.ProductCracks.Find(model.ID);
                product.Status = 1;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }
    }
}