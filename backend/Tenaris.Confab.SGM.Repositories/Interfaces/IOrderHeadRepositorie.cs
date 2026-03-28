using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface IOrderHeadRepositorie
    {
        List<OrderHead> GetOrderHeadSAP(List<string> planPlants, List<string> typeDocs, List<string> plantGroups, DateTime dateStart, DateTime dateEnd, ScheduledRangeType scheduleType);

        List<OrderHead> GetOrderHead(string plant, int ? idLocation, int idIndicator, int dtMonth, int dtYear);

        object InsertOrderHead(List<string> planPlants, List<string> typeDocs, List<string> plantGroups, DateTime dateStart, DateTime dateEnd, ScheduledRangeType scheduleType);

        List<OrderHead> GetOrderHead(List<string> plants, DateTime? dateStart, DateTime dateEnd, TypeStatus status, bool cdPredictive);
    }
}