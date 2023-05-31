using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.Models;

namespace Mediamart.Areas.Admin.Data
{
    public class CustomerView : Customer
    {
        public int CountActive { get; set; }
        public int CountWarranti { get; set; }
        public string Agent { get; set; }
    }
}