using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class OilManagementAlarmGroup
    {
        public int idAlarmGroup { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string TypeAlarmByAlarmGroup { get; set; }
        public string CreatedBy { get; set; }
        public DateTime InsDateTime { get; set; }
        public string ModificatedBy { get; set; }
        public DateTime? UpdDateTime { get; set; }
        public bool selected { get; set; }
        public List<OilManagementUser> Users { get; set; }
    }
}
