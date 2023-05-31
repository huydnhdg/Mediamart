using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.APIFORAPP.Model;

namespace Mediamart.APIFORAPP.Model_Teka
{
    public class ListKeyRes : Result
    {
        public List<UserModel> Data { get; set; }
    }
}