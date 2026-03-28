    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class OilManagementAlarmHistory
    {
        public int idAlarmHistory { get; set; }
        public OilManagementAlarm Alarm { get; set; }
        public OilSupply OilSupply { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime InsDateTime { get; set; }
        public string ModificatedBy { get; set; }
        public DateTime UpdDateTime { get; set; }
        public List<OilManagementAlarmHistorySetting> AlarmHistorySettings { get; set; }
    }
}
