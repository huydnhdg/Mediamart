using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mediamart.APIFORAPP.Model
{
    public class ReportWarrantiApi : Result
    {
        public List<RPWarranti> Data { get; set; }
    }
    public class RPWarranti
    {
        public string Title { get; set; }
        public int? Status { get; set; }
        public int? Quantity { get; set; }
    }
}