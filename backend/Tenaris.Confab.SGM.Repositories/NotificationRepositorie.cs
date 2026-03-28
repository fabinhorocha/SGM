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
    public class NotificationRepositorie : INotificationRepositorie
    {

        const int limitParams = 2050;

        public List<Notification> GetNotificationSAP(List<string> planPlants, List<string> typeNotes, List<string> plantGroups, DateTime dateStart)
        {
            try
            {

                var listNotifications = new List<Notification>();

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



                foreach (var plant in planPlants)
                {

                    foreach (var group in plantGroups)
                    {

                        var message = "";
                        var columns = new List<string>();

                        columns.Add("NOTIFICAT");
                        columns.Add("PRIORITY");
                        columns.Add("NOTIF_TYPE");
                        columns.Add("DESCRIPT");
                        columns.Add("S_STATUS");
                        columns.Add("FUNCLOC");
                        columns.Add("NOTIFDATE");
                        columns.Add("STARTDATE");
                        columns.Add("ENDDATE");




                        StringBuilder filterTypes = new StringBuilder("NOTIF_TYPE in (");

                        int count = 1;
                        foreach (var note in typeNotes)
                        {
                            if (count == typeNotes.Count)
                                filterTypes.AppendFormat("'{0}'", note);
                            else
                                filterTypes.AppendFormat("'{0}',", note);

                            count++;
                        }
                        filterTypes.Append(")");


                        var dateInitial = Properties.Settings.Default.InitialDateNotification;
                        var dsNotifications = funcSAP.BAPI_ALM_NOTIF_LIST_PLANGROUP(plant, group, dateInitial, false, columns, ref message);
                        FillNotifications(plant, group, dsNotifications, filterTypes.ToString(), funcSAP, ref listNotifications, ref message);
                        //FillNotifications(plant, group, dsNotifications, "S_STATUS not like '%ORDA%'" + filterTypes, funcSAP, ref listNotifications, ref message);

                        //Retorna Notas com ordem                        
                        //dsNotifications = funcSAP.BAPI_ALM_NOTIF_LIST_PLANGROUP(plant, group, dateStart, true, columns, ref message);
                        //FillNotifications(plant, group, dsNotifications, "S_STATUS like '%ORDA%' AND NOTIFDATE >='"+dateStart.ToString("yyyy-MM-dd")+"'" + filterTypes, funcSAP, ref listNotifications, ref message);

                    }

                }

                return listNotifications;

            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        public List<Notification> GetNotificationFailureSAP(List<string> planPlants, List<string> typeNotes, List<string> plantGroups, DateTime dateStart)
        {
            try
            {

                var listNotifications = new List<Notification>();

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



                foreach (var plant in planPlants)
                {

                    foreach (var group in plantGroups)
                    {

                        var message = "";
                        var columns = new List<string>();

                        columns.Add("NOTIFICAT");
                        columns.Add("PRIORITY");
                        columns.Add("NOTIF_TYPE");
                        columns.Add("DESCRIPT");
                        columns.Add("S_STATUS");
                        columns.Add("FUNCLOC");
                        columns.Add("NOTIFDATE");
                        columns.Add("STARTDATE");
                        columns.Add("ENDDATE");




                        StringBuilder filterTypes = new StringBuilder("NOTIF_TYPE in (");

                        int count = 1;
                        foreach (var note in typeNotes)
                        {
                            if (count == typeNotes.Count)
                                filterTypes.AppendFormat("'{0}'", note);
                            else
                                filterTypes.AppendFormat("'{0}',", note);

                            count++;
                        }
                        filterTypes.Append(")");


                        //var dateInitial = Properties.Settings.Default.InitialDateNotification;
                        var dateInitial = dateStart;
                        var dsNotifications = funcSAP.BAPI_ALM_NOTIF_LIST_PLANGROUP(plant, group, dateInitial, false, columns, ref message);
                        FillNotifications(plant, group, dsNotifications, filterTypes.ToString(), funcSAP, ref listNotifications, ref message, true);
                        //FillNotifications(plant, group, dsNotifications, "S_STATUS not like '%ORDA%'" + filterTypes, funcSAP, ref listNotifications, ref message);

                        //Retorna Notas com ordem                        
                        //dsNotifications = funcSAP.BAPI_ALM_NOTIF_LIST_PLANGROUP(plant, group, dateStart, true, columns, ref message);
                        //FillNotifications(plant, group, dsNotifications, "S_STATUS like '%ORDA%' AND NOTIFDATE >='"+dateStart.ToString("yyyy-MM-dd")+"'" + filterTypes, funcSAP, ref listNotifications, ref message);

                    }

                }

                return listNotifications;

            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        public List<Notification> GetNotificationFailureSAP(List<string> planPlants, List<string> typeNotes, List<string> plantGroups, DateTime dateStart, DateTime dateEnd)
        {
            try
            {

                var listNotifications = new List<Notification>();

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

                foreach (var plant in planPlants)
                {
                    foreach (var group in plantGroups)
                    {
                        foreach (var type in typeNotes)
                        {
                            GetNotifications(plant, group, type, dateStart, dateEnd, funcSAP, ref listNotifications, ref message);
                        }
                    }

                }

                return listNotifications;

            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        public Notification GetNotification(SqlConnection sqlConn, string idNote)
        {
            try
            {

                return sqlConn.Query<Notification>(@"
                        Select n.* 
                        from [Sap].[Notification] n where idNote = @idNote", new { idNote = idNote }).SingleOrDefault();

            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }

        public List<Notification> GetNotification(List<string> plants, DateTime? dateStart, DateTime dateEnd, TypeStatus status, bool cdPredictive)
        {


            switch (status)
            {

                case TypeStatus.Pending:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {

                        var sql = cdPredictive ?
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in ('M8')
                            and status NOT like '%ORDA%' 
                            AND PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%'
                            AND INSDATETIME <=  @dateEnd" :
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in (SELECT [Sheet] FROM [Common].[TypeNote] where Active = 1 and cdNote = 1)
                            and status NOT like '%ORDA%' 
                            AND PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%'
                            AND INSDATETIME <=  @dateEnd";


                        return sqlConn.Query<Notification>(sql, new { plants = plants, dateEnd = dateEnd }).ToList();
                    }
                case TypeStatus.PendingDelay:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {

                        var sql = cdPredictive ?
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in ('M8')
                            and status NOT like '%ORDA%' 
                            AND PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%'
                            AND DATEADD(MONTH,1,INSDATETIME) < @dateEnd" :
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in (SELECT [Sheet] FROM [Common].[TypeNote] where Active = 1 and cdNote = 1)
                            and status NOT like '%ORDA%' 
                            AND PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%'
                            AND DATEADD(MONTH,1,INSDATETIME) < @dateEnd";


                        return sqlConn.Query<Notification>(sql, new { plants = plants, dateEnd = dateEnd }).ToList();
                    }
                case TypeStatus.PendingOnTime:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {

                        var sql = cdPredictive ?
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in ('M8')
                            and status NOT like '%ORDA%' 
                            AND PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%'
                            AND DATEADD(MONTH,1,INSDATETIME) >= @dateEnd" :
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in (SELECT [Sheet] FROM [Common].[TypeNote] where Active = 1 and cdNote = 1)
                            and status NOT like '%ORDA%' 
                            AND PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%'
                            AND DATEADD(MONTH,1,INSDATETIME) >= @dateEnd";


                        return sqlConn.Query<Notification>(sql, new { plants = plants, dateEnd = dateEnd }).ToList();
                    }
                case TypeStatus.PendingEnfa:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                        @"select
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where
                            Type in ('M8')
                            and status NOT like '%ORDA%'                            
                            AND CENTERCOST LIKE '%ENFA%'
                            AND INSDATETIME <= @dateEnd" :
                         @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where
                            Type in (SELECT [Sheet] FROM [Common].[TypeNote] where Active = 1 and cdNote = 1)
                            and status NOT like '%ORDA%'                            
                            AND CENTERCOST LIKE '%ENFA%'
                            AND INSDATETIME <= @dateEnd";

                        return sqlConn.Query<Notification>(sql, new { dateEnd = dateEnd }).ToList();
                    }

                case TypeStatus.PendingOnTimeEnfa:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {

                        var sql = cdPredictive ?
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in ('M8')
                            and status NOT like '%ORDA%' 
                            AND PLANTGROUP in @plants
                            AND CENTERCOST LIKE '%ENFA%'
                            AND DATEADD(MONTH,1,INSDATETIME) >= @dateEnd" :
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in (SELECT [Sheet] FROM [Common].[TypeNote] where Active = 1 and cdNote = 1)
                            and status NOT like '%ORDA%' 
                            AND PLANTGROUP in @plants
                            AND CENTERCOST LIKE '%ENFA%'
                            AND DATEADD(MONTH,1,INSDATETIME) >= @dateEnd";


                        return sqlConn.Query<Notification>(sql, new { plants = plants, dateEnd = dateEnd }).ToList();
                    }
                case TypeStatus.PendingDelayEnfa:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {

                        var sql = cdPredictive ?
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in ('M8')
                            and status NOT like '%ORDA%' 
                            AND PLANTGROUP in @plants
                            AND CENTERCOST LIKE '%ENFA%'
                            AND DATEADD(MONTH,1,INSDATETIME) < @dateEnd" :
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in (SELECT [Sheet] FROM [Common].[TypeNote] where Active = 1 and cdNote = 1)
                            and status NOT like '%ORDA%' 
                            AND PLANTGROUP in @plants
                            AND CENTERCOST LIKE '%ENFA%'
                            AND DATEADD(MONTH,1,INSDATETIME) < @dateEnd";


                        return sqlConn.Query<Notification>(sql, new { plants = plants, dateEnd = dateEnd }).ToList();
                    }
                case TypeStatus.Open:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where
                            Type in ('M8')           
                             and status NOT like '%ORDA%'                  
                            AND PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%'
                            AND INSDATETIME BETWEEN  @dateStart and @dateEnd" :
                         @"select
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where
                            Type in (SELECT [Sheet] FROM [Common].[TypeNote] where Active = 1 and cdNote = 1)             
                            and status NOT like '%ORDA%'                
                            AND PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%'
                            AND INSDATETIME BETWEEN  @dateStart and @dateEnd";

                        return sqlConn.Query<Notification>(sql, new { plants = plants, dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }
                case TypeStatus.OpenEnfa:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                        var sql = cdPredictive ?
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where
                            Type in ('M8')            
                            and status NOT like '%ORDA%'                                             
                            AND CENTERCOST LIKE '%ENFA%'
                            AND INSDATETIME BETWEEN  @dateStart and @dateEnd" :
                         @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where
                            Type in (SELECT [Sheet] FROM [Common].[TypeNote] where Active = 1 and cdNote = 1)    
                            and status NOT like '%ORDA%'                                                     
                            AND CENTERCOST LIKE '%ENFA%'
                            AND INSDATETIME BETWEEN  @dateStart and @dateEnd";

                        return sqlConn.Query<Notification>(sql, new { dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }

                case TypeStatus.Closed:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {

                        var sql = cdPredictive ?
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in ('M8')
                            and status like '%ORDA%' 
                            AND PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%'
                            AND INSDATETIME  BETWEEN @dateStart AND @dateEnd" :
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in (SELECT [Sheet] FROM [Common].[TypeNote] where Active = 1 and cdNote = 1)
                            and status like '%ORDA%' 
                            AND PLANTGROUP in @plants
                            AND CENTERCOST NOT LIKE '%ENFA%'
                            AND DATEREF BETWEEN @dateStart AND @dateEnd";


                        return sqlConn.Query<Notification>(sql, new { plants = plants, dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }
                case TypeStatus.ClosedEnfa:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {

                        var sql = cdPredictive ?
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in ('M8')
                            and status like '%ORDA%' 
                            AND PLANTGROUP in @plants
                            AND CENTERCOST LIKE '%ENFA%'
                            AND INSDATETIME BETWEEN @dateStart AND @dateEnd" :
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in (SELECT [Sheet] FROM [Common].[TypeNote] where Active = 1 and cdNote = 1)
                            and status like '%ORDA%' 
                            AND PLANTGROUP in @plants
                            AND CENTERCOST LIKE '%ENFA%'
                            AND INSDATETIME BETWEEN @dateStart AND @dateEnd";


                        return sqlConn.Query<Notification>(sql, new { plants = plants, dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }

                case TypeStatus.RequestedM2ByFA:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {

                        var sql =
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in (SELECT [Sheet] FROM [Common].[TypeNote] where Active = 1 and idTypeNote = 8)                           
                            AND PLANTGROUP in @plants                            
                            AND INSDATETIME BETWEEN @dateStart AND @dateEnd";


                        return sqlConn.Query<Notification>(sql, new { plants = plants, dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }

                case TypeStatus.RequestedM2ByFAOpen:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {

                        var sql =
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in (SELECT [Sheet] FROM [Common].[TypeNote] where Active = 1 and idTypeNote = 9)                           
                            AND PLANTGROUP in @plants                            
                            AND INSDATETIME BETWEEN @dateStart AND @dateEnd";


                        return sqlConn.Query<Notification>(sql, new { plants = plants, dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }
                case TypeStatus.PendingFA:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {

                        var sql =
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in (SELECT [Sheet] FROM [Common].[TypeNote] where Active = 1 and idTypeNote = 9)   
                            AND (CHARINDEX('MSPN',[STATUS]) > 0 and CHARINDEX('MSPN TMEE', [STATUS]) = 0)                        
                            AND PLANTGROUP in @plants                            
                            AND INSDATETIME BETWEEN @dateStart AND @dateEnd";


                        return sqlConn.Query<Notification>(sql, new { plants = plants, dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }
                case TypeStatus.AnalyzedFA:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {

                        var sql =
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in (SELECT [Sheet] FROM [Common].[TypeNote] where Active = 1 and idTypeNote = 9)   
                            AND (CHARINDEX('MSPR',[STATUS]) > 0 and CHARINDEX('MSPR TMEE', [STATUS]) = 0)                       
                            AND PLANTGROUP in @plants                            
                            AND INSDATETIME BETWEEN @dateStart AND @dateEnd";


                        return sqlConn.Query<Notification>(sql, new { plants = plants, dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }
                case TypeStatus.ClosedMeasuresFA:
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {

                        var sql =
                        @"select 
                            DISTINCT
                                  [idNote]
                                  ,[cdPriority]
                                  ,[Description]
                                  ,[SheetLocation]
                                  ,[Location]
                                  ,[dateStart]
                                  ,[dateEnd]
                                  ,[dateRef]
                                  ,[Plant]
                                  ,[PlantGroup]
                                  ,[Type]
                                  ,[Status]
                                  ,[CenterCost]
                                  ,[InsDateTime]
                                  ,[UpdDateTime]
                                  ,[cdUser]
                                  ,[idOrder]
                            from Sap.Notification where 
                            Type in (SELECT [Sheet] FROM [Common].[TypeNote] where Active = 1 and idTypeNote = 9)   
                            AND CHARINDEX('TMEE',[STATUS]) > 0                        
                            AND PLANTGROUP in @plants                            
                            AND INSDATETIME BETWEEN @dateStart AND @dateEnd";


                        return sqlConn.Query<Notification>(sql, new { plants = plants, dateStart = dateStart, dateEnd = dateEnd }).ToList();
                    }
                default:
                    return null;

            }

        }

        public object InsertNotification(List<string> planPlants, List<string> typeNotes, List<string> plantGroups, DateTime dateStart, DateTime dateEnd)
        {

            var resultObj = new object();

            try
            {

                var listNotifications = GetNotificationSAP(planPlants, typeNotes, plantGroups, dateStart);


                var option = new TransactionOptions();
                option.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                option.Timeout = TimeSpan.FromMinutes(60);

                using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, option))
                {
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {


                        DeleteNotifications(sqlConn, planPlants, typeNotes, plantGroups, dateStart, dateEnd);

                        foreach (var note in listNotifications)
                        {

                            try
                            {
                                sqlConn.Execute(@"
                                Insert into [Sap].[Notification] (idNote, idOrder, cdPriority, [Description], SheetLocation, Location, dateStart, dateEnd, dateRef, Plant, PlantGroup, [Type], [Status], CenterCost, InsDateTime, UpdDateTime, cdUser) 
                                    values (@idNote, @idOrder, @cdPriority, @Description, @SheetLocation, @Location, @dateStart, @dateEnd, @dateRef, @Plant, @PlantGroup, @Type, @Status, @CenterCost, @InsDateTime, @UpdDateTime, @cdUser) ",
                                new
                                {
                                    idNote = note.idNote,
                                    idOrder = note.idOrder,
                                    cdPriority = note.cdPriority,
                                    Description = note.Description,
                                    SheetLocation = note.SheetLocation,
                                    Location = note.Location,
                                    dateStart = note.dateStart,
                                    dateEnd = note.dateEnd,
                                    dateRef = note.dateRef,
                                    Plant = note.Plant,
                                    PlantGroup = note.PlantGroup,
                                    Type = note.Type,
                                    Status = note.Status,
                                    CenterCost = note.CenterCost,
                                    InsDateTime = note.InsDateTime == DateTime.MinValue ? note.dateStart : note.InsDateTime,
                                    UpdDateTime = note.UpdDateTime,
                                    cdUser = note.cdUser
                                },
                                commandTimeout: 600
                                );
                            }
                            catch(Exception ex)
                            {
                                var test = "";
                            }


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

        public object InsertNotificationFailure(List<string> planPlants, List<string> typeNotes, List<string> plantGroups, DateTime dateStart, DateTime dateEnd)
        {

            var resultObj = new object();

            try
            {
                var listNotifications = GetNotificationFailureSAP(planPlants, typeNotes, plantGroups, dateStart, dateEnd);

                var option = new TransactionOptions();
                option.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                option.Timeout = TimeSpan.FromMinutes(60);

                using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, option))
                {
                    using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                    {
                       
                        if (listNotifications.Count > limitParams)
                        {       
                            var dictionary = GetPartialParameters(listNotifications.Select(s => s.idNote).ToList());                            

                            foreach(var value in dictionary)                            
                                DeleteNotificationsById(sqlConn, dictionary[value.Key], true);
                        }
                        else
                            DeleteNotificationsById(sqlConn, listNotifications.Select(s => s.idNote).ToList(), true);

                        foreach (var note in listNotifications)
                        {


                            var idNotification = sqlConn.Query<int>(@"
                                Insert into [Sap].[Notification] (idNote, idOrder, cdPriority, [Description], SheetLocation, Location, dateStart, dateEnd, dateRef, Plant, PlantGroup, [Type], [Status], CenterCost, InsDateTime, UpdDateTime, cdUser, cdTypeOperation) 
                                    values (@idNote, @idOrder, @cdPriority, @Description, @SheetLocation, @Location, @dateStart, @dateEnd, @dateRef, @Plant, @PlantGroup, @Type, @Status, @CenterCost, @InsDateTime, @UpdDateTime, @cdUser, @cdTypeOperation);
                                select cast(SCOPE_IDENTITY() as int)",
                            new
                            {
                                idNote = note.idNote,
                                idOrder = note.idOrder,
                                cdPriority = note.cdPriority,
                                Description = note.Description,
                                SheetLocation = note.SheetLocation,
                                Location = note.Location,
                                dateStart = note.dateStart,
                                dateEnd = note.dateEnd,
                                dateRef = note.dateRef,
                                Plant = note.Plant,
                                PlantGroup = note.PlantGroup,
                                Type = note.Type,
                                Status = note.Status,
                                CenterCost = note.CenterCost,
                                InsDateTime = note.InsDateTime,
                                UpdDateTime = note.UpdDateTime,
                                cdUser = note.cdUser,
                                cdTypeOperation = note.cdTypeOperation
                            },
                            commandTimeout: 600
                            ).Single();


                            InsertNotificationsRef(sqlConn, idNotification, note.notesRef);

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

        private object DeleteNotifications(SqlConnection sqlConn, List<string> planPlants, List<string> typeNotes, List<string> plantGroups, DateTime dateStart, DateTime dateEnd, bool failure = false)
        {

            //Deleta notas referenciadas se tipo de nota = falhas
            if (failure)
            {

                sqlConn.Execute(@"
                        delete [Sap].[NotificationsRef] 
                                where cdNotification in (Select n.idNotification from [Sap].[Notification] n
                        where Plant in @Plant and PlantGroup in @PlantGroup and [Type] in @TypeNotes and insDateTime between @dateStart and @dateEnd)",
                new
                {
                    Plant = planPlants,
                    PlantGroup = plantGroups,
                    TypeNotes = typeNotes,
                    dateStart = dateStart,
                    dateEnd = dateEnd
                },

                commandTimeout: 60);


                //Deleta Notas sem ordem
                sqlConn.Execute(@"
                delete [Sap].[Notification] 
                    where Plant in @Plant and PlantGroup in @PlantGroup and [Type] in @TypeNotes and insDateTime between @dateStart and @dateEnd",
                    new
                    {
                        Plant = planPlants,
                        PlantGroup = plantGroups,
                        TypeNotes = typeNotes,
                        dateStart = dateStart,
                        dateEnd = dateEnd
                    },
                    commandTimeout: 60);
            }


            else {
                //Deleta Notas sem ordem
                sqlConn.Execute(@"
                delete [Sap].[Notification] 
                    where Plant in @Plant and PlantGroup in @PlantGroup and [Type] in @TypeNotes ",
                    new
                    {
                        Plant = planPlants,
                        PlantGroup = plantGroups,
                        TypeNotes = typeNotes
                    },
                    commandTimeout: 60);
            }



            return new { status = true, message = "Dados excluídos com sucesso !" };
        }

        private object DeleteNotificationsById(SqlConnection sqlConn, List<string> notes, bool failure = false)
        {

            //Deleta notas referenciadas se tipo de nota = falhas
            if (failure)
            {

                sqlConn.Execute(@"
                        delete [Sap].[NotificationsRef] 
                                where cdNotification in (Select n.idNotification from [Sap].[Notification] n
                        where idNote in @idNote)",
                new
                {
                    idNote = notes
                },

                commandTimeout: 60);


                //Deleta Notas sem ordem
                sqlConn.Execute(@"
                delete [Sap].[Notification] 
                    where idNote in @idNote",
                    new
                    {
                        idNote = notes
                    },
                    commandTimeout: 60);
            }


            else {
                //Deleta Notas sem ordem
                sqlConn.Execute(@"
                delete [Sap].[Notification] 
                    where idNote in @idNote ",
                    new
                    {
                        idNote = notes
                    },
                    commandTimeout: 60);
            }



            return new { status = true, message = "Dados excluídos com sucesso !" };
        }


        private void InsertNotificationsRef(SqlConnection sqlConn, int idNotification, List<string> notesRef)
        {

            foreach (var idNote in notesRef)
            {

                sqlConn.Execute(@"
                                Insert into [Sap].[NotificationsRef] (cdNotification, idNote) 
                                    values (@cdNotification, @idNote) ",
                                new
                                {
                                    cdNotification = idNotification,
                                    idNote = idNote
                                },
                                commandTimeout: 600
                                );
            }


        }

        private void FillNotifications(string plant, string group, DataSet dsNotifications, string exprWhere, FunctionsSap funcSAP, ref List<Notification> listNotifications, ref string message, bool faiure = false)
        {
            if (dsNotifications.Tables.Count > 0)
            {
                DataRow[] dataRows = dsNotifications.Tables[0].Select(exprWhere);
                foreach (DataRow row in dataRows)
                {
                    try
                    {
                        var dsDetailNote = funcSAP.BAPI_ALM_NOTIF_GET_DETAIL(row["NOTIFICAT"].ToString(), ref message);

                        var note = new Notification();

                        if (dsDetailNote.Tables.Count > 0)
                        {
                            note.idNote = dsDetailNote.Tables[0].Rows[0]["ORDERID"].ToString();
                            note.InsDateTime = Convert.ToDateTime(dsDetailNote.Tables[0].Rows[0]["CREATED_ON"]);
                            note.UpdDateTime = dsDetailNote.Tables[0].Rows[0]["CHANGED_ON"].ToString() == "0000-00-00" ? null : (DateTime?)Convert.ToDateTime(dsDetailNote.Tables[0].Rows[0]["CHANGED_ON"]);
                            note.cdUser = dsDetailNote.Tables[0].Rows[0]["CREATED_BY"].ToString();
                            note.CenterCost = dsDetailNote.Tables[1].Rows[0]["WORK_CNTR"].ToString();



                        }


                        note.idNote = row["NOTIFICAT"].ToString();
                        note.cdPriority = row["PRIORITY"].ToString() == "" ? null : (int?)Convert.ToInt32(row["PRIORITY"]);
                        note.Type = row["NOTIF_TYPE"].ToString();
                        note.Plant = plant;
                        note.PlantGroup = group;
                        note.Description = row["DESCRIPT"].ToString();
                        note.Status = row["S_STATUS"].ToString();
                        note.SheetLocation = null;
                        note.Location = row["FUNCLOC"].ToString();
                        note.dateRef = row["NOTIFDATE"].ToString() == "0000-00-00" ? null : (DateTime?)Convert.ToDateTime(row["NOTIFDATE"]);
                        note.dateStart = row["STARTDATE"].ToString() == "0000-00-00" ? null : (DateTime?)Convert.ToDateTime(row["STARTDATE"]);
                        note.dateEnd = row["ENDDATE"].ToString() == "0000-00-00" ? null : (DateTime?)Convert.ToDateTime(row["ENDDATE"]);

                        if (faiure && !String.IsNullOrEmpty(note.idNote))
                            note.notesRef = FillNotificationsRef(funcSAP, note.idNote, ref message);


                        listNotifications.Add(note);
                    }
                    catch(Exception ex)
                    {
                        var test = "";
                    }
                }

            }
        }

        private void GetNotifications(string plant, string group, string type, DateTime dateStart, DateTime dateEnd, FunctionsSap funcSAP, ref List<Notification> listNotifications, ref string message)
        {


            var delimiters = '|';

            //Search Notes
            var fields = new List<RFC_DB_FLD>();
            fields.Add(new RFC_DB_FLD { FIELDNAME = "IWERK", FIELDTEXT = "IWERK", LENGTH = 4, TYPE = INTTYPE.C }); //Plant
            fields.Add(new RFC_DB_FLD { FIELDNAME = "QMNUM", FIELDTEXT = "QMNUM", LENGTH = 12, TYPE = INTTYPE.C }); //idNote
            fields.Add(new RFC_DB_FLD { FIELDNAME = "QMTXT", FIELDTEXT = "QMTXT", LENGTH = 40, TYPE = INTTYPE.C }); //Description
            fields.Add(new RFC_DB_FLD { FIELDNAME = "TPLNR", FIELDTEXT = "TPLNR", LENGTH = 30, TYPE = INTTYPE.C }); //Location
            fields.Add(new RFC_DB_FLD { FIELDNAME = "AUSVN", FIELDTEXT = "AUSVN", LENGTH = 8, TYPE = INTTYPE.D }); //dateStart
            fields.Add(new RFC_DB_FLD { FIELDNAME = "AUSBS", FIELDTEXT = "AUSBS", LENGTH = 8, TYPE = INTTYPE.D }); //dateEnd
            fields.Add(new RFC_DB_FLD { FIELDNAME = "BEZDT", FIELDTEXT = "BEZDT", LENGTH = 8, TYPE = INTTYPE.D }); //dateRef
            fields.Add(new RFC_DB_FLD { FIELDNAME = "INGRP", FIELDTEXT = "INGRP", LENGTH = 3, TYPE = INTTYPE.C }); //PlantGroup
            fields.Add(new RFC_DB_FLD { FIELDNAME = "QMART", FIELDTEXT = "QMART", LENGTH = 2, TYPE = INTTYPE.C }); //Type            
            fields.Add(new RFC_DB_FLD { FIELDNAME = "ERNAM", FIELDTEXT = "ERNAM", LENGTH = 12, TYPE = INTTYPE.C }); //cdUser Create
            fields.Add(new RFC_DB_FLD { FIELDNAME = "AENAM", FIELDTEXT = "AENAM", LENGTH = 12, TYPE = INTTYPE.C }); //cdUser Update
            fields.Add(new RFC_DB_FLD { FIELDNAME = "ERDAT", FIELDTEXT = "ERDAT", LENGTH = 8, TYPE = INTTYPE.D }); //InsDateTime 
            fields.Add(new RFC_DB_FLD { FIELDNAME = "AEDAT", FIELDTEXT = "AEDAT", LENGTH = 8, TYPE = INTTYPE.D }); //UpdDatetime
            fields.Add(new RFC_DB_FLD { FIELDNAME = "PRIOK", FIELDTEXT = "PRIOK", LENGTH = 1, TYPE = INTTYPE.C }); //cdPriority

            fields.Add(new RFC_DB_FLD { FIELDNAME = "AUSWK", FIELDTEXT = "AUSWK", LENGTH = 1, TYPE = INTTYPE.C }); //Effect Operation
            fields.Add(new RFC_DB_FLD { FIELDNAME = "ARBPL", FIELDTEXT = "ARBPL", LENGTH = 8, TYPE = INTTYPE.C }); //Obter CenterCost
            fields.Add(new RFC_DB_FLD { FIELDNAME = "OBJNR", FIELDTEXT = "OBJNR", LENGTH = 22, TYPE = INTTYPE.C }); //Obter Status

            var filters = new List<string>();
            filters.Add("IWERK EQ '" + plant + "' AND ");
            filters.Add("INGRP EQ '" + group + "' AND ");
            filters.Add("QMART EQ '" + type + "' AND ");
            filters.Add("BEZDT GE '" + dateStart.ToString("yyyyMMdd") + "' AND BEZDT LE '" + dateEnd.ToString("yyyyMMdd") + "'");

            var dataDoc = funcSAP.ReadTable("VIQMEL", fields, filters, delimiters, ref message);

            var notesRef = new List<string>();
            if (dataDoc.Tables.Count > 0)
            {
                foreach (DataRow row in dataDoc.Tables[0].Rows)
                {
                    var note = new Notification();

                    note.idNote = row["QMNUM"].ToString().Trim();
                    note.Type = row["QMART"].ToString().Trim();
                    note.Plant = row["IWERK"].ToString().Trim();
                    note.PlantGroup = row["INGRP"].ToString().Trim();
                    note.Description = row["QMTXT"].ToString().Trim();
                    note.Location = row["TPLNR"].ToString().Trim();
                    note.SheetLocation = null;
                    note.dateRef = row["BEZDT"].ToString().Trim() == "00000000" || String.IsNullOrEmpty(row["BEZDT"].ToString().Trim()) ? null :
                        (DateTime?)new DateTime(Convert.ToInt32(row["BEZDT"].ToString().Trim().Substring(0, 4)),
                        Convert.ToInt32(row["BEZDT"].ToString().Trim().Substring(4, 2)),
                        Convert.ToInt32(row["BEZDT"].ToString().Trim().Substring(6, 2)));

                    note.dateStart = row["AUSVN"].ToString().Trim() == "00000000" || String.IsNullOrEmpty(row["AUSVN"].ToString().Trim()) ? null :
                         (DateTime?)new DateTime(Convert.ToInt32(row["AUSVN"].ToString().Trim().Substring(0, 4)),
                        Convert.ToInt32(row["AUSVN"].ToString().Trim().Substring(4, 2)),
                        Convert.ToInt32(row["AUSVN"].ToString().Trim().Substring(6, 2)));

                    note.dateEnd = row["AUSBS"].ToString().Trim() == "00000000" || String.IsNullOrEmpty(row["AUSBS"].ToString().Trim()) ? null :
                         (DateTime?)new DateTime(Convert.ToInt32(row["AUSBS"].ToString().Trim().Substring(0, 4)),
                        Convert.ToInt32(row["AUSBS"].ToString().Trim().Substring(4, 2)),
                        Convert.ToInt32(row["AUSBS"].ToString().Trim().Substring(6, 2)));

                    note.InsDateTime =
                         new DateTime(Convert.ToInt32(row["ERDAT"].ToString().Trim().Substring(0, 4)),
                        Convert.ToInt32(row["ERDAT"].ToString().Trim().Substring(4, 2)),
                        Convert.ToInt32(row["ERDAT"].ToString().Trim().Substring(6, 2)));

                    note.UpdDateTime = row["AEDAT"].ToString().Trim() == "00000000" || String.IsNullOrEmpty(row["AEDAT"].ToString().Trim()) ? null :
                         (DateTime?)new DateTime(Convert.ToInt32(row["AEDAT"].ToString().Trim().Substring(0, 4)),
                        Convert.ToInt32(row["AEDAT"].ToString().Trim().Substring(4, 2)),
                        Convert.ToInt32(row["AEDAT"].ToString().Trim().Substring(6, 2)));

                    note.cdUser = row["ERNAM"].ToString().Trim();
                    note.cdPriority = row["PRIOK"].ToString().Trim() == "" ? null : (int?)Convert.ToInt32(row["PRIOK"].ToString().Trim());
                    note.CenterCost = !String.IsNullOrEmpty(row["ARBPL"].ToString().Trim()) ? GetNotificationWorkCenter(funcSAP, row["ARBPL"].ToString().Trim(), ref message) : "";
                    note.Status = !String.IsNullOrEmpty(row["OBJNR"].ToString().Trim()) ? GetNotificationStatus(funcSAP, row["OBJNR"].ToString().Trim(), ref message) : "";
                    note.notesRef = !String.IsNullOrEmpty(note.idNote) ? FillNotificationsRef(funcSAP, note.idNote, ref message) : null;
                    note.cdTypeOperation = row["AUSWK"].ToString().Trim() == "" ? null : (int?)Convert.ToInt32(row["AUSWK"].ToString().Trim());

                    listNotifications.Add(note);

                }
            }






            //if (dsNotifications.Tables.Count > 0)
            //{
            //    DataRow[] dataRows = dsNotifications.Tables[0].Select(exprWhere);
            //    foreach (DataRow row in dataRows)
            //    {

            //        var dsDetailNote = funcSAP.BAPI_ALM_NOTIF_GET_DETAIL(row["NOTIFICAT"].ToString(), ref message);

            //        var note = new Notification();

            //        if (dsDetailNote.Tables.Count > 0)
            //        {
            //            note.idNote = dsDetailNote.Tables[0].Rows[0]["ORDERID"].ToString();
            //            note.InsDateTime = Convert.ToDateTime(dsDetailNote.Tables[0].Rows[0]["CREATED_ON"]);
            //            note.UpdDateTime = dsDetailNote.Tables[0].Rows[0]["CHANGED_ON"].ToString() == "0000-00-00" ? null : (DateTime?)Convert.ToDateTime(dsDetailNote.Tables[0].Rows[0]["CHANGED_ON"]);
            //            note.cdUser = dsDetailNote.Tables[0].Rows[0]["CREATED_BY"].ToString();
            //            note.CenterCost = dsDetailNote.Tables[1].Rows[0]["WORK_CNTR"].ToString();



            //        }


            //        note.idNote = row["NOTIFICAT"].ToString();
            //        note.cdPriority = row["PRIORITY"].ToString() == "" ? null : (int?)Convert.ToInt32(row["PRIORITY"]);
            //        note.Type = row["NOTIF_TYPE"].ToString();
            //        note.Plant = plant;
            //        note.PlantGroup = group;
            //        note.Description = row["DESCRIPT"].ToString();
            //        note.Status = row["S_STATUS"].ToString();
            //        note.SheetLocation = null;
            //        note.Location = row["FUNCLOC"].ToString();
            //        note.dateRef = row["NOTIFDATE"].ToString() == "0000-00-00" ? null : (DateTime?)Convert.ToDateTime(row["NOTIFDATE"]);
            //        note.dateStart = row["STARTDATE"].ToString() == "0000-00-00" ? null : (DateTime?)Convert.ToDateTime(row["STARTDATE"]);
            //        note.dateEnd = row["ENDDATE"].ToString() == "0000-00-00" ? null : (DateTime?)Convert.ToDateTime(row["ENDDATE"]);

            //        if (faiure && !String.IsNullOrEmpty(note.idNote))
            //            note.notesRef = FillNotificationsRef(funcSAP, note.idNote, ref message);


            //        listNotifications.Add(note);
            //    }

            //}
        }

        private List<string> FillNotificationsRef(FunctionsSap functionSAP, string idNote, ref string message)
        {
            var notesRef = new List<string>();

            var delimiters = '|';

            //Search Document
            var fields = new List<RFC_DB_FLD>();
            fields.Add(new RFC_DB_FLD { FIELDNAME = "MANDT", FIELDTEXT = "MANDT", LENGTH = 3, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "/TENR/AVISOFA", FIELDTEXT = "/TENR/AVISOFA", LENGTH = 12, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "/TENR/AVISOM2", FIELDTEXT = "/TENR/AVISOM2", LENGTH = 12, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "/TENR/REF", FIELDTEXT = "/TENR/REF", LENGTH = 1, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "OBJNR", FIELDTEXT = "OBJNR", LENGTH = 22, TYPE = INTTYPE.C });

            var filters = new List<string>();
            filters.Add("/TENR/AVISOFA EQ '" + idNote + "'");
            var dataDoc = functionSAP.ReadTable("/TENR/T_PM_FA_M2", fields, filters, delimiters, ref message);

            if (dataDoc.Tables.Count > 0)
            {
                foreach (DataRow row in dataDoc.Tables[0].Rows)
                {
                    var value = row["/TENR/AVISOM2"].ToString();
                    if (!String.IsNullOrEmpty(value))
                        notesRef.Add(value);
                }
            }

            return notesRef;

        }

        private string GetNotificationStatus(FunctionsSap functionSAP, string objNumber, ref string message)
        {

            var delimiters = '|';
            var fields = new List<RFC_DB_FLD>();

            fields.Add(new RFC_DB_FLD { FIELDNAME = "MANDT", FIELDTEXT = "MANDT", LENGTH = 3, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "OBJNR", FIELDTEXT = "OBJNR", LENGTH = 22, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "STAT", FIELDTEXT = "STAT", LENGTH = 5, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "INACT", FIELDTEXT = "INACT", LENGTH = 1, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "CHGNR", FIELDTEXT = "CHGNR", LENGTH = 3, TYPE = INTTYPE.C });

            var filters = new List<string>();
            filters.Add("OBJNR EQ '" + objNumber + "' AND INACT EQ '' ");

            var dataStatus = functionSAP.ReadTable("JEST", fields, filters, delimiters, ref message);

            var listStatus = new List<string>();
            if (dataStatus.Tables.Count > 0)
            {
                foreach (DataRow row in dataStatus.Tables[0].Rows)
                {
                    fields = new List<RFC_DB_FLD>();
                    fields.Add(new RFC_DB_FLD { FIELDNAME = "ISTAT", FIELDTEXT = "ISTAT", LENGTH = 5, TYPE = INTTYPE.C });
                    fields.Add(new RFC_DB_FLD { FIELDNAME = "SPRAS", FIELDTEXT = "SPRAS", LENGTH = 1, TYPE = INTTYPE.C });
                    fields.Add(new RFC_DB_FLD { FIELDNAME = "TXT04", FIELDTEXT = "TXT04", LENGTH = 5, TYPE = INTTYPE.C });
                    fields.Add(new RFC_DB_FLD { FIELDNAME = "TXT30", FIELDTEXT = "TXT30", LENGTH = 30, TYPE = INTTYPE.C });


                    filters = new List<string>();
                    filters.Add("ISTAT EQ '" + row["STAT"].ToString() + "' AND SPRAS EQ 'P' ");


                    var dataDescStatus = functionSAP.ReadTable("TJ02T", fields, filters, delimiters, ref message);

                    foreach (DataRow row2 in dataDescStatus.Tables[0].Rows)
                        listStatus.Add(row2["TXT04"].ToString().Trim());
                }

            }

            return String.Join(" ", listStatus);
        }

        private string GetNotificationWorkCenter(FunctionsSap functionSAP, string objNumber, ref string message)
        {

            var delimiters = '|';

            var fields = new List<RFC_DB_FLD>();
            fields.Add(new RFC_DB_FLD { FIELDNAME = "OBJID", FIELDTEXT = "OBJID", LENGTH = 8, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "ARBPL", FIELDTEXT = "ARBPL", LENGTH = 8, TYPE = INTTYPE.C });


            var filters = new List<string>();
            filters.Add("OBJID EQ '" + objNumber + "'");

            var data = functionSAP.ReadTable("CRHD", fields, filters, delimiters, ref message);

            var workCenter = "";
            if (data.Tables.Count > 0)
            {
                foreach (DataRow row in data.Tables[0].Rows)
                {
                    workCenter = row["ARBPL"].ToString().Trim();
                }

            }
            return workCenter;

        }

        public Dictionary<int, List<string>> GetPartialParameters(List<string> listNotifications)
        {
           

            int index = 1;
            int count = 1;
            var dictionary = new Dictionary<int, List<string>>();

            foreach (var idNote in listNotifications.ToList())
            {
                if (count > limitParams)
                {
                    count = 1;
                    index++;
                }

                if (dictionary.ContainsKey(index))
                    dictionary[index].Add(idNote);
                else
                    dictionary.Add(index, new List<string>() { idNote });

                count++;
            }

            return dictionary;
        }

    }
}
