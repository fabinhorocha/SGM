
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.InterfaceService.Properties;
using Tenaris.Confab.SGM.Services;

namespace Tenaris.Confab.SGM.InterfaceService
{
    public class BusinessLogic : IDisposable
    {
        #region FIELDS

        //private Log _log;
        private ILog _log;
        private IOrderHeadService _OrderHeadServ;
        private INotificationService _NotificationServ;
        private ILocationDataService _LocationDataServ;
        private IPlantDataService _PlantDataServ;
        private IFailureDataService _FailureDataServ;
        private IScheduledRangeService _ScheduledRangeServ;
        private IPlantService _PlantServ;
        private IReportService _ReportServ;
        private ITypeDocService _TypeDocServ;
        private ITypeNoteService _TypeNoteServ;
        private IEquipmentPlantGroupService _EquipmentPlantGroupServ;
        private static readonly object _lock = new object();
        


        #endregion

        #region CONSTRUCTOR


        public BusinessLogic(ILog log)
        {
            _log = log;
            _OrderHeadServ = new OrderHeadService();
            _ScheduledRangeServ = new ScheduledRangeService();
            _PlantServ = new PlantService();
            _LocationDataServ = new LocationDataService();
            _PlantDataServ = new PlantDataService();
            _FailureDataServ = new FailureDataService();
            _ReportServ = new ReportService();
            _TypeDocServ = new TypeDocService();
            _TypeNoteServ = new TypeNoteService();
            _NotificationServ = new NotificationService();
            _EquipmentPlantGroupServ = new EquipmentPlantGroupService();

            EmailHelper.SendEmail("   Erro no serviço SGMInterfaceService", "BusinessLogic.CreateNoteSAP: ", _log);
        }

        #endregion

        #region IDISPOSABLE METHODS

        ~BusinessLogic()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(Boolean isSafeToFreeManagedObjects)
        {
            if (!isSafeToFreeManagedObjects)
            {
                return;
            }

            _log = null;

        }

        #endregion

        #region PUBLIC METHODS

        public bool CreateNoteSAP()
        {
            _log.Info(":: BusinessLogic.CreateNoteSAP");
            bool result = false;

            try
            {

                var listResult = new List<ResultClass>();
                var listReports = _ReportServ.GetReportsPendentsSAP();

                foreach (var rpt in listReports)
                {
                  
                    var obj = _ReportServ.CreateNoteSAP(rpt, Settings.Default.PathUpload);
                    listResult.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<ResultClass>(Newtonsoft.Json.JsonConvert.SerializeObject(obj)));                                       
                }


                if (listResult.Count > 0)
                {
                    var resultErrors = listResult.Where(w => w.status == false).ToList();

                    if (resultErrors.Count > 0)
                    {
                        foreach (var res in resultErrors)
                        {
                            _log.Warn(res.message);

                        }

                        result = false;
                    }
                    else
                        result = true;
                }
                else
                    result = true;


            }
            catch (Exception exception)
            {
                result = false;

                _log.Error(exception);
                EmailHelper.SendEmail("   Erro no serviço SGMInterfaceService", "BusinessLogic.CreateNoteSAP: " + exception.Message, _log);
            }

            return result;
        }

        public bool DownloadOrderHeadSAP()
        {


            _log.Info(":: BusinessLogic.DownloadOrderHeadSAP");
            bool result = true;
            try
            {
                result = DownloadOrder(ScheduledRangeType.OrderSAP);
                result = DownloadOrder(ScheduledRangeType.OtSAP);

            }

            catch (Exception exception)
            {
                result = false;

                _log.Error(exception);
                EmailHelper.SendEmail("   Erro no serviço SGMInterfaceService", "BusinessLogic.DownloadOrderHeadSAP: " + exception.Message, _log);
            }

            return result;
        }

