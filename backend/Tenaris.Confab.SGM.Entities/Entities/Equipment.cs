using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class Equipment
    {
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

        public string cdUser { get; set; }

        public Area Area { get; set; }

        public CenterCost CenterCost { get; set; }

        public Part Part { get; set; }

        public Equipment EquipmentParent { get; set; }

        public List<Equipment> EquipmentsChilds { get; set; }

        public int CountChilds { get; set; }

        public bool cdIntegrate { get; set; }

        public int CountReports { get; set; }



    }
}
