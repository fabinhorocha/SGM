using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class OilManagementAlarmsByComponentViewModel
    {
        public Component Component { get; set; }
        public List<OilManagementAlarmViewModel> Alarms { get; set; }
        public List<OilManagementAlarmType> AlarmTypes { get; set; }
        public bool EditionEnabled { get; set; }
        public string CreatedBy { get; set; }
        public string ModificatedBy { get; set; }

    }
}