using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain;
using Tenaris.Confab.SGM.Domain.Entities;
using Dapper;
using System.Data.SqlClient;
using System.Dynamic;
using System.Transactions;
using Tenaris.Confab.SAP.Connector;
using System.IO;

namespace Tenaris.Confab.SGM.Repositories
{
    public class BudgetRepositorie : IBudgetRepositorie
    {

        public Budget GetBudget(int id)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, Budget>();

                    sqlConn.Query<Budget, BudgetLocation, BudgetPlant, LocationPlant, Location, Plant, Plant, Budget>(@"
                        select b.*, bl.*, bp.*, lp.*,l.*,  p.*, p2.*
                        from [Plant].[Budget] b
                        inner join [Plant].[BudgetLocation] bl on bl.cdBudget = b.idBudget
                        inner join [Plant].[BudgetPlant] bp on bp.cdBudget = b.idBudget                        
                        inner join [Plant].[LocationPlant] lp on lp.idLocationPlant = bl.cdLocationPlant
                        inner join [Plant].[Location] l on l.idLocation = lp.cdLocation                     
                        inner join [Plant].[Plant] p on p.idPlant = lp.cdPlant   
                        inner join [Plant].[Plant] p2 on p2.idPlant = bp.cdPlant                                           
                        where b.idBudget = @idBudget",
                        (b, bl, bp, lp, l, p, p2) =>
                        {
                            Budget budget;

                            if (!lookup.TryGetValue(b.idBudget, out budget))
                            {
                                lookup.Add(b.idBudget, budget = b);

                            }

                            if (bl != null)
                            {
                                if (budget.budgetLocations == null)
                                    budget.budgetLocations = new List<BudgetLocation>();


                                bl.LocationPlant = lp;
                                bl.LocationPlant.Plant = p;
                                bl.LocationPlant.Location = l;

                                if (budget.budgetLocations.Where(w => w.idBudgetLocation == bl.idBudgetLocation).Count() == 0)
                                    budget.budgetLocations.Add(bl);
                            }

                            if (bp != null)
                            {
                                if (budget.budgetPlants == null)
                                    budget.budgetPlants = new List<BudgetPlant>();

                                if (budget.budgetPlantsPredictive == null)
                                    budget.budgetPlantsPredictive = new List<BudgetPlant>();


                                bp.Plant = p2;


                                if (bp.cdPredictive)
                                {
                                    if (budget.budgetPlantsPredictive.Where(w => w.idBudgetPlant == bp.idBudgetPlant && w.cdPredictive == bp.cdPredictive).Count() == 0)
                                        budget.budgetPlantsPredictive.Add(bp);
                                }
                                else {

                                    if (budget.budgetPlants.Where(w => w.idBudgetPlant == bp.idBudgetPlant && w.cdPredictive == bp.cdPredictive).Count() == 0)
                                        budget.budgetPlants.Add(bp);
                                }

                            }




                            return budget;

                        },
                        new { idBudget = id },
                        splitOn: "idBudgetLocation,idBudgetPlant,idLocationPlant,idLocation,idPlant,idPlant"
                        ).AsQueryable();


                    return lookup.Values.Cast<Budget>().SingleOrDefault();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }


        public Budget GetActualBudget()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, Budget>();

