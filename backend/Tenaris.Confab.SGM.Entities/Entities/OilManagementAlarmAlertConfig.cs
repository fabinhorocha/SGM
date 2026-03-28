using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class OilManagementAlarmAlertConfig
    {
        public int idAlarmAlertConfig { get; set; }
        public string SubjectMask { get; set; }
        public string ContentMask { get; set; }
        public string CreatedBy { get; set; }
        public DateTime InsDateTime { get; set; }
        public string ModificatedBy { get; set; }
        public DateTime? UpdDateTime { get; set; }

    }
}
