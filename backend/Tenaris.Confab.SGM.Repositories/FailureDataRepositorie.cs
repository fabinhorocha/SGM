using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Tenaris.Confab.SGM.Domain.Entities;
using System.Data.SqlClient;
using Tenaris.Confab.SGM.Domain;
using Tenaris.Confab.SGM.Repositories;
using System.Transactions;

namespace Tenaris.Confab.SGM.Repositories
{
    public class FailureDataRepositorie : IFailureDataRepositorie
    {

        public List<FailureData> GetFailureData(int idIndicator, int idBudget)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, FailureData>();

                    sqlConn.Query<FailureData, Budget, Plant, Indicator, FailureData>(@"
                       select 
                            pd.idFailureData,
	                        pd.cdBudget,	
	                        pd.cdIndicator,		                        
	                        pd.dtMonth,
	                        pd.dtYear,
	                        Right('00' + CAST(pd.dtMonth AS VARCHAR(4)),2) + '/' +CAST(pd.dtYear AS VARCHAR(4)) as monthYear,
	                        pd.vlCountRequestedM2ByFA,	
                            pd.vlCountRequestedM2ByFAOpen,	
                            pd.vlCountPendingFA,	
                            pd.vlCountAnalyzedFA,	
                            pd.vlCountClosedMeasuresFA,
                            pd.cdPlant,
	                        b.*,
                            p.*,	                        
	                        i.*                            
                        from [Plant].[FailureData] pd
                        inner join [Plant].[Budget] b on b.idBudget = pd.cdBudget                      
                        inner join [Plant].[Plant] p on p.idPlant = pd.cdPlant                        
                        inner join [Common].[Indicator] i on i.idIndicator = pd.cdIndicator                                          
                        where pd.cdIndicator = @idIndicator and b.Active = 1 and  YEAR(b.dateStart) between 
                              (Select Year(dateStart) from [Plant].[Budget] where idBudget = @idBudget)-5 and
                              (Select Year(dateStart) from [Plant].[Budget] where idBudget = @idBudget)",
                        (pd, b, p, i) =>
                        {
                            FailureData failureData;

                            if (!lookup.TryGetValue(pd.idFailureData, out failureData))
                            {

                                lookup.Add(pd.idFailureData, failureData = pd);

                            }


                            failureData.Budget = b;
                            failureData.Indicator = i;
                            failureData.Plant = p;


                            return failureData;

                        },
                        new { idIndicator = idIndicator, idBudget = idBudget },
                        splitOn: "idFailureData,idBudget,idPlant,idIndicator"
                        ).AsQueryable();


                    return lookup.Values.Cast<FailureData>().ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public List<FailureData> GetFailureDataByPlants(int idIndicator, int idBudget, List<string> plants)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, FailureData>();

                    sqlConn.Query<FailureData, Budget, Plant, Indicator, FailureData>(@"
                       select 
                            pd.idFailureData,
	                        pd.cdBudget,	
	                        pd.cdIndicator,		                        
	                        pd.dtMonth,
	                        pd.dtYear,
	                        Right('00' + CAST(pd.dtMonth AS VARCHAR(4)),2) + '/' +CAST(pd.dtYear AS VARCHAR(4)) as monthYear,
	                        pd.vlCountRequestedM2ByFA,	
                            pd.vlCountRequestedM2ByFAOpen,	
                            pd.vlCountPendingFA,	
                            pd.vlCountAnalyzedFA,	
                            pd.vlCountClosedMeasuresFA, 
                            pd.cdPlant,       	                        
                            b.*,
                            p.*,	                        
	                        i.*                            
                        from [Plant].[FailureData] pd
                        inner join [Plant].[Budget] b on b.idBudget = pd.cdBudget                      
                        inner join [Plant].[Plant] p on p.idPlant = pd.cdPlant                        
                        inner join [Common].[Indicator] i on i.idIndicator = pd.cdIndicator                                          
                        where pd.cdIndicator = @idIndicator and b.Active = 1 and  YEAR(b.dateStart) between 
                              (Select Year(dateStart) from [Plant].[Budget] where idBudget = @idBudget)-5 and
                              (Select Year(dateStart) from [Plant].[Budget] where idBudget = @idBudget) and p.Sheet in @plants",
                        (pd, b, p, i) =>
                        {
                            FailureData failureData;

                            if (!lookup.TryGetValue(pd.idFailureData, out failureData))
                            {

                                lookup.Add(pd.idFailureData, failureData = pd);

                            }


                            failureData.Budget = b;
                            failureData.Indicator = i;
                            failureData.Plant = p;


                            return failureData;

                        },
                        new { idIndicator = idIndicator, idBudget = idBudget, plants = plants },
                        splitOn: "idFailureData,idBudget,idPlant,idIndicator"
                        ).AsQueryable();


