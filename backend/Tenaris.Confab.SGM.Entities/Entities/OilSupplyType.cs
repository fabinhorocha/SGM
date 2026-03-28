using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class OilSupplyType
    {
        public int idOilSupplyType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime InsDateTime { get; set; }
        public DateTime UpdDateTime { get; set; }
    }
}
