using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class Budget
    {
        public int idBudget { get; set; }

        public DateTime dateStart { get; set; }

        public DateTime dateEnd { get; set; }

        public string cdUser { get; set; }

        public bool Active { get; set; }

        public DateTime InsDateTime { get; set; }

        public DateTime UpdDateTime { get; set; }

        public List<BudgetLocation> budgetLocations { get; set; }

        public List<BudgetPlant> budgetPlants{ get; set; }

        public List<BudgetPlant> budgetPlantsPredictive { get; set; }

    }
}
