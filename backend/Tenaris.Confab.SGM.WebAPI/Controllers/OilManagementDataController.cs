using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Services;
using Tenaris.Confab.SGM.WebAPI.Models;

namespace Tenaris.Confab.SGM.WebAPI.Controllers
{
    [RoutePrefix("api/OilManagementData")]
    public class OilManagementDataController : ApiController
    {
        private IComponentService _ComponentServ;
        private IOilSupplyService _OilSupplyServ;
        private IEquipmentService _EquipmenServ;
        private IOilTypeService _OilTypeServ;
        private IOilSupplyTypeService _OilSupplyTypeServ;
        private IStoppageTypeService _StoppageTypeServ;
        private IOilManagementReportService _OilManagementReportServ;
        private IOilManagementAlarmService _OilManagementAlarmServ;
        private IOilManagementIndicatorService _OilManagementIndicatorServ;


        private IMapper _Mapper;

        public OilManagementDataController(IMapper Mapper, IComponentService ComponentServ, IOilSupplyService OilSupplyServ, IEquipmentService EquipmenServ, IOilTypeService OilTypeServ, IOilSupplyTypeService OilSupplyTypeServ, IStoppageTypeService StoppageTypeServ, IOilManagementReportService OilManagementReportServ, IOilManagementAlarmService OilManagementAlarmServ, IOilManagementIndicatorService OilManagementIndicatorServ)
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
            _OilManagementIndicatorServ = OilManagementIndicatorServ;
        }

        [Route("GetFilterInfo")]
        public OilManagementIndicatorViewModel GetFilterInfo()
        {
            var oilTypes = new List<OilType>() { new OilType() { idOilType = -1, Code = "", Name = "-Todos-", Active = true } };
            oilTypes.AddRange(_OilTypeServ.GetOilTypes());

            var oilSupplyTypes = new List<OilSupplyType>() { new OilSupplyType() { idOilSupplyType = -1, Code = "", Name = "-Todos-", Active = true } };
            oilSupplyTypes.AddRange(_OilSupplyTypeServ.GetOilSupplyTypes());

            var stoppageTypes = new List<StoppageType>() { new StoppageType() { idStoppageType = -1, Code = "", Name = "-Todos-", Active = true } };
            stoppageTypes.AddRange(_StoppageTypeServ.GetStoppageTypes());

            return new OilManagementIndicatorViewModel()
            {
                OilTypes = oilTypes,
                OilSupplyTypes = oilSupplyTypes,
                StoppageTypes = stoppageTypes
            };
        }

        [Route("GetOilSupplyData")]
        public List<object> GetOilSupplyData(int idReportClass, int idReportType, int idFactory, int idArea, int idEquipment, int idComponent, DateTime startDate, DateTime endDate, int idOilType, int idOilSupplyType, int idStoppageType, bool OilManagement, bool OilUser)
        {
            var indicatorData = new List<object>();

            if (!OilManagement)
                return null;

            var idFactoryFormatted = idFactory == -1 ? (int?)null : idFactory;
            var idAreaFormatted = idArea == -1 ? (int?)null : idArea;
            var idEquipmentFormatted = idEquipment == -1 ? (int?)null : idEquipment;
            var idComponentFormatted = idComponent == -1 ? (int?)null : idComponent;
            var startDateFormatted = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            var endDateFormatted = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
            var idOilTypeFormatted = idOilType == -1 ? (int?)null : idOilType;
            var idOilSupplyTypeFormatted = idOilSupplyType == -1 ? (int?)null : idOilSupplyType;
            var idStoppageTypeFormatted = idStoppageType == -1 ? (int?)null : idStoppageType;


            indicatorData = _OilManagementIndicatorServ.GetOilManagementData(idReportClass, idReportType, idFactoryFormatted, idAreaFormatted, idEquipmentFormatted, idComponentFormatted, idOilTypeFormatted, idOilSupplyTypeFormatted, idStoppageTypeFormatted, startDate, endDate);

            return indicatorData;
        }
    }
}