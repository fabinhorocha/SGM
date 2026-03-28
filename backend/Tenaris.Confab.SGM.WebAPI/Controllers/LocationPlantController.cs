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
    [RoutePrefix("api/LocationPlant")]
    public class LocationPlantController : ApiController
    {
        private ILocationPlantService _LocationPlantServ;
        private IMapper _Mapper;


        public LocationPlantController(IMapper Mapper, ILocationPlantService LocationPlantServ)
        {

            _Mapper = Mapper;
            _LocationPlantServ = LocationPlantServ;

        }

        [Route("GetLocationsPlants")]
        public List<LocationPlantViewModel> GetLocationsPlants()
        {

            var locationsPlants = _LocationPlantServ.GetLocationsPlants();

            return _Mapper.Map<List<LocationPlant>, List<LocationPlantViewModel>>(locationsPlants);

        }


    }
}