        public bool DownloadNotificationSAP()
        {

            _log.Info(":: BusinessLogic.DownloadNotificationSAP");
            bool result = true;

            try
            {
                var listResult = new List<ResultClass>();
                var plants = Settings.Default.Plants.Cast<string>().ToList();

                lock (_lock)
                {
                    var scheduleds = _ScheduledRangeServ.GetScheduledRangeByStatus(ScheduledRangeStatus.Pending, ScheduledRangeType.NoteSAP);

                    if (scheduleds.Count > 0)
                    {

                        foreach (var scheduled in scheduleds)
                        {
                            var plantsGroups = scheduled.PlantGroups.Split('|').ToList();
                            var typeNotes = scheduled.TypeDocs.Split('|').ToList();

                            scheduled.Message = "Processando...";
                            _ScheduledRangeServ.UpdateScheduledRange(scheduled);

                            //Baixa dados do SAP
                            var resultClass = (Newtonsoft.Json.JsonConvert.DeserializeObject<ResultClass>(Newtonsoft.Json.JsonConvert.SerializeObject(_NotificationServ.InsertNotification(plants, typeNotes, plantsGroups, scheduled.startdate, scheduled.endDate))));
                            listResult.Add(resultClass);

                            //Atualiza status do ScheduleRange para processado                        
                            _ScheduledRangeServ.UpdateScheduledRange(
                                new ScheduledRange
                                {
                                    idScheduledRange = scheduled.idScheduledRange,
                                    cdStatus = resultClass.status ? 2 : 1,
                                    startdate = scheduled.startdate,
                                    endDate = scheduled.endDate,
                                    Message = resultClass.message,
                                    cdScheduledType = scheduled.cdScheduledType
                                });

                            if (!resultClass.status)
                            {
                                _log.Warn(resultClass.message);
                                result = false;
                            }


                        }

                    }
                }

            }

            catch (Exception exception)
            {
                result = false;

                _log.Error(exception);
                EmailHelper.SendEmail("   Erro no serviço SGMInterfaceService", "BusinessLogic.DownloadNotificationSAP: " + exception.Message, _log);
            }

            return result;
        }

        public bool DownloadNotificationFailureSAP()
        {

            _log.Info(":: BusinessLogic.DownloadNotificationFailureSAP");
            bool result = true;

            try
            {
                var listResult = new List<ResultClass>();
                var plants = Settings.Default.Plants.Cast<string>().ToList();

                lock (_lock)
                {
                    var scheduleds = _ScheduledRangeServ.GetScheduledRangeByStatus(ScheduledRangeStatus.Pending, ScheduledRangeType.FailureSAP);

                    if (scheduleds.Count > 0)
                    {

                        foreach (var scheduled in scheduleds)
                        {
                            var plantsGroups = scheduled.PlantGroups.Split('|').ToList();
                            var typeNotes = scheduled.TypeDocs.Split('|').ToList();

                            scheduled.Message = "Processando...";
                            _ScheduledRangeServ.UpdateScheduledRange(scheduled);

                            //Baixa dados do SAP
                            var resultClass = (Newtonsoft.Json.JsonConvert.DeserializeObject<ResultClass>(Newtonsoft.Json.JsonConvert.SerializeObject(_NotificationServ.InsertNotificationFailure(plants, typeNotes, plantsGroups, scheduled.startdate, scheduled.endDate))));
                            listResult.Add(resultClass);

                            //Atualiza status do ScheduleRange para processado                        
                            _ScheduledRangeServ.UpdateScheduledRange(
                                new ScheduledRange
                                {
                                    idScheduledRange = scheduled.idScheduledRange,
                                    cdStatus = resultClass.status ? 2 : 1,
                                    startdate = scheduled.startdate,
                                    endDate = scheduled.endDate,
                                    Message = resultClass.message,
                                    cdScheduledType = scheduled.cdScheduledType
                                });

                            if (!resultClass.status)
                            {
                                _log.Warn(resultClass.message);
                                result = false;
                            }


                        }

                    }
                }

            }

            catch (Exception exception)
            {
                result = false;

                _log.Error(exception);
                EmailHelper.SendEmail("   Erro no serviço SGMInterfaceService", "BusinessLogic.DownloadNotificationSAP: " + exception.Message, _log);
            }

            return result;
        }

