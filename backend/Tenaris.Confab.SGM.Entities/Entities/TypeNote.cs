using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class TypeNote
    {

        public int idTypeNote { get; set; }

        public string Sheet { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public int cdNote { get; set; }

    }
}
