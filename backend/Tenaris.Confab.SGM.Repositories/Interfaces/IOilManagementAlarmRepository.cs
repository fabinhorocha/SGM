using System;
using System.Collections.Generic;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface IOilManagementAlarmRepository
    {
        OilManagementAlarm GetAlarm(int idAlarm);
        List<OilManagementAlarm> GetAlarmsByComponent(int idComponent);
        List<OilManagementAlarmType> GetAlarmTypes();
        List<OilManagementAlarm> GetAlarms(int? idFactory, int? idArea, int? idEquipment, int? idComponent, bool onlyActive);
        List<OilManagementAlarmHistory> GetAlarmsHistory(int? idFactory, int? idArea, int? idEquipment, int? idComponent, bool onlyActive, DateTime startDate, DateTime endDate);
        List<OilManagementAlarmGroup> GetAlarmGroups();
        object Insert(ref OilManagementAlarm newAlarm);
        object Update(OilManagementAlarm editedAlarm);
        object UpdateGroups(OilManagementAlarm editedAlarm);
        OilManagementAlarmGroup GetAlarmGroup(int idAlarmGroup);
        object Insert(OilManagementAlarmGroup newAlarmGroup);
        object Update(OilManagementAlarmGroup editedAlarmGroup);
        List<OilManagementUser> GetUsersAll();
        List<OilManagementAlarmSendStatus> GetAllPendingsAlertToSend();
        object Update(OilManagementAlarmSendStatus sendStatus);
    }
}
