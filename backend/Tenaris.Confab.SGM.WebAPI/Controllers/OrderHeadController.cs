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
    [RoutePrefix("api/OrderHead")]
    public class OrderHeadController : ApiController
    {
        private IOrderHeadService _OrderHeadServ;
        private IPlantService _PlantServ;    
        private IMapper _Mapper;
        
        public OrderHeadController(IMapper Mapper, IOrderHeadService OrderHeadServ, IPlantService PlantServ)
        {
            _Mapper = Mapper;
            _OrderHeadServ = OrderHeadServ;
            _PlantServ = PlantServ;
            
        }



        [Route("GetOrderHead")]
        public List<OrderHeadViewModel> GetOrderHead(int idPlantGroup, int dtMonth, int dtYear, TypeStatus type, bool cdPredictive)
        {
            var dateStart = new DateTime(dtYear, dtMonth, 1);
            var dateEnd = dateStart.AddMonths(1).AddDays(-1);

            var plants = _PlantServ.GetPlantGroup(idPlantGroup).Plants.Select(s=>s.Sheet).Distinct().ToList();


            var orderHead = _OrderHeadServ.GetOrderHead(plants, dateStart, dateEnd, type, cdPredictive);
            return _Mapper.Map<List<OrderHead>, List<OrderHeadViewModel>>(orderHead);
        }

    }
}