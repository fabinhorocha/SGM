using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class AlarmGroupViewModel
    {
        public int idAlarmGroup { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime InsDateTime { get; set; }
        public string ModificatedBy { get; set; }
        public DateTime? UpdDateTime { get; set; }

        public List<OilManagementUser> UsersAll { get; set; }
        public List<OilManagementUser> Users { get; set; }
    }
}
