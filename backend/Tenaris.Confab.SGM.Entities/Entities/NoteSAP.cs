using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class NoteSAP
    {
        public int idNoteSAP { get; set; }
        public int? cdSAP { get; set; }
        public int cdReport { get; set; }
        public bool cdStatus { get; set; }
        public string statusMessage { get; set; }
        public DateTime InsDateTime { get; set; }
        public DateTime UpdDateTime { get; set; }
        public string cdUser { get; set; }
        public string cdOrder { get; set; }
        public bool ? cdAttachDoc { get; set; }
    }
}
