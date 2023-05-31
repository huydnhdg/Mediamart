using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.Models;

namespace Mediamart.Areas.Admin.Data
{
    public class UserView : AspNetUser
    {
        public string Role { get; set; }
    }
}