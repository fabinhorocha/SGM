using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public static class NinjectConfig
    {

        public static void Start(IKernel kernel)
        {
            kernel.Bind<IEquipmentRepositorie>().To<EquipmentRepositorie>();
            kernel.Bind<IAreaRepositorie>().To<AreaRepositorie>();
            kernel.Bind<ITypeRepositorie>().To<TypeRepositorie>();
            kernel.Bind<IPartRepositorie>().To<PartRepositorie>();
            kernel.Bind<IEquipmentBackupRepositorie>().To<EquipmentBackupRepositorie>();
            kernel.Bind<IReportRepositorie>().To<ReportRepositorie>();
            kernel.Bind<IReportStatusRepositorie>().To<ReportStatusRepositorie>();
            kernel.Bind<IReportTypeRepositorie>().To<ReportTypeRepositorie>();
            kernel.Bind<IReportFileRepositorie>().To<ReportFileRepositorie>();
            kernel.Bind<IPriorityRepositorie>().To<PriorityRepositorie>();
            kernel.Bind<ILocationRepositorie>().To<LocationRepositorie>();
            kernel.Bind<IBudgetRepositorie>().To<BudgetRepositorie>();
            kernel.Bind<ILocationDataRepositorie>().To<LocationDataRepositorie>();
            kernel.Bind<IPlantDataRepositorie>().To<PlantDataRepositorie>();
            kernel.Bind<IFailureDataRepositorie>().To<FailureDataRepositorie>();
            kernel.Bind<IEquipmentDataRepositorie>().To<EquipmentDataRepositorie>();
            kernel.Bind<IOrderHeadRepositorie>().To<OrderHeadRepositorie>();
            kernel.Bind<INotificationRepositorie>().To<NotificationRepositorie>();
            kernel.Bind<IPlantRepositorie>().To<PlantRepositorie>();
            kernel.Bind<IScheduledRangeRepositorie>().To<ScheduledRangeRepositorie>();
            kernel.Bind<ITypeDocRepositorie>().To<TypeDocRepositorie>();
            kernel.Bind<ILocationPlantRepositorie>().To<LocationPlantRepositorie>();
            kernel.Bind<ILocationLocalRepositorie>().To<LocationLocalRepositorie>();
            kernel.Bind<ITypeNoteRepositorie>().To<TypeNoteRepositorie>();
            kernel.Bind<IBudgetLocationRepositorie>().To<BudgetLocationRepositorie>();
            kernel.Bind<IBudgetPlantRepositorie>().To<BudgetPlantRepositorie>();
            kernel.Bind<IComponentRepository>().To<ComponentRepository>();
            kernel.Bind<IOilSupplyRepository>().To<OilSupplyRepository>();
            kernel.Bind<IOilTypeRepository>().To<OilTypeRepository>();
            kernel.Bind<IOilSupplyTypeRepository>().To<OilSupplyTypeRepository>();
            kernel.Bind<IStoppageTypeRepository>().To<StoppageTypeRepository>();
            kernel.Bind<IOilManagementReportRepository>().To<OilManagementReportRepository>();
            kernel.Bind<IOilManagementAccessRepository>().To<OilManagementAccessRepository>();
            kernel.Bind<IOilManagementAlarmRepository>().To<OilManagementAlarmRepository>();
            kernel.Bind<IEquipmentPlantGroupRepositorie>().To<EquipmentPlantGroupRepositorie>();
            kernel.Bind<INoteFailureRepositorie>().To<NoteFailureRepositorie>();
        }
    }
}
