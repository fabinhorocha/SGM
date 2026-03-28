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
using Tenaris.Confab.SGM.InterfaceService.Properties;
using Tenaris.Confab.SGM.Services;

namespace Tenaris.Confab.SGM.InterfaceService
{
    public partial class SGMInterfaceService : ServiceBase
    {
        private Thread _thread;
        private Timer _timer;
        private ILog _log;
        private object _lock = new object();
        private bool _serviceIsProcessing;
        private Mutex _mutex;
        

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

        public SGMInterfaceService()
        {
            InitializeComponent();            
            _log = new Log();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _log.Info(":: SGMInterfaceService.CurrentDomain_UnhandledException");
            _log.Error((Exception)e.ExceptionObject);
            EmailHelper.SendEmail("Erro no serviço Tenaris.Confab.SGM.InterfaceService", "SGMInterfaceService.CurrentDomain_UnhandledException: " + ((Exception)e.ExceptionObject).Message, _log);

            if (e.IsTerminating)
            {
                _log.Info("   TÉRMINO DO PROGRAMA COM ERRO GRAVE");
            }
        }

        protected override void OnStart(string[] args)
        {
            //while (!Debugger.IsAttached)
            //{
            //    Thread.Sleep(1000);
            //}

            _log.Info(":: SGMInterfaceService.OnStart");
            EmailHelper.SendEmail("Início do serviço Tenaris.Confab.SGM.InterfaceService", "SGMInterfaceService.OnStart", _log);
            Start();
        }

        protected override void OnStop()
        {
            _log.Info(":: SGMInterfaceService.OnStop");
            EmailHelper.SendEmail("Fim do serviço Tenaris.Confab.SGM.InterfaceService", "SGMInterfaceService.OnStop", _log);
            StopService();
        }

        public void Start()
        {
            _log.Info(":: SGMInterfaceService.Start");

            _mutex = new Mutex();
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

        public void StopService()
        {
            _log.Info(":: SGMInterfaceService.StopService");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            while (ServiceIsProcessing)
            {
                RequestAdditionalTime(1000); // Pedir tempo adicional para a finalização, pois a thread trabalhadora pode estar a dormir
                Thread.Sleep(1000);

                if (stopWatch.ElapsedMilliseconds > 30000) // Passou mais de 30 segundos desde que o código entrou no ciclo?
                {
                    stopWatch.Stop();
                    ServiceIsProcessing = false;
                }
            }

            _log = null;

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


        private void periodicVerificationElapsed(object state)
        {
            DateTime start = DateTime.Now;
            _log.Info(new string('*', 200));
            _log.Info(":: SGMInterfaceService.periodicVerificationElapsed");
            ServiceIsProcessing = true;
            int dueTime;

            if (_mutex.WaitOne(TimeSpan.FromSeconds(2), false))
            {

                try
                {


                    using (BusinessLogic businessLogic = new BusinessLogic(_log))
                    {
                        //Cria notas no SAP
                        bool flag1 = true;
                        if (Settings.Default.AutomaticCreateNoteSAP)
                            flag1 = businessLogic.CreateNoteSAP();
                        _log.Info(string.Format("CreateNoteSAP()   flag = {0}", flag1));


                        bool flag6 = businessLogic.InsertScheduledRange();
                        _log.Info(string.Format("InsertScheduledRange()   flag = {0}", flag6));


                        //Baixa localizações do SAP 
                        bool flag9 = businessLogic.DownloadLocationSAP();
                        _log.Info(string.Format("DownloadLocationSAP()   flag = {0}", flag9));


                        //Baixa notas do SAP 
                        bool flag2 = businessLogic.DownloadNotificationSAP();
                        _log.Info(string.Format("DownloadNotificationSAP()   flag = {0}", flag2));


                        //Baixa ordens do SAP 
                        bool flag3 = businessLogic.DownloadOrderHeadSAP();
                        _log.Info(string.Format("DownloadOrderHeadSAP()   flag = {0}", flag3));

                        //Baixa notas de falhas do SAP 
                        bool flag8 = businessLogic.DownloadNotificationFailureSAP();
                        _log.Info(string.Format("DownloadNotificationFailureSAP()   flag = {0}", flag8));


                        //Processa Localizações
                        bool flag4 = businessLogic.ProcessLocationData();
                        _log.Info(string.Format("ProcessLocation()   flag = {0}", flag4));

                        //Processa Plantas
                        bool flag5 = businessLogic.ProcessPlantData();
                        _log.Info(string.Format("ProcessPlantData()   flag = {0}", flag5));

                        //Processa Falhas
                        //bool flag7 = businessLogic.ProcessFailureData();
                        //_log.Info(string.Format("ProcessFailureData()   flag = {0}", flag7));





                        if (flag1 && flag2 && flag3 && flag4 && flag5 && flag6 && flag8 && flag9)
                        {
                            dueTime = Settings.Default.ProcessIntervalShort;
                        }
                        else
                        {
                            dueTime = Settings.Default.ProcessIntervalLong;
                        }


                        _log.Info(string.Format("   dueTime = {0}", dueTime));
                    }
                }
                catch (Exception exception)
                {
                    dueTime = Settings.Default.ProcessIntervalOnError;
                    _log.Error(exception);
                    EmailHelper.SendEmail("Erro no serviço Tenaris.Confab.SGM.InterfaceService", "SGMInterfaceService.periodicVerificationElapsed: " + exception.Message, _log);
                }
                finally
                {
                    _mutex.ReleaseMutex();
                    
                }

                TimeSpan end = DateTime.Now.Subtract(start);
                _log.Info(string.Format("   total runtime = {0} seconds", end.TotalSeconds));
                ServiceIsProcessing = false;
                _timer.Change(dueTime, Timeout.Infinite);

            }

            
        }
    }
}
