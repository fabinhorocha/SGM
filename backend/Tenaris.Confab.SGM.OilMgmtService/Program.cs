using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Tenaris.Confab.Common.Util.Log;
using Tenaris.Confab.SGM.OilMgmtService.Properties;

namespace Tenaris.Confab.SGM.OilMgmtService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            LoadSettings();
            Log.SaveLog(LogType.DEBUG, "Run Oil Management Service");
            Log.SaveLog(LogType.DEBUG, string.Format("Production Environment = {0}", Settings.Default.IsProduction));

            using (var OilManagementService = new OilMgmtService())
            {
                if (Settings.Default.IsProduction)
                {
                    ServiceBase.Run(OilManagementService);
                }
                else
                {
                    OilManagementService.Start();
                }
            }
        }

        private static void LoadSettings()
        {
            //Log Settings
            Log.StartLog();
            Log.Save = true;
            Log.C_MESSAGE_METHOD_ERROR = "Exception when executing the method [{0}]: {1}";

            Log.SaveLog(LogType.DEBUG, LogSettings.BreakExecutionString);
            Log.SaveLog(LogType.DEBUG, string.Format("Starting the system. Version: {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
        }
    }
}
