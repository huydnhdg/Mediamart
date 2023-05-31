using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mediamart.APIFORAPP.ModelRequest
{
    public class Payment
    {
        public string Phone { get; set; }
        public long Process { get; set; }
        public int Point { get; set; }
    }
}