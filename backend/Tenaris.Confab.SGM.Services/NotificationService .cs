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
    public class NotificationService : INotificationService
    {

        private INotificationRepositorie _NotificationRepo;

        public NotificationService(INotificationRepositorie NotificationRepo)
        {
            _NotificationRepo = NotificationRepo;
        }

        public NotificationService()
        {
            _NotificationRepo = new NotificationRepositorie();
        }

        public List<Notification> GetNotification(List<string> plants, DateTime? dateStart, DateTime dateEnd, TypeStatus status, bool cdPredictive)
        {
            return _NotificationRepo.GetNotification(plants, dateStart, dateEnd, status, cdPredictive);
        }

        public object InsertNotification(List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate)
        {
            return _NotificationRepo.InsertNotification(plants, typeDocs, plantsGroups, startdate, endDate);
        }

        public object InsertNotificationFailure(List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate)
        {
            return _NotificationRepo.InsertNotificationFailure(plants, typeDocs, plantsGroups, startdate, endDate);
        }
    }
}
