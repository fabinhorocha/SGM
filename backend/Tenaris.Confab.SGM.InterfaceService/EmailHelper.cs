using ConfabEmail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.InterfaceService.Properties;

namespace Tenaris.Confab.SGM.InterfaceService
{
    public static class EmailHelper
    {
        public static void SendEmail(string subject, string body, ILog log)
        {
          //  log.Info(string.Format(":: EmailHelper.SendEmail(subject: {0}, body: {1})", subject, body));

          //  if (Settings.Default.IsProduction)
           // {
               // string exceptionMessage;
               // bool emailResult = Email.Send(Settings.Default.EmailFrom, Settings.Default.EmailTo, subject, body, out exceptionMessage);
               // log.Info(string.Format("   Email enviado com sucesso? {0}", emailResult));

               // if (exceptionMessage != null)
               // {
              //      log.Warn("   Houve um erro ao enviar o email!!!");
              //      log.Warn(exceptionMessage);
              //  }
          //  }
        }
       
    }
}
