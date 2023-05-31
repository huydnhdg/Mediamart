﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mediamart.Areas.Admin.Data
{
    public class ProductResult
    {
        public string Name { get; set; }
        public string ProductCate { get; set; }
        public string Code { get; set; }
        public string Model { get; set; }
        public string Trademark { get; set; }
        public string Activedate { get; set; }
        public string Enddate { get; set; }
        public string Limited { get; set; }
        public string Serial { get; set; }
        public string AgentC1 { get; set; }

        public int count_warranti { get; set; }
        //public string Bonus { get; set; }
    }
}