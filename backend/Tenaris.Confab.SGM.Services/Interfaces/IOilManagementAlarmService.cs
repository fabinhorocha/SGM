using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IOilManagementAlarmService
    {
        OilManagementAlarm GetAlarm(int idAlarm);
        List<OilManagementAlarm> GetAlarmsByComponent(int idComponent);
        object UpdateAlarmsByComponent(int idComponent, ref List<OilManagementAlarm> alarms, string CreatedBy, string ModificatedBy);
        object UpdateGroups(OilManagementAlarm editedAlarm);
        List<OilManagementAlarmType> GetAlarmTypes();
        List<OilManagementAlarm> GetAlarms(int? idFactory, int? idArea, int? idEquipment, int? idComponent, bool onlyActive);
        List<OilManagementAlarmHistory> GetAlarmsHistory(int? idFactory, int? idArea, int? idEquipment, int? idComponent, bool onlyActive, DateTime startDate, DateTime endDate);
        OilManagementAlarmGroup GetAlarmGroup(int idAlarmGroup);
        List<OilManagementAlarmGroup> GetAlarmGroups(bool onlyActive);
        object Insert(OilManagementAlarmGroup newAlarmGroup);
        object Update(OilManagementAlarmGroup editedAlarmGroup);
        List<OilManagementUser> GetUsersAll();
        List<OilManagementAlarmSendStatus> GetAllPendingsAlertToSend();
        object Update(OilManagementAlarmSendStatus sendStatus);
    }
}
