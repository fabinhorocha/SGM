using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public  class OilManagementAlarmSetting
    {
        public int idAlarmSetting { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Um { get; set; }
        public decimal? Value { get; set; }
        public bool Active{ get; set; }
	    public DateTime InsDateTime{ get; set; }
        public DateTime UpdDateTime{ get; set; }
    }
}
