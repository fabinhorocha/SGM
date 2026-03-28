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
    [RoutePrefix("api/Notification")]
    public class NotificationController : ApiController
    {
        private INotificationService _NotificationServ;
        private IPlantService _PlantServ;    
        private IMapper _Mapper;
        
        public NotificationController(IMapper Mapper, INotificationService NotificationServ, IPlantService PlantServ)
        {
            _Mapper = Mapper;
            _NotificationServ = NotificationServ;
            _PlantServ = PlantServ;
            
        }



        [Route("GetNotification")]
        public List<object> GetNotification(int idPlantGroup, int dtMonth, int dtYear, TypeStatus type, bool cdPredictive)
        {
            var dateStart = new DateTime(dtYear, dtMonth, 1);
            var dateEnd = dateStart.AddMonths(1).AddDays(-1);

            var plants = _PlantServ.GetPlantGroup(idPlantGroup).Plants.Select(s=>s.Sheet).Distinct().ToList();
            
            return _NotificationServ.GetNotification(plants, dateStart, dateEnd, type, cdPredictive).Cast<object>().ToList();
            //return notification.Select(s => new { s.dateStart, s.idNote, s.cdPriority, s.Description, s.Location, s.cdUser, s.PlantGroup, s.Type, s.Status, s.SheetLocation, s.CenterCost }).Cast<object>().ToList();
            
        }

    }
}