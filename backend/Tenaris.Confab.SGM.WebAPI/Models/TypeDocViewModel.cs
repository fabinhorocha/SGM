using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class TypeDocViewModel
    {
        public int idTypeDoc { get; set; }

        public string Sheet { get; set; }

        public string Name { get; set; }

        public int cdIndicator { get; set; }

        public bool Active { get; set; }

    }
}