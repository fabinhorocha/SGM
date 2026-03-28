using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class PlantGroupViewModel
    {
        public int idPlantGroup { get; set; }

        public string Sheet { get; set; }

        public string Name { get; set; }

        public string cdUser { get; set; }

        public bool Active { get; set; }

        public DateTime InsDateTime { get; set; }

        public DateTime UpdDateTime { get; set; }

        public List<PlantViewModel> Plants { get; set; }

        public List<EquipmentPlantGroupViewModel> EquipmentsPlantGroup { get; set; }
    }
}