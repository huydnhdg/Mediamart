using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.APIFORAPP.Model;

namespace Mediamart.APIFORAPP.Model_Teka
{
    public class ActiveRes : Result
    {
        public List<ActiveModel> Data { get; set; }
    }
    public class ActiveModel
    {
        public string CusName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Serial { get; set; }
        public int? Limited { get; set; }
        public string Activedate { get; set; }
        public string Enddate { get; set; }
        public string Buydate { get; set; }
    }
}