        public bool ProcessPlantData()
        {



            _log.Info(":: BusinessLogic.ProcessPlantData");
            bool result = true;

            try
            {
                var listResult = new List<ResultClass>();
                var plants = Settings.Default.Plants.Cast<string>().ToList();

                lock (_lock)
                {
                    var scheduleds = _ScheduledRangeServ.GetScheduledRangeByStatus(ScheduledRangeStatus.Pending, ScheduledRangeType.PlantData);

                    if (scheduleds.Count > 0)
                    {

                        foreach (var scheduled in scheduleds.ToList())
                        {
                            var plantsGroups = scheduled.PlantGroups.Split('|').ToList();
                            var typeNotes = scheduled.TypeDocs.Split('|').ToList();

                            scheduled.Message = "Processando...";
                            _ScheduledRangeServ.UpdateScheduledRange(scheduled);

                           //Processa dados obtidos do SAP
                           var resultClass = (Newtonsoft.Json.JsonConvert.DeserializeObject<ResultClass>(Newtonsoft.Json.JsonConvert.SerializeObject(_PlantDataServ.InsertPlantData(plants, typeNotes, plantsGroups, scheduled.startdate, scheduled.endDate))));
                            listResult.Add(resultClass);

                            //Atualiza status do ScheduleRange para processado                        
                            _ScheduledRangeServ.UpdateScheduledRange(
                                new ScheduledRange
                                {
                                    idScheduledRange = scheduled.idScheduledRange,
                                    cdStatus = resultClass.status ? 2 : 1,
                                    startdate = scheduled.startdate,
                                    endDate = scheduled.endDate,
                                    Message = resultClass.message,
                                    cdScheduledType = scheduled.cdScheduledType
                                });

                            if (!resultClass.status)
                            {
                                _log.Warn(resultClass.message);
                                result = false;
                            }


                        }
                    }


                }

            }

            catch (Exception exception)
            {
                result = false;

                _log.Error(exception);
                EmailHelper.SendEmail("   Erro no serviço SGMInterfaceService", "BusinessLogic.ProcessPlantData: " + exception.Message, _log);
            }

            return result;
        }

        public bool ProcessLocationData()
        {


            _log.Info(":: BusinessLogic.ProcessLocationData");
            bool result = true;

            try
            {
                var listResult = new List<ResultClass>();
                var plants = Settings.Default.Plants.Cast<string>().ToList();

                lock (_lock)
                {
                    var scheduleds = _ScheduledRangeServ.GetScheduledRangeByStatus(ScheduledRangeStatus.Pending, ScheduledRangeType.LocationData);

                    if (scheduleds.Count > 0)
                    {

                        foreach (var scheduled in scheduleds)
                        {
                            var plantsGroups = scheduled.PlantGroups.Split('|').ToList();
                            var typeNotes = scheduled.TypeDocs.Split('|').ToList();

                            scheduled.Message = "Processando...";
                            _ScheduledRangeServ.UpdateScheduledRange(scheduled);

                            //Processa dados obtidos do SAP
                            var resultClass = (Newtonsoft.Json.JsonConvert.DeserializeObject<ResultClass>(Newtonsoft.Json.JsonConvert.SerializeObject(_LocationDataServ.InsertLocationsData(plants, typeNotes, plantsGroups, scheduled.startdate, scheduled.endDate))));
                            listResult.Add(resultClass);

                            //Atualiza status do ScheduleRange para processado                        
                            _ScheduledRangeServ.UpdateScheduledRange(
                                new ScheduledRange
                                {
                                    idScheduledRange = scheduled.idScheduledRange,
                                    cdStatus = resultClass.status ? 2 : 1,
                                    startdate = scheduled.startdate,
                                    endDate = scheduled.endDate,
                                    Message = resultClass.message,
                                    cdScheduledType = scheduled.cdScheduledType
                                });

                            if (!resultClass.status)
                            {
                                _log.Warn(resultClass.message);
                                result = false;
                            }


                        }

                    }
                }

            }

            catch (Exception exception)
            {
                result = false;

                _log.Error(exception);
                EmailHelper.SendEmail("   Erro no serviço SGMInterfaceService", "BusinessLogic.ProcessLocationData: " + exception.Message, _log);
            }

            return result;
        }

        //public bool ProcessFailureData()
        //{

        //    _log.Info(":: BusinessLogic.ProcessFailureData");
        //    bool result = true;

        //    try
        //    {
        //        var listResult = new List<ResultClass>();
        //        var plants = Settings.Default.Plants.Cast<string>().ToList();

        //        lock (_lock)
        //        {
        //            var scheduleds = _ScheduledRangeServ.GetScheduledRangeByStatus(ScheduledRangeStatus.Pending, ScheduledRangeType.FailureData);

        //            if (scheduleds.Count > 0)
        //            {

        //                foreach (var scheduled in scheduleds.ToList())
        //                {
        //                    var plantsGroups = scheduled.PlantGroups.Split('|').ToList();
        //                    var typeNotes = scheduled.TypeDocs.Split('|').ToList();

        //                    scheduled.Message = "Processando...";
        //                    _ScheduledRangeServ.UpdateScheduledRange(scheduled);

