using Mediamart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mediamart.Areas.Admin.Data.Accessary
{
    public class A_Import_Model : A_Import
    {
        public List<A_Import_Items> Items { get; set; }
    }
}