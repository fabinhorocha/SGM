using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class OilManagementAlarmSendStatus
    {
        public OilManagementAlarmHistory AlarmHistory { get; set; }
        public bool Sent { get; set; }
        public int AttemptsNumber { get; set; }
        public string LastMessage { get; set; }
        public DateTime InsDateTime { get; set; }
        public DateTime? UpdDateTime { get; set; }
    }
}
