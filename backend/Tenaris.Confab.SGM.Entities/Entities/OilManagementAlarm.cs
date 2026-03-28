using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class OilManagementAlarm
    {
        public int idAlarm { get; set; }
        public Component Component { get; set; }
        public OilManagementAlarmType AlarmType { get; set; }
        public bool SendAlert { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime InsDateTime { get; set; }
        public string ModificatedBy { get; set; }
        public DateTime UpdDateTime { get; set; }
        public List<OilManagementAlarmGroup> Groups { get; set; }
        public List<OilManagementAlarmGroup> GroupsCC { get; set; }
        public OilManagementAlarmAlertConfig AlertConfig { get; set; }
    }
}
