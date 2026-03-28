using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class OrderHeadService : IOrderHeadService
    {

        private IOrderHeadRepositorie _OrderHeadRepo;

        public OrderHeadService(IOrderHeadRepositorie OrderHeadRepo)
        {
            _OrderHeadRepo = OrderHeadRepo;
        }

        public OrderHeadService()
        {
            _OrderHeadRepo = new OrderHeadRepositorie();
        }


        public List<OrderHead> GetOrderHeadSAP(List<string> planPlants, List<string> typeDocs, List<string> plantGroups, DateTime dateStart, DateTime dateEnd, ScheduledRangeType scheduledType)
        {
            return _OrderHeadRepo.GetOrderHeadSAP(planPlants, typeDocs, plantGroups, dateStart, dateEnd, scheduledType);
        }
     
        
        
        public List<OrderHead> GetOrderHead(string plant, int ? idLocation, int idIndicator, int dtMonth, int dtYear)
        {
            return _OrderHeadRepo.GetOrderHead(plant, idLocation, idIndicator, dtMonth, dtYear);
        }


        public object InsertOrderHead(List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate, ScheduledRangeType scheduleType)
        {
            return _OrderHeadRepo.InsertOrderHead(plants, typeDocs, plantsGroups, startdate, endDate, scheduleType);
        }

        public List<OrderHead> GetOrderHead(List<string> plants, DateTime? dateStart, DateTime dateEnd, TypeStatus status, bool cdPredictive)
        {
            return _OrderHeadRepo.GetOrderHead(plants, dateStart, dateEnd, status, cdPredictive);
        }
    }
}
