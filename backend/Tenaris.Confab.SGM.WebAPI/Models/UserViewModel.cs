using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class UserViewModel
    {
        public string Login { get; set; }

        public string Name { get; set; }

        public string Domain { get; set; }

        public List<GroupViewModel> Groups { get; set; }

        public bool ReadOnly { get; set; }

        public bool Maintance { get; set; }

        public bool Configuration { get; set; }
        public bool OilUser { get; set; }

        public bool OilManagement { get; set; }

    }
}