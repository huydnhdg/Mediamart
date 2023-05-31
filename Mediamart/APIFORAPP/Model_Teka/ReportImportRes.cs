using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.APIFORAPP.Model;

namespace Mediamart.APIFORAPP.Model_Teka
{
    public class ReportImportRes : Result
    {
        public List<ReportImport> Data { get; set; }
    }
}