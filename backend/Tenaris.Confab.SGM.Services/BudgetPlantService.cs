using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class BudgetPlantService : IBudgetPlantService
    {

        private IBudgetPlantRepositorie _BudgetPlantRepo;

        public BudgetPlantService(IBudgetPlantRepositorie BudgetPlantRepo)
        {
            _BudgetPlantRepo = BudgetPlantRepo;
        }

        

        public int UpdateBudgetPlant(BudgetPlant budgetPlant)
        {
            return _BudgetPlantRepo.UpdateBudgetPlant(budgetPlant);
        }

       
    }
}
