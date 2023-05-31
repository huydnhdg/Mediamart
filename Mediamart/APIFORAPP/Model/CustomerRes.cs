using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.Areas.Admin.Data;
using Mediamart.APIFORAPP.Model;

namespace Mediamart.APIFORAPP.Model
{
    public class CustomerRes : Result
    {
        public List<CustomerResult> Data { get; set; }
    }
}