using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class TypeDoc
    {
        public int idTypeDoc { get; set; }

        public string Sheet { get; set; }

        public string Name { get; set; }

        public int cdIndicator { get; set; }

        public bool Active { get; set; }

    
    }
}
