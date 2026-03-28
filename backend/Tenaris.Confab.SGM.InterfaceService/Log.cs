using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.InterfaceService
{
    public class Log: Tenaris.Confab.SGM.InterfaceService.ILog
    {
        private readonly log4net.ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Info(string message)
        {
            _log.Info(message);
        }

        public void Warn(string message)
        {
            _log.Warn(message);
        }

        public void Error(Exception exception)
        {
            _log.Error(HelperFunctions.GetExceptionDetail(exception));
        }
    }
}
