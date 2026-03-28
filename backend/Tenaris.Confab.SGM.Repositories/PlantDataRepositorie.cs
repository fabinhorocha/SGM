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
    public class PlantDataRepositorie : IPlantDataRepositorie
    {

        public List<PlantData> GetPlantData(int idIndicator, int idBudget, bool cdPredictive)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, PlantData>();

                    sqlConn.Query<PlantData, Budget, Plant, Indicator, PlantData>(@"
                       select 
                            pd.idPlantData,
	                        pd.cdBudget,	
	                        pd.cdIndicator,		                        
	                        pd.dtMonth,
	                        pd.dtYear,
	                        Right('00' + CAST(pd.dtMonth AS VARCHAR(4)),2) + '/' +CAST(pd.dtYear AS VARCHAR(4)) as monthYear,
	                        pd.vlCountNotesPending,	
                            pd.vlCountNotesPendingDelay,	
                            pd.vlCountNotesClosed,	
                            pd.vlCountOTsPending,	
                            pd.vlCountOTsPendingOpen,
                            pd.vlCountOTsPendingReleased,
                            pd.vlCountNotesOpen,	
                            pd.vlCountOTsOpen,	
                            pd.vlCountOTsClosed,	
                            pd.vlCountOTsClosedDelay,	                            
                            pd.cdPlant,
                            pd.vlCountOTsVeryHigh, 
                            pd.vlCountOTsHigh, 
                            pd.vlCountNotesVeryHigh, 
                            pd.vlCountNotesHigh,    
                            pd.vlCountOTsOther,
                            pd.vlCountNotesOther,
                            cast(isnull(pd.vlHoursVeryHigh,0) as int) as vlHoursVeryHigh,
                            cast(isnull(pd.vlHoursHigh,0) as int) as vlHoursHigh,
                            cast(isnull(pd.vlHoursOther,0) as int) as vlHoursOther,
                            (Select top 1 bp.vlGoal from [Plant].[BudgetPlant] bp where bp.cdBudget = pd.cdBudget and bp.cdPlant = pd.cdPlant and bp.cdPredictive = pd.cdPredictive) as vlGoal,                                          
	                        b.*,
                            p.*,	                        
	                        i.*                            
                        from [Plant].[PlantData] pd
                        inner join [Plant].[Budget] b on b.idBudget = pd.cdBudget                      
                        inner join [Plant].[Plant] p on p.idPlant = pd.cdPlant                        
                        inner join [Common].[Indicator] i on i.idIndicator = pd.cdIndicator                                          
                        where pd.cdIndicator = @idIndicator and b.Active = 1 and  YEAR(b.dateStart) between 
                              (Select Year(dateStart) from [Plant].[Budget] where idBudget = @idBudget)-5 and
                              (Select Year(dateStart) from [Plant].[Budget] where idBudget = @idBudget) and pd.cdPredictive = @cdPredictive",
                        (pd, b, p, i) =>
                        {
                            PlantData plantData;

                            if (!lookup.TryGetValue(pd.idPlantData, out plantData))
                            {

                                lookup.Add(pd.idPlantData, plantData = pd);

                            }


                            plantData.Budget = b;
                            plantData.Indicator = i;
                            plantData.Plant = p;


                            return plantData;

                        },
                        new { idIndicator = idIndicator, idBudget = idBudget, cdPredictive = cdPredictive },
                        splitOn: "idPlantData,idBudget,idPlant,idIndicator"
                        ).AsQueryable();


                    return lookup.Values.Cast<PlantData>().ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public List<PlantData> GetPlantDataByPlants(int idIndicator, int idBudget, bool cdPredictive, List<string> plants)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, PlantData>();

                    sqlConn.Query<PlantData, Budget, Plant, Indicator, PlantData>(@"
                       select 
                            pd.idPlantData,
	                        pd.cdBudget,	
	                        pd.cdIndicator,		                        
	                        pd.dtMonth,
	                        pd.dtYear,
	                        Right('00' + CAST(pd.dtMonth AS VARCHAR(4)),2) + '/' +CAST(pd.dtYear AS VARCHAR(4)) as monthYear,
	                        pd.vlCountNotesPending,	
                            pd.vlCountNotesPendingDelay,	
                            pd.vlCountNotesClosed,	
                            pd.vlCountOTsPending,	
                            pd.vlCountOTsPendingOpen,
                            pd.vlCountOTsPendingReleased,
                            pd.vlCountNotesOpen,	
                            pd.vlCountOTsOpen,	
                            pd.vlCountOTsClosed,	
                            pd.vlCountOTsClosedDelay,	                            
                            pd.cdPlant,
                            pd.vlCountOTsVeryHigh, 
                            pd.vlCountOTsHigh, 
                            pd.vlCountNotesVeryHigh, 
                            pd.vlCountNotesHigh,    
                            pd.vlCountOTsOther,
                            pd.vlCountNotesOther,
                            cast(isnull(pd.vlHoursVeryHigh,0) as int) as vlHoursVeryHigh,
                            cast(isnull(pd.vlHoursHigh,0) as int) as vlHoursHigh,
                            cast(isnull(pd.vlHoursOther,0) as int) as vlHoursOther,
                           (Select bp.vlGoal from [Plant].[BudgetPlant] bp where bp.cdBudget = pd.cdBudget and bp.cdPlant = pd.cdPlant and bp.cdPredictive = pd.cdPredictive) as vlGoal,                                                                             
	                        b.*,
                            p.*,	                        
	                        i.*                            
                        from [Plant].[PlantData] pd
                        inner join [Plant].[Budget] b on b.idBudget = pd.cdBudget                      
                        inner join [Plant].[Plant] p on p.idPlant = pd.cdPlant                        
                        inner join [Common].[Indicator] i on i.idIndicator = pd.cdIndicator                                          
                        where pd.cdIndicator = @idIndicator and b.Active = 1 and  YEAR(b.dateStart) between 
                              (Select Year(dateStart) from [Plant].[Budget] where idBudget = @idBudget)-5 and
                              (Select Year(dateStart) from [Plant].[Budget] where idBudget = @idBudget) and cdPredictive = @cdPredictive and p.Sheet in @plants",
                        (pd, b, p, i) =>
                        {
                            PlantData plantData;

                            if (!lookup.TryGetValue(pd.idPlantData, out plantData))
                            {

                                lookup.Add(pd.idPlantData, plantData = pd);

                            }


                            plantData.Budget = b;
                            plantData.Indicator = i;
                            plantData.Plant = p;


                            return plantData;

                        },
                        new { idIndicator = idIndicator, idBudget = idBudget, cdPredictive = cdPredictive, plants = plants },
                        splitOn: "idPlantData,idBudget,idPlant,idIndicator"
                        ).AsQueryable();


                    return lookup.Values.Cast<PlantData>().ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public List<PlantData> GetPlantDataByDateRange(string Plant, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var list = new List<PlantData>();

                    sqlConn.Query<PlantData, Budget, PlantData>(@"
                       exec  [Plant].[SPGetPlantsData] @Plant , @dateStart, @dateEnd",
                       (t, b) =>
                       {
                           PlantData plantData;

                           plantData = t;

                           list.Add(plantData);

                           plantData.Budget = b;

                           return plantData;

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

        public object InsertPlantData(List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate)
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
                        var message = "";

                        foreach (string plant in plantsGroups)
                        {
                            DeletePlantData(sqlConn, plant, typeDocs, startdate, endDate);

                            foreach (var data in GetPlantDataByDateRange(plant, startdate, endDate))
                            {

                                sqlConn.Execute(@"
                                        Insert into [Plant].[PlantData] (cdBudget, cdPlant, dtMonth, dtYear, vlCountNotesPending, vlCountNotesPendingDelay, vlCountNotesClosed, vlCountOTsPending, vlCountOTsPendingOpen, vlCountOTsPendingReleased, vlCountNotesOpen, vlCountOTsOpen, vlCountOTsClosed, vlCountOTsClosedDelay, cdIndicator, InsDateTime, cdUser, vlCountOTsVeryHigh, vlCountOTsHigh, vlCountNotesVeryHigh, vlCountNotesHigh, vlCountOTsOther, vlCountNotesOther, vlHoursVeryHigh, vlHoursHigh, vlHoursOther, cdPredictive) 
                                                                values (@cdBudget, @cdPlant, @dtMonth, @dtYear, @vlCountNotesPending, @vlCountNotesPendingDelay, @vlCountNotesClosed, @vlCountOTsPending, @vlCountOTsPendingOpen, @vlCountOTsPendingReleased, @vlCountNotesOpen, @vlCountOTsOpen, @vlCountOTsClosed, @vlCountOTsClosedDelay, @cdIndicator, @InsDateTime, @cdUser, @vlCountOTsVeryHigh, @vlCountOTsHigh, @vlCountNotesVeryHigh, @vlCountNotesHigh, @vlCountOTsOther, @vlCountNotesOther, @vlHoursVeryHigh, @vlHoursHigh, @vlHoursOther, @cdPredictive) ",
                                new
                                {
                                    cdBudget = data.cdbudget,
                                    cdPlant = data.cdPlant,
                                    dtMonth = data.dtMonth,
                                    dtYear = data.dtYear,
                                    vlCountNotesPending = data.vlCountNotesPending,
                                    vlCountNotesPendingDelay = data.vlCountNotesPendingDelay,
                                    vlCountNotesClosed = data.vlCountNotesClosed,
                                    vlCountOTsPending = data.vlCountOTsPending,
                                    vlCountOTsPendingOpen = data.vlCountOTsPendingOpen,
                                    vlCountOTsPendingReleased = data.vlCountOTsPendingReleased,
                                    vlCountNotesOpen = data.vlCountNotesOpen,
                                    vlCountOTsOpen = data.vlCountOTsOpen,
                                    vlCountOTsClosed = data.vlCountOTsClosed,
                                    vlCountOTsClosedDelay = data.vlCountOTsClosedDelay,
                                    cdIndicator = data.cdIndicator,
                                    InsDateTime = data.InsDateTime,
                                    cdUser = data.cdUser,
                                    vlCountOTsVeryHigh = data.vlCountOTsVeryHigh,
                                    vlCountOTsHigh = data.vlCountOTsHigh,
                                    vlCountNotesVeryHigh = data.vlCountNotesVeryHigh,
                                    vlCountNotesHigh = data.vlCountNotesHigh,
                                    vlCountOTsOther = data.vlCountOTsOther,
                                    vlCountNotesOther = data.vlCountNotesOther,
                                    vlHoursVeryHigh = data.vlHoursVeryHigh,
                                    vlHoursHigh = data.vlHoursHigh,
                                    vlHoursOther = data.vlHoursOther,
                                    cdPredictive = data.cdPredictive

                                });


                            }
                        }

                        var repoEquipData = new EquipmentDataRepositorie();
                        var result = repoEquipData.InsertEquipmentsData(sqlConn, plants, typeDocs, plantsGroups, startdate, endDate, ref message);

                        if (result)
                        {
                            transactionScope.Complete();
                            resultObj = new { status = true, message = "Dados inseridos com sucesso !" };
                        }
                        else
                            resultObj = new { id = -1, status = false, message = message };

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
      
        private void UpdatePlantData(SqlConnection sqlConn, PlantData plantData)
        {

            try
            {
                sqlConn.Execute(@"
                        update [Plant].[PlantData] set 
                                                        vlCountNotesPending= @vlCountNotesPending, 
                                                        vlCountNotesPendingDelay= @vlCountNotesPendingDelay, 
                                                        vlCountNotesClosed = @vlCountNotesClosed, 
                                                        vlCountOTsPending = @vlCountOTsPending, 
                                                        vlCountOTsPendingOpen = @vlCountOTsPendingOpen, 
                                                        vlCountOTsPendingOReleased = @vlCountOTsPendingReleased, 
                                                        vlCountNotesOpen = @vlCountNotesOpen, 
                                                        vlCountOTsOpen = @vlCountOTsOpen , 
                                                        vlCountOTsClosed = @vlCountOTsClosed, 
                                                        vlCountOTsClosedDelay = @vlCountOTsClosedDelay,
                                                        UpdDateTime = @UpdDateTime, 
                                                        cdUser = @cdUser, 
                                                        vlCountOTsVeryHigh = @vlCountOTsVeryHigh, 
                                                        vlCountOTsHigh = @vlCountOTsHigh, 
                                                        vlCountNotesVeryHigh = @vlCountNotesVeryHigh, 
                                                        vlCountNotesHigh = @vlCountNotesHigh, 
                                                        vlCountOTsOther = @vlCountOTsOther, 
                                                        vlCountNotesOther = @vlCountNotesOther, 
                                                        vlHoursVeryHigh = @vlHoursVeryHigh, 
                                                        vlHoursHigh = @vlHoursHigh, 
                                                        vlHoursOther = @vlHoursOther
                            where  cdPlant = @cdPlant and cdIndicator = @cdIndicator and  cdBudget = @cdBudget and dtMonth = @dtMonth and dtYear = @dtYear and cdPredictive = @cdPredictive",
                new
                {
                    cdBudget = plantData.cdbudget,
                    cdPlant = plantData.cdPlant,
                    dtMonth = plantData.dtMonth,
                    dtYear = plantData.dtYear,
                    vlCountNotesPending = plantData.vlCountNotesPending,
                    vlCountNotesPendingDelay = plantData.vlCountNotesPendingDelay,
                    vlCountNotesClosed = plantData.vlCountNotesClosed,
                    vlCountOTsPending = plantData.vlCountOTsPending,
                    vlCountOTsPendingOpen = plantData.vlCountOTsPendingOpen,
                    vlCountOTsPendingReleased = plantData.vlCountOTsPendingReleased,
                    vlCountNotesOpen = plantData.vlCountNotesOpen,
                    vlCountOTsOpen = plantData.vlCountOTsOpen,
                    vlCountOTsClosed = plantData.vlCountOTsClosed,
                    vlCountOTsClosedDelay = plantData.vlCountOTsClosedDelay,
                    cdIndicator = plantData.cdIndicator,
                    UpdDateTime = plantData.UpdDateTime,
                    cdUser = plantData.cdUser,
                    vlCountOTsVeryHigh = plantData.vlCountOTsVeryHigh,
                    vlCountOTsHigh = plantData.vlCountOTsHigh,
                    vlCountNotesVeryHigh = plantData.vlCountNotesVeryHigh,
                    vlCountNotesHigh = plantData.vlCountNotesHigh,
                    vlCountOTsOther = plantData.vlCountOTsOther,
                    vlCountNotesOther = plantData.vlCountNotesOther,
                    vlHoursVeryHigh = plantData.vlHoursVeryHigh,
                    vlHoursHigh = plantData.vlHoursHigh,
                    vlHoursOther = plantData.vlHoursOther,
                    cdPredictive = plantData.cdPredictive
                });


            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }


        }

        public void DeletePlantData(SqlConnection sqlConn, string plant, List<string> typeDocs, DateTime dateStart, DateTime dateEnd)
        {
            try
            {

                foreach (var doc in typeDocs)
                {
                    sqlConn.Execute(@"                       
                        delete pd FROM [Plant].[PlantData] pd
                        inner join 	[Plant].[Plant] p on p.idPlant = pd.cdPlant                                              
                        inner join  [Common].[Indicator] i on i.idIndicator = pd.cdIndicator
                        inner join  [Common].[TypeDoc] t on t.cdIndicator = i.idIndicator
                        WHERE ((pd.DTMONTH = @monthStart AND pd.DTYEAR= @yearStart) OR (pd.DTMONTH = @monthEnd AND pd.DTYEAR= @yearEnd)) AND p.Sheet = @plant
                        AND t.Sheet in (@typeDoc)",
                        new
                        {
                            monthStart = dateStart.Month,
                            yearStart = dateStart.Year,
                            monthEnd = dateEnd.Month,
                            yearEnd = dateEnd.Year,
                            plant = plant,
                            typeDoc = doc

                        });
                }


            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public List<PlantData> GetPlantDataFailured(int idIndicator, int idBudget)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, PlantData>();

                    sqlConn.Query<PlantData, Budget, Plant, Indicator, PlantData>(@"
                       select 
                            pd.idPlantData,
	                        pd.cdBudget,	
	                        pd.cdIndicator,		                        
	                        pd.dtMonth,
	                        pd.dtYear,
	                        Right('00' + CAST(pd.dtMonth AS VARCHAR(4)),2) + '/' +CAST(pd.dtYear AS VARCHAR(4)) as monthYear,
	                        pd.vlCountNotesPending,	
                            pd.vlCountNotesPendingDelay,	
                            pd.vlCountNotesClosed,	
                            pd.vlCountOTsPending,	
                            pd.vlCountOTsPendingOpen,
                            pd.vlCountOTsPendingReleased,
                            pd.vlCountNotesOpen,	
                            pd.vlCountOTsOpen,	
                            pd.vlCountOTsClosed,	
                            pd.vlCountOTsClosedDelay,	                            
                            pd.cdPlant,
                            pd.vlCountOTsVeryHigh, 
                            pd.vlCountOTsHigh, 
                            pd.vlCountNotesVeryHigh, 
                            pd.vlCountNotesHigh,    
                            pd.vlCountOTsOther,
                            pd.vlCountNotesOther,
                            cast(isnull(pd.vlHoursVeryHigh,0) as int) as vlHoursVeryHigh,
                            cast(isnull(pd.vlHoursHigh,0) as int) as vlHoursHigh,
                            cast(isnull(pd.vlHoursOther,0) as int) as vlHoursOther,
                            (Select bp.vlGoal from [Plant].[BudgetPlant] bp where bp.cdBudget = pd.cdBudget and bp.cdPlant = pd.cdPlant and bp.cdPredictive = pd.cdPredictive) as vlGoal,                                          
	                        b.*,
                            p.*,	                        
	                        i.*                            
                        from [Plant].[PlantData] pd
                        inner join [Plant].[Budget] b on b.idBudget = pd.cdBudget                      
                        inner join [Plant].[Plant] p on p.idPlant = pd.cdPlant                        
                        inner join [Common].[Indicator] i on i.idIndicator = pd.cdIndicator                                          
                        where pd.cdIndicator = @idIndicator and b.Active = 1 and  YEAR(b.dateStart) between 
                              (Select Year(dateStart) from [Plant].[Budget] where idBudget = @idBudget)-5 and
                              (Select Year(dateStart) from [Plant].[Budget] where idBudget = @idBudget)",
                        (pd, b, p, i) =>
                        {
                            PlantData plantData;

                            if (!lookup.TryGetValue(pd.idPlantData, out plantData))
                            {

                                lookup.Add(pd.idPlantData, plantData = pd);

                            }


                            plantData.Budget = b;
                            plantData.Indicator = i;
                            plantData.Plant = p;


                            return plantData;

                        },
                        new { idIndicator = idIndicator, idBudget = idBudget},
                        splitOn: "idPlantData,idBudget,idPlant,idIndicator"
                        ).AsQueryable();


                    return lookup.Values.Cast<PlantData>().ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

    }
}
