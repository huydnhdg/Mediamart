using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mediamart.Areas.Admin.Data
{
    public class HomeView
    {
        public int baohanh { get; set; }
        public int kichhoat { get; set; }
        public int sanpham { get; set; }
        public int khachhang { get; set; }


        public int daily { get; set; }
        public int trambh { get; set; }
        public int thobh { get; set; }
        public int ketoan { get; set; }
        public int thukho { get; set; }
        public int nvtiepnhan { get; set; }
        public int nvcskh { get; set; }
        public int tramuq { get; set; }
        public int thouq { get; set; }
        

        public int ac_SMS { get; set; }
        public int ac_APP { get; set; }
        public int ac_WEB { get; set; }

        public float ac_SMS_per { get; set; }
        public float ac_APP_per { get; set; }
        public float ac_WEB_per { get; set; }

        public int wa_SMS { get; set; }
        public int wa_APP { get; set; }
        public int wa_WEB { get; set; }
        public int wa_CMS { get; set; }

        public float wa_SMS_per { get; set; }
        public float wa_APP_per { get; set; }
        public float wa_WEB_per { get; set; }
        public float wa_CMS_per { get; set; }

        public List<KeyReport> keyreport { get; set; }
    }
    public class KeyReport
    {
        public string UserName { get; set; }
        public string Zone { get; set; }
        public int Warranti_Create { get; set; }
        public int Warranti_Receive { get; set; }

        public int Process { get; set; }
        public int Station { get; set; }
        public int Success { get; set; }
        public int Cancel { get; set; }
        public int? Service { get; set; }
        public int? Move { get; set; }
        public int? Amount { get; set; }

        public DateTime? Createdate { get; set; }
    }
}