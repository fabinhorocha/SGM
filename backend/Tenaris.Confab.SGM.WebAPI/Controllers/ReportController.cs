using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Services;
using AutoMapper;
using Tenaris.Confab.SGM.WebAPI.Models;
using System.Web.Http.Cors;
using System.Dynamic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.IO;
using Tenaris.Confab.SGM.WebAPI.Filter;

namespace Tenaris.Confab.SGM.WebAPI.Controllers
{
    [RoutePrefix("api/Report")]
    public class ReportController : ApiController
    {
        private IReportService _ReportServ;
        private IMapper _Mapper;

        public ReportController(IMapper Mapper, IReportService ReportServ)
        {
            _Mapper = Mapper;
            _ReportServ = ReportServ;

        }

        [Route("GetReportsByDateRange")]
        public object GetReportsByDateRange(DateTime startDate, DateTime endDate, int idEquipment, int? idType = null)
        {
            var reports = _ReportServ.GetReportsByDateRange(startDate, endDate, idEquipment, idType);

            var cols = new object();
            var rows = new List<object>();

            foreach (var repo in reports)
            {
                cols = ((IDictionary<string, object>)repo).Keys;

                var values = new List<object>();
                foreach (var val in ((IDictionary<string, object>)repo).Values)
                {
                    try
                    {
                        values.Add(JsonConvert.DeserializeObject(val.ToString()));
                    }
                    catch (Exception ex)
                    {
                        values.Add(val);
                    }

                }

                rows.Add(values);
            }


            return new { cols = cols, rows = rows };
        }

        [Route("GetMeasurementsByDateRange")]
        public object GetMeasurementsByDateRange(DateTime startDate, DateTime endDate, int idEquipment, int? idType = null)
        {
            var reports = _ReportServ.GetMeasurementsByDateRange(startDate, endDate, idEquipment, idType);

            var labels = new List<string>();
            var colors = new List<string>();
            var series = new List<string>();
            var data = new List<List<int>>();

            foreach (var repo in reports)
            {

                  int index = 0;
                foreach (var key in ((IDictionary<string, object>)repo).Keys)
                {
                    if (index > 0 && !labels.Contains(key.ToString()))
                        labels.Add(key.ToString());

                    index = index + 1;
                }

                index = 0;
                var values = new List<int>();
                foreach (var val in ((IDictionary<string, object>)repo).Values)
                {
                    if (index == 0)
                    {
                        series.Add(val.ToString());

                        switch (val.ToString())
                        {
                            case "Aceitável":
                                colors.Add("#739e73");
                                break;
                            case "Em Alerta":
                                colors.Add("#c79121");
                                break;
                            case "Em Perigo":
                                colors.Add("#a90329");
                                break;

                        }
                    }

                    else
                        values.Add(Convert.ToInt32(val));

                    index = index + 1;
                }

                data.Add(values);
            }


            return new { labels, series, data, colors };
        }

        [Route("GetAnalysisValues")]
        public List<object> GetAnalysisValues(DateTime startDate, DateTime endDate, int idEquipment, int idType)
        {
            
          return  _ReportServ.GetAnalysisValues(startDate, endDate, idEquipment, idType);      

        }

        [Route("GetReportsOTs")]
        public List<ReportViewModel> GetReportsOTs(DateTime startDate, DateTime endDate)
        {
            var reports = _ReportServ.GetReportsOTs(startDate, endDate);
            return _Mapper.Map<List<Report>, List<ReportViewModel>>(reports);

        }

        [Route("GetReport")]
        public ReportViewModel GetReport(int id)
        {
            var report = _ReportServ.GetReport(id);

            return _Mapper.Map<Report, ReportViewModel>(report);


        }

        [AuthorizeRolesMaintance]
        [HttpPost]
        [Route("InsertReport")]
        public object InsertReport(ReportViewModel report)
        {
            var rpt = _Mapper.Map<ReportViewModel, Report>(report);            
            return _ReportServ.InsertReport(rpt, HttpContext.Current.Server.MapPath("~/Uploads"));
        }

        [AuthorizeRolesMaintance]
        [HttpPost]
        [Route("UpdateReport")]
        public object UpdateReport(ReportViewModel report)
        {
            var rpt = _Mapper.Map<ReportViewModel, Report>(report);
            //return _ReportServ.UpdateReport(rpt, HttpContext.Current.Server.MapPath("~/Uploads"));
            return _ReportServ.UpdateReport(rpt);
        }

        [AuthorizeRolesMaintance]
        [HttpPost]
        [Route("DeleteReport")]
        public object DeleteReport(ReportViewModel report)
        {
            var rpt = _Mapper.Map<ReportViewModel, Report>(report);
            return _ReportServ.DeleteReport(rpt);
        }

        [AuthorizeRolesMaintance]
        [HttpPost]
        [Route("CreateNoteSAP")]
        public object CreateNoteSAP(ReportViewModel report)
        {
            var rpt = _Mapper.Map<ReportViewModel, Report>(report);
            return _ReportServ.CreateNoteSAP(rpt, HttpContext.Current.Server.MapPath("~/Uploads"));
        }
     
        [HttpPost]
        [Route("CreateNotesSAP")]
        public List<object> CreateNotesSAP()
        {
            var listObjects = new List<object>();
            var listReports = _ReportServ.GetReportsPendentsSAP();

            foreach (var rpt in listReports)
            {
                listObjects.Add(_ReportServ.CreateNoteSAP(rpt, HttpContext.Current.Server.MapPath("~/Uploads")));
            }

            return listObjects;
        }

    }
}