using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface INotificationRepositorie
    {
        List<Notification> GetNotificationSAP(List<string> planPlants, List<string> typeNotes, List<string> plantGroups, DateTime dateStart);

        Notification GetNotification(SqlConnection sqlConn, string idNote);        

        List<Notification> GetNotification(List<string> plants, DateTime? dateStart, DateTime dateEnd, TypeStatus status, bool cdPredictive);

        object InsertNotification(List<string> planPlants, List<string> typeNotes, List<string> plantGroups, DateTime dateStart, DateTime dateEnd);

        object InsertNotificationFailure(List<string> planPlants, List<string> typeNotes, List<string> plantGroups, DateTime dateStart, DateTime dateEnd);

    }
}