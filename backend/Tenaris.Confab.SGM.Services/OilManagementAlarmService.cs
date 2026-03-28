using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class OilManagementAlarmService : IOilManagementAlarmService
    {
        IOilManagementAlarmRepository _repo;
        public OilManagementAlarmService(IOilManagementAlarmRepository repo)
        {
            _repo = repo;
        }
        public OilManagementAlarmService()
        {
            _repo = new OilManagementAlarmRepository();
        }

        public OilManagementAlarm GetAlarm(int idAlarm)
        {
            return _repo.GetAlarm(idAlarm);
        }

        public List<OilManagementAlarm> GetAlarmsByComponent(int idComponent)
        {
            return _repo.GetAlarmsByComponent(idComponent);
        }
        public List<OilManagementAlarm> GetAlarms(int? idFactory, int? idArea, int? idEquipment, int? idComponent, bool onlyActive)
        {
            return _repo.GetAlarms(idFactory, idArea, idEquipment, idComponent, onlyActive);
        }
        public List<OilManagementAlarmType> GetAlarmTypes()
        {
            return _repo.GetAlarmTypes();
        }
        public OilManagementAlarmGroup GetAlarmGroup(int idAlarmGroup)
        {
            var alarmGroup = _repo.GetAlarmGroup(idAlarmGroup);

            var users = _repo.GetUsersAll();

            var usersByAlarmGroup = new List<OilManagementUser>();
            foreach (var u in alarmGroup.Users)
            {
                var user = users.Where(x => x.idUser == u.idUser).FirstOrDefault();
                if (user != null)
                    usersByAlarmGroup.Add(user);
            }

            alarmGroup.Users = usersByAlarmGroup;

            return alarmGroup;
        }
        public List<OilManagementAlarmGroup> GetAlarmGroups(bool onlyActives)
        {
            return _repo.GetAlarmGroups();
        }
        public object UpdateAlarmsByComponent(int idComponent, ref List<OilManagementAlarm> alarms, string CreatedBy, string ModificatedBy)
        {
            object vReturn = null;

            alarms.ForEach(a =>
            {
                try
                {
                    if (a.idAlarm < 0)
                    {
                        a.CreatedBy = CreatedBy;
                        vReturn = _repo.Insert(ref a);
                    }
                    else
                    {
                        a.ModificatedBy = ModificatedBy;
                        vReturn = _repo.Update(a);
                    }                        
                }
                catch (Exception)
                {
                    vReturn = new
                    {
                        status = false,
                        message = "Erro ao tentar atualizar o alarme!"
                    };
                }
            });

            return vReturn;
        }
        public object UpdateGroups(OilManagementAlarm editedAlarm)
        {
            return _repo.UpdateGroups(editedAlarm);
        }
        public List<OilManagementAlarmHistory> GetAlarmsHistory(int? idFactory, int? idArea, int? idEquipment, int? idComponent, bool onlyActive, DateTime startDate, DateTime endDate)
        {
            return _repo.GetAlarmsHistory(idFactory, idArea, idEquipment, idComponent, onlyActive, startDate, endDate);
        }
        public object Insert(OilManagementAlarmGroup newAlarmGroup)
        {
            return _repo.Insert(newAlarmGroup);
        }
        public object Update(OilManagementAlarmGroup editedAlarmGroup)
        {
            return _repo.Update(editedAlarmGroup);
        }
        public List<OilManagementUser> GetUsersAll()
        {
            return _repo.GetUsersAll();
        }
        public List<OilManagementAlarmSendStatus> GetAllPendingsAlertToSend()
        {
            return _repo.GetAllPendingsAlertToSend();
        }
        public object Update(OilManagementAlarmSendStatus sendStatus)
        {
            return _repo.Update(sendStatus);
        }
    }
}
