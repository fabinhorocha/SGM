using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class EquipmentBackupViewModel
    {
        public int id { get; set; }

        public DateTime dateInput { get; set; }

        public string cdUser { get; set; }

        public string cls { get; set; }

        public List<EquipmentBackupDetails> EquipmentBackupDetails { get; set; }

    }
}
