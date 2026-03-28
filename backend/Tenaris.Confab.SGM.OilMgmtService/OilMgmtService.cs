using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tenaris.Confab.Common.Util;
using Tenaris.Confab.Common.Util.Log;
using Tenaris.Confab.SGM.OilMgmtService.Properties;

namespace Tenaris.Confab.SGM.OilMgmtService
{
    public partial class OilMgmtService : ServiceBase
    {
        private Thread _thread;
        private Timer _timer;
        private object _lock = new object();
        private bool _serviceIsProcessing;
        private bool ServiceIsProcessing
        {
            get
            {
                lock (_lock)
                {
                    return _serviceIsProcessing;
                }
            }
            set
            {
                lock (_lock)
                {
                    _serviceIsProcessing = value;
                }
            }
        }

        public OilMgmtService()
        {
            InitializeComponent();
        }

        protected override void OnStop()
        {
            StopService();
        }

        protected override void OnStart(string[] args)
        {
            Start();
        }

        private void StopService()
        {
            Log.SaveLog(LogType.ERROR, "Stop Oil Management Service!");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            while (ServiceIsProcessing)
            {
                RequestAdditionalTime(1000);
                Thread.Sleep(1000);

                if (stopWatch.ElapsedMilliseconds > 30000)
                {
                    stopWatch.Stop();
                    ServiceIsProcessing = false;
                }
            }

            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timer.Dispose();
            _timer = null;

            _thread.Join(1000);

            if (_thread.IsAlive)
            {
                _thread.Abort();
            }

            _thread = null;
            _lock = null;
        }

        public void Start()
        {
            Log.SaveLog(LogType.DEBUG, "Start Oil Management Service");
            _thread = new Thread(
                delegate ()
                {
                    using (_timer = new Timer(periodicVerificationElapsed, null, 0, Timeout.Infinite))
                    {
                        Thread.Sleep(Timeout.Infinite);
                    }
                }
                );

            _thread.Start();
        }

        private void periodicVerificationElapsed(object state)
        {
            DateTime start = DateTime.Now;
            ServiceIsProcessing = true;
            int dueTime = 0;

            Log.SaveLog(LogType.DEBUG, "Periodic check of Oil Management Service.");
            try
            {
                using (BusinessLogic businessLogic = new BusinessLogic())
                {
                    bool flag1 = true;
                    flag1 = businessLogic.SendAlerts();
                    Log.SaveLog(LogType.DEBUG, string.Format("SendAlerts() flag = {0}", flag1));
                }
            }
            catch (Exception ex)
            {
                Log.ProcessException("periodicVerificationElapsed", ex);
            }
            finally
            {
            }

            try
            {
                dueTime = Settings.Default.ProcessIntervalShort;
            }
            catch (Exception exception)
            {
                Log.ProcessException("periodicVerificationElapsed  > dueTime", exception);
            }

            ServiceIsProcessing = false;
            try
            {
                _timer.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception exception)
            {
                Log.ProcessException("periodicVerificationElapsed > TimerChange", exception);
            }
        }
    }
}
