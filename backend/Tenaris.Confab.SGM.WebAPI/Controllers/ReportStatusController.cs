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
    [RoutePrefix("api/ReportStatus")]
    public class ReportStatusController : ApiController
    {
        
        IMapper _Mapper;
        IReportStatusService _StatusServ;

        public ReportStatusController(IMapper Mapper, IReportStatusService StatusServ)
        {
            _Mapper = Mapper;
            _StatusServ = StatusServ;
        }


        [Route("GetReportsStatus")]
        public List<ReportStatusViewModel> GetReportsStatus(){

            var status = _StatusServ.GetReportsStatus();

            return _Mapper.Map<List<ReportStatus>, List<ReportStatusViewModel>>(status);

        }


    }
}
