using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.APIFORAPP.Model;

namespace Mediamart.APIFORAPP.Model_Teka
{
    public class TK_Banner : Result
    {
        public List<Banner> Data { get; set; }
    }
    public class Banner
    {
        public long Id { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public Nullable<System.DateTime> Createdate { get; set; }
    }
}