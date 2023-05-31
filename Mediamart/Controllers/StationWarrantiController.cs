using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Mediamart.Models;
using Mediamart.Areas.Admin.Data;

namespace Mediamart.Controllers
{
    [RoutePrefix("tram-bao-hanh")]
    public class StationWarrantiController : Controller
    {
        MediaMEntities db = new MediaMEntities();
        static IEnumerable<UserView> listexc = null;
        [Route]
        // GET: StationWarranti
        public ActionResult Index(int? page, string textsearch, string station, string phone, string province)
        {
            var model = from a in db.AspNetUsers
                        from b in db.AspNetRoles

                        select new UserView()
                        {
                            Id = a.Id,
                            UserName = a.UserName,
                            PhoneNumber = a.PhoneNumber,
                            Email = a.Email,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Address = a.Address + " " + a.Ward + " " + a.District + " " + a.Province,
                            Province = a.Province,
                            Role = b.Name,
                            FullName = a.FullName
                        };
            //hiển thị theo quyền
            if (User.IsInRole("Trạm - Trưởng trạm"))
            {
                model = model.Where(a => a.Createby == User.Identity.Name || a.UserName == User.Identity.Name);
            }
            if (!string.IsNullOrEmpty(phone))
            {
                model = model.Where(a => a.PhoneNumber == phone);
                ViewBag.phone = phone;
            }
            if (!string.IsNullOrEmpty(province))
            {
                model = model.Where(a => a.Province == province);
                ViewBag.province = province;
            }
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.UserName.Contains(textsearch)
                || a.PhoneNumber.Contains(textsearch)
                || a.Address.Contains(textsearch)
                || a.FullName.Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }
            if (!string.IsNullOrEmpty(station))
            {
                var aspNetUser = db.AspNetUsers.SingleOrDefault(a => a.FullName == station);
                if (station != null)
                {
                    ViewBag.station = station;
                }
            }
            
            listexc = model as IEnumerable<UserView>;
            
            return View(listexc.OrderByDescending(a => a.Createdate));
        }
    }
}