                    return lookup.Values.Cast<FailureData>().ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public List<FailureData> GetFailureDataByDateRange(string Plant, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var list = new List<FailureData>();

                    sqlConn.Query<FailureData, Budget, FailureData>(@"
                       exec  [Plant].[SPGetFailuresData] @Plant , @dateStart, @dateEnd",
                       (t, b) =>
                       {
                           FailureData failureData;

                           failureData = t;

                           list.Add(failureData);

                           failureData.Budget = b;

                           return failureData;

                       },
                       new { Plant = Plant, dateStart = startDate, dateEnd = endDate },
                         splitOn: "idBudget"
                        ).AsQueryable();


                    return list;
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public object InsertFailureData(List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate)
        {

            var resultObj = new object();

            try
            {
                var option = new TransactionOptions();
                option.IsolationLevel = IsolationLevel.ReadCommitted;
                option.Timeout = TimeSpan.FromMinutes(30);

                using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, option))
                {
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        
                        foreach (string plant in plantsGroups)
                        {
                            DeleteFailureData(sqlConn, plant, startdate, endDate);

                            foreach (var data in GetFailureDataByDateRange(plant, startdate, endDate))
                            {

                                sqlConn.Execute(@"
                                        Insert into [Plant].[FailureData] (cdBudget, cdPlant, dtMonth, dtYear, vlCountRequestedM2ByFA, vlCountRequestedM2ByFAOpen, vlCountPendingFA, vlCountAnalyzedFA, vlCountClosedMeasuresFA, cdIndicator, InsDateTime, cdUser) 
                                               values (@cdBudget, @cdPlant, @dtMonth, @dtYear, @vlCountRequestedM2ByFA, @vlCountRequestedM2ByFAOpen, @vlCountPendingFA, @vlCountAnalyzedFA, @vlCountClosedMeasuresFA, @cdIndicator, @InsDateTime, @cdUser) ",
                                new
                                {
                                    cdBudget = data.cdbudget,
                                    cdPlant = data.cdPlant,
                                    dtMonth = data.dtMonth,
                                    dtYear = data.dtYear,
                                    vlCountRequestedM2ByFA = data.vlCountRequestedM2ByFA,
                                    vlCountRequestedM2ByFAOpen = data.vlCountRequestedM2ByFAOpen,
                                    vlCountPendingFA = data.vlCountPendingFA,
                                    vlCountAnalyzedFA = data.vlCountAnalyzedFA,
                                    vlCountClosedMeasuresFA = data.vlCountClosedMeasuresFA,
                                    cdIndicator = data.cdIndicator,
                                    InsDateTime = data.InsDateTime,
                                    cdUser = data.cdUser
                                });


                            }
                        }

                        transactionScope.Complete();
                        resultObj = new { status = true, message = "Dados inseridos com sucesso !" };

                    }

                }

                return resultObj;
            }

