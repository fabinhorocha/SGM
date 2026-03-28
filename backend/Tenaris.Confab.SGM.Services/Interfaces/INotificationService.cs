using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface INotificationService
    {
              

        object InsertNotification(List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate);

        object InsertNotificationFailure(List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate);

        List<Notification> GetNotification(List<string> plants, DateTime? dateStart, DateTime dateEnd, TypeStatus status, bool cdPredictive);
    }
}
