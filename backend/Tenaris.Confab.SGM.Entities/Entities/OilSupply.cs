using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class OilSupply
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
        public string CreatedBy { get; set; }
        public string ModificatedBy { get; set; }
        public DateTime InsDateTime { get; set; }
        public DateTime UpdDateTime { get; set; }

    }
}
