using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class OilSupplyViewModel
    {
        public int idOilSupply { get; set; }
        public Component Component { get; set; }
        public int Quantity { get; set; }
        public OilType OilType { get; set; }
        public OilSupplyType OilSupplyType { get; set; }
        public DateTime SupplyDateTime { get; set; }
        public bool IsReuseOil { get; set; }
        public int Shift { get; set; }
        public string Comment { get; set; }
        public StoppageType StoppageType { get; set; }
        public int StoppageTime { get; set; }
        public string cdUser { get; set; }
        public DateTime InsDateTime { get; set; }
        public DateTime UpdDateTime { get; set; }
        public bool OilManagement { get; set; }
        public bool OilUser { get; set; }


        public List<OilType> OilTypes { get; set; }
        public List<OilSupplyType> OilSupplyTypes { get; set; }
        public List<StoppageType> StoppageTypes { get; set; }

        public bool ViewEnabled { get; set; }
        public bool EditionEnabled { get; set; }
    }
}