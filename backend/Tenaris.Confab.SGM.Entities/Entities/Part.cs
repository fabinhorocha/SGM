using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class Part
    {

        public int idPart { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int ? idType { get; set; }

        public int ? idArea { get; set; }

        public int ? idLocal { get; set; }

        public bool Active { get; set; }

        public DateTime InsDateTime { get; set; }

        public DateTime UpdDateTime { get; set; }

        public Area Area { get; set; }

        public Local Local { get; set; }
             


    }
}