                    sqlConn.Query<Budget, BudgetLocation, BudgetPlant, LocationPlant, Location, Plant, Plant, Budget>(@"
                        select b.*, bl.*, bp.*, lp.*, l.*,  p.*, p2.*
                        from [Plant].[Budget] b
                        inner join [Plant].[BudgetLocation] bl on bl.cdBudget = b.idBudget
                        inner join [Plant].[BudgetPlant] bp on bp.cdBudget = b.idBudget                        
                        inner join [Plant].[LocationPlant] lp on lp.idLocationPlant = bl.cdLocationPlant
                        inner join [Plant].[Location] l on l.idLocation = lp.cdLocation                     
                        inner join [Plant].[Plant] p on p.idPlant = lp.cdPlant  
                        inner join [Plant].[Plant] p2 on p2.idPlant = bp.cdPlant                                          
                        where b.dateStart = (select max(dateStart) from Plant.Budget where Active = 1)",
                        (b, bl, bp, lp, l, p, p2) =>
                        {
                            Budget budget;

                            if (!lookup.TryGetValue(b.idBudget, out budget))
                            {
                                lookup.Add(b.idBudget, budget = b);

                            }

                            if (bl != null)
                            {
                                if (budget.budgetLocations == null)
                                    budget.budgetLocations = new List<BudgetLocation>();


                                bl.LocationPlant = lp;
                                bl.LocationPlant.Plant = p;
                                bl.LocationPlant.Location = l;

                                if (budget.budgetLocations.Where(w => w.idBudgetLocation == bl.idBudgetLocation).Count() == 0)
                                    budget.budgetLocations.Add(bl);
                            }

                            if (bp != null)
                            {
                                if (budget.budgetPlants == null)
                                    budget.budgetPlants = new List<BudgetPlant>();

                                if (budget.budgetPlantsPredictive == null)
                                    budget.budgetPlantsPredictive = new List<BudgetPlant>();


                                bp.Plant = p2;


                                if (bp.cdPredictive)
                                {
                                    if (budget.budgetPlants.Where(w => w.idBudgetPlant == bp.idBudgetPlant && w.cdPredictive == bp.cdPredictive).Count() == 0)
                                        budget.budgetPlantsPredictive.Add(bp);
                                }
                                else {
                                    if (budget.budgetPlants.Where(w => w.idBudgetPlant == bp.idBudgetPlant && w.cdPredictive == bp.cdPredictive).Count() == 0)
                                        budget.budgetPlants.Add(bp);
                                }


                            }


                            return budget;

                        },
                        splitOn: "idBudgetLocation,idBudgetPlant,idLocationPlant,idLocation,idPlant,idPlant"
                        ).AsQueryable();


                    return lookup.Values.Cast<Budget>().SingleOrDefault();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public List<Budget> GetBudgets(bool? active)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, Budget>();

