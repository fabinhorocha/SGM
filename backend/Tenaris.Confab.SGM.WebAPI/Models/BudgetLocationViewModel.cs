using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class BudgetLocationViewModel
    {
        public int idBudgetLocation { get; set; }

        public int cdBudget { get; set; }

        public int cdLocationPlant { get; set; }

        public float? vlGoal { get; set; }

        public bool Active { get; set; }

        public DateTime InsDateTime { get; set; }

        public DateTime UpdDateTime { get; set; }

        public string cdUser { get; set; }

        public LocationPlant LocationPlant { get; set; }
    }
}