using Mediamart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mediamart.Areas.Admin.Data.Accessary
{
    public class A_Revoke_Model:A_Revoke
    {
        public List<A_Revoke_Items> Items { get; set; }
    }
}