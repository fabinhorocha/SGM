using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Services;
using Tenaris.Confab.SGM.WebAPI.Models;

namespace Tenaris.Confab.SGM.WebAPI.Controllers
{
    [RoutePrefix("api/OilManagement")]
    public class OilManagementController : ApiController
    {
        private IComponentService _ComponentServ;
        private IOilSupplyService _OilSupplyServ;
        private IEquipmentService _EquipmenServ;
        private IOilTypeService _OilTypeServ;
        private IOilSupplyTypeService _OilSupplyTypeServ;
        private IStoppageTypeService _StoppageTypeServ;
        private IOilManagementReportService _OilManagementReportServ;
        private IOilManagementAlarmService _OilManagementAlarmServ;

        private IMapper _Mapper;

        public OilManagementController(IMapper Mapper, IComponentService ComponentServ, IOilSupplyService OilSupplyServ, IEquipmentService EquipmenServ, IOilTypeService OilTypeServ, IOilSupplyTypeService OilSupplyTypeServ, IStoppageTypeService StoppageTypeServ, IOilManagementReportService OilManagementReportServ, IOilManagementAlarmService OilManagementAlarmServ)
        {
            _Mapper = Mapper;
            _ComponentServ = ComponentServ;
            _OilSupplyServ = OilSupplyServ;
            _EquipmenServ = EquipmenServ;
            _OilTypeServ = OilTypeServ;
            _OilSupplyTypeServ = OilSupplyTypeServ;
            _StoppageTypeServ = StoppageTypeServ;
            _OilManagementReportServ = OilManagementReportServ;
            _OilManagementAlarmServ = OilManagementAlarmServ;
        }

        [Route("GetAllEquipments")]
        public OilManagementEquipmentViewModel GetAllEquipments(bool status)
        {
            var equipmentViewModel = new OilManagementEquipmentViewModel()
            {
                Factories = _ComponentServ.GetAllFactories(),
                Areas = _ComponentServ.GetAllAreas(),
                Equipments = _ComponentServ.GetAllEquipments()
            };

            return equipmentViewModel;
        }

        [Route("GetComponents")]
        public List<Component> GetComponents()
        {
            var components = _ComponentServ.GetComponents();
            return components;
        }

        [Route("GetComponentsSettings")]
        public object GetComponentsSettings(string userCode, int idFactory, int idArea, int idEquipment, int idComponent, int reportType, bool onlyActives, bool OilManagement, bool OilUser)
        {
            //if (!OilManagement)
            //    return null;

            var idFactoryFormatted = idFactory == -1 ? (int?)null : idFactory;
            var idAreaFormatted = idArea == -1 ? (int?)null : idArea;
            var idEquipmentFormatted = idEquipment == -1 ? (int?)null : idEquipment;
            var idComponentFormatted = idComponent == -1 ? (int?)null : idComponent;

            var result = _ComponentServ.GetComponents(idFactoryFormatted, idAreaFormatted, idEquipmentFormatted, idComponentFormatted);

            var idReport = 2;
            var cols = _OilManagementReportServ.GetReportLayout(idReport);

            var filters = new List<object>();
            var filterColumns = cols.Where(x => x.haveFilterFunctionality).Select(y => y.id).ToList();

            var columnsToExport = cols.Where(y => y.id != "idOilSupply").Select(x => x.name).ToArray();

            foreach (var repo in result)
            {
                var filterValue = new List<object>();
                try
                {
                    filters.Add(
                        new
                        {
                            factoryName = repo.Equipment.Area.Factory.Name,
                            areaName = repo.Equipment.Area.Name,
                            equipmentName = repo.Equipment.Name,
                            componentName = repo.Name,
                            Capacity = repo.Capacity.ToString(),
                            OilTypes = string.Join("/", repo.EnabledOilTypes.Select(x => x.Name).ToList()),
                            FlowRate = repo.FlowRate.ToString(),
                            Preassure = repo.Preassure.ToString(),
                            Active = repo.Active ? "SIM" : "NÃO",
                            OilGradeISO = repo.OilGradeISO == null ? "" : repo.OilGradeISO,
                            ISOLimitCode = repo.ISOLimitCode == null ? "" : repo.ISOLimitCode,
                            CriticalComponent = repo.CriticalComponent == null ? "" : repo.CriticalComponent,
                            MachinesServed = repo.MachinesServed == null ? "" : repo.MachinesServed,
                            Columns = cols.Select(x => x.id).ToList()
                        }
                   );
                }
                catch (Exception ex)
                {
                }
            }

