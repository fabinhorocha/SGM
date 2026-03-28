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
    [RoutePrefix("api/Area")]
    public class AreaController : ApiController
    {
        private IAreaService _AreaServ;
        private IMapper _Mapper;


        public AreaController(IMapper Mapper, IAreaService AreaServ)
        {

            _Mapper = Mapper;
            _AreaServ = AreaServ;

        }

        [Route("GetAreas")]
        public List<AreaViewModel> GetAreas()
        {

            var areas = _AreaServ.GetAreas();

            return _Mapper.Map<List<Area>, List<AreaViewModel>>(areas);

        }


    }
}
