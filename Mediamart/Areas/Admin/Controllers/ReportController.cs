using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mediamart.Areas.Admin.Data;
using Mediamart.Models;
using Mediamart.Utils;
using static Mediamart.Utils.Common;

namespace Mediamart.Areas.Admin.Controllers
{
    [Permission(Module = "report")]
    public class ReportController : BaseController
    {
        public ActionResult Index(string station, string im_start_date, string im_end_date)
        {
            var model = new HomeView();

            var warranti = from a in db.Warrantis select a;
            var user = from a in db.AspNetUsers select a;
            var product = from a in db.Products select a;
            var customer = from a in db.Customers select a;


            model.sanpham = product.Count();
            model.baohanh = warranti.Count();
            model.kichhoat = product.Where(a => a.Status == 1).Count();
            model.khachhang = customer.Count();


            model.ac_APP = product.Where(a => a.Status == 1 && a.Active_chanel == "APP").Count();
            model.ac_SMS = product.Where(a => a.Status == 1 && a.Active_chanel == "SMS").Count();
            model.ac_WEB = product.Where(a => a.Status == 1 && a.Active_chanel == "WEB").Count();

            model.ac_APP_per = ((float)model.ac_APP / (float)model.kichhoat) * 100;
            model.ac_SMS_per = ((float)model.ac_SMS / (float)model.kichhoat) * 100;
            model.ac_WEB_per = ((float)model.ac_WEB / (float)model.kichhoat) * 100;

            model.wa_APP = warranti.Where(a => a.Chanel == "APP").Count();
            model.wa_SMS = warranti.Where(a => a.Chanel == "SMS").Count();
            model.wa_WEB = warranti.Where(a => a.Chanel == "WEB").Count();
            model.wa_CMS = warranti.Where(a => a.Chanel == "CMS").Count();


            model.wa_APP_per = ((float)model.wa_APP / (float)model.baohanh) * 100;
            model.wa_SMS_per = ((float)model.wa_SMS / (float)model.baohanh) * 100;
            model.wa_WEB_per = ((float)model.wa_WEB / (float)model.baohanh) * 100;
            model.wa_CMS_per = ((float)model.wa_CMS / (float)model.baohanh) * 100;

            var warranti_station = from a in db.Warrantis select a;

            if (!string.IsNullOrEmpty(im_start_date))
            {
                DateTime s = DateTime.ParseExact(im_start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                warranti_station = warranti_station.Where(a => a.Createdate >= s);
                ViewBag.im_start_date = im_start_date;
            }
            if (!string.IsNullOrEmpty(im_end_date))
            {
                DateTime s = DateTime.ParseExact(im_end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                warranti_station = warranti_station.Where(a => a.Createdate <= s);
                ViewBag.im_end_date = im_end_date;
            }


            var key = from a in db.AspNetUsers
                      from b in a.AspNetRoles
                      where b.Id == Roles.KEY
                      select new KeyReport()
                      {
                          UserName = a.UserName,
                          Zone = a.Zone,
                          Warranti_Create = warranti_station.Where(w => w.Createby == a.UserName).Count(),
                          Warranti_Receive = warranti_station.Where(w => w.Station_Warranti == a.UserName && w.Createby != a.UserName).Count(),//danh sach duoc phan cong tru di tu tao
                          Station = warranti_station.Where(w => w.Station_Warranti == a.UserName && w.Status == 2).Count(),
                          Process = warranti_station.Where(w => w.Station_Warranti == a.UserName && (w.Status == 5 || w.Status == 6 || w.Status == 7 || w.Status == 8)).Count(),
                          Success = warranti_station.Where(w => w.Station_Warranti == a.UserName && w.Status == 1).Count(),
                          Cancel = warranti_station.Where(w => w.Station_Warranti == a.UserName && w.Status == 9).Count(),
                          Amount = warranti_station.Where(w => w.Station_Warranti == a.UserName).Sum(w => w.Amount),
                          Move = warranti_station.Where(w => w.Station_Warranti == a.UserName).Sum(w => w.Price_Move),
                          Service = warranti_station.Where(w => w.Station_Warranti == a.UserName).Sum(w => w.Price_Accessary) + warranti_station.Where(w => w.Station_Warranti == a.UserName).Sum(w => w.Price) + warranti_station.Where(w => w.Station_Warranti == a.UserName).Sum(w => w.Price_Home),
                      };
            if (User.IsInModule("report.user"))
            {
                key = key.Where(a => a.UserName == User.Identity.Name);
            }

            if (!string.IsNullOrEmpty(station))
            {
                key = key.Where(a => a.Zone.Contains(station));
                ViewBag.station = station;
            }
            model.keyreport = key.ToList();

            return View(model);
        }
    }
}