using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.Models;

namespace Mediamart.Areas.Admin.Data
{
    public class ProductView : Product
    {
        public string CusName { get; set; }
        public string Address { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Email { get; set; }

        public string AgentPhone { get; set; }

        public string AgentC1Name { get; set; }
    }
}