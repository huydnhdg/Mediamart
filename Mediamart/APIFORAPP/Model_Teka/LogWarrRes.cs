using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.APIFORAPP.Model;
using Mediamart.Models;

namespace Mediamart.APIFORAPP.Model_Teka
{
    public class LogWarrRes : Result
    {
        public List<Warranti_Log> Data { get; set; }
    }
}