            return new { cols = cols, filters = filters, filterColumns = filterColumns, columnsToExport = columnsToExport };
        }
        [Route("GetLastOilSupplyHistory")]
        public object GetLastOilSupplyHistory(string userCode, int idEquipment, int idComponent)
        {
            //var userAdmin = _UserServ.GetAdministrators().Where(x => x.Domain + @"\" + x.Login == userCode).FirstOrDefault();
            //if (userAdmin != null)
            var reports = _OilSupplyServ.GetOilSupplyHistory(idEquipment, idComponent);

            var cols = new List<string>() { "id", "Unidade Hidráulica", "Equipamento", "Quantitade", "Capacidade", "Data Inserção", "Data Abastecimento", "Observação", "Tipo de Óleo", "Geró Parada", "Turno", "Inserido por", "Ult. Modif." };
            var rows = new List<object>();

            foreach (var repo in reports)
            {
                var values = new List<object>();
                values.Add(repo.idOilSupply); //id
                values.Add(repo.Component.Name); //UH
                values.Add(repo.Component.Equipment.Name); //Equipamento
                values.Add(repo.Quantity.ToString()); //Quantidade
                values.Add(repo.Component.Capacity.ToString()); //Capacidade
                values.Add(repo.InsDateTime.ToString("dd/MM/yyyy")); //Data de Inserção
                values.Add(repo.SupplyDateTime.ToString("dd/MM/yyyy")); //Data Abastecimento
                values.Add(repo.Comment); //Obsercação
                values.Add(repo.OilType.Name); //Tipo de Óleo
                values.Add(repo.StoppageType == null ? "" : repo.StoppageType.Name); //Tipo de Parada
                values.Add(repo.Shift.ToString()); //Turno
                values.Add(repo.CreatedBy); //Usuário
                values.Add(repo.ModificatedBy); //Usuário

                rows.Add(values);
            }


            return new { cols = cols, rows = rows };
        }

        [Route("GetOilSupplyHistory")]
        public object GetOilSupplyHistory(string userCode, int idFactory, int idArea, int idEquipment, int idComponent, int reportType, bool onlyActives, DateTime startDate, DateTime endDate, bool OilManagement, bool OilUser)
        {
            //if (!OilManagement)
            //    return null;

            var idFactoryFormatted = idFactory == -1 ? (int?)null : idFactory;
            var idAreaFormatted = idArea == -1 ? (int?)null : idArea;
            var idEquipmentFormatted = idEquipment == -1 ? (int?)null : idEquipment;
            var idComponentFormatted = idComponent == -1 ? (int?)null : idComponent;
            var startDateFormatted = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            var endDateFormatted = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            var result = _OilSupplyServ.GetOilSupplyHistory(idFactoryFormatted, idAreaFormatted, idEquipmentFormatted, idComponentFormatted, startDateFormatted, endDateFormatted, false);

            var idReport = 1;
            var cols = _OilManagementReportServ.GetReportLayout(idReport);

            var filters = new List<object>();
            var filterColumns = cols.Where(x => x.haveFilterFunctionality).Select(y => y.id).ToList();

            var columnsToExport = cols.Where(y => y.id != "idOilSupply").Select(x => x.name).ToArray();

            foreach (var repo in result)
            {
                var filterValue = new List<object>();
                try
                {
                    filters.Add(
                        new
                        {
                            idOilSupply = repo.idOilSupply.ToString(),
                            factoryName = repo.Component.Equipment.Area.Factory.Name,
                            areaName = repo.Component.Equipment.Area.Name,
                            equipmentName = repo.Component.Equipment.Name,
                            componentName = repo.Component.Name,
                            Quantity = repo.Quantity.ToString(),
                            Capacity = repo.Component.Capacity.ToString(),
                            SupplyDateTime = repo.SupplyDateTime.ToString("dd/MM/yyyy"),
                            Shift = repo.Shift.ToString(),
                            OilType = repo.OilType.Name,
                            OilSupplyType = repo.OilSupplyType.Name,
                            Comment = repo.Comment,
                            StoppageType = repo.StoppageType == null ? "" : repo.StoppageType.Name,
                            StoppageTime = repo.StoppageTime == 0 ? "" : repo.StoppageTime.ToString(),
                            IsReuseOil = repo.IsReuseOil ? "SIM" : "NÃO",
                            CreatedBy = repo.CreatedBy,
                            InsDateTime = repo.InsDateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                            ModificatedBy = repo.ModificatedBy,
                            UpdDateTime = repo.UpdDateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                            FlowRate = repo.Component.FlowRate.ToString(),
                            Preassure = repo.Component.Preassure.ToString(),
                            Active = repo.Component.Active ? "SIM" : "NÃO",
                            OilGradeISO = repo.Component.OilGradeISO == null ? "" : repo.Component.OilGradeISO,
                            ISOLimitCode = repo.Component.ISOLimitCode == null ? "" : repo.Component.ISOLimitCode,
                            CriticalComponent = repo.Component.CriticalComponent == null ? "" : repo.Component.CriticalComponent,
                            MachinesServed = repo.Component.MachinesServed == null ? "" : repo.Component.MachinesServed,
                            Columns = cols.Select(x => x.id).ToList()
                        }
                   );
                }
                catch (Exception ex)
                {
                }
            }

