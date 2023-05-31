using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.Models;

namespace Mediamart.Areas.Admin.Data
{
    public class Agent_Export : Agent_Log_Active
    {
        public string Role { get; set; }
        public string FullName { get; set; }
    }
}