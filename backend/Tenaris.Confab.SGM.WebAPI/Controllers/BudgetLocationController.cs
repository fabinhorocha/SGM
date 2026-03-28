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
    [RoutePrefix("api/BudgetLocation")]
    public class BudgetLocationController : ApiController
    {
        private IBudgetLocationService _BudgetLocationServ;
        private IMapper _Mapper;


        public BudgetLocationController(IMapper Mapper, IBudgetLocationService BudgetLocationServ)
        {

            _Mapper = Mapper;
            _BudgetLocationServ = BudgetLocationServ;

        }

        [AuthorizeRolesConfiguration]
        [HttpPost]
        [Route("UpdateBudgetLocation")]
        public object UpdateBudgetLocation(BudgetLocationViewModel budgetLocation)
        {
            var bud = _Mapper.Map<BudgetLocationViewModel, BudgetLocation>(budgetLocation);
            return _BudgetLocationServ.UpdateBudgetLocation(bud);
        }


    }
}
