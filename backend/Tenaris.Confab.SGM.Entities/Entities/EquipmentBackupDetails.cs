using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class EquipmentBackupDetails
    {
        public int id { get; set; }

        public int cdEquipmentBackup { get; set; }

        public int idEquipment { get; set; }

        public string Sheet { get; set; }

        public string Name { get; set; }

        public int ? idType { get; set; }

        public int ? idArea { get; set; }

        public int ? idCenterCost { get; set; }

        public int ? idPart { get; set; }

        public int ? cdEquipment { get; set; }

        public bool Active { get; set; }

        public DateTime InsDateTime { get; set; }

        public DateTime UpdDateTime { get; set; }

        public bool cdIntegrate { get; set; }



    }
}
