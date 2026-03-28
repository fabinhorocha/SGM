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
    public class BudgetService : IBudgetService
    {

        private IBudgetRepositorie _BudgetRepo;

        public BudgetService(BudgetRepositorie BudgetRepo)
        {
            _BudgetRepo = BudgetRepo;
        }

        public Budget GetBudget(int id)
        {
            return _BudgetRepo.GetBudget(id);
        }

        public Budget GetActualBudget()
        {
            return _BudgetRepo.GetActualBudget();
        }

        public List<Budget> GetBudgets(bool? active)
        {
            return _BudgetRepo.GetBudgets(active);
        }

        public List<Budget> GetBudgetsActive()
        {
            return _BudgetRepo.GetBudgetsActive();
        }


        public object InsertBudget(Budget budget)
        {
            string message = "";

            if (ValidBudget(budget, ref message))
                return _BudgetRepo.InsertBudget(budget);
            else
               return new { id= -1, status = false, message = message  };
           
        }

        public object UpdateBudget(Budget budget)
        {
            string message = "";

            if (ValidBudget(budget, ref message))
                return _BudgetRepo.UpdateBudget(budget);
            else
                return new { id = -1, status = false, message = message };
        }

        public object UpdateBudgetStatus(Budget budget)
        {            
            string message = "";

            if (ValidBudget(budget, ref message))
                return _BudgetRepo.UpdateBudgetStatus(budget);
            else
                return new { id = -1, status = false, message = message };
        }

        private bool ValidBudget(Budget budget, ref string message)
        {
            try {
                var budgets = _BudgetRepo.GetBudgets(true);

                var result = budgets.Where(w => ((budget.dateStart >= w.dateStart && budget.dateStart <= w.dateEnd) || (budget.dateEnd >= w.dateEnd && budget.dateEnd <= w.dateEnd)) && (w.idBudget != budget.idBudget)).ToList();

                if (result.Count > 0)
                {
                    message = "Já existe Budget cadastrado para esse período !";
                    return false;
                }
                else
                    return true;
            }
            catch(Exception ex)
            {
                message = ex.Message;

                return false;
            }


        }


    }
}
