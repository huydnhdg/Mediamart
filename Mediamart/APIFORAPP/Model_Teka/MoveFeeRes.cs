using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.APIFORAPP.Model;

namespace Mediamart.APIFORAPP.Model_Teka
{
    public class MoveFeeRes : Result
    {
        public List<MoveFee> Data { get; set; }
    }

    public class MoveFee
    {
        public long ID { get; set; }
        public string Cate { get; set; }
        public Nullable<int> Price { get; set; }
        public string Note { get; set; }
    }
}