            return new { cols = cols, filters = filters, filterColumns = filterColumns, columnsToExport = columnsToExport };
        }

        [Route("GetNewEmptyComponent")]
        public ComponentViewModel GetNewEmptyComponent(int idEquipment)
        {
            var comp = new ComponentViewModel()
            {
                idComponent = -1,
                Equipment = _ComponentServ.GetAllEquipments().Where(x => x.idEquipment == idEquipment).FirstOrDefault(),
                EnabledOilTypes = null,
                Active = true,
                InsDateTime = DateTime.Now,
                UpdDateTime = DateTime.Now,
                cdIntegrate = false,
                OilTypes = _OilTypeServ.GetOilTypes()
            };

            return comp;
        }

        [Route("GetComponent")]
        public ComponentViewModel GetComponent(int idComponent)
        {
            var comp = _ComponentServ.GetComponent(idComponent);
            ComponentViewModel compViewModel = null;
            try
            {
                compViewModel = _Mapper.Map<Component, ComponentViewModel>(comp);
                compViewModel.OilTypes = _OilTypeServ.GetOilTypes();
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }

            return compViewModel;
        }

        [Route("NewComponent")]
        public object NewComponent(ComponentViewModel newComponentViewModel)
        {
            if (!newComponentViewModel.OilManagement && !newComponentViewModel.OilUser)
            {
                return new
                {
                    status = false,
                    message = "O usuário não tem permissões para registro de Unidades Hidráulicas!"
                };
            }

            object result = null;
            if (newComponentViewModel.idComponent == -1)
            {
                var newComponent = _Mapper.Map<ComponentViewModel, Component>(newComponentViewModel);
                try
                {
                    result = _ComponentServ.Insert(newComponent);
                }
                catch (Exception ex)
                {
                    result = new
                    {
                        status = false,
                        message = ex.Message
                    };
                }
            }
            return result;
        }

        [Route("EditComponent")]
        public object EditComponent(Component editComponent)
        {
            object result = null;
            if (editComponent.idComponent != -1)
            {
                try
                {
                    result = _ComponentServ.Update(editComponent, editComponent.ModificatedBy);
                }
                catch (Exception ex)
                {
                    result = new
                    {
                        status = false,
                        message = ex.Message
                    };
                }

            }
            return result;
        }

        [Route("GetNewEmptyOilSupply")]
        public OilSupplyViewModel GetNewEmptyOilSupply(int idComponent)
        {
            var comp = new OilSupplyViewModel()
            {
                idOilSupply = -1,
                Component = _ComponentServ.GetComponent(idComponent),
                OilType = null,
                StoppageType = null,
                InsDateTime = DateTime.Now,
                UpdDateTime = DateTime.Now,
                OilTypes = _OilTypeServ.GetOilTypes(),
                OilSupplyTypes = _OilSupplyTypeServ.GetOilSupplyTypes(),
                StoppageTypes = _StoppageTypeServ.GetStoppageTypes()
            };

            return comp;
        }

