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
    [RoutePrefix("api/Location")]
    public class LocationController : ApiController
    {
        private ILocationService _LocationServ;
        private IMapper _Mapper;


        public LocationController(IMapper Mapper, ILocationService LocationServ)
        {

            _Mapper = Mapper;
            _LocationServ = LocationServ;

        }

        [Route("GetLocations")]
        public List<LocationViewModel> GetLocations()
        {

            var locations = _LocationServ.GetLocations();

            return _Mapper.Map<List<Location>, List<LocationViewModel>>(locations);

        }


    }
}