                    if (active == null)
                    {
                        sqlConn.Query<Budget, BudgetLocation, BudgetPlant, LocationPlant, Location, Plant, Plant, Budget>(@"
                        select b.*, bl.*, bp.*, lp.*, l.*,  p.*, p2.*
                        from [Plant].[Budget] b
                        inner join [Plant].[BudgetLocation] bl on bl.cdBudget = b.idBudget
                        inner join [Plant].[BudgetPlant] bp on bp.cdBudget = b.idBudget
                        inner join [Plant].[LocationPlant] lp on lp.idLocationPlant = bl.cdLocationPlant
                        inner join [Plant].[Location] l on l.idLocation = lp.cdLocation                     
                        inner join [Plant].[Plant] p on p.idPlant = lp.cdPlant       
                        inner join [Plant].[Plant] p2 on p2.idPlant = bp.cdPlant                                                         
                        order by b.dateStart desc",
                            (b, bl, bp, lp, l, p, p2) =>
                            {
                                Budget budget;

                                if (!lookup.TryGetValue(b.idBudget, out budget))
                                {
                                    lookup.Add(b.idBudget, budget = b);

                                }

                                if (bl != null)
                                {
                                    if (budget.budgetLocations == null)
                                        budget.budgetLocations = new List<BudgetLocation>();


                                    bl.LocationPlant = lp;
                                    bl.LocationPlant.Plant = p;
                                    bl.LocationPlant.Location = l;

                                    if (budget.budgetLocations.Where(w => w.idBudgetLocation == bl.idBudgetLocation).Count() == 0)
                                        budget.budgetLocations.Add(bl);
                                }

                                if (bp != null)
                                {
                                    if (budget.budgetPlants == null)
                                        budget.budgetPlants = new List<BudgetPlant>();

                                    if (budget.budgetPlantsPredictive == null)
                                        budget.budgetPlantsPredictive = new List<BudgetPlant>();


                                    bp.Plant = p2;


                                    if (bp.cdPredictive)
                                    {
                                        if (budget.budgetPlants.Where(w => w.idBudgetPlant == bp.idBudgetPlant && w.cdPredictive == bp.cdPredictive).Count() == 0)
                                            budget.budgetPlantsPredictive.Add(bp);
                                    }
                                    else {

                                        if (budget.budgetPlants.Where(w => w.idBudgetPlant == bp.idBudgetPlant && w.cdPredictive == bp.cdPredictive).Count() == 0)
                                            budget.budgetPlants.Add(bp);
                                    }

                                }



                                return budget;

                            },
                            splitOn: "idBudgetLocation,idBudgetPlant,idLocationPlant,idLocation,idPlant, idPlant"
                            ).AsQueryable();
                    }
                    else
                    {
                        sqlConn.Query<Budget, BudgetLocation, BudgetPlant, LocationPlant, Location, Plant, Plant, Budget>(@"
                        select  b.*, bl.*, bp.*, lp.*, l.*,  p.*, p2.*
                        from [Plant].[Budget] b
                        inner join [Plant].[BudgetLocation] bl on bl.cdBudget = b.idBudget
                        inner join [Plant].[BudgetPlant] bp on bp.cdBudget = b.idBudget
                        inner join [Plant].[LocationPlant] lp on lp.idLocationPlant = bl.cdLocationPlant
                        inner join [Plant].[Location] l on l.idLocation = lp.cdLocation                     
                        inner join [Plant].[Plant] p on p.idPlant = lp.cdPlant 
                        inner join [Plant].[Plant] p2 on p2.idPlant = bp.cdPlant     
                        where b.Active = @active                                            
                        order by b.dateStart desc",
                          (b, bl, bp, lp, l, p, p2) =>
                          {
                              Budget budget;

                              if (!lookup.TryGetValue(b.idBudget, out budget))
                              {
                                  lookup.Add(b.idBudget, budget = b);

                              }

                              if (bl != null)
                              {
                                  if (budget.budgetLocations == null)
                                      budget.budgetLocations = new List<BudgetLocation>();


                                  bl.LocationPlant = lp;
                                  bl.LocationPlant.Plant = p;
                                  bl.LocationPlant.Location = l;


                                  budget.budgetLocations.Add(bl);
                              }

                              if (bp != null)
                              {
                                  if (budget.budgetPlants == null)
                                      budget.budgetPlants = new List<BudgetPlant>();

                                  if (budget.budgetPlantsPredictive == null)
                                      budget.budgetPlantsPredictive = new List<BudgetPlant>();


                                  bp.Plant = p2;


                                  if (bp.cdPredictive)
                                  {
                                      if (budget.budgetPlants.Where(w => w.idBudgetPlant == bp.idBudgetPlant && w.cdPredictive == bp.cdPredictive).Count() == 0)
                                          budget.budgetPlantsPredictive.Add(bp);
                                  }
                                  else {

                                      if (budget.budgetPlants.Where(w => w.idBudgetPlant == bp.idBudgetPlant && w.cdPredictive == bp.cdPredictive).Count() == 0)
                                          budget.budgetPlants.Add(bp);
                                  }

                              }




                              return budget;

                          },
                            new { active = active },
                            splitOn: "idBudgetLocation,idBudgetPlant,idLocationPlant,idLocation,idPlant, idPlant"
                            ).AsQueryable();
                    }