        [Route("GetOilSupply")]
        public OilSupplyViewModel GetOilSupply(int idOilSupply, string cdUser, bool OilManagement, bool OilUser)
        {
            var oil = _OilSupplyServ.GetOilSupply(idOilSupply);
            OilSupplyViewModel oilViewModel = null;
            try
            {
                oilViewModel = _Mapper.Map<OilSupply, OilSupplyViewModel>(oil);
                oilViewModel.Component = _ComponentServ.GetComponent(oilViewModel.Component.idComponent);
                oilViewModel.OilTypes = _OilTypeServ.GetOilTypes();
                oilViewModel.OilSupplyTypes = _OilSupplyTypeServ.GetOilSupplyTypes();
                oilViewModel.StoppageTypes = _StoppageTypeServ.GetStoppageTypes();

                //check permissions
                if (OilManagement)
                {
                    oilViewModel.EditionEnabled = true;
                    oilViewModel.ViewEnabled = true;
                }
                else if (OilUser)
                {
                    if (oil.CreatedBy == cdUser)
                    {
                        var diff = DateTime.Now.Subtract(oil.InsDateTime).TotalHours;
                        if (diff < 30)
                        {
                            oilViewModel.EditionEnabled = true;
                            oilViewModel.ViewEnabled = true;
                        }
                        else
                        {
                            oilViewModel.EditionEnabled = false;
                            oilViewModel.ViewEnabled = true;
                        }
                    }
                    else
                    {
                        oilViewModel.EditionEnabled = false;
                        oilViewModel.ViewEnabled = true;
                    }
                }
                else
                {
                    oilViewModel.EditionEnabled = false;
                    oilViewModel.ViewEnabled = false;
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }

            return oilViewModel;
        }

        [HttpPost]
        [Route("NewOilSupply")]
        public object NewOilSupply(OilSupplyViewModel newOilSupplyViewModel)
        {
            if (!newOilSupplyViewModel.OilManagement && !newOilSupplyViewModel.OilUser)
            {
                return new
                {
                    status = false,
                    message = "O usuário não tem permissões para registro de abastecimento!"
                };
            }

            object result = null;
            if (newOilSupplyViewModel.idOilSupply == -1)
            {
                var newOilSupply = _Mapper.Map<OilSupplyViewModel, OilSupply>(newOilSupplyViewModel);
                newOilSupply.CreatedBy = newOilSupplyViewModel.cdUser;
                try
                {
                    result = _OilSupplyServ.Insert(newOilSupply);
                }
                catch (Exception ex)
                {
                    result = new
                    {
                        status = false,
                        message = ex.Message
                    };
                }

            }
            return result;
        }

        [Route("EditOilSupply")]
        public object EditOilSupply(OilSupplyViewModel editOilSupplyViewModel)
        {
            if (!editOilSupplyViewModel.OilManagement && !editOilSupplyViewModel.OilUser)
            {
                return new
                {
                    status = false,
                    message = "O usuário não tem permissões para editar abastecimentos!"
                };
            }

            object result = null;
            if (editOilSupplyViewModel.idOilSupply != -1)
            {
                var editOilSupply = _Mapper.Map<OilSupplyViewModel, OilSupply>(editOilSupplyViewModel);
                try
                {
                    result = _OilSupplyServ.Update(editOilSupply, editOilSupplyViewModel.cdUser);
                }
                catch (Exception ex)
                {
                    result = new
                    {
                        status = false,
                        message = ex.Message
                    };
                }
            }
            return result;
        }

        [Route("GetAlarmsByComponent")]
        public OilManagementAlarmsByComponentViewModel GetAlarmByComponent(string userCode, int idComponent, bool OilManagement, bool OilUser)
        {
            //if (!OilManagement)
            //    return null;

            var alarms = (from a in _OilManagementAlarmServ.GetAlarmsByComponent(idComponent)
                          select new OilManagementAlarmViewModel()
                          {
                              Alarm = a,
                              GroupsAll = _OilManagementAlarmServ.GetAlarmGroups(true)
                              .Where(x =>
                                !a.Groups.Select(y => y.idAlarmGroup).ToList().Contains(x.idAlarmGroup)
                                && !a.GroupsCC.Select(y => y.idAlarmGroup).ToList().Contains(x.idAlarmGroup)
                              ).ToList(),
                              Edited = false
                          }).ToList();

            var alarmViewModel = new OilManagementAlarmsByComponentViewModel()
            {
                Component = _ComponentServ.GetComponent(idComponent),
                Alarms = alarms,
                AlarmTypes = _OilManagementAlarmServ.GetAlarmTypes(),
                EditionEnabled = OilManagement
            };

            return alarmViewModel;
        }

        [Route("GetAlarms")]
        public object GetAlarms(string userCode, int idFactory, int idArea, int idEquipment, int idComponent, bool onlyActives, bool OilManagement, bool OilUser)
        {
            //if (!OilManagement)
            //    return null;

            var idFactoryFormatted = idFactory == -1 ? (int?)null : idFactory;
            var idAreaFormatted = idArea == -1 ? (int?)null : idArea;
            var idEquipmentFormatted = idEquipment == -1 ? (int?)null : idEquipment;
            var idComponentFormatted = idComponent == -1 ? (int?)null : idComponent;

            var result = _OilManagementAlarmServ.GetAlarms(idFactoryFormatted, idAreaFormatted, idEquipmentFormatted, idComponentFormatted, onlyActives);

            var idReport = 3;
            var cols = _OilManagementReportServ.GetReportLayout(idReport);

            var filters = new List<object>();
            var filterColumns = cols.Where(x => x.haveFilterFunctionality).Select(y => y.id).ToList();

            var columnsToExport = cols.Where(y => y.id != "idAlarm").Select(x => x.name).ToArray();

            foreach (var repo in result)
            {
                var filterValue = new List<object>();
                try
                {
                    filters.Add(
                        new
                        {
                            idAlarm = repo.idAlarm.ToString(),
                            factoryName = repo.Component.Equipment.Area.Factory.Name,
                            areaName = repo.Component.Equipment.Area.Name,
                            equipmentName = repo.Component.Equipment.Name,
                            componentName = repo.Component.Name,
                            alarmTypeName = repo.AlarmType.Name,
                            CreatedBy = repo.CreatedBy,
                            InsDateTime = repo.InsDateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                            ModificatedBy = repo.ModificatedBy,
                            UpdDateTime = repo.UpdDateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                            Columns = cols.Select(x => x.id).ToList()
                        }
                   );
                }
                catch (Exception ex)
                {
                }
            }

            return new { cols = cols, filters = filters, filterColumns = filterColumns, columnsToExport = columnsToExport };
        }

        [Route("GetAlarmsHistory")]
        public object GetAlarmsHistory(string userCode, int idFactory, int idArea, int idEquipment, int idComponent, bool onlyActives, DateTime startDate, DateTime endDate, bool OilManagement, bool OilUser)
        {
            //if (!OilManagement)
            //    return null;

            var idFactoryFormatted = idFactory == -1 ? (int?)null : idFactory;
            var idAreaFormatted = idArea == -1 ? (int?)null : idArea;
            var idEquipmentFormatted = idEquipment == -1 ? (int?)null : idEquipment;
            var idComponentFormatted = idComponent == -1 ? (int?)null : idComponent;
            var startDateFormatted = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            var endDateFormatted = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            var result = _OilManagementAlarmServ.GetAlarmsHistory(idFactoryFormatted, idAreaFormatted, idEquipmentFormatted, idComponentFormatted, onlyActives, startDateFormatted, endDateFormatted);

            var idReport = 4;
            var cols = _OilManagementReportServ.GetReportLayout(idReport);

            var filters = new List<object>();
            var filterColumns = cols.Where(x => x.haveFilterFunctionality).Select(y => y.id).ToList();

            var columnsToExport = cols.Where(y => y.id != "idAlarmHistory").Select(x => x.name).ToArray();

            foreach (var repo in result)
            {
                var filterValue = new List<object>();
                try
                {
                    filters.Add(
                        new
                        {
                            idAlarmHistory = repo.idAlarmHistory.ToString(),
                            alarmTypeName = repo.Alarm.AlarmType.Name,
                            factoryName = repo.Alarm.Component.Equipment.Area.Factory.Name,
                            areaName = repo.Alarm.Component.Equipment.Area.Name,
                            equipmentName = repo.Alarm.Component.Equipment.Name,
                            componentName = repo.Alarm.Component.Name,
                            InsDateTime = repo.InsDateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                            Quantity = repo.OilSupply.Quantity.ToString(),
                            SupplyDateTime = repo.OilSupply.SupplyDateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                            Columns = cols.Select(x => x.id).ToList()
                        }
                   );
                }
                catch (Exception ex)
                {
                }
            }

            return new { cols = cols, filters = filters, filterColumns = filterColumns, columnsToExport = columnsToExport };
        }

        [Route("EditAlarmsByComponent")]
        public object EditAlarmsByComponent(OilManagementAlarmsByComponentViewModel alarms)
        {
            object result = null;

            var alarmsToUpdate = alarms.Alarms.Where(x => x.Edited).Select(a => a.Alarm).ToList();
            result = _OilManagementAlarmServ.UpdateAlarmsByComponent(alarms.Component.idComponent, ref alarmsToUpdate, alarms.CreatedBy, alarms.ModificatedBy);

            return result;
        }
        [Route("EditGroupsByAlarm")]
        public object EditGroupsByAlarm(OilManagementAlarmViewModel alarmViewModel)
        {
            object result = null;

            result = _OilManagementAlarmServ.UpdateGroups(alarmViewModel.Alarm);

            return result;
        }

        [Route("GetAlarmGroup")]
        public AlarmGroupViewModel GetAlarmGroup(int idAlarmGroup, string userCode, bool OilManagement, bool OilUser)
        {
            var alarmGroup = _OilManagementAlarmServ.GetAlarmGroup(idAlarmGroup);
            AlarmGroupViewModel alarmGroupViewModel = null;
            try
            {
                var ids = alarmGroup.Users.Select(x => x.idUser).ToList();
                alarmGroupViewModel = _Mapper.Map<OilManagementAlarmGroup, AlarmGroupViewModel>(alarmGroup);
                alarmGroupViewModel.UsersAll = _OilManagementAlarmServ.GetUsersAll().Where(x => !ids.Contains(x.idUser)).ToList();
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }

            return alarmGroupViewModel;
        }
        [Route("GetNewEmptyAlarmGroup")]
        public AlarmGroupViewModel GetNewEmptyAlarmGroup()
        {
            var group = new AlarmGroupViewModel()
            {
                idAlarmGroup = -1,
                Name = null,
                Active = true,
                InsDateTime = DateTime.Now,
                UpdDateTime = null,
                UsersAll = _OilManagementAlarmServ.GetUsersAll(),
                Users = new List<OilManagementUser>()
            };

            return group;
        }

        [Route("NewAlarmGroup")]
        public object NewAlarmGroup(AlarmGroupViewModel newAlarmGroupViewModel)
        {
            object result = null;
            if (newAlarmGroupViewModel.idAlarmGroup == -1)
            {
                try
                {
                    var newAlarmGroup = _Mapper.Map<AlarmGroupViewModel, OilManagementAlarmGroup>(newAlarmGroupViewModel);
                    result = _OilManagementAlarmServ.Insert(newAlarmGroup);
                }
                catch (Exception ex)
                {
                    result = new
                    {
                        status = false,
                        message = ex.Message
                    };
                }

            }
            return result;
        }

        [Route("EditAlarmGroup")]
        public object EditAlarmGroup(AlarmGroupViewModel editedAlarmGroupViewModel)
        {
            object result = null;
            if (editedAlarmGroupViewModel.idAlarmGroup > -1)
            {
                try
                {
                    var editedAlarmGroup = _Mapper.Map<AlarmGroupViewModel, OilManagementAlarmGroup>(editedAlarmGroupViewModel);
                    result = _OilManagementAlarmServ.Update(editedAlarmGroup);
                }
                catch (Exception ex)
                {
                    result = new
                    {
                        status = false,
                        message = ex.Message
                    };
                }

            }
            return result;
        }

        [Route("GetAlarmGroups")]
        public object GetAlarmGroups(string userCode, bool onlyActives, bool OilManagement, bool OilUser)
        {
            //if (!OilManagement)
            //    return null;

            var result = _OilManagementAlarmServ.GetAlarmGroups(onlyActives);

            var idReport = 5;
            var cols = _OilManagementReportServ.GetReportLayout(idReport);

            var filters = new List<object>();
            var filterColumns = cols.Where(x => x.haveFilterFunctionality).Select(y => y.id).ToList();

            var columnsToExport = cols.Where(y => y.id != "idAlarmGroup").Select(x => x.name).ToArray();

            foreach (var repo in result)
            {
                var filterValue = new List<object>();
                try
                {
                    filters.Add(
                        new
                        {
                            idAlarmGroup = repo.idAlarmGroup.ToString(),
                            alarmGroupName = repo.Name,
                            InsDateTime = repo.InsDateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                            UpdDateTime = repo.UpdDateTime.HasValue ? repo.UpdDateTime.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                            Active = repo.Active ? "SIM" : "NÃO",
                            Columns = cols.Select(x => x.id).ToList()
                        }
                   );
                }
                catch (Exception ex)
                {
                }
            }

            return new { cols = cols, filters = filters, filterColumns = filterColumns, columnsToExport = columnsToExport };
        }

    }
}
