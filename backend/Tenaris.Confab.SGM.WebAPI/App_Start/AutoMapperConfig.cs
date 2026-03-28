using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.WebAPI;
using Tenaris.Confab.SGM.WebAPI.Models;


namespace Tenaris.Confab.SGM.WebAPI
{
    public class AutoMapperConfig
    {
        public static IConfigurationProvider Initialize()
        {
            Mapper.Initialize((config) =>
            {
                config.CreateMap<Equipment, EquipmentViewModel>();
                config.CreateMap<EquipmentViewModel, Equipment>();
                config.CreateMap<Area, AreaViewModel>();
                config.CreateMap<Priority, PriorityViewModel>();
                config.CreateMap<TypeEquipment, TypeViewModel>();
                config.CreateMap<Part, PartViewModel>();
                config.CreateMap<EquipmentBackup, EquipmentBackupViewModel>();
                config.CreateMap<Report, ReportViewModel>();
                config.CreateMap<ReportViewModel, Report>();
                config.CreateMap<ReportType, ReportTypeViewModel>();
                config.CreateMap<ReportStatus, ReportStatusViewModel>();
                config.CreateMap<ReportFileViewModel, ReportFile>();
                config.CreateMap<NoteSAPViewModel, NoteSAP>();
                config.CreateMap<BudgetViewModel, Budget>();
                config.CreateMap<Budget, BudgetViewModel>();
                config.CreateMap<BudgetLocationViewModel, BudgetLocation>();
                config.CreateMap<BudgetLocation, BudgetLocationViewModel>();
                config.CreateMap<BudgetPlantViewModel, BudgetPlant>();
                config.CreateMap<BudgetPlant, BudgetPlantViewModel>();                
                config.CreateMap<Location, LocationViewModel>();
                config.CreateMap<LocationViewModel, Location>();
                config.CreateMap<LocationPlant, LocationPlantViewModel>();
                config.CreateMap<LocationPlantViewModel, LocationPlant>();
                config.CreateMap<LocationData, LocationDataViewModel>();
                config.CreateMap<PlantData, PlantDataViewModel>();
                config.CreateMap<FailureData, FailureDataViewModel>();
                config.CreateMap<EquipmentData, EquipmentDataViewModel>();
                config.CreateMap<Plant, PlantViewModel>();
                config.CreateMap<OrderHead, OrderHeadViewModel>();
                config.CreateMap<Notification, NotificationViewModel>();
                config.CreateMap<ScheduledRange, ScheduledRangeViewModel>();
                config.CreateMap<ComponentViewModel, Component>();
                config.CreateMap<Component, ComponentViewModel>();
                config.CreateMap<OilSupplyViewModel, OilSupply>();
                config.CreateMap<OilSupply, OilSupplyViewModel>();
                config.CreateMap<AlarmGroupViewModel, OilManagementAlarmGroup>();
                config.CreateMap<OilManagementAlarmGroup, AlarmGroupViewModel>();
                config.CreateMap<ScheduledRangeViewModel, ScheduledRange>();
                config.CreateMap<PlantGroupViewModel, PlantGroup>();
                config.CreateMap<EquipmentPlantGroupViewModel, EquipmentPlantGroup>();
                config.CreateMap<PlantGroup, PlantGroupViewModel>();
                config.CreateMap<EquipmentPlantGroup, EquipmentPlantGroupViewModel>();
                config.CreateMap<NoteFailure, NoteFailureViewModel>();
                config.CreateMap<NoteFailureViewModel, NoteFailure>();

            });

            return Mapper.Configuration;
        }
    }
}