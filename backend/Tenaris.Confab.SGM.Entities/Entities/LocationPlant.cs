using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class LocationPlant
    {
        public int idLocationPlant { get; set; }
        
        public int cdLocation { get; set; }

        public int cdPlant { get; set; }

        public string cdUser { get; set; }

        public bool Active { get; set; }

        public DateTime InsDateTime { get; set; }

        public DateTime UpdDateTime { get; set; }

        public Location Location { get; set; }

        public Plant Plant { get; set; }

    }
}
