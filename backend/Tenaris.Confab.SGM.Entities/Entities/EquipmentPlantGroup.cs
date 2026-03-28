using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class EquipmentPlantGroup
    {

        public int ? idEquipmentPlantGroup { get; set; }

        public string Sheet { get; set; }

        public string Name { get; set; }

        public int cdPlantGroup { get; set; }

        public string cdUser { get; set; }

        public bool Active { get; set; }

        public DateTime InsDateTime { get; set; }

        public DateTime UpdDateTime { get; set; }


        public PlantGroup PlantGroup { get; set; }


    }
}
