using System.Collections.Generic;
using System.Data.SqlClient;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface IBudgetPlantRepositorie
    {
        void InsertBudgetPlant(SqlConnection sqlConn, BudgetPlant budgetPlant);

        void UpdateBudgetPlant(SqlConnection sqlConn, BudgetPlant budgetPlant);

        int UpdateBudgetPlant(BudgetPlant budgetPlant);
    }
}