        //                    //Processa dados obtidos do SAP
        //                    var resultClass = (Newtonsoft.Json.JsonConvert.DeserializeObject<ResultClass>(Newtonsoft.Json.JsonConvert.SerializeObject(_FailureDataServ.InsertFailureData(plants, typeNotes, plantsGroups, scheduled.startdate, scheduled.endDate))));
        //                    listResult.Add(resultClass);

        //                    //Atualiza status do ScheduleRange para processado                        
        //                    _ScheduledRangeServ.UpdateScheduledRange(
        //                        new ScheduledRange
        //                        {
        //                            idScheduledRange = scheduled.idScheduledRange,
        //                            cdStatus = resultClass.status ? 2 : 1,
        //                            startdate = scheduled.startdate,
        //                            endDate = scheduled.endDate,
        //                            Message = resultClass.message,
        //                            cdScheduledType = scheduled.cdScheduledType
        //                        });

        //                    if (!resultClass.status)
        //                    {
        //                        _log.Warn(resultClass.message);
        //                        result = false;
        //                    }


        //                }
        //            }


        //        }

        //    }

        //    catch (Exception exception)
        //    {
        //        result = false;

        //        _log.Error(exception);
        //        EmailHelper.SendEmail("   Erro no serviço SGMInterfaceService", "BusinessLogic.ProcessFailureData: " + exception.Message, _log);
        //    }

        //    return result;
        //}

        public bool DownloadLocationSAP()
        {

            _log.Info(":: BusinessLogic.DownloadLocationSAP");
            bool result = true;

            try
            {
                var listResult = new List<ResultClass>();
                var plants = Settings.Default.Plants.Cast<string>().ToList();

                lock (_lock)
                {
                    var scheduleds = _ScheduledRangeServ.GetScheduledRangeByStatus(ScheduledRangeStatus.Pending, ScheduledRangeType.Location);

                    if (scheduleds.Count > 0)
                    {

                        foreach (var scheduled in scheduleds)
                        {
                            var plantsGroups = scheduled.PlantGroups.Split('|').ToList();
                            
                            scheduled.Message = "Processando...";
                            _ScheduledRangeServ.UpdateScheduledRange(scheduled);

                            //Baixa dados do SAP
                            var resultClass = (Newtonsoft.Json.JsonConvert.DeserializeObject<ResultClass>(Newtonsoft.Json.JsonConvert.SerializeObject(_EquipmentPlantGroupServ.InsertEquipmentsPlantGroup(plants, plantsGroups))));
                            listResult.Add(resultClass);

                            //Atualiza status do ScheduleRange para processado                        
                            _ScheduledRangeServ.UpdateScheduledRange(
                                new ScheduledRange
                                {
                                    idScheduledRange = scheduled.idScheduledRange,
                                    cdStatus = resultClass.status ? 2 : 1,
                                    startdate = scheduled.startdate,
                                    endDate = scheduled.endDate,
                                    Message = resultClass.message,
                                    cdScheduledType = scheduled.cdScheduledType
                                });

                            if (!resultClass.status)
                            {
                                _log.Warn(resultClass.message);
                                result = false;
                            }


                        }

                    }
                }

            }

            catch (Exception exception)
            {
                result = false;

                _log.Error(exception);
                EmailHelper.SendEmail("   Erro no serviço SGMInterfaceService", "BusinessLogic.DownloadNotificationSAP: " + exception.Message, _log);
            }

            return result;
        }

