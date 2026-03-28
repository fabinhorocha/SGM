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
    public class OrderHeadRepositorie : IOrderHeadRepositorie
    {


        public List<OrderHead> GetOrderHeadSAP(List<string> planPlants, List<string> typeDocs, List<string> plantGroups, DateTime dateStart, DateTime dateEnd, ScheduledRangeType scheduleType)
        {
            try
            {

                var listOrderHead = new List<OrderHead>();

                var funcSAP =
                   new FunctionsSap(
                       Properties.Settings.Default.Name,
                       Properties.Settings.Default.Sap_Host,
                       Properties.Settings.Default.Sap_System,
                       Properties.Settings.Default.Sap_Client,
                       Properties.Settings.Default.Sap_User,
                       Properties.Settings.Default.Sap_Password,
                       Properties.Settings.Default.Sap_Language,
                       Properties.Settings.Default.Sap_PoolSize);



                var message = "";


                switch (scheduleType)
                {
                    case ScheduledRangeType.OrderSAP:
                        var dsOrder = funcSAP.BAPI_ALM_ORDERHEADER_GETLIST(planPlants, typeDocs, plantGroups, dateStart, dateEnd, ref message);
                        FillOrders(dsOrder, "", ref listOrderHead);
                        break;
                    case ScheduledRangeType.OtSAP:
                        //Retorna Ordens pendentes            
                        var dsOT = funcSAP.BAPI_ALM_ORDERHEADER_GETLIST_OT(planPlants, typeDocs, plantGroups, dateStart, dateEnd, false, ref message);
                        FillOrders(dsOT, "S_STATUS NOT LIKE '%ENCE%' AND S_STATUS NOT LIKE '%ENTE%'", ref listOrderHead);

                        //Retorna ordens encerradas
                        dsOT = funcSAP.BAPI_ALM_ORDERHEADER_GETLIST_OT(planPlants, typeDocs, plantGroups, dateStart, dateEnd, true, ref message);
                        FillOrders(dsOT, "S_STATUS LIKE '%ENCE%' OR S_STATUS LIKE '%ENTE%'", ref listOrderHead);

                        var dsHoursWorked = funcSAP.BAPI_ALM_ORDEROPER_GETLIST(listOrderHead.Select(s => s.idOrder).ToList(), ref message);
                        SetHoursWorked(dsHoursWorked, ref listOrderHead);

                        break;
                }


                return listOrderHead;

            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        public OrderHead GetOrderHead(SqlConnection sqlConn, string idOrder, string typeOrder)
        {
            try
            {

                return sqlConn.Query<OrderHead>(@"
                        Select o.* 
                        from [Sap].[OrderHead] o where idOrder = @idOrder and [Type] = @Type", new { idOrder = idOrder, Type = typeOrder }).SingleOrDefault();

            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }

        public List<OrderHead> GetOrderHead(string plant, int ? idLocation, int idIndicator, int dtMonth, int dtYear)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, OrderHead>();

                    sqlConn.Query<OrderHead, Speciality, OrderHead>(@"
                       exec  [SAP].[SPGetOrderHead] @Plant , @idLocation, @idIndicator, @dtMonth, @dtYear",
                        (t, s) =>
                        {
                            OrderHead orderHead;

                            if (!lookup.TryGetValue(t.idOrderHead, out orderHead))
                            {

                                lookup.Add(t.idOrderHead, orderHead = t);

                            }


                            orderHead.speciality = s;


                            return orderHead;

                        },
                        new { Plant = plant, idLocation = idLocation, idIndicator = idIndicator, dtMonth = dtMonth, dtYear = dtYear },
                        splitOn: "idOrderhead,idSpeciality"
                        ).AsQueryable();


                    return lookup.Values.Cast<OrderHead>().ToList();

                }

            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        public object InsertOrderHead(List<string> planPlants, List<string> typeDocs, List<string> plantGroups, DateTime dateStart, DateTime dateEnd, ScheduledRangeType scheduleType)
        {
            var resultObj = new object();

            try
            {



                var listOrderHead = GetOrderHeadSAP(planPlants, typeDocs, plantGroups, dateStart, dateEnd, scheduleType);


                var option = new TransactionOptions();
                option.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                option.Timeout = TimeSpan.FromMinutes(30);

                using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, option))
                {
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        DeleteOrderhead(sqlConn, planPlants, typeDocs, plantGroups, dateStart, dateEnd, scheduleType);

                        foreach (var orderHead in listOrderHead)
                        {


                            sqlConn.Execute(@"
                                Insert into [Sap].[OrderHead] (idOrder, [Type], Plant, PlantGroup, CenterCost, [Description], [Status], SheetLocation, Location, TypeActivity, dateStart, dateEnd, dateRef, InsDateTime, UpdDateTime, cdUser, cdPriority, hoursWorked) 
                                    values (@idOrder, @Type, @Plant, @PlantGroup, @CenterCost, @Description, @Status, @SheetLocation, @Location, @TypeActivity, @dateStart, @dateEnd, @dateRef, @InsDateTime, @UpdDateTime, @cdUser, @cdPriority, @hoursWorked) ",
                            new
                            {
                                idOrder = orderHead.idOrder,
                                Type = orderHead.Type,
                                Plant = orderHead.Plant,
                                PlantGroup = orderHead.PlantGroup,
                                CenterCost = orderHead.CenterCost,
                                Description = orderHead.Description,
                                Status = orderHead.Status,
                                SheetLocation = orderHead.SheetLocation,
                                Location = orderHead.Location,
                                TypeActivity = orderHead.TypeActivity,
                                dateStart = orderHead.dateStart,
                                dateEnd = orderHead.dateEnd,
                                dateRef = orderHead.dateRef,
                                InsDateTime = orderHead.InsDateTime,
                                UpdDateTime = orderHead.UpdDateTime,
                                cdUser = orderHead.cdUser,
                                cdPriority = orderHead.cdPriority,
                                hoursWorked = orderHead.hoursWorked
                            });


                        }


                        transactionScope.Complete();
                        resultObj = new { status = true, message = "Dados processados com sucesso !" };

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


        public List<OrderHead> GetOrderHead(List<string> plants, DateTime? dateStart, DateTime dateEnd, TypeStatus status, bool cdPredictive)
        {


            switch (status)
            {

                case TypeStatus.Pending:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                        @"SELECT DISTINCT
                                  [idOrder]
                                  ,[Type]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[CenterCost]
                                  ,[Description]
                                  ,[Status]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[TypeActivity]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[cdPriority]
                                  ,[hoursWorked] 
                            FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                            AND (STATUS not LIKE '%ENCE%' and STATUS not LIKE '%ENTE%') AND PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                            and INSDATETIME <=  @dateEnd and CHARINDEX('PRED',[Description]) = 1" :
                        @"SELECT DISTINCT
                                  [idOrder]
                                  ,[Type]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[CenterCost]
                                  ,[Description]
                                  ,[Status]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[TypeActivity]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[cdPriority]
                                  ,[hoursWorked] 
                            FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                            AND (STATUS not LIKE '%ENCE%' and STATUS not LIKE '%ENTE%') AND PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                            and INSDATETIME <=  @dateEnd";
                        return sqlConn.Query<OrderHead>(sql, 
                        new { plants = plants, dateEnd = dateEnd }).ToList();
                    }

                case TypeStatus.PendingOpen:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                        @"SELECT DISTINCT
                                  [idOrder]
                                  ,[Type]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[CenterCost]
                                  ,[Description]
                                  ,[Status]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[TypeActivity]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[cdPriority]
                                  ,[hoursWorked] 
                            FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                            AND (STATUS not LIKE '%ENCE%' and STATUS not LIKE '%ENTE%') AND PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                            and INSDATETIME <=  @dateEnd and CHARINDEX('PRED',[Description]) = 1 and CHARINDEX('ABER',[STATUS]) = 1" :
                        @"SELECT DISTINCT
                                  [idOrder]
                                  ,[Type]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[CenterCost]
                                  ,[Description]
                                  ,[Status]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[TypeActivity]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[cdPriority]
                                  ,[hoursWorked] 
                            FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                            AND (STATUS not LIKE '%ENCE%' and STATUS not LIKE '%ENTE%') AND PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                            and INSDATETIME <=  @dateEnd and CHARINDEX('ABER',[STATUS]) = 1";
                        return sqlConn.Query<OrderHead>(sql,
                        new { plants = plants, dateEnd = dateEnd }).ToList();
                    }

                case TypeStatus.PendingReleased:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                        @"SELECT DISTINCT
                                  [idOrder]
                                  ,[Type]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[CenterCost]
                                  ,[Description]
                                  ,[Status]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[TypeActivity]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[cdPriority]
                                  ,[hoursWorked] 
                            FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                            AND (STATUS not LIKE '%ENCE%' and STATUS not LIKE '%ENTE%') AND PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                            and INSDATETIME <=  @dateEnd and CHARINDEX('PRED',[Description]) = 1 and CHARINDEX('LIB',[STATUS]) = 1" :
                        @"SELECT DISTINCT
                                  [idOrder]
                                  ,[Type]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[CenterCost]
                                  ,[Description]
                                  ,[Status]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[TypeActivity]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[cdPriority]
                                  ,[hoursWorked] 
                            FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                            AND (STATUS not LIKE '%ENCE%' and STATUS not LIKE '%ENTE%') AND PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                            and INSDATETIME <=  @dateEnd and CHARINDEX('LIB',[STATUS]) = 1";
                        return sqlConn.Query<OrderHead>(sql,
                        new { plants = plants, dateEnd = dateEnd }).ToList();
                    }
                case TypeStatus.PendingOpenEnfa:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                        @"SELECT DISTINCT
                                  [idOrder]
                                  ,[Type]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[CenterCost]
                                  ,[Description]
                                  ,[Status]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[TypeActivity]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[cdPriority]
                                  ,[hoursWorked] 
                            FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                            AND (STATUS not LIKE '%ENCE%' and STATUS not LIKE '%ENTE%') AND PLANTGROUP in @plants
                            AND CENTERCOST LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                            and INSDATETIME <=  @dateEnd and CHARINDEX('PRED',[Description]) = 1 and CHARINDEX('ABER',[STATUS]) = 1" :
                        @"SELECT DISTINCT
                                  [idOrder]
                                  ,[Type]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[CenterCost]
                                  ,[Description]
                                  ,[Status]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[TypeActivity]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[cdPriority]
                                  ,[hoursWorked] 
                            FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                            AND (STATUS not LIKE '%ENCE%' and STATUS not LIKE '%ENTE%') AND PLANTGROUP in @plants
                            AND CENTERCOST LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                            and INSDATETIME <=  @dateEnd and CHARINDEX('ABER',[STATUS]) = 1";
                        return sqlConn.Query<OrderHead>(sql,
                        new { plants = plants, dateEnd = dateEnd }).ToList();
                    }

                case TypeStatus.PendingReleasedEnfa:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                        @"SELECT DISTINCT
                                  [idOrder]
                                  ,[Type]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[CenterCost]
                                  ,[Description]
                                  ,[Status]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[TypeActivity]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[cdPriority]
                                  ,[hoursWorked] 
                            FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                            AND (STATUS not LIKE '%ENCE%' and STATUS not LIKE '%ENTE%') AND PLANTGROUP in @plants
                            AND CENTERCOST LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                            and INSDATETIME <=  @dateEnd and CHARINDEX('PRED',[Description]) = 1 and CHARINDEX('LIB',[STATUS]) = 1" :
                        @"SELECT DISTINCT
                                  [idOrder]
                                  ,[Type]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[CenterCost]
                                  ,[Description]
                                  ,[Status]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[TypeActivity]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[cdPriority]
                                  ,[hoursWorked] 
                            FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                            AND (STATUS not LIKE '%ENCE%' and STATUS not LIKE '%ENTE%') AND PLANTGROUP in @plants
                            AND CENTERCOST LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                            and INSDATETIME <=  @dateEnd and CHARINDEX('LIB',[STATUS]) = 1";
                        return sqlConn.Query<OrderHead>(sql,
                        new { plants = plants, dateEnd = dateEnd }).ToList();
                    }
                case TypeStatus.PendingEnfa:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                         @"SELECT 
                                DISTINCT
                                  [idOrder]
                                  ,[Type]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[CenterCost]
                                  ,[Description]
                                  ,[Status]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[TypeActivity]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[cdPriority]
                                  ,[hoursWorked] 
                            FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                            AND(STATUS not LIKE '%ENCE%' and STATUS not LIKE '%ENTE%')
                            AND CENTERCOST LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                            and INSDATETIME <= @dateEnd and CHARINDEX('PRED',[Description]) = 1" :
                        @"SELECT 
                            DISTINCT
                                [idOrder]
                                ,[Type]
                                ,[Plant]
                                ,[PlantGroup]
                                ,[CenterCost]
                                ,[Description]
                                ,[Status]
                                ,[SheetLocation]
                                ,[Location]
                                ,[TypeActivity]
                                ,[dateStart]
                                ,[dateEnd]
                                ,[dateRef]
                                ,[InsDateTime]
                                ,[UpdDateTime]
                                ,[cdUser]
                                ,[cdPriority]
                                ,[hoursWorked] 
                            FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                            AND(STATUS not LIKE '%ENCE%' and STATUS not LIKE '%ENTE%')
                            AND CENTERCOST LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                            and INSDATETIME <= @dateEnd";
                        return sqlConn.Query<OrderHead>(sql, new { dateEnd = dateEnd }).ToList();
                    }

                case TypeStatus.Open:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                        @"SELECT 
                            DISTINCT
                                    [idOrder]
                                    ,[Type]
                                    ,[Plant]
                                    ,[PlantGroup]
                                    ,[CenterCost]
                                    ,[Description]
                                    ,[Status]
                                    ,[SheetLocation]
                                    ,[Location]
                                    ,[TypeActivity]
                                    ,[dateStart]
                                    ,[dateEnd]
                                    ,[dateRef]
                                    ,[InsDateTime]
                                    ,[UpdDateTime]
                                    ,[cdUser]
                                    ,[cdPriority]
                                    ,[hoursWorked] 
                            FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                            AND  PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                            AND INSDATETIME BETWEEN @dateStart AND @dateEnd and CHARINDEX('PRED',[Description]) = 1" :
                        @"SELECT 
                            DISTINCT
                                    [idOrder]
                                    ,[Type]
                                    ,[Plant]
                                    ,[PlantGroup]
                                    ,[CenterCost]
                                    ,[Description]
                                    ,[Status]
                                    ,[SheetLocation]
                                    ,[Location]
                                    ,[TypeActivity]
                                    ,[dateStart]
                                    ,[dateEnd]
                                    ,[dateRef]
                                    ,[InsDateTime]
                                    ,[UpdDateTime]
                                    ,[cdUser]
                                    ,[cdPriority]
                                    ,[hoursWorked] 
                            FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                            AND  PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                            AND INSDATETIME BETWEEN @dateStart AND @dateEnd";
                        return sqlConn.Query<OrderHead>(sql, new { plants = plants, dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }

                case TypeStatus.OpenEnfa:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                            @"SELECT 
                                DISTINCT
                                        [idOrder]
                                        ,[Type]
                                        ,[Plant]
                                        ,[PlantGroup]
                                        ,[CenterCost]
                                        ,[Description]
                                        ,[Status]
                                        ,[SheetLocation]
                                        ,[Location]
                                        ,[TypeActivity]
                                        ,[dateStart]
                                        ,[dateEnd]
                                        ,[dateRef]
                                        ,[InsDateTime]
                                        ,[UpdDateTime]
                                        ,[cdUser]
                                        ,[cdPriority]
                                        ,[hoursWorked] 
                                FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)                                
                                AND CENTERCOST LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                                AND INSDATETIME BETWEEN @dateStart AND @dateEnd and CHARINDEX('PRED',[Description]) = 1" :
                             @"SELECT 
                                DISTINCT
                                        [idOrder]
                                        ,[Type]
                                        ,[Plant]
                                        ,[PlantGroup]
                                        ,[CenterCost]
                                        ,[Description]
                                        ,[Status]
                                        ,[SheetLocation]
                                        ,[Location]
                                        ,[TypeActivity]
                                        ,[dateStart]
                                        ,[dateEnd]
                                        ,[dateRef]
                                        ,[InsDateTime]
                                        ,[UpdDateTime]
                                        ,[cdUser]
                                        ,[cdPriority]
                                        ,[hoursWorked] 
                                FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)                                
                                AND CENTERCOST LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                                AND INSDATETIME BETWEEN @dateStart AND @dateEnd";

                        return sqlConn.Query<OrderHead>(sql, new { dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }


                case TypeStatus.Closed:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                            @"SELECT 
                                DISTINCT
                                        [idOrder]
                                        ,[Type]
                                        ,[Plant]
                                        ,[PlantGroup]
                                        ,[CenterCost]
                                        ,[Description]
                                        ,[Status]
                                        ,[SheetLocation]
                                        ,[Location]
                                        ,[TypeActivity]
                                        ,[dateStart]
                                        ,[dateEnd]
                                        ,[dateRef]
                                        ,[InsDateTime]
                                        ,[UpdDateTime]
                                        ,[cdUser]
                                        ,[cdPriority]
                                        ,[hoursWorked] 
                                FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                                AND (STATUS LIKE '%ENCE%' OR STATUS LIKE '%ENTE%')                        
                                AND  PLANTGROUP in @plants
                                AND CENTERCOST not LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                                AND DATEREF BETWEEN @dateStart AND @dateEnd and CHARINDEX('PRED',[Description]) = 1" :
                            @"SELECT 
                                DISTINCT
                                        [idOrder]
                                        ,[Type]
                                        ,[Plant]
                                        ,[PlantGroup]
                                        ,[CenterCost]
                                        ,[Description]
                                        ,[Status]
                                        ,[SheetLocation]
                                        ,[Location]
                                        ,[TypeActivity]
                                        ,[dateStart]
                                        ,[dateEnd]
                                        ,[dateRef]
                                        ,[InsDateTime]
                                        ,[UpdDateTime]
                                        ,[cdUser]
                                        ,[cdPriority]
                                        ,[hoursWorked] 
                                FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                                AND (STATUS LIKE '%ENCE%' OR STATUS LIKE '%ENTE%')                        
                                AND  PLANTGROUP in @plants
                                AND CENTERCOST not LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                                AND DATEREF BETWEEN @dateStart AND @dateEnd";


                        return sqlConn.Query<OrderHead>(sql, new { plants = plants, dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }

                case TypeStatus.ClosedOnTime:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                            @"SELECT 
                                DISTINCT
                                        [idOrder]
                                        ,[Type]
                                        ,[Plant]
                                        ,[PlantGroup]
                                        ,[CenterCost]
                                        ,[Description]
                                        ,[Status]
                                        ,[SheetLocation]
                                        ,[Location]
                                        ,[TypeActivity]
                                        ,[dateStart]
                                        ,[dateEnd]
                                        ,[dateRef]
                                        ,[InsDateTime]
                                        ,[UpdDateTime]
                                        ,[cdUser]
                                        ,[cdPriority]
                                        ,[hoursWorked] 
                                FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                                AND (STATUS LIKE '%ENCE%' OR STATUS LIKE '%ENTE%')                        
                                AND  PLANTGROUP in @plants
                                AND CENTERCOST not LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                                AND DATEREF BETWEEN @dateStart AND @dateEnd and CHARINDEX('PRED',[Description]) = 1 AND [dateEnd] >= DATEREF" :
                            @"SELECT 
                                DISTINCT
                                        [idOrder]
                                        ,[Type]
                                        ,[Plant]
                                        ,[PlantGroup]
                                        ,[CenterCost]
                                        ,[Description]
                                        ,[Status]
                                        ,[SheetLocation]
                                        ,[Location]
                                        ,[TypeActivity]
                                        ,[dateStart]
                                        ,[dateEnd]
                                        ,[dateRef]
                                        ,[InsDateTime]
                                        ,[UpdDateTime]
                                        ,[cdUser]
                                        ,[cdPriority]
                                        ,[hoursWorked] 
                                FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                                AND (STATUS LIKE '%ENCE%' OR STATUS LIKE '%ENTE%')                        
                                AND  PLANTGROUP in @plants
                                AND CENTERCOST not LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                                AND DATEREF BETWEEN @dateStart AND @dateEnd AND [dateEnd] >= DATEREF";


                        return sqlConn.Query<OrderHead>(sql, new { plants = plants, dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }

                case TypeStatus.ClosedDelay:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                            @"SELECT 
                                DISTINCT
                                        [idOrder]
                                        ,[Type]
                                        ,[Plant]
                                        ,[PlantGroup]
                                        ,[CenterCost]
                                        ,[Description]
                                        ,[Status]
                                        ,[SheetLocation]
                                        ,[Location]
                                        ,[TypeActivity]
                                        ,[dateStart]
                                        ,[dateEnd]
                                        ,[dateRef]
                                        ,[InsDateTime]
                                        ,[UpdDateTime]
                                        ,[cdUser]
                                        ,[cdPriority]
                                        ,[hoursWorked] 
                                FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                                AND (STATUS LIKE '%ENCE%' OR STATUS LIKE '%ENTE%')                        
                                AND  PLANTGROUP in @plants
                                AND CENTERCOST not LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                                AND DATEREF BETWEEN @dateStart AND @dateEnd and CHARINDEX('PRED',[Description]) = 1 AND [dateEnd] < DATEREF" :
                            @"SELECT 
                                DISTINCT
                                        [idOrder]
                                        ,[Type]
                                        ,[Plant]
                                        ,[PlantGroup]
                                        ,[CenterCost]
                                        ,[Description]
                                        ,[Status]
                                        ,[SheetLocation]
                                        ,[Location]
                                        ,[TypeActivity]
                                        ,[dateStart]
                                        ,[dateEnd]
                                        ,[dateRef]
                                        ,[InsDateTime]
                                        ,[UpdDateTime]
                                        ,[cdUser]
                                        ,[cdPriority]
                                        ,[hoursWorked] 
                                FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                                AND (STATUS LIKE '%ENCE%' OR STATUS LIKE '%ENTE%')                        
                                AND  PLANTGROUP in @plants
                                AND CENTERCOST not LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                                AND DATEREF BETWEEN @dateStart AND @dateEnd AND [dateEnd] < DATEREF";


                        return sqlConn.Query<OrderHead>(sql, new { plants = plants, dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }

                case TypeStatus.ClosedEnfa:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                            @"SELECT 
                                DISTINCT
                                        [idOrder]
                                        ,[Type]
                                        ,[Plant]
                                        ,[PlantGroup]
                                        ,[CenterCost]
                                        ,[Description]
                                        ,[Status]
                                        ,[SheetLocation]
                                        ,[Location]
                                        ,[TypeActivity]
                                        ,[dateStart]
                                        ,[dateEnd]
                                        ,[dateRef]
                                        ,[InsDateTime]
                                        ,[UpdDateTime]
                                        ,[cdUser]
                                        ,[cdPriority]
                                        ,[hoursWorked] 
                                FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                                AND (STATUS LIKE '%ENCE%' OR STATUS LIKE '%ENTE%')                                                        
                                AND CENTERCOST LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                                AND DATEREF BETWEEN @dateStart AND @dateEnd and CHARINDEX('PRED',[Description]) = 1" :
                            @"SELECT 
                                 DISTINCT
                                        [idOrder]
                                        ,[Type]
                                        ,[Plant]
                                        ,[PlantGroup]
                                        ,[CenterCost]
                                        ,[Description]
                                        ,[Status]
                                        ,[SheetLocation]
                                        ,[Location]
                                        ,[TypeActivity]
                                        ,[dateStart]
                                        ,[dateEnd]
                                        ,[dateRef]
                                        ,[InsDateTime]
                                        ,[UpdDateTime]
                                        ,[cdUser]
                                        ,[cdPriority]
                                        ,[hoursWorked] 
                                FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                                AND (STATUS LIKE '%ENCE%' OR STATUS LIKE '%ENTE%')                                                        
                                AND CENTERCOST LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                                AND DATEREF BETWEEN @dateStart AND @dateEnd";

                        return sqlConn.Query<OrderHead>(sql, new { dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }

                case TypeStatus.ClosedOnTimeEnfa:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                            @"SELECT 
                                DISTINCT
                                        [idOrder]
                                        ,[Type]
                                        ,[Plant]
                                        ,[PlantGroup]
                                        ,[CenterCost]
                                        ,[Description]
                                        ,[Status]
                                        ,[SheetLocation]
                                        ,[Location]
                                        ,[TypeActivity]
                                        ,[dateStart]
                                        ,[dateEnd]
                                        ,[dateRef]
                                        ,[InsDateTime]
                                        ,[UpdDateTime]
                                        ,[cdUser]
                                        ,[cdPriority]
                                        ,[hoursWorked] 
                                FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                                AND (STATUS LIKE '%ENCE%' OR STATUS LIKE '%ENTE%')                        
                                AND  PLANTGROUP in @plants
                                AND CENTERCOST LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                                AND DATEREF BETWEEN @dateStart AND @dateEnd and CHARINDEX('PRED',[Description]) = 1 AND [dateEnd] >= DATEREF" :
                            @"SELECT 
                                DISTINCT
                                        [idOrder]
                                        ,[Type]
                                        ,[Plant]
                                        ,[PlantGroup]
                                        ,[CenterCost]
                                        ,[Description]
                                        ,[Status]
                                        ,[SheetLocation]
                                        ,[Location]
                                        ,[TypeActivity]
                                        ,[dateStart]
                                        ,[dateEnd]
                                        ,[dateRef]
                                        ,[InsDateTime]
                                        ,[UpdDateTime]
                                        ,[cdUser]
                                        ,[cdPriority]
                                        ,[hoursWorked] 
                                FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                                AND (STATUS LIKE '%ENCE%' OR STATUS LIKE '%ENTE%')                        
                                AND  PLANTGROUP in @plants
                                AND CENTERCOST LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                                AND DATEREF BETWEEN @dateStart AND @dateEnd AND [dateEnd] >= DATEREF";


                        return sqlConn.Query<OrderHead>(sql, new { plants = plants, dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }

                case TypeStatus.ClosedDelayEnfa:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                            @"SELECT 
                                DISTINCT
                                        [idOrder]
                                        ,[Type]
                                        ,[Plant]
                                        ,[PlantGroup]
                                        ,[CenterCost]
                                        ,[Description]
                                        ,[Status]
                                        ,[SheetLocation]
                                        ,[Location]
                                        ,[TypeActivity]
                                        ,[dateStart]
                                        ,[dateEnd]
                                        ,[dateRef]
                                        ,[InsDateTime]
                                        ,[UpdDateTime]
                                        ,[cdUser]
                                        ,[cdPriority]
                                        ,[hoursWorked] 
                                FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                                AND (STATUS LIKE '%ENCE%' OR STATUS LIKE '%ENTE%')                        
                                AND  PLANTGROUP in @plants
                                AND CENTERCOST LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                                AND DATEREF BETWEEN @dateStart AND @dateEnd and CHARINDEX('PRED',[Description]) = 1 AND [dateEnd] < DATEREF" :
                            @"SELECT 
                                DISTINCT
                                        [idOrder]
                                        ,[Type]
                                        ,[Plant]
                                        ,[PlantGroup]
                                        ,[CenterCost]
                                        ,[Description]
                                        ,[Status]
                                        ,[SheetLocation]
                                        ,[Location]
                                        ,[TypeActivity]
                                        ,[dateStart]
                                        ,[dateEnd]
                                        ,[dateRef]
                                        ,[InsDateTime]
                                        ,[UpdDateTime]
                                        ,[cdUser]
                                        ,[cdPriority]
                                        ,[hoursWorked] 
                                FROM SAP.orderHead where Type in (SELECT SHEET FROM [Common].[TypeDoc] WHERE cdIndicator = 3)
                                AND (STATUS LIKE '%ENCE%' OR STATUS LIKE '%ENTE%')                        
                                AND  PLANTGROUP in @plants
                                AND CENTERCOST LIKE '%ENFA%' AND TYPEACTIVITY NOT LIKE 'X%'
                                AND DATEREF BETWEEN @dateStart AND @dateEnd AND [dateEnd] < DATEREF";


                        return sqlConn.Query<OrderHead>(sql, new { plants = plants, dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }

                default:
                    return null;

            }



        }


        private object UpdateOrderHead(SqlConnection sqlConn, OrderHead orderHead)
        {

            try
            {

                sqlConn.Execute(@"
                        update [Sap].[OrderHead] set                                  
                                Plant = @Plant, 
                                PlantGroup = @PlantGroup, 
                                CenterCost = @CenterCost, 
                                [Description] = @Description, 
                                [Status] = @Status, 
                                SheetLocation = @SheetLocation, 
                                Location = @Location, 
                                TypeActivity = @TypeActivity, 
                                dateStart = @dateStart, 
                                dateEnd = @dateEnd, 
                                dateRef = @dateRef, 
                                InsDateTime = @InsDateTime, 
                                UpdDateTime = @UpdDateTime, 
                                cdUser = @cdUser,
                                cdPriority = @cdPriority,
                                hoursWorked = @hoursWorked
                        where idOrder = @idOrder and [Type] = @Type",
             new
             {
                 idOrder = orderHead.idOrder,
                 Type = orderHead.Type,
                 Plant = orderHead.Plant,
                 PlantGroup = orderHead.PlantGroup,
                 CenterCost = orderHead.CenterCost,
                 Description = orderHead.Description,
                 Status = orderHead.Status,
                 SheetLocation = orderHead.SheetLocation,
                 Location = orderHead.Location,
                 TypeActivity = orderHead.TypeActivity,
                 dateStart = orderHead.dateStart,
                 dateEnd = orderHead.dateEnd,
                 dateRef = orderHead.dateRef,
                 InsDateTime = orderHead.InsDateTime,
                 UpdDateTime = orderHead.UpdDateTime,
                 cdUser = orderHead.cdUser,
                 cdPriority = orderHead.cdPriority
             });


                return new { status = true, message = "Dados atualizados com sucesso !" };

            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }


        }

        private object DeleteOrderhead(SqlConnection sqlConn, List<string> planPlants, List<string> typeDocs, List<string> plantGroups, DateTime dateStart, DateTime dateEnd, ScheduledRangeType scheduleType)
        {


            switch (scheduleType)
            {

                case ScheduledRangeType.OrderSAP:
                    sqlConn.Execute(@"
                                delete [Sap].[OrderHead] 
                                    where Plant in @Plant and PlantGroup in @PlantGroup and [Type] in @Type and dateStart between @dateStart and @dateEnd",
                       new
                       {
                           Plant = planPlants,
                           PlantGroup = plantGroups,
                           Type = typeDocs,
                           dateStart = dateStart,
                           dateEnd = dateEnd
                       });
                    break;
                case ScheduledRangeType.OtSAP:
                    //Deleta ordens pendentes
                    sqlConn.Execute(@"
                        delete [Sap].[OrderHead] 
                            where Plant in @Plant and PlantGroup in @PlantGroup and [Type] in @Type and InsDatetime <= @dateEnd and CHARINDEX('ENCE',[STATUS]) = 0  and  CHARINDEX('ENTE',[STATUS]) = 0",
                        new
                        {
                            Plant = planPlants,
                            PlantGroup = plantGroups,
                            Type = typeDocs,
                            dateEnd = dateEnd
                        });

                    //Deleta ordens encerradas
                    sqlConn.Execute(@"
                        delete [Sap].[OrderHead] 
                            where Plant in @Plant and PlantGroup in @PlantGroup and [Type] in @Type and InsDatetime between @dateStart and @dateEnd and (CHARINDEX('ENCE',[STATUS]) > 0  or  CHARINDEX('ENTE',[STATUS]) > 0)",
                        new
                        {
                            Plant = planPlants,
                            PlantGroup = plantGroups,
                            Type = typeDocs,
                            dateStart = dateStart,
                            dateEnd = dateEnd
                        });
                    break;
            }






            return new { status = true, message = "Dados excluídos com sucesso !" };
        }


        private void FillOrders(DataSet dsOrder, string exprWhere, ref List<OrderHead> listOrderHead)
        {

            if (dsOrder.Tables.Count > 0)
            {
                DataRow[] dataRows = dsOrder.Tables[0].Select(exprWhere);
                foreach (DataRow row in dataRows)
                {

                    var order = new OrderHead
                    {
                        idOrder = row["ORDERID"].ToString(),
                        Type = row["ORDER_TYPE"].ToString(),
                        cdPriority = row["PRIORITY"].ToString() == "" ? null : (int?)Convert.ToInt32(row["PRIORITY"]),
                        Plant = row["PLANPLANT"].ToString(),
                        PlantGroup = row["PLANGROUP"].ToString(),
                        CenterCost = row["MN_WK_CTR"].ToString(),
                        Description = row["SHORT_TEXT"].ToString(),
                        Status = row["S_STATUS"].ToString(),
                        SheetLocation = row["MAINTLOC"].ToString(),
                        Location = row["FUNCLOC"].ToString(),
                        TypeActivity = row["PMACTTYPE"].ToString(),
                        dateStart = row["START_DATE"].ToString() == "0000-00-00" ? null : (DateTime?)Convert.ToDateTime(row["START_DATE"]),
                        dateEnd = row["FINISH_DATE"].ToString() == "0000-00-00" ? null : (DateTime?)Convert.ToDateTime(row["FINISH_DATE"]),
                        dateRef = row["REFDATE"].ToString() == "0000-00-00" ? null : (DateTime?)Convert.ToDateTime(row["REFDATE"]),
                        InsDateTime = row["ENTER_DATE"].ToString() == "0000-00-00" ? null : (DateTime?)Convert.ToDateTime(row["ENTER_DATE"]),
                        UpdDateTime = row["CHANGE_DATE"].ToString() == "0000-00-00" ? null : (DateTime?)Convert.ToDateTime(row["CHANGE_DATE"]),
                        cdUser = row["CHANGED_BY"].ToString()

                    };


                    listOrderHead.Add(order);


                }


            }

        }

        private void SetHoursWorked(DataSet dsHoursWorked, ref List<OrderHead> listOrderHead)
        {

            if (dsHoursWorked.Tables.Count > 0)
            {
                foreach (var orderHead in listOrderHead)
                {
                    orderHead.hoursWorked = (float?)dsHoursWorked.Tables[0].Select("ORDERID = " + orderHead.idOrder).Sum(f => Convert.ToDouble(f[11]));

                }

            }
        }
    }
}
