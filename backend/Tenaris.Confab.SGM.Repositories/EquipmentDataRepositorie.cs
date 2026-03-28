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
    public class EquipmentDataRepositorie : IEquipmentDataRepositorie
    {

        public List<EquipmentData> GetEquipmentData(int idIndicator, int idBudget)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, EquipmentData>();

                    sqlConn.Query<EquipmentData, Budget, EquipmentPlantGroup, PlantGroup, Indicator, EquipmentData>(@"
                       select 
                            ed.idEquipmentData,
	                        ed.cdBudget,	
	                        ed.cdIndicator,		                        
	                        ed.dtMonth,
	                        ed.dtYear,                            
	                        Right('00' + CAST(ed.dtMonth AS VARCHAR(4)),2) + '/' +CAST(ed.dtYear AS VARCHAR(4)) as monthYear,
	                        ed.vlCountNotesPending,	
                            ed.vlCountOTsPending,	
                            ed.vlCountNotesOpen,	
                            ed.vlCountOTsOpen,	
                            ed.vlCountOTsClosed,	                            
                            ed.cdEquipmentPlantGroup,
                            ed.vlCountOTsVeryHigh, 
                            ed.vlCountOTsHigh, 
                            ed.vlCountNotesVeryHigh, 
                            ed.vlCountNotesHigh,                             
                            (Case ed.cdIndicator	
                                when 3 /*OT*/ then (Select bp.vlGoal from [Plant].[BudgetPlant] bp where bp.cdBudget = ed.cdBudget and bp.cdPlant = ppg.cdPlant and bp.cdPredictive = 0)	                               
                                else 0
                            end) as vlGoal,                                          
	                        b.*,
                            eq.*,
                            pg.*,	                        
	                        i.*                            
                        from [Plant].[EquipmentData] ed
                        inner join [Plant].[Budget] b on b.idBudget = ed.cdBudget                      
                        inner join [Plant].[EquipmentPlantGroup] eq on eq.idEquipmentPlantGroup = ed.cdEquipmentPlantGroup  
                        inner join [Plant].[PlantGroup] pg on pg.idPlantGroup = eq.cdPlantGroup                        
                        inner join [Plant].[PlantPlantGroup] ppg on ppg.cdPlantGroup = pg.idPlantGroup 
                        inner join [Common].[Indicator] i on i.idIndicator = ed.cdIndicator                                          
                        where ed.cdIndicator = @idIndicator and b.Active = 1 and  YEAR(b.dateStart) between 
                              (Select Year(dateStart) from [Plant].[Budget] where idBudget = @idBudget)-5 and
                              (Select Year(dateStart) from [Plant].[Budget] where idBudget = @idBudget)",
                        (ed, b, eq, pg, i) =>
                        {
                            EquipmentData equipmentData;

                            if (!lookup.TryGetValue(ed.idEquipmentData, out equipmentData))
                            {

                                lookup.Add(ed.idEquipmentData, equipmentData = ed);

                            }


                            equipmentData.Budget = b;
                            equipmentData.Indicator = i;
                            equipmentData.EquipmentPlantGroup = eq;
                            equipmentData.EquipmentPlantGroup.PlantGroup = pg;


                            return equipmentData;

                        },
                        new { idIndicator = idIndicator, idBudget = idBudget },
                        splitOn: "idEquipmentData,idBudget,idEquipmentPlantGroup,idPlantGroup,idIndicator"
                        ).AsQueryable();


                    return lookup.Values.Cast<EquipmentData>().ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public List<EquipmentData> GetEquipmentDataByDateRange(string Plant, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var list = new List<EquipmentData>();

                    sqlConn.Query<EquipmentData, Budget, EquipmentData>(@"
                       exec  [Plant].[SPGetEquipmentsData] @Plant , @dateStart, @dateEnd",
                       (t, b) =>
                       {
                           EquipmentData equipmentData;

                           equipmentData = t;

                           list.Add(equipmentData);

                           equipmentData.Budget = b;

                           return equipmentData;

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


        public bool InsertEquipmentsData(SqlConnection sqlConn, List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate, ref string message)
        {

            try
            {
                DeleteEquipmentData(sqlConn, plantsGroups, typeDocs, startdate, endDate);

                //Processa Equipment Data para todas as plantas
                foreach (string plant in plantsGroups)
                {
                   

                    foreach (var data in GetEquipmentDataByDateRange(plant, startdate, endDate))
                    {

                        sqlConn.Execute(@"
                                        Insert into [Plant].[EquipmentData] (cdBudget, cdEquipmentPlantGroup, dtMonth, dtYear, vlCountNotesPending, vlCountOTsPending, vlCountNotesOpen, vlCountOTsOpen, vlCountOTsClosed, cdIndicator, InsDateTime, cdUser, vlCountOTsVeryHigh, vlCountOTsHigh, vlCountNotesVeryHigh, vlCountNotesHigh) 
                                        values (@cdBudget, @cdEquipmentPlantGroup, @dtMonth, @dtYear, @vlCountNotesPending, @vlCountOTsPending, @vlCountNotesOpen, @vlCountOTsOpen, @vlCountOTsClosed, @cdIndicator, @InsDateTime, @cdUser, @vlCountOTsVeryHigh, @vlCountOTsHigh, @vlCountNotesVeryHigh, @vlCountNotesHigh) ",
                        new
                        {
                            cdBudget = data.cdbudget,
                            cdEquipmentPlantGroup = data.cdEquipmentPlantGroup,
                            dtMonth = data.dtMonth,
                            dtYear = data.dtYear,
                            vlCountNotesPending = data.vlCountNotesPending,
                            vlCountOTsPending = data.vlCountOTsPending,
                            vlCountNotesOpen = data.vlCountNotesOpen,
                            vlCountOTsOpen = data.vlCountOTsOpen,
                            vlCountOTsClosed = data.vlCountOTsClosed,
                            cdIndicator = data.cdIndicator,
                            InsDateTime = data.InsDateTime,
                            cdUser = data.cdUser,
                            vlCountOTsVeryHigh = data.vlCountOTsVeryHigh,
                            vlCountOTsHigh = data.vlCountOTsHigh,
                            vlCountNotesVeryHigh = data.vlCountNotesVeryHigh,
                            vlCountNotesHigh = data.vlCountNotesHigh

                        });

                    }
                }



                return true;
            }

            catch (Exception ex)
            {
                message = ex.Message;

                return false;
            }


        }


        public void DeleteEquipmentData(SqlConnection sqlConn, List<string> plants, List<string> typeDocs, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
               
                    sqlConn.Execute(@"                       
                        delete ed FROM [Plant].[EquipmentData] ed
                        inner join 	[Plant].[EquipmentPlantGroup] eq on eq.idEquipmentPlantGroup = ed.cdEquipmentPlantGroup                                                                 
                        inner join [Plant].[PlantPlantGroup] ppg on ppg.cdPlantGroup = eq.cdPlantGroup 
                        inner join [Plant].[Plant] p ON p.idPlant = ppg.cdPlant
                        inner join  [Common].[Indicator] i on i.idIndicator = ed.cdIndicator
                        inner join  [Common].[TypeDoc] t on t.cdIndicator = i.idIndicator
                        WHERE ((ed.DTMONTH = @monthStart AND ed.DTYEAR= @yearStart) OR (ed.DTMONTH = @monthEnd AND ed.DTYEAR= @yearEnd)) AND p.Sheet in @plants
                        AND t.Sheet in @typeDocs",
                        new
                        {
                            monthStart = dateStart.Month,
                            yearStart = dateStart.Year,
                            monthEnd = dateEnd.Month,
                            yearEnd = dateEnd.Year,
                            plants = plants,
                            typeDocs = typeDocs

                        });
                


            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

    }
}
