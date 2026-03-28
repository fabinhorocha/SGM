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
    public class LocationDataRepositorie : ILocationDataRepositorie
    {

        public List<LocationData> GetLocationData(int idIndicator, int idBudget)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, LocationData>();

                    sqlConn.Query<LocationData, Budget, LocationPlant, Location, Indicator, Plant, Speciality, LocationData>(@"
                       select 
                            ld.idLocationData,
	                        ld.cdBudget,	
	                        ld.cdIndicator,		                        
	                        ld.dtMonth,
	                        ld.dtYear,
	                        Right('00' + CAST(ld.dtMonth AS VARCHAR(4)),2) + '/' +CAST(ld.dtYear AS VARCHAR(4)) as monthYear,
	                        ld.vlCount,	
                            ld.vlExec,	
                            ld.vlNExec,
                            ld.cdLocationPlant,
                            (Case ld.cdIndicator	
                                when 1 /*Preventiva*/ then (Select bd.vlGoal from [Plant].[BudgetLocation] bd where bd.cdBudget = ld.cdBudget and bd.cdLocationPlant = lp.idLocationPlant)	
                                when 2 /*Preditiva*/ then (Select bd.vlGoal from [Plant].[BudgetLocation] bd where bd.cdBudget = ld.cdBudget and bd.cdLocationPlant = lp.idLocationPlant)                              
                                else 0
                            end) as vlGoal,
                            ld.cdSpeciality,  
                            ld.cdLocationLocal,                      
	                        b.*,
                            lp.*,
	                        l.*,                            
	                        i.*,
                            p.*,
                            s.*		
                        from [Plant].[LocationData] ld
                        inner join [Plant].[Budget] b on b.idBudget = ld.cdBudget                      
                        inner join [Plant].[LocationPlant] lp on lp.idLocationPlant = ld.cdLocationPlant
                        inner join [Plant].[Location] l on l.idLocation = lp.cdLocation
                        inner join [Common].[Indicator] i on i.idIndicator = ld.cdIndicator
                        inner join [Plant].[Plant] p on p.idPlant = lp.cdPlant
                        left join [Common].[Speciality] s on s.idSpeciality = ld.cdSpeciality
                        where ld.cdIndicator = @idIndicator and b.Active = 1 and  YEAR(b.dateStart) between 
                              (Select Year(dateStart) from [Plant].[Budget] where idBudget = @idBudget)-5 and
                              (Select Year(dateStart) from [Plant].[Budget] where idBudget = @idBudget)",
                        (ld, b, lp, l, i, p, s) =>
                        {
                            LocationData locationData;

                            if (!lookup.TryGetValue(ld.idLocationData, out locationData))
                            {

                                lookup.Add(ld.idLocationData, locationData = ld);

                            }


                            locationData.Budget = b;
                            locationData.Indicator = i;
                            locationData.LocationPlant = lp;
                            locationData.LocationPlant.Location = l;
                            locationData.LocationPlant.Plant = p;


                            locationData.speciality = s;

                            return locationData;

                        },
                        new { idIndicator = idIndicator, idBudget = idBudget },
                        splitOn: "idLocationData,idBudget,idLocationPlant,idLocation,idIndicator,idPlant,idSpeciality"
                        ).AsQueryable();


                    return lookup.Values.Cast<LocationData>().ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public List<LocationData> GetLocationDataByDateRange(string Plant, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var list = new List<LocationData>();

                    sqlConn.Query<LocationData, Budget, LocationData>(@"
                       exec  [Plant].[SPGetLocations] @Plant , @dateStart, @dateEnd",
                       (t, b) =>
                       {
                           LocationData locationData;

                           locationData = t;

                           list.Add(locationData);

                           locationData.Budget = b;

                           return locationData;

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

        public object InsertLocationData(List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate)
        {

            var resultObj = new object();

            try
            {
                var option = new TransactionOptions();
                option.IsolationLevel = IsolationLevel.ReadCommitted;
                option.Timeout = TimeSpan.FromMinutes(10);

                using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, option))
                {
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                      
                        //Processa Location Data para todas as plantas
                        foreach (string plant in plantsGroups)
                        {

                            DeleteLocationData(sqlConn, plant, typeDocs, startdate, endDate);

                            foreach (var data in GetLocationDataByDateRange(plant, startdate, endDate))
                            {


                                sqlConn.Execute(@"
                                        Insert into [Plant].[LocationData] (cdBudget, cdLocationPlant, dtMonth, dtYear, vlCount, vlExec, vlNExec, cdIndicator, InsDateTime, cdUser, cdSpeciality, cdLocationLocal) 
                                        values (@cdBudget, @cdLocationPlant, @dtMonth, @dtYear, @vlCount, @vlExec, @vlNExec, @cdIndicator, @InsDateTime, @cdUser, @cdSpeciality, @cdLocationLocal) ",
                                new
                                {
                                    cdBudget = data.cdbudget,
                                    cdLocationPlant = data.cdLocationPlant,
                                    dtMonth = data.dtMonth,
                                    dtYear = data.dtYear,
                                    vlCount = data.vlCount,
                                    vlExec = data.vlExec,
                                    vlNExec = data.vlNExec,
                                    cdIndicator = data.cdIndicator,
                                    InsDateTime = data.InsDateTime,
                                    cdUser = data.cdUser,
                                    cdSpeciality = data.cdSpeciality,
                                    cdLocationLocal = data.cdLocationLocal
                                });




                            }
                        }


                        transactionScope.Complete();

                        resultObj = new { status = true, message = "Dados processados com sucesso !" };



                    }

                    return resultObj;
                }
            }

            catch (Exception ex)
            {
                resultObj = new { id = -1, status = false, message = ex.Message };

                return resultObj;
            }


        }

        private void UpdateLocationData(SqlConnection sqlConn, LocationData locationData)
        {

            try
            {
                sqlConn.Execute(@"
                        update [Plant].[LocationData] set vlCount= @vlCount, vlExec = @vlExec, vlNExec = @vlNExec, UpdDateTime = @UpdDateTime, cdUser = @cdUser 
                            where  cdLocationPlant = @cdLocationPlant and cdIndicator = @cdIndicator and  cdBudget = @cdBudget and dtMonth = @dtMonth and dtYear = @dtYear and cdSpeciality = @cdSpeciality",
                new
                {
                    cdBudget = locationData.cdbudget,
                    cdLocation = locationData.cdLocationPlant,
                    dtMonth = locationData.dtMonth,
                    dtYear = locationData.dtYear,
                    vlCount = locationData.vlCount,
                    vlExec = locationData.vlExec,
                    vlNExec = locationData.vlNExec,
                    cdIndicator = locationData.cdIndicator,
                    UpdDateTime = locationData.UpdDateTime,
                    cdUser = locationData.cdUser,
                    cdSpeciality = locationData.cdSpeciality

                });


            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }


        }

        public void DeleteLocationData(SqlConnection sqlConn, string plant, List<string> typeDocs, DateTime dateStart, DateTime dateEnd)
        {
            try
            {

                foreach (var doc in typeDocs)
                {
                    sqlConn.Execute(@"                       
                        delete ld FROM [Plant].[LocationData] ld
                        inner join 	[Plant].[LocationPlant] lp on lp.idLocationPlant = ld.cdLocationPlant                      
                        inner join 	[Plant].[Plant] p on p.idPlant= lp.cdPlant
                        inner join  [Common].[Indicator] i on i.idIndicator = ld.cdIndicator
                        inner join  [Common].[TypeDoc] t on t.cdIndicator = i.idIndicator
                        WHERE ((ld.DTMONTH = @monthStart AND ld.DTYEAR= @yearStart) OR (ld.DTMONTH = @monthEnd AND ld.DTYEAR= @yearEnd)) AND p.Sheet = @plant
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

    }
}
