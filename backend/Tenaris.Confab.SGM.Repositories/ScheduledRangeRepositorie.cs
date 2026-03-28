using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Tenaris.Confab.SGM.Domain.Entities;
using System.Data.SqlClient;
using Tenaris.Confab.SGM.Domain;
using Tenaris.Confab.SAP.Connector;
using System.Data;
using System.Transactions;

namespace Tenaris.Confab.SGM.Repositories
{
    public class ScheduledRangeRepositorie : IScheduledRangeRepositorie
    {
        public List<ScheduledRange> GetScheduledRangeByStatus(int idStatus, int idScheduledType)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, ScheduledRange>();

                    sqlConn.Query<ScheduledRange, Status, ScheduledRange>(@"
                       select 
                            sq.*,
                            s.*
                        from [Common].[ScheduledRange] sq
                        inner join [Common].[Status] s on s.idStatus = sq.cdStatus                        
                        where sq.cdStatus = @idStatus and sq.cdScheduledType = @idScheduledType",
                        (sq, s) =>
                        {
                            ScheduledRange scheduled;

                            if (!lookup.TryGetValue(sq.idScheduledRange, out scheduled))
                            {
                                lookup.Add(sq.idScheduledRange, scheduled = sq);
                            }

                            scheduled.Status = s;

                            return scheduled;

                        },
                        new { idStatus = idStatus, idScheduledType = idScheduledType },
                        splitOn: "idStatus"
                        ).AsQueryable();


                    return lookup.Values.Cast<ScheduledRange>().ToList();

                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        public List<ScheduledRange> GetScheduledRangeByDate(DateTime startDate, DateTime endDate, int idScheduledType)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, ScheduledRange>();

                    sqlConn.Query<ScheduledRange, Status, ScheduledRange>(@"
                       select 
                            sq.*,
                            s.*
                        from [Common].[ScheduledRange] sq
                        inner join [Common].[Status] s on s.idStatus = sq.cdStatus                        
                        where sq.startDate = @startDate and sq.endDate = @endDate and sq.cdScheduledType = @idScheduledType",
                        (sq, s) =>
                        {
                            ScheduledRange scheduled;

                            if (!lookup.TryGetValue(sq.idScheduledRange, out scheduled))
                            {
                                lookup.Add(sq.idScheduledRange, scheduled = sq);
                            }

                            scheduled.Status = s;

                            return scheduled;

                        },
                        new { startDate = startDate, endDate = endDate, idScheduledType = idScheduledType },
                        splitOn: "idStatus"
                        ).AsQueryable();


                    return lookup.Values.Cast<ScheduledRange>().ToList();

                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        public object InsertScheduledRange(ScheduledRange scheduledRange)
        {

            try
            {

                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {


                    var id = sqlConn.Query<int>(@"
                                Insert into [Common].[ScheduledRange] (StartDate, EndDate, cdStatus, [Message], InsDateTime, PlantGroups, TypeDocs, cdScheduledType) 
                                    values (@StartDate, @EndDate, @cdStatus, @Message, @InsDateTime, @PlantGroups, @TypeDocs, @cdScheduledType);
                                select cast(SCOPE_IDENTITY() as int)",
                        new
                        {
                            StartDate = scheduledRange.startdate,
                            EndDate = scheduledRange.endDate,
                            cdStatus = scheduledRange.cdStatus,
                            Message = scheduledRange.Message,
                            InsDateTime = DateTime.Now,
                            PlantGroups = scheduledRange.PlantGroups,
                            TypeDocs = scheduledRange.TypeDocs,
                            cdScheduledType = scheduledRange.cdScheduledType

                        }).Single();


                    return new { id = id, status = true, message = "Dados inseridos com sucesso !" };
                    

                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }


        }

        public object UpdateScheduledRange(ScheduledRange scheduledRange)
        {

            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Execute(@"
                        update [Common].[ScheduledRange] set                                  
                                cdStatus = @cdStatus, 
                                [Message] = @Message, 
                                UpdDateTime = @UpdDateTime                                                            
                        where idScheduledRange = @idScheduledRange and cdScheduledType = @cdScheduledType",
                         new
                         {
                             cdStatus = scheduledRange.cdStatus,
                             Message = scheduledRange.Message,
                             UpdDateTime = DateTime.Now,
                             idScheduledRange = scheduledRange.idScheduledRange ,
                             cdScheduledType = scheduledRange.cdScheduledType

                         });
                }

                return new { id = scheduledRange.idScheduledRange, status = true, message = "Dados atualizados com sucesso !" };
                

            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }


        }
    }
}
