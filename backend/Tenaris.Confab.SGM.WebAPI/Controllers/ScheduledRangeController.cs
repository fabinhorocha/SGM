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
    [RoutePrefix("api/ScheduledRange")]
    public class ScheduledRangeController : ApiController
    {
        private IScheduledRangeService _ScheduledRangeServ;
        private IMapper _Mapper;

        public ScheduledRangeController(IMapper Mapper, IScheduledRangeService ScheduledRangeServ)
        {
            _Mapper = Mapper;
            _ScheduledRangeServ = ScheduledRangeServ;
        }
        

        [AuthorizeRolesMaintance]
        [HttpPost]
        [Route("InsertScheduledRange")]
        public object InsertScheduledRange(ScheduledRangeViewModel scheduledRange)
        {
            var _scheduledRange = _ScheduledRangeServ.GetScheduledRangeByDate(scheduledRange.startdate, scheduledRange.endDate,(ScheduledRangeType)scheduledRange.cdScheduledType);

            if (_scheduledRange.Count == 0)
            {
                var scheduled = _Mapper.Map<ScheduledRangeViewModel, ScheduledRange>(scheduledRange);
                return _ScheduledRangeServ.InsertScheduledRange(scheduled);
            }
            else
            {
                return new { id = -1, status = true, message = "Já existe Range cadastrado para esse período !" };
            }
        }

        

    }
}