using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IOrderHeadService
    {
       
        List<OrderHead> GetOrderHeadSAP(List<string> planPlants, List<string> typeDocs, List<string> plantGroups, DateTime dateStart, DateTime dateEnd, ScheduledRangeType scheduledType);

        List<OrderHead> GetOrderHead(string plant, int ? idLocation, int idIndicator, int dtMonth, int dtYear);

        List<OrderHead> GetOrderHead(List<string> plants, DateTime? dateStart, DateTime dateEnd, TypeStatus status, bool cdPredictive);

        object InsertOrderHead(List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate, ScheduledRangeType scheduleType);
    }
}