        public bool InsertScheduledRange()
        {
            try
            {
                
               
                InsertScheduledRangeMonthly(ScheduledRangeType.OrderSAP);
                InsertScheduledRangeMonthly(ScheduledRangeType.OtSAP);
                InsertScheduledRangeMonthly(ScheduledRangeType.NoteSAP);                
                InsertScheduledRangeMonthly(ScheduledRangeType.LocationData);
                InsertScheduledRangeMonthly(ScheduledRangeType.PlantData);
                InsertScheduledRangeWeekly(ScheduledRangeType.Location);
                InsertScheduledRangeDaily(ScheduledRangeType.FailureSAP);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void InsertScheduledRangeMonthly(ScheduledRangeType type)
        {


            if (DateTime.Now.Day == Settings.Default.DayCloseOfMonth && DateTime.Now.TimeOfDay >= Settings.Default.TimeCloseOfMonth)
            {
                _log.Info(":: BusinessLogic.InsertScheduledRangeMonthly");

                try
                {
                    var dateRef = DateTime.Now.AddMonths(-1);
                    var startDate = new DateTime(dateRef.Year, dateRef.Month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);

                    var _scheduledRange = _ScheduledRangeServ.GetScheduledRangeByDate(startDate, endDate, type);

                    ResultClass result = null;

                    if (_scheduledRange.Count == 0)
                    {
                        var plants = String.Join("|", _PlantServ.GetPlants().Select(s => s.Sheet).Cast<string>().ToArray());
                        var typeDocs = "";

                        switch (type)
                        {
                            case ScheduledRangeType.NoteSAP:
                                typeDocs = String.Join("|", _TypeNoteServ.GetTypeNotes().Where(w => w.cdNote == 1).Select(s => s.Sheet).Cast<string>().ToArray());
                                break;                            
                            case ScheduledRangeType.FailureSAP:
                                typeDocs = String.Join("|", _TypeNoteServ.GetTypeNotes().Where(w => w.cdNote == 2).Select(s => s.Sheet).Cast<string>().ToArray());
                                break;
                            default:
                                typeDocs = String.Join("|", _TypeDocServ.GetTypeDocs(type).Select(s => s.Sheet).Cast<string>().ToArray());
                                break;

                        }
                        
                        result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultClass>(Newtonsoft.Json.JsonConvert.SerializeObject(_ScheduledRangeServ.InsertScheduledRange(new Domain.Entities.ScheduledRange { startdate = startDate, endDate = endDate, cdStatus = 1, Message = "", PlantGroups = plants, TypeDocs = typeDocs, cdScheduledType = Convert.ToInt32(type) })));
                    }
                    else
                    {
                        result = new ResultClass { id = -1, status = true, message = "Já existe Range cadastrado para esse período !" };
                    }


                    if (!result.status)
                        _log.Warn(result.message);
                    else
                        _log.Info(result.message);




                }
                catch (Exception exception)
                {

                    _log.Error(exception);
                    EmailHelper.SendEmail("   Erro no serviço SGMInterfaceService", "BusinessLogic.InsertScheduledRange: " + exception.Message, _log);
                }
            }

        }

        private void InsertScheduledRangeWeekly(ScheduledRangeType type)
        {


            if (DateTime.Now.DayOfWeek == (DayOfWeek)Settings.Default.DayCloseOfWeek && DateTime.Now.TimeOfDay >= Settings.Default.TimeCloseOfWeek)
            {
                _log.Info(":: BusinessLogic.InsertScheduledRangeWeekly");

                try
                {
                    var dateRef = DateTime.Now.AddDays(-7);
                    var startDate = new DateTime(dateRef.Year, dateRef.Month, dateRef.Day);
                    var endDate = startDate.AddDays(7);

                    var _scheduledRange = _ScheduledRangeServ.GetScheduledRangeByDate(startDate, endDate, type);

                    ResultClass result = null;

                    if (_scheduledRange.Count == 0)
                    {
                        var plants = String.Join("|", _PlantServ.GetPlants().Select(s => s.Sheet).Cast<string>().ToArray());
                        var typeDocs = "";

                        switch (type)
                        {
                            case ScheduledRangeType.NoteSAP:
                                typeDocs = String.Join("|", _TypeNoteServ.GetTypeNotes().Where(w => w.cdNote == 1).Select(s => s.Sheet).Cast<string>().ToArray());
                                break;
                            case ScheduledRangeType.FailureSAP:
                                typeDocs = String.Join("|", _TypeNoteServ.GetTypeNotes().Where(w => w.cdNote == 2).Select(s => s.Sheet).Cast<string>().ToArray());
                                break;
                            default:
                                typeDocs = _TypeDocServ.GetTypeDocs(type) == null ? "" : String.Join("|", _TypeDocServ.GetTypeDocs(type).Select(s => s.Sheet).Cast<string>().ToArray());
                                break;

                        }

                        result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultClass>(Newtonsoft.Json.JsonConvert.SerializeObject(_ScheduledRangeServ.InsertScheduledRange(new Domain.Entities.ScheduledRange { startdate = startDate, endDate = endDate, cdStatus = 1, Message = "", PlantGroups = plants, TypeDocs = typeDocs, cdScheduledType = Convert.ToInt32(type) })));
                    }
                    else
                    {
                        result = new ResultClass { id = -1, status = true, message = "Já existe Range cadastrado para esse período !" };
                    }


                    if (!result.status)
                        _log.Warn(result.message);
                    else
                        _log.Info(result.message);




                }
                catch (Exception exception)
                {

                    _log.Error(exception);
                    EmailHelper.SendEmail("   Erro no serviço SGMInterfaceService", "BusinessLogic.InsertScheduledRange: " + exception.Message, _log);
                }
            }

        }

        private void InsertScheduledRangeDaily(ScheduledRangeType type)
        {


            if (DateTime.Now.TimeOfDay >= Settings.Default.TimeCloseOfDaily)
            {
                _log.Info(":: BusinessLogic.InsertScheduledRangeDaily");

                try
                {
                    var dateRef = DateTime.Now;
                    var startDate = new DateTime(dateRef.Year, dateRef.Month, dateRef.Day);
                    var endDate = startDate.AddDays(1);

                    var _scheduledRange = _ScheduledRangeServ.GetScheduledRangeByDate(startDate, endDate, type);

                    ResultClass result = null;

                    if (_scheduledRange.Count == 0)
                    {
                        var plants = String.Join("|", _PlantServ.GetPlants().Select(s => s.Sheet).Cast<string>().ToArray());
                        var typeDocs = "";

                        switch (type)
                        {
                            case ScheduledRangeType.NoteSAP:
                                typeDocs = String.Join("|", _TypeNoteServ.GetTypeNotes().Where(w => w.cdNote == 1).Select(s => s.Sheet).Cast<string>().ToArray());
                                break;
                            case ScheduledRangeType.FailureSAP:
                                typeDocs = String.Join("|", _TypeNoteServ.GetTypeNotes().Where(w => w.cdNote == 2).Select(s => s.Sheet).Cast<string>().ToArray());
                                break;
                            default:
                                typeDocs = _TypeDocServ.GetTypeDocs(type) == null ? "" : String.Join("|", _TypeDocServ.GetTypeDocs(type).Select(s => s.Sheet).Cast<string>().ToArray());
                                break;

                        }

                        result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultClass>(Newtonsoft.Json.JsonConvert.SerializeObject(_ScheduledRangeServ.InsertScheduledRange(new Domain.Entities.ScheduledRange { startdate = startDate, endDate = endDate, cdStatus = 1, Message = "", PlantGroups = plants, TypeDocs = typeDocs, cdScheduledType = Convert.ToInt32(type) })));
                    }
                    else
                    {
                        result = new ResultClass { id = -1, status = true, message = "Já existe Range cadastrado para esse período !" };
                    }


                    if (!result.status)
                        _log.Warn(result.message);
                    else
                        _log.Info(result.message);




                }
                catch (Exception exception)
                {

                    _log.Error(exception);
                    EmailHelper.SendEmail("   Erro no serviço SGMInterfaceService", "BusinessLogic.InsertScheduledRange: " + exception.Message, _log);
                }
            }

        }

        private bool DownloadOrder(ScheduledRangeType type)
        {

            bool result = true;

            var listResult = new List<ResultClass>();

            var plants = Settings.Default.Plants.Cast<string>().ToList();

            lock (_lock)
            {
                var scheduleds = _ScheduledRangeServ.GetScheduledRangeByStatus(ScheduledRangeStatus.Pending, type);

                if (scheduleds.Count > 0)
                {

                    foreach (var scheduled in scheduleds)
                    {
                        var plantsGroups = scheduled.PlantGroups.Split('|').ToList();
                        var typeDocs = scheduled.TypeDocs.Split('|').ToList();

                        scheduled.Message = "Processando...";
                        _ScheduledRangeServ.UpdateScheduledRange(scheduled);

                        //Baixa dados do SAP
                        var resultClass =
                        (Newtonsoft.Json.JsonConvert.DeserializeObject<ResultClass>(Newtonsoft.Json.JsonConvert.SerializeObject(_OrderHeadServ.InsertOrderHead(plants, typeDocs, plantsGroups, scheduled.startdate, scheduled.endDate, type))));

                        listResult.Add(resultClass);

                        //Atualiza status do ScheduleRange para processado                        
                        _ScheduledRangeServ.UpdateScheduledRange(
                            new ScheduledRange
                            {
                                idScheduledRange = scheduled.idScheduledRange,
                                cdStatus = resultClass.status ? 2 : 1,
                                startdate = scheduled.startdate,
                                endDate = scheduled.endDate,
                                Message = resultClass.message,
                                cdScheduledType = scheduled.cdScheduledType
                            });

                        if (!resultClass.status)
                        {
                            _log.Warn(resultClass.message);
                            result = false;
                        }
                    }

                }

            }

            return result;

        }

        #endregion


    }
}
