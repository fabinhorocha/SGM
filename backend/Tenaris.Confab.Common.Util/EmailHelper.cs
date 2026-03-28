using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfabEmail;

namespace Tenaris.Confab.Common.Util
{
    public static class EmailHelper
    {
        public static bool IsProduction { get; set;  }

        public static void SendEmail(string emailFrom, string emailTo, string subject, string body)
        {
            Log.Log.SaveLog(Log.LogType.DEBUG, string.Format("Email.SendEmail(subject: {0}, body: {1})", subject, body));
            if (IsProduction)
            {
                string exceptionMessage;
                bool emailResult = Email.Send(emailFrom, emailTo, subject, body, out exceptionMessage);
                Log.Log.SaveLog(Log.LogType.DEBUG, string.Format("   Email enviado com sucesso? {0}", emailResult));
                if (exceptionMessage != null)
                {
                    Log.Log.SaveLog(Log.LogType.WARNING, "   Houve um erro ao enviar o email!!!");
                    Log.Log.SaveLog(Log.LogType.WARNING, exceptionMessage);
                }
            }
        }

    }
}
