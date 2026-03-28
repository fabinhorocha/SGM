using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Services;
using Tenaris.Confab.SGM.WebAPI.Models;

namespace Tenaris.Confab.SGM.WebAPI.Controllers
{
    [RoutePrefix("api/FailureData")]
    public class FailureDataController : ApiController
    {
        private IFailureDataService _FailureDataServ;
        private IPlantService _PlantServ;
        private IEquipmentDataService _EquipDataServ;
        private IOrderHeadService _OrderHeadService;
        private INotificationService _NotificationServ;

        private IMapper _Mapper;


        public FailureDataController(IMapper Mapper, IFailureDataService FailureDataServ, IOrderHeadService OrderHeadService, IPlantService PlantServ, IEquipmentDataService EquipDataServ, INotificationService NotificationServ)
        {

            _Mapper = Mapper;
            _FailureDataServ = FailureDataServ;
            _OrderHeadService = OrderHeadService;
            _PlantServ = PlantServ;
            _EquipDataServ = EquipDataServ;
            _NotificationServ = NotificationServ;

        }

        [Route("GetFailureData")]
        public List<object> GetFailureData(int idIndicator, int idBudget)
        {
            var list = new List<object>();


            var PlantsGroups = _PlantServ.GetPlantsGroups();
            var FailuresData = _FailureDataServ.GetFailureData(idIndicator, idBudget);

            //var EquipsData = _EquipDataServ.GetEquipmentData(idIndicator, idBudget);


            foreach (var plantGroup in PlantsGroups.OrderBy(o => o.idPlantGroup))
            {
                var plantsName = plantGroup.Plants.Select(s => s.Sheet).ToList();
                var plants = plantGroup.Plants.Select(s => s.idPlant).ToList();
                var FailureData = FailuresData
                        .Where(w => plants.Contains(w.cdPlant) && w.cdbudget == idBudget)
                        .OrderBy(O => O.dtYear)
                        .ThenBy(t => t.dtMonth)
                        .Select(s => new { s.monthYear, s.dtMonth, s.dtYear, vlAnalyzedFA = (s.vlCountAnalyzedFA), vlClosedMeasuresFA = (s.vlCountClosedMeasuresFA), vlPendingFA = (s.vlCountPendingFA), vlRequestedM2ByFA = s.vlCountRequestedM2ByFA, vlRequestedM2ByFAOpen = s.vlCountRequestedM2ByFAOpen })
                        .GroupBy(g => new { g.monthYear, g.dtMonth, g.dtYear})
                        .Select(s => new { s.Key.monthYear, s.Key.dtMonth, s.Key.dtYear, vlAnalyzedFA = s.Sum(s2 => s2.vlAnalyzedFA).Value, vlClosedMeasuresFA = s.Sum(s2 => s2.vlClosedMeasuresFA).Value, vlPendingFA = s.Sum(s2 => s2.vlPendingFA).Value, vlRequestedM2ByFA = s.Sum(s2 => s2.vlRequestedM2ByFA).Value, vlRequestedM2ByFAOpen = s.Sum(s2 => s2.vlRequestedM2ByFAOpen).Value })
                        .Where(w=> w.vlAnalyzedFA > 0 || w.vlClosedMeasuresFA > 0 || w.vlPendingFA > 0 || w.vlRequestedM2ByFA > 0 || w.vlRequestedM2ByFAOpen > 0)
                        .ToList();


                var FailureDataAccActual = FailuresData
                        .Where(w => plants.Contains(w.cdPlant) && w.cdbudget == idBudget)
                        .OrderBy(O => O.dtYear)
                        .ThenBy(t => t.dtMonth)
                        .Select(s => new { yearToYear = s.Budget.dateStart.Year.ToString().Substring(2) + "/" + s.Budget.dateEnd.Year.ToString().Substring(2), s.monthYear, s.dtMonth, s.dtYear, vlAnalyzedFA = (s.vlCountAnalyzedFA), vlClosedMeasuresFA = (s.vlCountClosedMeasuresFA), vlPendingFA = (s.vlCountPendingFA), vlRequestedM2ByFA = s.vlCountRequestedM2ByFA, vlRequestedM2ByFAOpen = s.vlCountRequestedM2ByFAOpen })
                        .GroupBy(g => new { g.yearToYear, g.monthYear, g.dtMonth, g.dtYear })
                        .Select(s => new { s.Key.yearToYear, s.Key.dtMonth, s.Key.dtYear, vlAnalyzedFA = s.Sum(s2 => s2.vlAnalyzedFA).Value, vlClosedMeasuresFA = s.Sum(s2 => s2.vlClosedMeasuresFA).Value, vlPendingFA = s.Sum(s2 => s2.vlPendingFA).Value, vlRequestedM2ByFA = s.Sum(s2 => s2.vlRequestedM2ByFA).Value, vlRequestedM2ByFAOpen = s.Sum(s2 => s2.vlRequestedM2ByFAOpen).Value })
                        .LastOrDefault();


                var FailureDataAcc = new List<object>();
                var budgets = FailuresData.Where(w => plants.Contains(w.cdPlant) && w.cdbudget != idBudget).Select(s => new { s.cdbudget, startYear = s.Budget.dateStart.Year, endYear = s.Budget.dateEnd.Year }).Distinct().OrderBy(o => o.startYear).ThenBy(t => t.endYear).ToList();
                foreach (var budget in budgets)
                {

                    FailureDataAcc.Add(FailuresData
                        .Where(w => plants.Contains(w.cdPlant) && w.cdbudget == budget.cdbudget)
                        .OrderBy(O => O.dtYear)
                        .ThenBy(t => t.dtMonth)
                        .Select(s => new { yearToYear = s.Budget.dateStart.Year.ToString().Substring(2) + "/" + s.Budget.dateEnd.Year.ToString().Substring(2), s.monthYear, s.dtMonth, s.dtYear, vlAnalyzedFA = (s.vlCountAnalyzedFA), vlClosedMeasuresFA = (s.vlCountClosedMeasuresFA), vlPendingFA = (s.vlCountPendingFA), vlRequestedM2ByFA = s.vlCountRequestedM2ByFA, vlRequestedM2ByFAOpen = s.vlCountRequestedM2ByFAOpen })
                        .GroupBy(g => new { g.yearToYear, g.monthYear, g.dtMonth, g.dtYear })
                        .Select(s => new { s.Key.yearToYear, vlAnalyzedFA = s.Sum(s2 => s2.vlAnalyzedFA).Value, vlClosedMeasuresFA = s.Sum(s2 => s2.vlClosedMeasuresFA).Value, vlPendingFA = s.Sum(s2 => s2.vlPendingFA).Value, vlRequestedM2ByFA = s.Sum(s2 => s2.vlRequestedM2ByFA).Value, vlRequestedM2ByFAOpen = s.Sum(s2 => s2.vlRequestedM2ByFAOpen).Value })
                        .LastOrDefault());
                }



                //var dataEquip = EquipsData
                //            .Where(w => w.cdbudget == idBudget && w.EquipmentPlantGroup.cdPlantGroup == plantGroup.idPlantGroup)
                //            .GroupBy(g => new { id = g.EquipmentPlantGroup.idEquipmentPlantGroup })
                //            .SelectMany(m => m.Where(w => new DateTime(w.dtYear, w.dtMonth, 1) == m.Max(d => new DateTime(d.dtYear, d.dtMonth, 1))))
                //            .Select(s => new { id = s.EquipmentPlantGroup.idEquipmentPlantGroup, name = s.EquipmentPlantGroup.Name, vlPending = s.vlCountOTsPending + s.vlCountNotesPending, vlHigh = s.vlCountNotesVeryHigh + s.vlCountNotesHigh + s.vlCountOTsVeryHigh + s.vlCountOTsHigh })
                //            .OrderByDescending(o => o.vlPending).Take(15).ToList();




                   var dateStart = new DateTime(FailureDataAccActual.dtYear, FailureDataAccActual.dtMonth, 1);
                    var dateEnd = dateStart.AddMonths(1).AddDays(-1);


                    var tableNotesAnalyzedFA = new List<object>();
                    var tableNotesClosedMeasuresFA = new List<object>();
                    var tableNotesPendingFA = new List<object>();
                    var tableNotesRequestedM2ByFA = new List<object>();
                    var tableNotesRequestedM2ByFAOpen = new List<object>();



                    tableNotesAnalyzedFA = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.AnalyzedFA, false).Cast<object>().ToList();
                    tableNotesClosedMeasuresFA = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.ClosedMeasuresFA, false).Cast<object>().ToList();
                    tableNotesPendingFA = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.PendingFA, false).Cast<object>().ToList();
                    tableNotesRequestedM2ByFA = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.RequestedM2ByFA, false).Cast<object>().ToList();
                    tableNotesRequestedM2ByFAOpen = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.RequestedM2ByFAOpen, false).Cast<object>().ToList();



                if (FailureData.Count > 0)
                {

                    //list.Add(new { idPlant = plantGroup.idPlantGroup, Plant = plantGroup.Name, data = FailureData, dataAccActual = FailureDataAccActual, dataAcc = FailureDataAcc, dataEquip, tableNotesAnalyzedFA = tableNotesAnalyzedFA, tableNotesClosedMeasuresFA = tableNotesClosedMeasuresFA, tableNotesPendingFA = tableNotesPendingFA, tableNotesRequestedM2ByFA = tableNotesRequestedM2ByFA, tableNotesRequestedM2ByFAOpen = tableNotesRequestedM2ByFAOpen });
                    list.Add(new { idPlant = plantGroup.idPlantGroup, Plant = plantGroup.Name, data = FailureData, dataAccActual = FailureDataAccActual, dataAcc = FailureDataAcc, tableNotesAnalyzedFA = tableNotesAnalyzedFA, tableNotesClosedMeasuresFA = tableNotesClosedMeasuresFA, tableNotesPendingFA = tableNotesPendingFA, tableNotesRequestedM2ByFA = tableNotesRequestedM2ByFA, tableNotesRequestedM2ByFAOpen = tableNotesRequestedM2ByFAOpen });
                }

            }


            //Total

            
            var FailureDataTotal = FailuresData
                    .Where(w => w.cdbudget == idBudget)
                    .OrderBy(O => O.dtYear)
                    .ThenBy(t => t.dtMonth)
                    .Select(s => new { s.monthYear, s.dtMonth, s.dtYear, vlAnalyzedFA = (s.vlCountAnalyzedFA), vlClosedMeasuresFA = (s.vlCountClosedMeasuresFA), vlPendingFA = (s.vlCountPendingFA), vlRequestedM2ByFA = s.vlCountRequestedM2ByFA, vlRequestedM2ByFAOpen = s.vlCountRequestedM2ByFAOpen })
                    .GroupBy(g => new { g.monthYear, g.dtMonth, g.dtYear})
                    .Select(s => new { s.Key.monthYear, s.Key.dtMonth, s.Key.dtYear, vlAnalyzedFA = s.Sum(s2 => s2.vlAnalyzedFA).Value, vlClosedMeasuresFA = s.Sum(s2 => s2.vlClosedMeasuresFA).Value, vlPendingFA = s.Sum(s2 => s2.vlPendingFA).Value, vlRequestedM2ByFA = s.Sum(s2 => s2.vlRequestedM2ByFA).Value, vlRequestedM2ByFAOpen = s.Sum(s2 => s2.vlRequestedM2ByFAOpen).Value })
                    .ToList();


            var FailureDataAccActualTotal = FailuresData
                    .Where(w => w.cdbudget == idBudget)
                    .OrderBy(O => O.dtYear)
                    .ThenBy(t => t.dtMonth)
                    .Select(s => new { yearToYear = s.Budget.dateStart.Year.ToString().Substring(2) + "/" + s.Budget.dateEnd.Year.ToString().Substring(2), s.monthYear, s.dtMonth, s.dtYear, vlAnalyzedFA = (s.vlCountAnalyzedFA), vlClosedMeasuresFA = (s.vlCountClosedMeasuresFA), vlPendingFA = (s.vlCountPendingFA), vlRequestedM2ByFA = s.vlCountRequestedM2ByFA, vlRequestedM2ByFAOpen = s.vlCountRequestedM2ByFAOpen })
                    .GroupBy(g => new { g.yearToYear, g.monthYear, g.dtMonth, g.dtYear})
                    .Select(s => new { s.Key.yearToYear, vlAnalyzedFA = s.Sum(s2 => s2.vlAnalyzedFA).Value, vlClosedMeasuresFA = s.Sum(s2 => s2.vlClosedMeasuresFA).Value, vlPendingFA = s.Sum(s2 => s2.vlPendingFA).Value, vlRequestedM2ByFA = s.Sum(s2 => s2.vlRequestedM2ByFA).Value, vlRequestedM2ByFAOpen = s.Sum(s2 => s2.vlRequestedM2ByFAOpen).Value })
                    .LastOrDefault();


        
            var FailureDataAccTotal = new List<object>();
            var budgetsTotal = FailuresData.Where(w => w.cdbudget != idBudget).Select(s => new { s.cdbudget, startYear = s.Budget.dateStart.Year, endYear = s.Budget.dateEnd.Year }).Distinct().OrderBy(o => o.startYear).ThenBy(t => t.endYear).ToList();
            foreach (var budget in budgetsTotal)
            {

                FailureDataAccTotal.Add(FailuresData
                    .Where(w => w.cdbudget == budget.cdbudget)
                    .OrderBy(O => O.dtYear)
                    .ThenBy(t => t.dtMonth)
                    .Select(s => new { yearToYear = s.Budget.dateStart.Year.ToString().Substring(2) + "/" + s.Budget.dateEnd.Year.ToString().Substring(2), s.monthYear, s.dtMonth, s.dtYear, vlAnalyzedFA = (s.vlCountAnalyzedFA), vlClosedMeasuresFA = (s.vlCountClosedMeasuresFA), vlPendingFA = s.vlCountPendingFA, vlRequestedM2ByFA = s.vlCountRequestedM2ByFA, vlRequestedM2ByFAOpen = s.vlCountRequestedM2ByFAOpen })
                    .GroupBy(g => new { g.yearToYear, g.monthYear, g.dtMonth, g.dtYear })
                    .Select(s => new { s.Key.yearToYear, vlAnalyzedFA = s.Sum(s2 => s2.vlAnalyzedFA).Value, vlClosedMeasuresFA = s.Sum(s2 => s2.vlClosedMeasuresFA).Value, vlPendingFA = s.Sum(s2 => s2.vlPendingFA).Value, vlRequestedM2ByFA = s.Sum(s2 => s2.vlRequestedM2ByFA).Value, vlRequestedM2ByFAOpen = s.Sum(s2 => s2.vlRequestedM2ByFAOpen).Value })
                    .LastOrDefault());
            }



            var dataEquipTotal = new List<object>();

            var tableNotesAnalyzedFATotal = new List<object>();
            var tableNotesClosedMeasuresFATotal = new List<object>();
            var tableNotesPendingFATotal = new List<object>();
            var tableNotesRequestedM2ByFATotal = new List<object>();
            var tableNotesRequestedM2ByFAOpenTotal = new List<object>();



            if (FailureDataTotal.Count > 0)
                list.Add(new { idPlant = -2, Plant = "TOTAL MAIN", data = FailureDataTotal, dataAccActual = FailureDataAccActualTotal, dataAcc = FailureDataAccTotal, dataEquipTotal, tableNotesAnalyzedFATotal, tableNotesClosedMeasuresFATotal, tableNotesPendingFATotal, tableNotesRequestedM2ByFATotal, tableNotesRequestedM2ByFAOpenTotal });






            return list;

        }

        [Route("GetFailureDataByPlant")]
        public List<object> GetFailureDataByPlant(int idIndicator, int idBudget, string plant)
        {
            var list = new List<object>();


            var PlantGroup = _PlantServ.GetPlantGroupBySheet(plant);
            var Plants = PlantGroup.Plants.Select(s => s.Sheet).ToList();
            var FailuresData = _FailureDataServ.GetFailureDataByPlants(idIndicator, idBudget, Plants);          


            var FailureData = FailuresData
                    .Where(w => w.cdbudget == idBudget)
                    .OrderBy(O => O.dtYear)
                    .ThenBy(t => t.dtMonth)
                    .Select(s => new { s.monthYear, s.dtMonth, s.dtYear, vlAnalyzedFA = (s.vlCountAnalyzedFA), vlClosedMeasuresFA = (s.vlCountClosedMeasuresFA), vlPendingFA = (s.vlCountPendingFA), vlRequestedM2ByFA = s.vlCountRequestedM2ByFA, vlRequestedM2ByFAOpen = s.vlCountRequestedM2ByFAOpen })
                    .GroupBy(g => new { g.monthYear, g.dtMonth, g.dtYear})
                    .Select(s => new { s.Key.monthYear, s.Key.dtMonth, s.Key.dtYear, vlAnalyzedFA = s.Sum(s2 => s2.vlAnalyzedFA).Value, vlClosedMeasuresFA = s.Sum(s2 => s2.vlClosedMeasuresFA).Value, vlPendingFA = s.Sum(s2 => s2.vlPendingFA).Value, vlRequestedM2ByFA = s.Sum(s2 => s2.vlRequestedM2ByFA).Value, vlRequestedM2ByFAOpen = s.Sum(s2 => s2.vlRequestedM2ByFAOpen).Value })                    
                    .ToList();

            var FailureDataAccActual = FailuresData
                       .Where(w => w.cdbudget == idBudget)
                       .OrderBy(O => O.dtYear)
                       .ThenBy(t => t.dtMonth)
                       .Select(s => new { yearToYear = s.Budget.dateStart.Year.ToString().Substring(2) + "/" + s.Budget.dateEnd.Year.ToString().Substring(2), s.monthYear, s.dtMonth, s.dtYear, vlAnalyzedFA = (s.vlCountAnalyzedFA), vlClosedMeasuresFA = (s.vlCountClosedMeasuresFA), vlPendingFA = (s.vlCountPendingFA), vlRequestedM2ByFA = s.vlCountRequestedM2ByFA, vlRequestedM2ByFAOpen = s.vlCountRequestedM2ByFAOpen })
                       .GroupBy(g => new { g.yearToYear, g.monthYear, g.dtMonth, g.dtYear})
                       .Select(s => new { s.Key.yearToYear, s.Key.dtMonth, s.Key.dtYear, vlAnalyzedFA = s.Sum(s2 => s2.vlAnalyzedFA).Value, vlClosedMeasuresFA = s.Sum(s2 => s2.vlClosedMeasuresFA).Value, vlPendingFA = s.Sum(s2 => s2.vlPendingFA).Value, vlRequestedM2ByFA = s.Sum(s2 => s2.vlRequestedM2ByFA).Value, vlRequestedM2ByFAOpen = s.Sum(s2 => s2.vlRequestedM2ByFAOpen).Value })
                       .LastOrDefault();

            var FailureDataTotal = FailuresData
                       .Where(w => w.cdbudget == idBudget)                      
                       .Select(s => new { yearToYear = s.Budget.dateStart.Year.ToString().Substring(2) + "/" + s.Budget.dateEnd.Year.ToString().Substring(2), vlAnalyzedFA = (s.vlCountAnalyzedFA), vlClosedMeasuresFA = (s.vlCountClosedMeasuresFA), vlPendingFA = (s.vlCountPendingFA), vlRequestedM2ByFA = s.vlCountRequestedM2ByFA, vlRequestedM2ByFAOpen = s.vlCountRequestedM2ByFAOpen })
                       .GroupBy(g => new { g.yearToYear})
                       .Select(s => new { s.Key.yearToYear, vlAnalyzedFA = s.Sum(s2 => s2.vlAnalyzedFA).Value, vlClosedMeasuresFA = s.Sum(s2 => s2.vlClosedMeasuresFA).Value, vlPendingFA = s.Sum(s2 => s2.vlPendingFA).Value, vlRequestedM2ByFA = s.Sum(s2 => s2.vlRequestedM2ByFA).Value, vlRequestedM2ByFAOpen = s.Sum(s2 => s2.vlRequestedM2ByFAOpen).Value })
                       .SingleOrDefault();

            var dateStart = new DateTime(FailureData.FirstOrDefault().dtYear, FailureData.FirstOrDefault().dtMonth, 1);
            var dateMonthEnd= new DateTime(FailureDataAccActual.dtYear, FailureDataAccActual.dtMonth, 1);
            var dateEnd = dateMonthEnd.AddMonths(1).AddDays(-1);



            var tableNotesAnalyzedFA = _NotificationServ.GetNotification(Plants, dateStart, dateEnd, TypeStatus.AnalyzedFA, false).Cast<object>().ToList();
            var tableNotesClosedMeasuresFA = _NotificationServ.GetNotification(Plants, dateStart, dateEnd, TypeStatus.ClosedMeasuresFA, false).Cast<object>().ToList();
            var tableNotesPendingFA = _NotificationServ.GetNotification(Plants, dateStart, dateEnd, TypeStatus.PendingFA, false).Cast<object>().ToList();
            var tableNotesRequestedM2ByFA = _NotificationServ.GetNotification(Plants, dateStart, dateEnd, TypeStatus.RequestedM2ByFA, false).Cast<object>().ToList();
            var tableNotesRequestedM2ByFAOpen = _NotificationServ.GetNotification(Plants, dateStart, dateEnd, TypeStatus.RequestedM2ByFAOpen, false).Cast<object>().ToList();




            list.Add(new { idPlant = PlantGroup.idPlantGroup, Plant = PlantGroup.Name, data = FailureData, dataAcc = FailureDataAccActual, dataTotal = FailureDataTotal, tableNotesAnalyzedFA, tableNotesClosedMeasuresFA, tableNotesPendingFA, tableNotesRequestedM2ByFA, tableNotesRequestedM2ByFAOpen });

            return list;


        }

        [Route("GetFailureDataNote")]
        public List<object> GetFailureDataNote(int idIndicator, int idBudget)
        {
            var list = new List<object>();


            var PlantsGroups = _PlantServ.GetPlantsGroups();
            var FailuresData = _FailureDataServ.GetFailureData(idIndicator, idBudget);

           
            foreach (var plantGroup in PlantsGroups.OrderBy(o => o.idPlantGroup))
            {
                var plantsName = plantGroup.Plants.Select(s => s.Sheet).ToList();
                var plants = plantGroup.Plants.Select(s => s.idPlant).ToList();
                var FailureData = FailuresData
                        .Where(w => plants.Contains(w.cdPlant) && w.cdbudget == idBudget)
                        .OrderBy(O => O.dtYear)
                        .ThenBy(t => t.dtMonth)
                        .Select(s => new { s.monthYear, s.dtMonth, s.dtYear, vlAnalyzedFA = (s.vlCountAnalyzedFA), vlClosedMeasuresFA = (s.vlCountClosedMeasuresFA), vlPendingFA = (s.vlCountPendingFA), vlRequestedM2ByFA = s.vlCountRequestedM2ByFA, vlRequestedM2ByFAOpen = s.vlCountRequestedM2ByFAOpen})
                        .GroupBy(g => new { g.monthYear, g.dtMonth, g.dtYear})
                        .Select(s => new { s.Key.monthYear, s.Key.dtMonth, s.Key.dtYear, vlAnalyzedFA = s.Sum(s2 => s2.vlAnalyzedFA).Value, vlClosedMeasuresFA = s.Sum(s2 => s2.vlClosedMeasuresFA).Value, vlPendingFA = s.Sum(s2 => s2.vlPendingFA).Value, vlRequestedM2ByFA = s.Sum(s2 => s2.vlRequestedM2ByFA).Value, vlRequestedM2ByFAOpen = s.Sum(s2 => s2.vlRequestedM2ByFAOpen).Value })                        
                        .ToList();


                var dateStart = new DateTime(FailureData.FirstOrDefault().dtYear, FailureData.FirstOrDefault().dtMonth, 1);
                var dateMonthEnd = new DateTime(FailureData.LastOrDefault().dtYear, FailureData.LastOrDefault().dtMonth, 1);
                var dateEnd = dateMonthEnd.AddMonths(1).AddDays(-1);

                var tableNotesAnalyzedFA = new List<object>();
                var tableNotesClosedMeasuresFA = new List<object>();
                var tableNotesPendingFA = new List<object>();
                var tableNotesRequestedM2ByFA = new List<object>();
                var tableNotesRequestedM2ByFAOpen = new List<object>();



                tableNotesAnalyzedFA = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.AnalyzedFA, false).Cast<object>().ToList();
                tableNotesClosedMeasuresFA = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.ClosedMeasuresFA, false).Cast<object>().ToList();
                tableNotesPendingFA = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.PendingFA, false).Cast<object>().ToList();
                tableNotesRequestedM2ByFA = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.RequestedM2ByFA, false).Cast<object>().ToList();
                tableNotesRequestedM2ByFAOpen = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.RequestedM2ByFAOpen, false).Cast<object>().ToList();
                


                if (FailureData.Count > 0)
                    list.Add(new { idPlant = plantGroup.idPlantGroup, Plant = plantGroup.Name, data = FailureData, tableNotesAnalyzedFA, tableNotesClosedMeasuresFA, tableNotesPendingFA, tableNotesRequestedM2ByFA, tableNotesRequestedM2ByFAOpen });

            }


            

            return list;

        }

       
    }
}
