using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IBudgetService
    {
        Budget GetBudget(int id);

        List<Budget> GetBudgets(bool? active);

        Budget GetActualBudget();

        List<Budget> GetBudgetsActive();
        object InsertBudget(Budget budget);

        object UpdateBudget(Budget budget);

        object UpdateBudgetStatus(Budget budget);





    }
}
