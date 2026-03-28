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
    [RoutePrefix("api/Plant")]
    public class PlantController : ApiController
    {
        private IPlantService _PlantServ;
        private IMapper _Mapper;


        public PlantController(IMapper Mapper, IPlantService PlantServ)
        {

            _Mapper = Mapper;
            _PlantServ = PlantServ;

        }


        [Route("GetPlants")]
        public List<PlantViewModel> GetPlants()
        {
            var plants = _PlantServ.GetPlants();
            return _Mapper.Map<List<Plant>, List<PlantViewModel>>(plants);
        }

        [Route("GetPlantsSheet")]
        public List<string> GetPlantsSheet()
        {
            var plants = _PlantServ.GetPlants();
            return plants.Select(s => s.Sheet).Cast<string>().ToList();
        }


    }
}
