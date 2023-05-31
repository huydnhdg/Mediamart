using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mediamart.Utils
{
    public class TitleNotif
    {
        public static string sent_station = "Phiếu {0} cần bạn xử lý.";
        public static string hand_over = "{0} bàn giao phiếu {1}.";
        public static string hand_ss_over = "{0} xác nhận bàn giao phiếu {1}.";
        public static string ss_warranti = "Hoàn thành phiếu {0}.";

        public static string accessary_ktv_use = "Phiếu {0} đề xuất linh kiện.";
        public static string acc_ss_ktv_use = "Phiếu {0} được duyệt đề xuất.";


        public static string url_warranti = "/admin/warranti/detail/{0}";
        public static string url_accessary = "/admin/a_export";

    }
}