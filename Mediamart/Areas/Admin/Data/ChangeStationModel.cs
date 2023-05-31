using Mediamart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mediamart.Areas.Admin.Data
{
    public class ChangeStationModel
    {
        public Warranti Bill { get; set; }

        public string Send_Station { get; set; }
        public string Send_Name { get; set; }
        public string Send_Address { get; set; }

        public string Recevice_Station { get; set; }
        public string Recevice_Name { get; set; }
        public string Recevice_Address { get; set; }

        public string Status { get; set; }
    }
}