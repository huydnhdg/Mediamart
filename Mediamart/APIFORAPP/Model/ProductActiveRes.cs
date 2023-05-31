using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.Areas.Admin.Data;
using Mediamart.Models;
using Mediamart.APIFORAPP.Model;

namespace Mediamart.APIFORAPP.Model
{
    public class ProductActiveRes : Result
    {
        public List<ProductResult> Data { get; set; }
    }
}