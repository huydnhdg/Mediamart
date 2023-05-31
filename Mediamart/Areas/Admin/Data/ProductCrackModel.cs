using Mediamart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mediamart.Areas.Admin.Data
{
    public class ProductCrackModel : ProductCrack
    {
        public List<ProductCrack_Item> Items { get; set; }
    }
}