            catch (Exception ex)
            {
                resultObj = new { id = -1, status = false, message = ex.Message };

                return resultObj;
            }


        }

        private void UpdateFailureData(SqlConnection sqlConn, FailureData failureData)
        {

            try
            {
                sqlConn.Execute(@"
                        update [Plant].[FailureData] set 
                                                        vlCountAnalyzedFA= @vlCountAnalyzedFA, 
                                                        vlCountClosedMeasuresFA= @vlCountClosedMeasuresFA, 
                                                        vlCountPendingFA = @vlCountPendingFA, 
                                                        vlCountRequestedM2ByFA = @vlCountRequestedM2ByFA, 
                                                        vlCountRequestedM2ByFAOpen = @vlCountRequestedM2ByFAOpen,                                                         
                                                        UpdDateTime = @UpdDateTime, 
                                                        cdUser = @cdUser                                                        
                            where  cdPlant = @cdPlant and cdIndicator = @cdIndicator and  cdBudget = @cdBudget and dtMonth = @dtMonth and dtYear = @dtYear",
                new
                {
                    cdBudget = failureData.cdbudget,
                    cdPlant = failureData.cdPlant,
                    dtMonth = failureData.dtMonth,
                    dtYear = failureData.dtYear,
                    vlCountAnalyzedFA = failureData.vlCountAnalyzedFA,
                    vlCountClosedMeasuresFA = failureData.vlCountClosedMeasuresFA,
                    vlCountPendingFA = failureData.vlCountPendingFA,
                    vlCountRequestedM2ByFA = failureData.vlCountRequestedM2ByFA,
                    vlCountRequestedM2ByFAOpen = failureData.vlCountRequestedM2ByFAOpen,
                    cdIndicator = failureData.cdIndicator,
                    UpdDateTime = failureData.UpdDateTime,
                    cdUser = failureData.cdUser,
                });


            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }


        }

        public void DeleteFailureData(SqlConnection sqlConn, string plant, DateTime dateStart, DateTime dateEnd)
        {
            try
            {

               
                    sqlConn.Execute(@"                       
                        delete pd FROM [Plant].[FailureData] pd
                        inner join 	[Plant].[Plant] p on p.idPlant = pd.cdPlant                                                                                              
                        WHERE ((pd.DTMONTH = @monthStart AND pd.DTYEAR= @yearStart) OR (pd.DTMONTH = @monthEnd AND pd.DTYEAR= @yearEnd)) AND p.Sheet = @plant",
                        new
                        {
                            monthStart = dateStart.Month,
                            yearStart = dateStart.Year,
                            monthEnd = dateEnd.Month,
                            yearEnd = dateEnd.Year,
                            plant = plant

                        });
                


            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public List<FailureData> GetFailureDataFailured(int idIndicator, int idBudget)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, FailureData>();

                    sqlConn.Query<FailureData, Budget, Plant, Indicator, FailureData>(@"
                       select 
                            pd.idFailureData,
	                        pd.cdBudget,	
	                        pd.cdIndicator,		                        
	                        pd.dtMonth,
	                        pd.dtYear,
	                        Right('00' + CAST(pd.dtMonth AS VARCHAR(4)),2) + '/' +CAST(pd.dtYear AS VARCHAR(4)) as monthYear,
	                        pd.vlCountRequestedM2ByFA,	
                            pd.vlCountRequestedM2ByFAOpen,	
                            pd.vlCountPendingFA,	
                            pd.vlCountAnalyzedFA,	
                            pd.vlCountClosedMeasuresFA,                                                  
                            pd.cdPlant,                                                        
	                        b.*,
                            p.*,	                        
	                        i.*                            
                        from [Plant].[FailureData] pd
                        inner join [Plant].[Budget] b on b.idBudget = pd.cdBudget                      
                        inner join [Plant].[Plant] p on p.idPlant = pd.cdPlant                        
                        inner join [Common].[Indicator] i on i.idIndicator = pd.cdIndicator                                          
                        where pd.cdIndicator = @idIndicator and b.Active = 1 and  YEAR(b.dateStart) between 
                              (Select Year(dateStart) from [Plant].[Budget] where idBudget = @idBudget)-5 and
                              (Select Year(dateStart) from [Plant].[Budget] where idBudget = @idBudget)",
                        (pd, b, p, i) =>
                        {
                            FailureData failureData;

                            if (!lookup.TryGetValue(pd.idFailureData, out failureData))
                            {

                                lookup.Add(pd.idFailureData, failureData = pd);

                            }


                            failureData.Budget = b;
                            failureData.Indicator = i;
                            failureData.Plant = p;


                            return failureData;

                        },
                        new { idIndicator = idIndicator, idBudget = idBudget },
                        splitOn: "idFailureData,idBudget,idPlant,idIndicator"
                        ).AsQueryable();


                    return lookup.Values.Cast<FailureData>().ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

    }
}
