using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class OilManagementEquipmentViewModel
    {
        public List<OilManagementFactory> Factories { get; set; }
        public List<OilManagementArea> Areas { get; set; }
        public List<OilManagementEquipment> Equipments { get; set; }

    }
}