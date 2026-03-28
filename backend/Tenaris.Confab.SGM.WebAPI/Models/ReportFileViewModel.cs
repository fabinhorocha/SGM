using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class ReportFileViewModel
    {
        public int idFile { get; set; }
        public int cdReport { get; set; }
        public string Name { get; set; }
        public float Size { get; set; }
        public string Type { get; set; }
        public DateTime InsDateTime { get; set; }
        public DateTime UpdDateTime { get; set; }
        public string cdUser { get; set; }
    }
}