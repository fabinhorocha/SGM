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
    public class BudgetLocationRepositorie : IBudgetLocationRepositorie
    {


        public void InsertBudgetLocation(SqlConnection sqlConn, BudgetLocation budgetLocation)
        {

            try
            {

                sqlConn.Execute(@"
                        Insert into [Plant].[BudgetLocation] (cdBudget, cdLocationPlant, vlGoal, Active, cdUser, InsDateTime) values (@cdBudget, @cdLocationPlant, @vlGoal, @Active, @cdUser, @InsDateTime) ",
                    new
                    {
                        cdBudget = budgetLocation.cdBudget,
                        cdLocationPlant = budgetLocation.cdLocationPlant,
                        vlGoal = budgetLocation.vlGoal,                                         
                        Active = budgetLocation.Active,
                        cdUser = budgetLocation.cdUser,
                        InsDateTime = budgetLocation.InsDateTime

                    });

            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }


        }

        public void UpdateBudgetLocation(SqlConnection sqlConn, BudgetLocation budgetLocation)
        {

            try
            {

                sqlConn.Execute(@"
                        update [Plant].[BudgetLocation] set vlGoal = @vlGoal, Active = @Active, cdUser = @cdUser, UpdDateTime = @UpdDateTime where idBudgetLocation = @idBudgetLocation",
                     new
                     {
                         vlGoal = budgetLocation.vlGoal,                       
                         Active = budgetLocation.Active,
                         cdUser = budgetLocation.cdUser,
                         UpdDateTime = budgetLocation.UpdDateTime,
                         idBudgetLocation = budgetLocation.idBudgetLocation

                     });


            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }

     
        public int UpdateBudgetLocation(BudgetLocation budgetLocation)
        {

            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    sqlConn.Execute(@"
                        update [Plant].[BudgetLocation] set vlGoal = @vlGoal, Active = @Active, cdUser = @cdUser, UpdDateTime = @UpdDateTime where idBudgetLocation = @idBudgetLocation",
                     new
                     {
                         vlGoalOT = budgetLocation.vlGoal,                                            
                         Active = budgetLocation.Active,
                         cdUser = budgetLocation.cdUser,
                         UpdDateTime = budgetLocation.UpdDateTime,
                         idBudgetLocation = budgetLocation.idBudgetLocation

                     });

                    return budgetLocation.idBudgetLocation;
                }


            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }

   

    }
}
