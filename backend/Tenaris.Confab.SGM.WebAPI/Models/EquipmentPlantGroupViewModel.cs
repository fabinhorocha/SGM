using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class EquipmentPlantGroupViewModel
    {
        public int? idEquipmentPlantGroup { get; set; }

        public string Sheet { get; set; }

        public string Name { get; set; }

        public int cdPlantGroup { get; set; }

        public string cdUser { get; set; }

        public bool Active { get; set; }

        public DateTime InsDateTime { get; set; }

        public DateTime UpdDateTime { get; set; }


        public PlantGroupViewModel PlantGroup { get; set; }
    }
}