using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class BudgetPlantViewModel
    {
        public int idBudgetPlant{ get; set; }

        public int cdBudget { get; set; }

        public int cdPlant { get; set; }

        public float? vlGoal { get; set; }
       
        public bool Active { get; set; }

        public DateTime InsDateTime { get; set; }

        public DateTime UpdDateTime { get; set; }

        public string cdUser { get; set; }

        public Plant Plant { get; set; }

        public bool cdPredictive { get; set; }
    }
}