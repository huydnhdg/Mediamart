using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Mediamart.Utils
{
    public class Common
    {
        public static string domain = WebConfigurationManager.AppSettings["DOMAIN"];

        public static DateTime lastDayOfMonth()
        {
            DateTime date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return lastDayOfMonth;
        }
        public static string getStatusstring(int status)
        {
            switch (status)
            {
                case 0:
                    return "Mới tạo";
                case 1:
                    return "Hoàn thành";
                case 2:
                    return "Chuyển trạm";
                //case 3:
                //    return "Đang bàn giao";
                case 4:
                    return "Trạm từ chối";
                case 5:
                    return "Đang xử lý";
                case 6:
                    return "Đem về trạm";
                case 7:
                    return "Đợi linh kiện";
                case 8:
                    return "Chờ phản hồi";
                case 9:
                    return "Hủy bỏ";

                default:
                    //default
                    return null;
            }
        }

        public class Create_Accessary
        {
            public const int KTV_CREATE_ORDER = -1;
            public const int KTV_CREATE_USE = -2;
        }
        public class Status
        {
            public const int News = 0;
            public const int Success = 1;
            public const int Station = 2;
            public const int Feedback = 3;
            public const int Send = -1;
            public const int Recevice = -2;
            public const int Refuse = 4;
            public const int Processing = 5;
            public const int Back = 6;
            public const int Accessary = 7;
            public const int Fixed = 8;
            public const int Cancel = 9;
            public const int Outdate = -99;
        }
        public class StationID
        {
            public const string S000 = "S000";
            public const string S026 = "S026";
        }
        public class Roles
        {
            public const string ADMIN = "Admin";
            public const string CSKH = "CSKH";
            public const string AGENT = "Agent";
            public const string ACCOUNTANT = "Acountant";
            public const string CUSTOMER = "Customer";
            public const string STAFF = "Staff";
            public const string STOCKER = "Stocker";
            public const string KTV = "KTV";
            public const string KEY = "Key";
            public const string KTVUQ = "KTVUQ";
            public const string KEYUQ = "KeyUQ";
        }
    }
}