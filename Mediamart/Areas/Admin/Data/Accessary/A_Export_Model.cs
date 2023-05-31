using Mediamart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mediamart.Areas.Admin.Data.Accessary
{
    public class A_Export_Model : A_Export
    {
        public A_Export_Model()
        {
            Items = new List<A_Export_Items>();
        }
        public List<A_Export_Items> Items { get; set; }
    }
}