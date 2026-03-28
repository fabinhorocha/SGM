using System.Collections.Generic;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface IBudgetRepositorie
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