                    return lookup.Values.Cast<Budget>().ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        
        public List<Budget> GetBudgetsActive()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    return sqlConn.Query<Budget>(@"
                        Select b.* 
                        from [Plant].[Budget] b where Active = 1 order by datestart desc").ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }


        public object InsertBudget(Budget budget)
        {

            try
            {
                using (var transactionScope = new TransactionScope())
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    budget.InsDateTime = DateTime.Now;
                    var id = sqlConn.Query<int>(@"
                        Insert into [Plant].[Budget] (dateStart, dateEnd, Active, InsDateTime, cdUser) 
                            values (@dateStart, @dateEnd, @Active, @InsDateTime, @cdUser);                        
                        select cast(SCOPE_IDENTITY() as int)",
                    new
                    {
                        dateStart = budget.dateStart,
                        dateEnd = budget.dateEnd,
                        Active = budget.Active,
                        InsDateTime = budget.InsDateTime,
                        cdUser = budget.cdUser


                    }).Single();


                    var budgetLocations = new BudgetLocationRepositorie();

                    foreach (var detail in budget.budgetLocations)
                    {
                        detail.cdBudget = id;
                        detail.cdUser = budget.cdUser;
                        detail.InsDateTime = budget.InsDateTime;

                        budgetLocations.InsertBudgetLocation(sqlConn, detail);
                    }

                    var budgetPlants = new BudgetPlantRepositorie();

                    foreach (var detail in budget.budgetPlants)
                    {
                        detail.cdBudget = id;
                        detail.cdUser = budget.cdUser;
                        detail.InsDateTime = budget.InsDateTime;

                        budgetPlants.InsertBudgetPlant(sqlConn, detail);
                    }


                    foreach (var detail in budget.budgetPlantsPredictive)
                    {
                        detail.cdBudget = id;
                        detail.cdUser = budget.cdUser;
                        detail.InsDateTime = budget.InsDateTime;

                        budgetPlants.InsertBudgetPlant(sqlConn, detail);
                    }


                    transactionScope.Complete();

                    return new { id = id, status = true, message = "Dados inseridos com sucesso !" };

                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }


        }


        public object UpdateBudget(Budget budget)
        {

            try
            {
                using (var transactionScope = new TransactionScope())
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Execute(@"
                        update [Plant].[Budget] set  
                            dateStart = @dateStart, 
                            dateEnd = @dateEnd, 
                            Active = @Active, 
                            UpdDateTime = @UpdDateTime,                            
                            cdUser = @cdUser                            
                        where idBudget = @idBudget",
                    new
                    {
                        dateStart = budget.dateStart,
                        dateEnd = budget.dateEnd,
                        Active = budget.Active,
                        cdUser = budget.cdUser,
                        UpdDateTime = DateTime.Now,
                        idBudget = budget.idBudget
                    });

                    var budgetLocations = new BudgetLocationRepositorie();

                    foreach (var detail in budget.budgetLocations)
                    {

                        detail.cdUser = budget.cdUser;
                        detail.UpdDateTime = DateTime.Now;

                        budgetLocations.UpdateBudgetLocation(sqlConn, detail);
                    }

                    var budgetPlants = new BudgetPlantRepositorie();

                    foreach (var detail in budget.budgetPlants)
                    {

                        detail.cdUser = budget.cdUser;
                        detail.UpdDateTime = DateTime.Now;

                        budgetPlants.UpdateBudgetPlant(sqlConn, detail);
                    }

                    foreach (var detail in budget.budgetPlantsPredictive)
                    {

                        detail.cdUser = budget.cdUser;
                        detail.UpdDateTime = DateTime.Now;

                        budgetPlants.UpdateBudgetPlant(sqlConn, detail);
                    }


                    transactionScope.Complete();


                    return new { id = budget.idBudget, status = true, message = "Dados atualizados com sucesso !" };

                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }

        public object UpdateBudgetStatus(Budget budget)
        {

            try
            {

                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Execute(@"
                        update [Plant].[Budget] set                              
                            Active = @Active, 
                            UpdDateTime = @UpdDateTime,                            
                            cdUser = @cdUser                            
                        where idBudget = @idBudget",
                    new
                    {
                        Active = budget.Active,
                        cdUser = budget.cdUser,
                        UpdDateTime = DateTime.Now,
                        idBudget = budget.idBudget
                    });


                    return new { id = budget.idBudget, status = true, message = "Dados atualizados com sucesso !" };

                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }

    }
}
