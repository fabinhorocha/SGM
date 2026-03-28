using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Tenaris.Confab.Common.Util;
using Tenaris.Confab.Common.Util.Log;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Services;

namespace Tenaris.Confab.SGM.OilMgmtService
{
    public class BusinessLogic : IDisposable
    {
        #region FIELDS

        private IOilManagementAlarmService _AlarmServ;
        private static readonly object _lock = new object();

        #endregion

        #region CONSTRUCTOR

        public BusinessLogic()
        {
            _AlarmServ = new OilManagementAlarmService();
            EmailHelper.IsProduction = Properties.Settings.Default.IsProduction;
        }

        #endregion

        #region IDISPOSABLE METHODS

        ~BusinessLogic()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(Boolean isSafeToFreeManagedObjects)
        {
            if (!isSafeToFreeManagedObjects)
            {
                return;
            }
        }

        #endregion

        #region PUBLIC METHODS

        public bool SendAlerts()
        {
            Log.SaveLog(LogType.DEBUG, ":: BusinessLogic.SendAlerts");
            bool result = false;
            try
            {
                var listResult = new List<ResultClass>();
                var listPendingAlerts = _AlarmServ.GetAllPendingsAlertToSend();

                Log.SaveLog(LogType.DEBUG, string.Format("Processing {0} pending alerts", listPendingAlerts.Count));

                foreach (var alert in listPendingAlerts)
                {
                    Log.SaveLog(LogType.DEBUG, string.Format("Processing idAlarmHistory = {0} with pending alert to send.", alert.AlarmHistory.idAlarmHistory));

                    object vResultObject = null;
                    ResultClass vResult = null;
                    try
                    {
                        var emailsUsersTo = new List<string>();
                        foreach (var group in alert.AlarmHistory.Alarm.Groups)
                        {
                            var groupAlarm = _AlarmServ.GetAlarmGroup(group.idAlarmGroup);
                            if (groupAlarm.Users == null) continue;
                            emailsUsersTo = emailsUsersTo.Union(groupAlarm.Users.Select(x => x.Email).ToList()).ToList();
                        }

                        var emailsUsersCC = new List<string>();
                        foreach (var group in alert.AlarmHistory.Alarm.GroupsCC)
                        {
                            var groupAlarm = _AlarmServ.GetAlarmGroup(group.idAlarmGroup);
                            if (groupAlarm.Users == null) continue;
                            emailsUsersCC = emailsUsersCC.Union(group.Users.Select(x => x.Email).ToList()).ToList();
                        }

                        vResultObject = SendEmailTo(emailsUsersTo, emailsUsersCC, alert);
                        vResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultClass>(Newtonsoft.Json.JsonConvert.SerializeObject(vResultObject));
                        listResult.Add(vResult);


                        alert.AttemptsNumber += 1;
                        alert.Sent = true;
                    }
                    catch (Exception ex)
                    {
                        listResult.Add(new ResultClass() { id = 0, status = false, message = ex.Message });
                        alert.LastMessage = ex.Message;
                        alert.AttemptsNumber += 1;
                    }
                    finally
                    {
                        vResultObject = _AlarmServ.Update(alert);
                    }
                }

                if (listResult.Count > 0)
                {
                    var resultErrors = listResult.Where(w => w.status == false).ToList();
                    if (resultErrors.Count > 0)
                    {
                        foreach (var res in resultErrors)
                            Log.SaveLog(LogType.ERROR, res.message);
                        result = false;
                    }
                    else
                        result = true;
                }
                else
                    result = true;
            }
            catch (Exception exception)
            {
                result = false;
                Log.ProcessException("BusinessLogic.SendAlerts", exception);
                EmailHelper.SendEmail(Properties.Settings.Default.AdminEmailsFrom, Properties.Settings.Default.AdminEmailsTo, "   Erro no serviço OilManagementService", "BusinessLogic.SendAlerts: " + exception.Message);
            }

            return result;
        }

        #endregion

        private object SendEmailTo(List<string> emailUsersTo, List<string> emailUsersCC, OilManagementAlarmSendStatus info)
        {
            var messageSubject = info.AlarmHistory.Alarm.AlertConfig.SubjectMask;
            var messageContent = info.AlarmHistory.Alarm.AlertConfig.ContentMask;

            messageSubject = messageSubject.Replace("#HidraulicUnit", info.AlarmHistory.Alarm.Component.Name);
            messageSubject = messageSubject.Replace("#AlarmName", info.AlarmHistory.Alarm.AlarmType.Name);

            messageContent = messageContent.Replace("#HidraulicUnit", info.AlarmHistory.Alarm.Component.Name);
            messageContent = messageContent.Replace("#AlarmName", info.AlarmHistory.Alarm.AlarmType.Name);
            messageContent = messageContent.Replace("#eol", Environment.NewLine);
            messageContent = messageContent.Replace("#SupplyDateTime", info.AlarmHistory.OilSupply.SupplyDateTime.ToString("dd/MM/yyyy"));

            var emailFrom = Properties.Settings.Default.AdminEmailsFrom;

            var emailTo = String.Join(";", emailUsersTo) + (emailUsersTo.Count == 0 ? "" :  ";") + String.Join(";", emailUsersCC) + (emailUsersCC.Count == 0 ? "" : ";") + Properties.Settings.Default.AdminEmailsTo;

            //EmailHelper.SendEmail(emailFrom, emailTo, messageSubject, messageContent);
            foreach (string userEmailTo in emailTo.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList())
            {
                EmailHelper.SendEmail(emailFrom, userEmailTo, messageSubject, messageContent);
            }

            return new ResultClass() { id = 0, status = true, message = "Emails enviados com sucesso!"};
        }
    }
}
