using System.Collections.Generic;
using System.Data.SqlClient;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface IBudgetLocationRepositorie
    {
        void InsertBudgetLocation(SqlConnection sqlConn, BudgetLocation budgetLocation);

        void UpdateBudgetLocation(SqlConnection sqlConn, BudgetLocation budgetLocation);

        int UpdateBudgetLocation(BudgetLocation budgetLocation);
    }
}