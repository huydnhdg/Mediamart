﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.Models;

namespace Mediamart.Areas.Admin.Data
{
    public class AgentBonusModel:Agent_Bonus
    {
        public string Role { get; set; }
        public string FullName { get; set; }
    }
}