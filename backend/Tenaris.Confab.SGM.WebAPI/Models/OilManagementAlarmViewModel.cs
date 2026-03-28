using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class OilManagementAlarmViewModel
    {
        public OilManagementAlarm Alarm { get; set; }
        public bool Edited { get; set; }
        public List<OilManagementAlarmGroup> GroupsAll { get; set; }
    }
}