using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class LocationLocal
    {

        public int idLocationLocal { get; set; }

        public int cdLocal { get; set; }

        public int cdLocation { get; set; }

        public bool Active { get; set; }

        public DateTime InsDateTime { get; set; }

        public DateTime UpdDateTime { get; set; }

        public string cduser { get; set; }

        public Local Local { get; set; }


        public Location Location { get; set; }

    }
}
