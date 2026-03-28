using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class AreaViewModel
    {
        public int idArea { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public DateTime InsDateTime { get; set; }

        public DateTime UpdDateTime { get; set; }
    }
}