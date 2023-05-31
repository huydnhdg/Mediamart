using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.Models;

namespace Mediamart.APIFORAPP.Model
{
    public class ProductRes : Result
    {
        public List<Product> Data { get; set; }
    }
}