using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Tenaris.Confab.Common.Util.Log
{
    public enum LogType { DEBUG, ERROR, WARNING };

    public static class Log
    {
        public static readonly ILog log = LogManager.GetLogger("Tenaris.Confab.LogManager");
        public static bool Save { get; set; }
        public static string C_MESSAGE_METHOD_ERROR { get; set; }

        /// <summary>
        /// Method to start logging
        /// </summary>
        public static void StartLog()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public static void SaveLog(LogType logType, string message)
        {
            if (logType == LogType.ERROR)
            {
                log.Error(message);
                return;
            }

            if (Save)
            {
                switch (logType)
                {
                    case LogType.DEBUG:
                        log.Debug(message);
                        break;
                    case LogType.ERROR:
                        log.Error(message);
                        break;
                    case LogType.WARNING:
                        log.Warn(message);
                        break;
                    default:
                        break;
                }
            }
        }

        public static void ProcessException(string methodName, Exception ex)
        {
            SaveLog(LogType.ERROR, "ProcessException");
            string errorMessage = ex.Message;

            if (ex.InnerException != null)
                errorMessage += " " + ex.InnerException.Message;

            SaveLog(LogType.ERROR, string.Format(C_MESSAGE_METHOD_ERROR, methodName, errorMessage));
        }
    }

    public static class LogSettings
    {
        public static String BreakExecutionString = new String('-', 100);
    }
}