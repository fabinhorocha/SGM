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
    [RoutePrefix("api/Priority")]
    public class PriorityController : ApiController
    {
        private IPriorityService _PriorityServ;
        private IMapper _Mapper;


        public PriorityController(IMapper Mapper, IPriorityService PriorityServ)
        {

            _Mapper = Mapper;
            _PriorityServ = PriorityServ;

        }

        [Route("GetPriorities")]
        public List<PriorityViewModel> GetPriorities()
        {

            var priorities = _PriorityServ.GetPriorities();

            return _Mapper.Map<List<Priority>, List<PriorityViewModel>>(priorities);

        }


    }
}
