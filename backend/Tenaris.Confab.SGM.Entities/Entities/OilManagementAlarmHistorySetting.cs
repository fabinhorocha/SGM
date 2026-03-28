using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class OilManagementAlarmHistorySetting
    {
        public int idAlarmHistorySetting { get; set; }
        public int idAlarmHistory { get; set; }
        public int idAlarmSetting { get; set; }
        public decimal Value { get; set; }
        public DateTime InsDateTime { get; set; }
        public DateTime UpdDateTime { get; set; }

    }
}
