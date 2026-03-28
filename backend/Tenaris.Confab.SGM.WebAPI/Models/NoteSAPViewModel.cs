using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tenaris.Confab.SGM.WebAPI.Models
{

    public class NoteSAPViewModel
    {
        int idNoteSAP { get; set; }
        int? cdSAP { get; set; }
        int cdReport { get; set; }
        bool cdStatus { get; set; }
        string statusMessage { get; set; }
        public DateTime InsDateTime { get; set; }
        public DateTime UpdDateTime { get; set; }
        public string cdUser { get; set; }

        public string cdOrder { get; set; }
    }
}