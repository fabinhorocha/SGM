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
    [RoutePrefix("api/ReportType")]
    public class ReportTypeController : ApiController
    {
        
        IMapper _Mapper;
        IReportTypeService _TypeServ;

        public ReportTypeController(IMapper Mapper, IReportTypeService TypeServ)
        {
            _Mapper = Mapper;
            _TypeServ = TypeServ;
        }


        [Route("GetReportsTypes")]
        public List<ReportTypeViewModel> GetReportsTypes(){

            var types = _TypeServ.GetReportsTypes();

            return _Mapper.Map<List<ReportType>, List<ReportTypeViewModel>>(types);

        }


    }
}
