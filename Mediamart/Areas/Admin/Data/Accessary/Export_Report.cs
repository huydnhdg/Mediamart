using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mediamart.Areas.Admin.Data.Accessary
{
    public class Export_Report
    {
        public string Name { get; set; }
        public int all { get; set; }
        public int news { get; set; }
        public int admin { get; set; }
        public int stocker { get; set; }
        public int accountant { get; set; }

        public int cancel { get; set; }
        public int success { get; set; }
    }

    public class Model_Export_Report
    {
        public List<Export_Report> Station { get; set; }
        public List<Export_Report> Model { get; set; }
        public List<Export_Report> Accessary { get; set; }
    }
}