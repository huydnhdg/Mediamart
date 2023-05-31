using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.APIFORAPP.Model;

namespace Mediamart.APIFORAPP.Model_Teka
{
    public class DetailWarrantiRes : Result
    {
        public List<UpdateDetailReq> Data { get; set; }
    }
}