using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class EquipmentBackup
    {
        public int id { get; set; }

        public DateTime dateInput { get; set; }

        public string cdUser { get; set; }

        public List<EquipmentBackupDetails> EquipmentBackupDetails { get; set; }

    }
}
