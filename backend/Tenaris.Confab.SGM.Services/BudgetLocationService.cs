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
    public class BudgetLocationService : IBudgetLocationService
    {

        private IBudgetLocationRepositorie _BudgetLocationRepo;

        public BudgetLocationService(IBudgetLocationRepositorie BudgetLocationRepo)
        {
            _BudgetLocationRepo = BudgetLocationRepo;
        }

        

        public int UpdateBudgetLocation(BudgetLocation budgetLocation)
        {
            return _BudgetLocationRepo.UpdateBudgetLocation(budgetLocation);
        }

       
    }
}
