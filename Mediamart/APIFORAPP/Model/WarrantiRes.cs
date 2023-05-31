using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.Areas.Admin.Data;

namespace Mediamart.APIFORAPP.Model
{
    public class WarrantiRes : Result
    {
        public List<WarrantiModel> Data { get; set; }
    }
}