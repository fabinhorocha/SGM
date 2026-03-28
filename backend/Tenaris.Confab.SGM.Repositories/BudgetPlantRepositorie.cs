using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain;
using Tenaris.Confab.SGM.Domain.Entities;
using Dapper;
using System.Data.SqlClient;
using System.IO;

namespace Tenaris.Confab.SGM.Repositories
{
    public class BudgetPlantRepositorie : IBudgetPlantRepositorie
    {


        public void InsertBudgetPlant(SqlConnection sqlConn, BudgetPlant budgetPlant)
        {

            try
            {

                sqlConn.Execute(@"
                        Insert into [Plant].[BudgetPlant] (cdBudget, cdPlant, vlGoal, Active, cdUser, InsDateTime, cdPredictive) values (@cdBudget, @cdPlant, @vlGoal, @Active, @cdUser, @InsDateTime, @cdPredictive) ",
                    new
                    {
                        cdBudget = budgetPlant.cdBudget,
                        cdPlant = budgetPlant.cdPlant,
                        vlGoal = budgetPlant.vlGoal,                                         
                        Active = budgetPlant.Active,
                        cdUser = budgetPlant.cdUser,
                        InsDateTime = budgetPlant.InsDateTime,
                        cdPredictive = budgetPlant.cdPredictive
                    });

            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }


        }

        public void UpdateBudgetPlant(SqlConnection sqlConn, BudgetPlant budgetPlant)
        {

            try
            {

                sqlConn.Execute(@"
                        update [Plant].[BudgetPlant] set vlGoal = @vlGoal, Active = @Active, cdUser = @cdUser, UpdDateTime = @UpdDateTime where idBudgetPlant = @idBudgetPlant",
                     new
                     {
                         vlGoal = budgetPlant.vlGoal,                       
                         Active = budgetPlant.Active,
                         cdUser = budgetPlant.cdUser,
                         UpdDateTime = budgetPlant.UpdDateTime,
                         idBudgetPlant = budgetPlant.idBudgetPlant

                     });


            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }

     
        public int UpdateBudgetPlant(BudgetPlant budgetPlant)
        {

            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    sqlConn.Execute(@"
                        update [Plant].[BudgetPlant] set vlGoal = @vlGoal, Active = @Active, cdUser = @cdUser, UpdDateTime = @UpdDateTime where idBudgetPlant = @idBudgetPlant",
                     new
                     {
                         vlGoalOT = budgetPlant.vlGoal,                                            
                         Active = budgetPlant.Active,
                         cdUser = budgetPlant.cdUser,
                         UpdDateTime = budgetPlant.UpdDateTime,
                         idBudgetPlant = budgetPlant.idBudgetPlant

                     });

                    return budgetPlant.idBudgetPlant;
                }


            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }

   

    }
}
