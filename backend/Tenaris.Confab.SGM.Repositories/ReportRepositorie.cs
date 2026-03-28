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
    public class ReportRepositorie : IReportRepositorie
    {

        public Report GetReport(int id)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, Report>();

                    sqlConn.Query<Report, ReportFile, NoteSAP, Equipment, Report>(@"
                        select r.*, f.*, n.*, e.*
                        from [Report].[Report] r
                        inner join [Equipment].[Equipment] e on e.idEquipment = r.cdEquipment 
                        left join [Report].[File] f on f.cdReport = r.idReport   
                        left join [Report].[NoteSAP] n on n.cdReport = r.idReport                                             
                        where r.idReport = @idReport",
                        (r, f, n, e) =>
                        {
                            Report report;

                            if (!lookup.TryGetValue(r.idReport, out report))
                            {
                                lookup.Add(r.idReport, report = r);

                                report.Equip = e;

                                if (n != null)
                                    report.noteSAP = n;


                            }

                            if (f != null)
                            {
                                if (report.Files == null)
                                    report.Files = new List<ReportFile>();

                                report.Files.Add(f);
                            }




                            return report;

                        },
                        new { idReport = id },
                        splitOn: "idFile,idNoteSAP,idEquipment"
                        ).AsQueryable();


                    return lookup.Values.Cast<Report>().SingleOrDefault();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }


        public List<Report> GetReportsPendentsSAP()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, Report>();

                    sqlConn.Query<Report, ReportFile, NoteSAP, Equipment, Report>(@"
                        select r.*, f.*, n.*, e.*
                        from [Report].[Report] r
                        inner join [Equipment].[Equipment] e on e.idEquipment = r.cdEquipment 
                        left join [Report].[File] f on f.cdReport = r.idReport   
                        left join [Report].[NoteSAP] n on n.cdReport = r.idReport                                             
                        where r.cdStatus = 3 and e.cdIntegrate = 1 and (n.idNoteSAP is null or n.cdStatus = 0) and r.Active = 1 and r.dateMeasure >= '2018-03-20'",
                        (r, f, n, e) =>
                        {
                            Report report;

                            if (!lookup.TryGetValue(r.idReport, out report))
                            {
                                lookup.Add(r.idReport, report = r);

                                report.Equip = e;

                                if (n != null)
                                    report.noteSAP = n;


                            }

                            if (f != null)
                            {
                                if (report.Files == null)
                                    report.Files = new List<ReportFile>();

                                report.Files.Add(f);
                            }




                            return report;

                        },
                        splitOn: "idFile,idNoteSAP,idEquipment"
                        ).AsQueryable();


                    return lookup.Values.Cast<Report>().ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }


        public List<Report> GetReportsOTs(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, Report>();

                    sqlConn.Query<Report, Equipment, ReportStatus, ReportType, NoteSAP, Report>(@"
                       select r.*, e.*, s.*, t.*, sp.* from [Report].[Report] r 
                        inner join [Equipment].[Equipment] e on e.idEquipment = r.cdEquipment
                        inner join [Report].[Status] s on s.idStatus = r.cdStatus
                        inner join [Report].[Type] t on t.idType = r.cdType
                        left join [Report].[NoteSAP] sp on sp.cdReport = r.idReport
                        where r.dateMeasure between @dateStart and @dateEnd and r.cdStatus in (2,3) and and r.Active = 1 
                        order by r.idReport",
                        (r, e, s, t, sp) =>
                        {
                            Report report;

                            if (!lookup.TryGetValue(r.idReport, out report))
                            {
                                lookup.Add(r.idReport, report = r);

                                report.Equip = e;
                                report.Status = s;
                                report.Type = t;


                                if (sp != null)
                                    report.noteSAP = sp;


                            }



                            return report;

                        },
                        new { dateStart = startDate, dateEnd = endDate },
                        splitOn: "idReport,idEquipment,idStatus,idType,idNoteSap"
                        ).AsQueryable();


                    return lookup.Values.Cast<Report>().ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public List<object> GetReportsByDateRange(DateTime startDate, DateTime endDate, int idEquipment, int? idType)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    var equipRepo = new EquipmentRepositorie();
                    var equips = equipRepo.GetEquipmentsWithReports(startDate, endDate, idEquipment);

                    return sqlConn.Query<object>(@"
                       exec  [Report].[SPGetReports] @startDate, @endDate, @equipments, @idType", new { startDate = startDate, endDate = endDate, equipments = String.Join(",", equips.Select(s => s.idEquipment).ToArray()), idType = idType }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public List<object> GetAnalysisValues(DateTime startDate, DateTime endDate, int idEquipment, int idType)
        {


            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {


                    return sqlConn.Query<object>(@"
                       select
                            r.dateMeasure,                            
		                    cast(isnull(r.vlVelocity, 0) as decimal(10,2)) as vlVelocity,
                            cast(isnull(r.vlAcceleration, 0) as decimal(10,2)) as vlAcceleration,
                            cast(isnull(r.vlRotation, 0) as decimal(10,2)) as vlRotation,
                            cast(isnull(r.vlPressure, 0) as decimal(10,2)) as vlPressure,
		                    cast(isnull(r.vlTempMax, 0) as decimal(10,2)) as vlTempMax,
                            cast(isnull(r.vlTempEnvironment, 0) as decimal(10,2)) as vlTempEnvironment,
		                    cast(isnull(r.vlMore4Micron, 0) as decimal(10,2)) as vlMore4Micron,
		                    cast(isnull(r.vlMore6Micron, 0) as decimal(10,2)) as vlMore6Micron,
		                    cast(isnull(r.vlMore14Micron, 0) as decimal(10,2)) as vlMore14Micron,
                            cast(isnull(r.vlWaterKarlFischer, 0) as decimal(10,2)) as vlWaterKarlFischer,
		                    cast(isnull(r.vlViscosity40, 0) as decimal(10,2)) as vlViscosity40,
		                    cast(isnull(r.vlViscosity100, 0) as decimal(10,2)) as vlViscosity100,
                            cast(isnull(r.vlTAN, 0) as decimal(10,2)) as vlTAN,
                            cast(isnull(r.vlEmissivity, 0) as decimal(10,2)) as vlEmissivity
                       from[Report].[Report] r
                       inner join[Equipment].[Equipment] e on r.cdEquipment = e.idEquipment
                       inner join[Report].[Type] t on t.idType = r.cdType
                       where e.Active = 1 and
                             r.cdEquipment = @cdEquipment and
                             r.dateMeasure between @startDate and @endDate and
                             r.cdType = @cdType and
                             r.Active = 1
                       order by r.dateMeasure",
                       new { startDate = startDate, endDate = endDate, cdEquipment = idEquipment, cdType = idType }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }




        }


        public List<object> GetMeasurementsByDateRange(DateTime startDate, DateTime endDate, int idEquipment, int? idType)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    var equipRepo = new EquipmentRepositorie();
                    var equips = equipRepo.GetEquipmentsWithReports(startDate, endDate, idEquipment);

                    return sqlConn.Query<object>(@"
                       exec  [Report].[SPGetMeasurements] @startDate, @endDate, @equipments, @idType", new { startDate = startDate, endDate = endDate, equipments = String.Join(",", equips.Select(s => s.idEquipment).ToArray()), idType = idType }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }


        public object InsertReport(Report report, string pathUpload)
        {

            try
            {
                using (var transactionScope = new TransactionScope())
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    report.InsDateTime = DateTime.Now;
                    var id = sqlConn.Query<int>(@"
                        Insert into [Report].[Report] (dateInput, cdEquipment, cdStatus, cdType, dateinfo, dateMeasure, Diagnostic, cdUser, InsDateTime, Title, cdPriority, vlVelocity, vlAcceleration, vlTempMax, vlISOMax, vlISOAlarm, vlISO, vlMore4Micron, vlMore6Micron, vlMore14Micron, vlWaterKarlFischer, vlViscosity40, vlViscosity100, vlTAN, vlRotation, vlPressure, vlTempEnvironment, vlEmissivity, cdRecurrent, cdOTExecuted, Active) 
                            values (@dateInput, @cdEquipment, @cdStatus, @cdType, @dateinfo,  @dateMeasure, @Diagnostic, @cdUser, @InsDateTime, @Title, @cdPriority, @vlVelocity, @vlAcceleration, @vlTempMax, @vlISOMax, @vlISOAlarm, @vlISO, @vlMore4Micron, @vlMore6Micron, @vlMore14Micron, @vlWaterKarlFischer, @vlViscosity40, @vlViscosity100, @vlTAN, @vlRotation, @vlPressure, @vlTempEnvironment, @vlEmissivity, @cdRecurrent, @cdOTExecuted, @Active);                        
                        select cast(SCOPE_IDENTITY() as int)",
                    new
                    {
                        dateInput = report.dateInput,
                        cdEquipment = report.cdEquipment,
                        cdStatus = report.cdStatus,
                        cdType = report.cdType,
                        dateinfo = report.dateInfo,
                        dateMeasure = report.dateMeasure,
                        Diagnostic = report.Diagnostic,
                        cdUser = report.cdUser,
                        InsDateTime = report.InsDateTime,
                        Title = report.Title,
                        cdPriority = report.cdPriority,
                        vlVelocity = report.vlVelocity,
                        vlAcceleration = report.vlAcceleration,
                        vlRotation = report.vlRotation,
                        vlPressure = report.vlPressure,
                        vlTempMax = report.vlTempMax,
                        vlTempEnvironment = report.vlTempEnvironment,
                        vlEmissivity = report.vlEmissivity,
                        vlISOMax = report.vlISOMax,
                        vlISOAlarm = report.vlISOAlarm,
                        vlISO = report.vlISO,
                        vlMore4Micron = report.vlMore4Micron,
                        vlMore6Micron = report.vlMore6Micron,
                        vlMore14Micron = report.vlMore14Micron,
                        vlWaterKarlFischer = report.vlWaterKarlFischer,
                        vlViscosity40 = report.vlViscosity40,
                        vlViscosity100 = report.vlViscosity100,
                        vlTAN = report.vlTAN,
                        cdRecurrent = report.cdRecurrent,
                        cdOTExecuted = report.cdOTExecuted,
                        Active = report.Active

                    }).Single();


                    var repoReportFile = new ReportFileRepositorie();

                    foreach (var file in report.Files)
                    {
                        file.cdReport = id;
                        file.cdUser = report.cdUser;
                        file.InsDateTime = report.InsDateTime;

                        repoReportFile.InsertReportFile(sqlConn, file, pathUpload);
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


        public object UpdateReport(Report report)
        {

            try
            {
                using (var transactionScope = new TransactionScope())
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Execute(@"
                        update [Report].[Report] set  
                            dateInput = @dateInput, 
                            cdEquipment = @cdEquipment, 
                            cdStatus = @cdStatus, 
                            cdType = @cdType, 
                            dateinfo = @dateinfo,                             
                            dateMeasure = @dateMeasure, 
                            Diagnostic = @Diagnostic, 
                            cdUser = @cdUser,
                            UpdDateTime = @UpdDateTime,
                            Title = @Title, 
                            cdPriority = @cdPriority, 
                            vlVelocity = @vlVelocity, 
                            vlAcceleration = @vlAcceleration, 
                            vlTempMax = @vlTempMax, 
                            vlISOMax = @vlISOMax, 
                            vlISOAlarm = @vlISOAlarm, 
                            vlISO = @vlISO, 
                            vlMore4Micron = @vlMore4Micron, 
                            vlMore6Micron = @vlMore6Micron, 
                            vlMore14Micron = @vlMore14Micron,
                            vlWaterKarlFischer = @vlWaterKarlFischer,
                            vlViscosity40 = @vlViscosity40,
                            vlViscosity100 = @vlViscosity100,
                            vlTAN = @vlTAN,
                            vlRotation = @vlRotation,
                            vlPressure = @vlPressure,
                            vlTempEnvironment = @vlTempEnvironment,
                            vlEmissivity = @vlEmissivity,
                            cdRecurrent = @cdRecurrent,
                            cdOTExecuted = @cdOTExecuted
                        where idReport = @idReport",
                    new
                    {
                        idReport = report.idReport,
                        dateInput = report.dateInput,
                        cdEquipment = report.cdEquipment,
                        cdStatus = report.cdStatus,
                        cdType = report.cdType,
                        dateinfo = report.dateInfo,
                        dateMeasure = report.dateMeasure,
                        Diagnostic = report.Diagnostic,
                        cdUser = report.cdUser,
                        UpdDateTime = DateTime.Now,
                        Title = report.Title,
                        cdPriority = report.cdPriority,
                        vlVelocity = report.vlVelocity,
                        vlAcceleration = report.vlAcceleration,
                        vlRotation = report.vlRotation,
                        vlPressure = report.vlPressure,
                        vlTempMax = report.vlTempMax,
                        vlTempEnvironment = report.vlTempEnvironment,
                        vlISOMax = report.vlISOMax,
                        vlISOAlarm = report.vlISOAlarm,
                        vlISO = report.vlISO,
                        vlMore4Micron = report.vlMore4Micron,
                        vlMore6Micron = report.vlMore6Micron,
                        vlMore14Micron = report.vlMore14Micron,
                        vlWaterKarlFischer = report.vlWaterKarlFischer,
                        vlViscosity40 = report.vlViscosity40,
                        vlViscosity100 = report.vlViscosity100,
                        vlTAN = report.vlTAN,
                        vlEmissivity = report.vlEmissivity,
                        cdRecurrent = report.cdRecurrent,
                        cdOTExecuted = report.cdOTExecuted
                    });



                    transactionScope.Complete();


                    return new { id = report.idReport, status = true, message = "Dados atualizados com sucesso !" };

                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }


        public object DeleteReport(Report report)
        {

            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Execute(@"Update [Report].[Report] set Active = 0, cdUser = @cdUser, UpdDateTime = getdate()  where idReport = @idReport", new { idReport = report.idReport, cdUser = report.cdUser });

                    return new { id = report.idReport, status = true, message = "Dados excluídos com sucesso !" };
                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }


        public object CreateNoteSAP(Report report, string pathUpload)
        {

            var noteSAPRepo = new NoteSAPRepositorie();

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

            var docName = report.Files != null && report.Files.Count > 0 ? "GAM_CON_GMP_" + report.idReport.ToString() : "";

            var messageDoc = "";

            var doc = CreateDocument(funcSAP, report, docName, pathUpload, ref messageDoc);

            var note = OpenNoteSap(funcSAP, report, docName, ref messageDoc);


            if (report.noteSAP == null)
                noteSAPRepo.InsertNoteSAP(note, report.idReport, (note != null && doc) ? true : false, messageDoc, report.cdUser, doc);
            else
                noteSAPRepo.UpdateNoteSAP(report.noteSAP.idNoteSAP, note, report.idReport, (note != null && doc) ? true : false, messageDoc, report.cdUser, doc);


            return new { id = note == null ? 0 : note, status = (note != null && doc) ? true : false, message = messageDoc };

        }

        private bool CreateDocument(FunctionsSap funcSAP, Report report, string docName, string pathUpload, ref string messageDoc)
        {

            try
            {
                var returnResult = true;
                var values = new Dictionary<string, string>();

                messageDoc = "Documento anexado com sucesso !";

                if (report.noteSAP == null || (bool)!report.noteSAP.cdAttachDoc)
                {

                    if (report.Files != null && report.Files.Count > 0)
                    {
                        foreach (var file in report.Files)
                        {
                            values = new Dictionary<string, string>();

                            values.Add("P_IDSAP", docName.ToUpper());
                            values.Add("P_IDDOC", file.Name.ToUpper());
                            values.Add("P_CLASE", "GA8");
                            values.Add("P_DESCR", report.Title);
                            values.Add("P_APLICA", Path.GetExtension(file.Name).Replace(".", "").ToUpper());
                            values.Add("P_PATH", Properties.Settings.Default.Path_File);

                            using (new Impersonator(Properties.Settings.Default.User, Properties.Settings.Default.Domain, Properties.Settings.Default.Password))
                            {
                                File.Copy(pathUpload + "\\" + report.idReport.ToString() + "\\" + file.Name, Properties.Settings.Default.Path_File + file.Name, true);
                            }

                            var message = "";
                            if (!funcSAP.ZPM_CREAR_DOCUMENTO(values, ref message))
                            {
                                returnResult = false;
                                messageDoc = "Problemas ao anexar arquivo: " + message;
                            }                           
                        }

                    }
                    else
                        messageDoc = "Aviso sem documento anexado !";
                }
              
                return returnResult;
            }
            catch (Exception ex)
            {
                messageDoc = "Problemas ao anexar arquivo: " + ex.Message;

                return false;
            }
        }


        private int? OpenNoteSap(FunctionsSap funcSAP, Report report, string docName, ref string messageDoc)
        {

            try
            {

                if (report.noteSAP == null || report.noteSAP.cdSAP == null)
                {
                    var values = new Dictionary<string, string>();


                    values.Add("P_CLASE", "M8");
                    values.Add("P_DESCR", report.Title);
                    values.Add("P_EQUIPO", report.Equip.Sheet.PadLeft(18, '0'));
                    values.Add("P_FECHAINICIO", report.InsDateTime.ToString("yyyy-MM-dd"));
                    values.Add("P_HORAINICIO", report.InsDateTime.ToString("HH:mm:ss"));
                    values.Add("P_PRIORIDAD", report.cdPriority.ToString());
                    values.Add("P_DOCUMENTO", docName.ToUpper());
                    values.Add("P_OBSERVACION", report.Diagnostic);


                    var messageNote = "";
                    int? note = funcSAP.ZPM_CREAR_AVISO_GAM2(report.idReport, values, ref messageNote);

                    messageDoc = messageNote + "\n" + messageDoc;

                    return note;
                }
                else {

                    messageDoc = "Aviso criado com sucesso para o laudo " + report.idReport.ToString() + " !\n" + messageDoc;
                    return report.noteSAP.cdSAP;
                }
            }
            catch (Exception ex)
            {
                messageDoc = "Problema na abertuda da nota: " + ex.Message;

                return null;
            }

        }
        //public object CreateNoteSAP(Report report, string pathUpload)
        //{

        //    try {


        //        foreach (var file in report.Files)
        //        {



        //            using (new Impersonator(Properties.Settings.Default.User, Properties.Settings.Default.Domain, Properties.Settings.Default.Password))
        //            {
        //                File.Copy(pathUpload + "\\" + report.idReport.ToString() + "\\" + file.Name, Properties.Settings.Default.Path_File + file.Name, true);
        //            }



        //        }





        //        return new { id = 0 , status =  true, message = "OK" };
        //    }
        //    catch(Exception ex)
        //    {
        //        return new { id = 0, status = false, message = ex.Message};
        //    }

        //}


    }
}
