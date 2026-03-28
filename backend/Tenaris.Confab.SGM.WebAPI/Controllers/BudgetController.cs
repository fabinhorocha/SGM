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
    [RoutePrefix("api/Budget")]
    public class BudgetController : ApiController
    {
        private IBudgetService _BudgetServ;
        private IMapper _Mapper;

        public BudgetController(IMapper Mapper, IBudgetService BudgetServ)
        {
            _Mapper = Mapper;
            _BudgetServ = BudgetServ;

        }
        

        [Route("GetBudgets")]
        public List<BudgetViewModel> GetBudgets(bool? active = null)
        {

            var budgets = _BudgetServ.GetBudgets(active);


          return _Mapper.Map<List<Budget>, List<BudgetViewModel>>(budgets);      

        }

        [Route("GetBudgetsActive")]
        public List<BudgetViewModel> GetBudgetsActive()
        {

            var budgets = _BudgetServ.GetBudgetsActive();


            return _Mapper.Map<List<Budget>, List<BudgetViewModel>>(budgets);

        }

        [Route("GetBudget")]
        public BudgetViewModel GetBudget(int id)
        {
           
            var budget = _BudgetServ.GetBudget(id);

            return _Mapper.Map<Budget, BudgetViewModel>(budget);

        }

        [Route("GetActualBudget")]
        public BudgetViewModel GetActualBudget()
        {
            var budget = _BudgetServ.GetActualBudget();

            return _Mapper.Map<Budget, BudgetViewModel>(budget);

        }

        [AuthorizeRolesConfiguration]
        [HttpPost]
        [Route("InsertBudget")]
        public object InsertBudget(BudgetViewModel budget)
        {
            var bud = _Mapper.Map<BudgetViewModel, Budget>(budget);            
            return _BudgetServ.InsertBudget(bud);
        }

        [AuthorizeRolesConfiguration]
        [HttpPost]
        [Route("UpdateBudget")]
        public object UpdateBudget(BudgetViewModel budget)
        {
            var bud = _Mapper.Map<BudgetViewModel, Budget>(budget);
            return _BudgetServ.UpdateBudget(bud);
        }

        [AuthorizeRolesConfiguration]
        [HttpPost]
        [Route("UpdateBudgetStatus")]
        public object UpdateBudgetStatus(BudgetViewModel budget)
        {
            var bud = _Mapper.Map<BudgetViewModel, Budget>(budget);
            return _BudgetServ.UpdateBudgetStatus(bud);
        }


    }
}