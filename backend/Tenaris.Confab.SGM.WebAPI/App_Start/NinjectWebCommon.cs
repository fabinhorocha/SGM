using System;
using System.Web;
using System.Web.Http;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.WebApi;
using Tenaris.Confab.SGM.Services;
using AutoMapper;
using Tenaris.Confab.SGM.WebAPI;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NinjectWebCommon), "Stop")]

namespace Tenaris.Confab.SGM.WebAPI
{
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
          
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);

                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IEquipmentService>().To<EquipmentService>();
            kernel.Bind<IAreaService>().To<AreaService>();
            kernel.Bind<ITypeService>().To<TypeService>();
            kernel.Bind<IPartService>().To<PartService>();
            kernel.Bind<IEquipmentBackupService>().To<EquipmentBackupService>();
            kernel.Bind<IReportService>().To<ReportService>();
            kernel.Bind<IReportStatusService>().To<ReportStatusService>();
            kernel.Bind<IReportTypeService>().To<ReportTypeService>();
            kernel.Bind<IReportFileService>().To<ReportFileService>();
            kernel.Bind<IPriorityService>().To<PriorityService>();
            kernel.Bind<IBudgetService>().To<BudgetService>();            
            kernel.Bind<ILocationService>().To<LocationService>();
            kernel.Bind<ILocationDataService>().To<LocationDataService>();
            kernel.Bind<IPlantDataService>().To<PlantDataService>();
            kernel.Bind<IFailureDataService>().To<FailureDataService>();
            kernel.Bind<IEquipmentDataService>().To<EquipmentDataService>();
            kernel.Bind<IPlantService>().To<PlantService>();
            kernel.Bind<IOrderHeadService>().To<OrderHeadService>();
            kernel.Bind<IScheduledRangeService>().To<ScheduledRangeService>();
            kernel.Bind<ILocationPlantService>().To<LocationPlantService>();
            kernel.Bind<ILocationLocalService>().To<LocationLocalService>();
            kernel.Bind<IBudgetLocationService>().To<BudgetLocationService>();
            kernel.Bind<IBudgetPlantService>().To<BudgetPlantService>();
            kernel.Bind<INotificationService>().To<NotificationService>();
            kernel.Bind<IComponentService>().To<ComponentService>();
            kernel.Bind<IOilSupplyService>().To<OilSupplyService>();
            kernel.Bind<IOilTypeService>().To<OilTypeService>();
            kernel.Bind<IOilSupplyTypeService>().To<OilSupplyTypeService>();
            kernel.Bind<IStoppageTypeService>().To<StoppageTypeService>();
            kernel.Bind<IOilManagementReportService>().To<OilManagementReportService>();
            kernel.Bind<IOilManagementAccessService>().To<OilManagementAccessService>();
            kernel.Bind<IOilManagementAlarmService>().To<OilManagementAlarmService>();
            kernel.Bind<IOilManagementIndicatorService>().To<OilManagementIndicatorService>();
            kernel.Bind<IEquipmentPlantGroupService>().To<EquipmentPlantGroupService>();
            kernel.Bind<INoteFailureService>().To<NoteFailureService>();

            var config = AutoMapperConfig.Initialize();
            kernel.Bind<IMapper>().To<Mapper>().WithConstructorArgument(config);
            NinjectConfig.Start(kernel);
        }
    }
}