using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Services;
using Tenaris.Confab.SGM.WebAPI.Filter;
using Tenaris.Confab.SGM.WebAPI.Models;

namespace Tenaris.Confab.SGM.WebAPI.Controllers
{
    [RoutePrefix("api/BudgetPlant")]
    public class BudgetPlantController : ApiController
    {
        private IBudgetPlantService _BudgetPlantServ;
        private IMapper _Mapper;


        public BudgetPlantController(IMapper Mapper, IBudgetPlantService BudgetPlantServ)
        {

            _Mapper = Mapper;
            _BudgetPlantServ = BudgetPlantServ;

        }

        [AuthorizeRolesConfiguration]
        [HttpPost]
        [Route("UpdateBudgetPlant")]
        public object UpdateBudgetPlant(BudgetPlantViewModel budgetPlant)
        {
            var bud = _Mapper.Map<BudgetPlantViewModel, BudgetPlant>(budgetPlant);
            return _BudgetPlantServ.UpdateBudgetPlant(bud);
        }


    }
}
