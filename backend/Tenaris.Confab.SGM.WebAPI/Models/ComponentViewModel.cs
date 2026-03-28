using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class ComponentViewModel
    {
        public int idComponent { get; set; }
        public OilManagementEquipment Equipment { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public List<OilType> EnabledOilTypes{ get; set; }
        public string OilGradeISO { get; set; }
        public string ISOLimitCode { get; set; }
        public string CriticalComponent { get; set; }
        public string MachinesServed { get; set; }
        public decimal FlowRate { get; set; }
        public int Preassure { get; set; }
        public bool Active { get; set; }
        public bool cdIntegrate { get; set; }
        public string CreatedBy { get; set; }
        public string ModificatedBy { get; set; }
        public DateTime InsDateTime { get; set; }
        public DateTime UpdDateTime { get; set; }
        public bool OilManagement { get; set; }
        public bool OilUser { get; set; }

        public List<OilType> OilTypes { get; set; }
    }
}