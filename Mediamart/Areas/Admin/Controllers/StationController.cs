using Mediamart.Areas.Admin.Data;
using Mediamart.Models;
using Mediamart.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mediamart.Areas.Admin.Controllers
{
    [Permission(Module = "sysstation")]
    public class StationController : BaseController
    {
        public ActionResult Index(string textSearch)
        {
            var query = from a in db.Station_Level
                        select a;
            if (!string.IsNullOrEmpty(textSearch))
            {
                query = query.Where(a=>a.S1 == textSearch);
                ViewBag.textSearch = textSearch;
            }
            var model = new StationLevelListView()
            {
                Items = query.ToList()
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm([Bind(Include = "")] StationLevelListView model, List<Station_Level> Items)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in Items)
                {
                    if (item.Id == 0)
                    {
                        var station = new Station_Level()
                        {
                            S1 = item.S1,
                            S2 = item.S2,
                            S3 = item.S3
                        };
                        db.Station_Level.Add(station);
                    }
                    else
                    {
                        var edit = db.Station_Level.Find(item.Id);
                        edit.S1 = item.S1;
                        edit.S2 = item.S2;
                        edit.S3 = item.S3;
                        db.Entry(edit).State = EntityState.Modified;
                    }

                }
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index", "Station");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index", "Station");
        }
        public ActionResult Delete(long Id)
        {
            var station = db.Station_Level.Find(Id);
            db.Station_Level.Remove(station);
            db.SaveChanges();
            SetAlert("Đã xóa liên kết thành công.", "success");
            return RedirectToAction("Index", "Station");
        }

        public ActionResult UploadFile()
        {
            List<Station_Level> list_product = new List<Station_Level>();
            return View(list_product);
        }

        [HttpPost]
        public ActionResult UploadFile(FormCollection collection)
        {
            List<Station_Level> list_product = new List<Station_Level>();
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
                            string s1;
                            string s2;
                            string s3;
                            try { s1 = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { s1 = ""; }
                            try { s2 = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { s2 = ""; }
                            try { s3 = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception) { s3 = ""; }

                            //var user_S1 = db.AspNetUsers.FirstOrDefault(a => a.UserName == s1);
                            //user_S1.Station_Level = "S1";
                            //db.Entry(user_S1).State = EntityState.Modified;

                            //var user_S2 = db.AspNetUsers.FirstOrDefault(a => a.UserName == s2);
                            //user_S2.Station_Level = "S2";
                            //db.Entry(user_S2).State = EntityState.Modified;

                            //var user_S3 = db.AspNetUsers.FirstOrDefault(a => a.UserName == s3);
                            //user_S3.Station_Level = "S3";
                            //db.Entry(user_S3).State = EntityState.Modified;

                            var product = new Station_Level()
                            {
                                S1 = s1,
                                S2 = s2,
                                S3 = s3
                            };
                            db.Station_Level.Add(product);
                            db.SaveChanges();
                            list_product.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return View(list_product);
        }
    }
}