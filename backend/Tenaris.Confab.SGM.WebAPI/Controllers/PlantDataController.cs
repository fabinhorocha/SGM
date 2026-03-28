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
    [RoutePrefix("api/PlantData")]
    public class PlantDataController : ApiController
    {
        private IPlantDataService _PlantDataServ;
        private IPlantService _PlantServ;
        private IEquipmentDataService _EquipDataServ;
        private IOrderHeadService _OrderHeadService;
        private INotificationService _NotificationServ;

        private IMapper _Mapper;


        public PlantDataController(IMapper Mapper, IPlantDataService PlantDataServ, IOrderHeadService OrderHeadService, IPlantService PlantServ, IEquipmentDataService EquipDataServ, INotificationService NotificationServ)
        {

            _Mapper = Mapper;
            _PlantDataServ = PlantDataServ;
            _OrderHeadService = OrderHeadService;
            _PlantServ = PlantServ;
            _EquipDataServ = EquipDataServ;
            _NotificationServ = NotificationServ;

        }

        [Route("GetPlantData")]
        public List<object> GetPlantData(int idIndicator, int idBudget, bool cdPredictive)
        {
            var list = new List<object>();


            var PlantsGroups = _PlantServ.GetPlantsGroups();
            var PlantsData = _PlantDataServ.GetPlantData(idIndicator, idBudget, cdPredictive);

            var EquipsData = _EquipDataServ.GetEquipmentData(idIndicator, idBudget);


            foreach (var plantGroup in PlantsGroups.OrderBy(o => o.idPlantGroup))
            {
                var plantsName = plantGroup.Plants.Select(s => s.Sheet).ToList();
                var plants = plantGroup.Plants.Select(s => s.idPlant).ToList();
                var PlantData = PlantsData
                        .Where(w => plants.Contains(w.cdPlant) && w.cdbudget == idBudget)
                        .OrderBy(O => O.dtYear)
                        .ThenBy(t => t.dtMonth)
                        .Select(s => new { s.monthYear, s.dtMonth, s.dtYear, vlGoal = s.vlGoal, vlPending = (s.vlCountNotesPending + s.vlCountOTsPending), vlOpen = (s.vlCountNotesOpen + s.vlCountOTsOpen), vlClosed = s.vlCountOTsClosed, vlHours = s.vlHoursHigh + s.vlHoursVeryHigh + s.vlHoursOther, s.vlHoursHigh, s.vlHoursVeryHigh, s.vlHoursOther })
                        .GroupBy(g => new { g.monthYear, g.dtMonth, g.dtYear, g.vlGoal })
                        .Select(s => new { s.Key.monthYear, s.Key.dtMonth, s.Key.dtYear, s.Key.vlGoal, vlPending = s.Sum(s2 => s2.vlPending).Value, vlOpen = s.Sum(s2 => s2.vlOpen).Value, vlClosed = s.Sum(s2 => s2.vlClosed).Value, vlHours = s.Sum(s3 => s3.vlHours).Value, vlHoursHigh = s.Sum(s4 => s4.vlHoursHigh).Value, vlHoursVeryHigh = s.Sum(s5 => s5.vlHoursVeryHigh).Value, vlHoursOther = s.Sum(s6 => s6.vlHoursOther).Value })
                        .Where(w=> w.vlClosed > 0 || w.vlHours > 0 || w.vlHoursHigh > 0 || w.vlHoursOther > 0 || w.vlHoursVeryHigh > 0 || w.vlOpen > 0 || w.vlPending > 0)
                        .ToList();


                var PlantDataAccActual = PlantsData
                        .Where(w => plants.Contains(w.cdPlant) && w.cdbudget == idBudget)
                        .OrderBy(O => O.dtYear)
                        .ThenBy(t => t.dtMonth)
                        .Select(s => new { yearToYear = s.Budget.dateStart.Year.ToString().Substring(2) + "/" + s.Budget.dateEnd.Year.ToString().Substring(2), s.monthYear, s.dtMonth, s.dtYear, vlGoal = s.vlGoal, vlPending = (s.vlCountNotesPending + s.vlCountOTsPending), vlOpen = (s.vlCountNotesOpen + s.vlCountOTsOpen), vlClosed = s.vlCountOTsClosed })
                        .GroupBy(g => new { g.yearToYear, g.monthYear, g.dtMonth, g.dtYear, g.vlGoal })
                        .Select(s => new { s.Key.yearToYear, s.Key.dtMonth, s.Key.dtYear, s.Key.vlGoal, vlPending = s.Sum(s2 => s2.vlPending).Value, vlOpen = s.Sum(s2 => s2.vlOpen).Value, vlClosed = s.Sum(s2 => s2.vlClosed).Value })
                        .LastOrDefault();


                var PlantTotalHours = PlantsData
                       .Where(w => plants.Contains(w.cdPlant) && w.cdbudget == idBudget)
                       .GroupBy(g => g.cdPlant)
                       .SelectMany(m => m.Where(w => new DateTime(w.dtYear, w.dtMonth, 1) == m.Max(d => new DateTime(d.dtYear, d.dtMonth, 1))))
                       .Select(s => new { s.Budget.idBudget, s.cdPlant, vlHours = s.vlHoursHigh + s.vlHoursVeryHigh + s.vlHoursOther, s.vlHoursHigh, s.vlHoursVeryHigh, s.vlHoursOther, vlCountHigh = s.vlCountOTsHigh + s.vlCountNotesHigh, vlCountVeryHigh = s.vlCountNotesVeryHigh + s.vlCountOTsVeryHigh, vlCountOther = s.vlCountOTsOther + s.vlCountNotesOther })
                       .GroupBy(g => g.idBudget)                                              
                       .Select(s => new { vlHours = s.Sum(s3 => s3.vlHours).Value, vlHoursHigh = s.Sum(s4 => s4.vlHoursHigh).Value, vlHoursVeryHigh = s.Sum(s5 => s5.vlHoursVeryHigh).Value, vlHoursOther = s.Sum(s6 => s6.vlHoursOther).Value, vlCountHigh = s.Sum(s7 => s7.vlCountHigh).Value, vlCountVeryHigh = s.Sum(s7 => s7.vlCountVeryHigh).Value, vlCountOther = s.Sum(s7 => s7.vlCountOther).Value })
                       .ToList();

                var PlantDataAcc = new List<object>();
                var budgets = PlantsData.Where(w => plants.Contains(w.cdPlant) && w.cdbudget != idBudget).Select(s => new { s.cdbudget, startYear = s.Budget.dateStart.Year, endYear = s.Budget.dateEnd.Year }).Distinct().OrderBy(o => o.startYear).ThenBy(t => t.endYear).ToList();
                foreach (var budget in budgets)
                {

                    PlantDataAcc.Add(PlantsData
                        .Where(w => plants.Contains(w.cdPlant) && w.cdbudget == budget.cdbudget)
                        .OrderBy(O => O.dtYear)
                        .ThenBy(t => t.dtMonth)
                        .Select(s => new { yearToYear = s.Budget.dateStart.Year.ToString().Substring(2) + "/" + s.Budget.dateEnd.Year.ToString().Substring(2), s.monthYear, s.dtMonth, s.dtYear, vlGoal = s.vlGoal, vlPending = (s.vlCountNotesPending + s.vlCountOTsPending), vlOpen = (s.vlCountNotesOpen + s.vlCountOTsOpen), vlClosed = s.vlCountOTsClosed })
                        .GroupBy(g => new { g.yearToYear, g.monthYear, g.dtMonth, g.dtYear, g.vlGoal })
                        .Select(s => new { s.Key.yearToYear, s.Key.vlGoal, vlPending = s.Sum(s2 => s2.vlPending).Value, vlOpen = s.Sum(s2 => s2.vlOpen).Value, vlClosed = s.Sum(s2 => s2.vlClosed).Value })
                        .LastOrDefault());
                }

                

                var dataEquip = EquipsData
                            .Where(w => w.cdbudget == idBudget && w.EquipmentPlantGroup.cdPlantGroup == plantGroup.idPlantGroup)
                            .GroupBy(g => new { id = g.EquipmentPlantGroup.idEquipmentPlantGroup })
                            .SelectMany(m => m.Where(w => new DateTime(w.dtYear, w.dtMonth, 1) == m.Max(d => new DateTime(d.dtYear, d.dtMonth, 1))))
                            .Select(s => new { id = s.EquipmentPlantGroup.idEquipmentPlantGroup, name = s.EquipmentPlantGroup.Name, vlPending = s.vlCountOTsPending + s.vlCountNotesPending, vlHigh = s.vlCountNotesVeryHigh + s.vlCountNotesHigh + s.vlCountOTsVeryHigh + s.vlCountOTsHigh })
                            .OrderByDescending(o => o.vlPending).Take(15).ToList();


                var dateStart = new DateTime(PlantDataAccActual.dtYear, PlantDataAccActual.dtMonth, 1);
                var dateEnd = dateStart.AddMonths(1).AddDays(-1);
                var tableNotesPending = new List<object>();
                var tableNotesOpen = new List<object>();
                var tableOtsPending = new List<object>();
                var tableOtsOpen = new List<object>();
                var tableOtsClosed = new List<object>();

                if(plantGroup.Name == "ENFA")
                {
                    tableNotesPending = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.PendingEnfa, cdPredictive).Cast<object>().ToList();
                    tableNotesOpen = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.OpenEnfa, cdPredictive).Cast<object>().ToList();
                    tableOtsPending = _OrderHeadService.GetOrderHead(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.PendingEnfa, cdPredictive).Cast<object>().ToList();
                    tableOtsOpen = _OrderHeadService.GetOrderHead(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.OpenEnfa, cdPredictive).Cast<object>().ToList();
                    tableOtsClosed = _OrderHeadService.GetOrderHead(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.ClosedEnfa, cdPredictive).Cast<object>().ToList();
                }
                else
                {
                    tableNotesPending = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.Pending, cdPredictive).Cast<object>().ToList();
                    tableNotesOpen = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.Open, cdPredictive).Cast<object>().ToList();
                    tableOtsPending = _OrderHeadService.GetOrderHead(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.Pending, cdPredictive).Cast<object>().ToList();
                    tableOtsOpen = _OrderHeadService.GetOrderHead(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.Open, cdPredictive).Cast<object>().ToList();
                    tableOtsClosed = _OrderHeadService.GetOrderHead(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.Closed, cdPredictive).Cast<object>().ToList();
                }

                if (PlantData.Count > 0)
                    list.Add(new { idPlant = plantGroup.idPlantGroup, Plant = plantGroup.Name, data = PlantData, dataAccActual = PlantDataAccActual, dataAcc = PlantDataAcc, dataEquip, dataTotalHours = PlantTotalHours, tableNotesPending = tableNotesPending, tableOtsPending = tableOtsPending, tableOtsOpen = tableOtsOpen,  tableOtsClosed = tableOtsClosed, tableNotesOpen = tableNotesOpen });

            }


            //Total

            
            var PlantDataTotal = PlantsData
                    .Where(w => w.cdbudget == idBudget)
                    .OrderBy(O => O.dtYear)
                    .ThenBy(t => t.dtMonth)
                    .Select(s => new { s.monthYear, s.dtMonth, s.dtYear, vlGoal = s.vlGoal, vlPending = (s.vlCountNotesPending + s.vlCountOTsPending), vlOpen = (s.vlCountNotesOpen + s.vlCountOTsOpen), vlClosed = s.vlCountOTsClosed, vlHours = s.vlHoursHigh + s.vlHoursVeryHigh + s.vlHoursOther, s.vlHoursHigh, s.vlHoursVeryHigh, s.vlHoursOther })
                    .GroupBy(g => new { g.monthYear, g.dtMonth, g.dtYear})
                    .Select(s => new { s.Key.monthYear, s.Key.dtMonth, s.Key.dtYear, vlGoal = s.Sum(s2=>s2.vlGoal).Value , vlPending = s.Sum(s2 => s2.vlPending).Value, vlOpen = s.Sum(s2 => s2.vlOpen).Value, vlClosed = s.Sum(s2 => s2.vlClosed).Value, vlHours = s.Sum(s3 => s3.vlHours).Value, vlHoursHigh = s.Sum(s4 => s4.vlHoursHigh).Value, vlHoursVeryHigh = s.Sum(s5 => s5.vlHoursVeryHigh).Value, vlHoursOther = s.Sum(s6 => s6.vlHoursOther).Value })
                    .ToList();


            var PlantDataAccActualTotal = PlantsData
                    .Where(w => w.cdbudget == idBudget)
                    .OrderBy(O => O.dtYear)
                    .ThenBy(t => t.dtMonth)
                    .Select(s => new { yearToYear = s.Budget.dateStart.Year.ToString().Substring(2) + "/" + s.Budget.dateEnd.Year.ToString().Substring(2), s.monthYear, s.dtMonth, s.dtYear, vlGoal = s.vlGoal, vlPending = (s.vlCountNotesPending + s.vlCountOTsPending), vlOpen = (s.vlCountNotesOpen + s.vlCountOTsOpen), vlClosed = s.vlCountOTsClosed })
                    .GroupBy(g => new { g.yearToYear, g.monthYear, g.dtMonth, g.dtYear})
                    .Select(s => new { s.Key.yearToYear, vlGoal = s.Sum(s2 => s2.vlGoal).Value, vlPending = s.Sum(s2 => s2.vlPending).Value, vlOpen = s.Sum(s2 => s2.vlOpen).Value, vlClosed = s.Sum(s2 => s2.vlClosed).Value })
                    .LastOrDefault();


            var PlantTotalHoursTotal = PlantsData
                   .Where(w => w.cdbudget == idBudget)
                   .GroupBy(g => g.cdPlant)
                   .SelectMany(m => m.Where(w => new DateTime(w.dtYear, w.dtMonth, 1) == m.Max(d => new DateTime(d.dtYear, d.dtMonth, 1))))
                   .Select(s => new { s.Budget.idBudget, s.cdPlant, vlHours = s.vlHoursHigh + s.vlHoursVeryHigh + s.vlHoursOther, s.vlHoursHigh, s.vlHoursVeryHigh, s.vlHoursOther, vlCountHigh = s.vlCountOTsHigh + s.vlCountNotesHigh, vlCountVeryHigh = s.vlCountNotesVeryHigh + s.vlCountOTsVeryHigh, vlCountOther = s.vlCountOTsOther + s.vlCountNotesOther })
                   .GroupBy(g => g.idBudget)
                   .Select(s => new { vlHours = s.Sum(s3 => s3.vlHours).Value, vlHoursHigh = s.Sum(s4 => s4.vlHoursHigh).Value, vlHoursVeryHigh = s.Sum(s5 => s5.vlHoursVeryHigh).Value, vlHoursOther = s.Sum(s6 => s6.vlHoursOther).Value, vlCountHigh = s.Sum(s7 => s7.vlCountHigh).Value, vlCountVeryHigh = s.Sum(s7 => s7.vlCountVeryHigh).Value, vlCountOther = s.Sum(s7 => s7.vlCountOther).Value })
                   .ToList();

            var PlantDataAccTotal = new List<object>();
            var budgetsTotal = PlantsData.Where(w => w.cdbudget != idBudget).Select(s => new { s.cdbudget, startYear = s.Budget.dateStart.Year, endYear = s.Budget.dateEnd.Year }).Distinct().OrderBy(o => o.startYear).ThenBy(t => t.endYear).ToList();
            foreach (var budget in budgetsTotal)
            {

                PlantDataAccTotal.Add(PlantsData
                    .Where(w => w.cdbudget == budget.cdbudget)
                    .OrderBy(O => O.dtYear)
                    .ThenBy(t => t.dtMonth)
                    .Select(s => new { yearToYear = s.Budget.dateStart.Year.ToString().Substring(2) + "/" + s.Budget.dateEnd.Year.ToString().Substring(2), s.monthYear, s.dtMonth, s.dtYear, vlGoal = s.vlGoal, vlPending = (s.vlCountNotesPending + s.vlCountOTsPending), vlOpen = (s.vlCountNotesOpen + s.vlCountOTsOpen), vlClosed = s.vlCountOTsClosed })
                    .GroupBy(g => new { g.yearToYear, g.monthYear, g.dtMonth, g.dtYear })
                    .Select(s => new { s.Key.yearToYear, vlGoal = s.Sum(s2 => s2.vlGoal).Value, vlPending = s.Sum(s2 => s2.vlPending).Value, vlOpen = s.Sum(s2 => s2.vlOpen).Value, vlClosed = s.Sum(s2 => s2.vlClosed).Value })
                    .LastOrDefault());
            }



            var dataEquipTotal = new List<object>();

            var tableNotesPendingTotal = new List<object>();
            var tableNotesOpenTotal = new List<object>();

            var tableOtsPendingTotal = new List<object>();
            var tableOtsOpenTotal = new List<object>();
            var tableOtsClosedTotal = new List<object>();



            if (PlantDataTotal.Count > 0)
                list.Add(new { idPlant = -2, Plant = "TOTAL MAIN", data = PlantDataTotal, dataAccActual = PlantDataAccActualTotal, dataAcc = PlantDataAccTotal, dataEquipTotal, dataTotalHours = PlantTotalHoursTotal, tableNotesPending = tableNotesPendingTotal, tableOtsPending = tableOtsPendingTotal, tableOtsOpen = tableOtsOpenTotal, tableOtsClosed = tableOtsClosedTotal, tableNotesOpen = tableNotesOpenTotal });






            return list;

        }

        [Route("GetPlantDataByPlant")]
        public List<object> GetPlantDataByPlant(int idIndicator, int idBudget, bool cdPredictive, string plant)
        {
            var list = new List<object>();


            var PlantGroup = _PlantServ.GetPlantGroupBySheet(plant);
            var Plants = PlantGroup.Plants.Select(s => s.Sheet).ToList();
            var PlantsData = _PlantDataServ.GetPlantDataByPlants(idIndicator, idBudget, cdPredictive, Plants);          


            var PlantData = PlantsData
                    .Where(w => w.cdbudget == idBudget)
                    .OrderBy(O => O.dtYear)
                    .ThenBy(t => t.dtMonth)
                    .Select(s => new { s.monthYear, s.dtMonth, s.dtYear, vlPending = s.vlCountNotesPending, vlOpen = s.vlCountOTsPendingOpen, vlReleased = s.vlCountOTsPendingReleased, vlClosedOnTime = s.vlCountOTsClosed - s.vlCountOTsClosedDelay, vlClosed = s.vlCountOTsClosed })
                    .GroupBy(g => new { g.monthYear, g.dtMonth, g.dtYear})
                    .Select(s => new { s.Key.monthYear, s.Key.dtMonth, s.Key.dtYear, vlPending = s.Sum(s1 => s1.vlPending).Value, vlOpen = s.Sum(s1 => s1.vlOpen).Value, vlReleased = s.Sum(s1 => s1.vlReleased).Value, vlClosedOnTime = s.Sum(s1 => s1.vlClosedOnTime).Value, vlClosed = s.Sum(s1 => s1.vlClosed).Value })                    
                    .ToList();

            var PlantDataAccActual = PlantsData
                       .Where(w => w.cdbudget == idBudget)
                       .OrderBy(O => O.dtYear)
                       .ThenBy(t => t.dtMonth)
                       .Select(s => new { yearToYear = s.Budget.dateStart.Year.ToString().Substring(2) + "/" + s.Budget.dateEnd.Year.ToString().Substring(2), s.monthYear, s.dtMonth, s.dtYear, vlPending = s.vlCountNotesPending, vlOpen = s.vlCountOTsPendingOpen, vlReleased = s.vlCountOTsPendingReleased, vlClosedOnTime = s.vlCountOTsClosed - s.vlCountOTsClosedDelay, vlClosed = s.vlCountOTsClosed })
                       .GroupBy(g => new { g.yearToYear, g.monthYear, g.dtMonth, g.dtYear})
                       .Select(s => new { s.Key.yearToYear, s.Key.dtMonth, s.Key.dtYear, vlPending = s.Sum(s1 => s1.vlPending).Value, vlOpen = s.Sum(s1 => s1.vlOpen).Value, vlReleased = s.Sum(s1 => s1.vlReleased).Value, vlClosedOnTime = s.Sum(s1 => s1.vlClosedOnTime).Value, vlClosed = s.Sum(s1 => s1.vlClosed).Value})
                       .LastOrDefault();

            var PlantDataTotal = PlantsData
                       .Where(w => w.cdbudget == idBudget)                      
                       .Select(s => new { yearToYear = s.Budget.dateStart.Year.ToString().Substring(2) + "/" + s.Budget.dateEnd.Year.ToString().Substring(2), vlPending = s.vlCountNotesPending, vlOpen = s.vlCountOTsPendingOpen, vlReleased = s.vlCountOTsPendingReleased, vlClosedOnTime = s.vlCountOTsClosed - s.vlCountOTsClosedDelay, vlClosed = s.vlCountOTsClosed })
                       .GroupBy(g => new { g.yearToYear})
                       .Select(s => new { s.Key.yearToYear, vlPending = s.Sum(s1 => s1.vlPending).Value, vlOpen = s.Sum(s1 => s1.vlOpen).Value, vlReleased = s.Sum(s1 => s1.vlReleased).Value, vlClosedOnTime = s.Sum(s1 => s1.vlClosedOnTime).Value, vlClosed = s.Sum(s1 => s1.vlClosed).Value })
                       .SingleOrDefault();

            var dateStart = new DateTime(PlantData.FirstOrDefault().dtYear, PlantData.FirstOrDefault().dtMonth, 1);
            var dateMonthEnd= new DateTime(PlantDataAccActual.dtYear, PlantDataAccActual.dtMonth, 1);
            var dateEnd = dateMonthEnd.AddMonths(1).AddDays(-1);


            var tablePending = _NotificationServ.GetNotification(Plants, dateStart, dateEnd, TypeStatus.Pending, cdPredictive).Cast<object>().ToList();
            var tableOpen = _OrderHeadService.GetOrderHead(Plants, dateStart, dateEnd, TypeStatus.PendingOpen, cdPredictive).Cast<object>().ToList();
            var tableReleased = _OrderHeadService.GetOrderHead(Plants, dateStart, dateEnd, TypeStatus.PendingReleased, cdPredictive).Cast<object>().ToList();
            var tableClosedOnTime = _OrderHeadService.GetOrderHead(Plants, dateStart, dateEnd, TypeStatus.ClosedOnTime, cdPredictive).Cast<object>().ToList();
            var tableClosedDelay = _OrderHeadService.GetOrderHead(Plants, dateStart, dateEnd, TypeStatus.ClosedDelay, cdPredictive).Cast<object>().ToList();


            list.Add(new { idPlant = PlantGroup.idPlantGroup, Plant = PlantGroup.Name, data = PlantData, dataAcc = PlantDataAccActual, dataTotal = PlantDataTotal, tablePending, tableOpen, tableReleased, tableClosedOnTime, tableClosedDelay });

            return list;


        }

        [Route("GetPlantDataNote")]
        public List<object> GetPlantDataNote(int idIndicator, int idBudget, bool cdPredictive)
        {
            var list = new List<object>();


            var PlantsGroups = _PlantServ.GetPlantsGroups();
            var PlantsData = _PlantDataServ.GetPlantData(idIndicator, idBudget, cdPredictive);

           
            foreach (var plantGroup in PlantsGroups.OrderBy(o => o.idPlantGroup))
            {
                var plantsName = plantGroup.Plants.Select(s => s.Sheet).ToList();
                var plants = plantGroup.Plants.Select(s => s.idPlant).ToList();
                var PlantData = PlantsData
                        .Where(w => plants.Contains(w.cdPlant) && w.cdbudget == idBudget)
                        .OrderBy(O => O.dtYear)
                        .ThenBy(t => t.dtMonth)
                        .Select(s => new { s.monthYear, s.dtMonth, s.dtYear, vlGoal = s.vlGoal, vlPending = (s.vlCountNotesPending), vlPendingDelay = (s.vlCountNotesPendingDelay), vlOpen = (s.vlCountNotesOpen), vlClosed = s.vlCountNotesClosed})
                        .GroupBy(g => new { g.monthYear, g.dtMonth, g.dtYear, g.vlGoal })
                        .Select(s => new { s.Key.monthYear, s.Key.dtMonth, s.Key.dtYear, s.Key.vlGoal, vlPending = s.Sum(s2 => s2.vlPending).Value, vlPendingDelay = s.Sum(s2 => s2.vlPendingDelay).Value, vlOpen = s.Sum(s2 => s2.vlOpen).Value, vlClosed = s.Sum(s2 => s2.vlClosed).Value})                        
                        .ToList();


                var dateStart = new DateTime(PlantData.FirstOrDefault().dtYear, PlantData.FirstOrDefault().dtMonth, 1);
                var dateMonthEnd = new DateTime(PlantData.LastOrDefault().dtYear, PlantData.LastOrDefault().dtMonth, 1);
                var dateEnd = dateMonthEnd.AddMonths(1).AddDays(-1);

                var tableNotesClosed = new List<object>();
                var tableNotesPendingDelay = new List<object>();
                var tableNotesPendingOnTime = new List<object>();
               

                if (plantGroup.Name == "ENFA")
                {
                    tableNotesClosed = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.ClosedEnfa, cdPredictive).Cast<object>().ToList();
                    tableNotesPendingDelay = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.PendingDelayEnfa, cdPredictive).Cast<object>().ToList();
                    tableNotesPendingOnTime = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.PendingOnTimeEnfa, cdPredictive).Cast<object>().ToList();
                    
                }
                else
                {
                    tableNotesClosed = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.Closed, cdPredictive).Cast<object>().ToList();
                    tableNotesPendingDelay = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.PendingDelay, cdPredictive).Cast<object>().ToList();
                    tableNotesPendingOnTime = _NotificationServ.GetNotification(plantsName.Distinct().ToList(), dateStart, dateEnd, TypeStatus.PendingOnTime, cdPredictive).Cast<object>().ToList();
                }

                if (PlantData.Count > 0)
                    list.Add(new { idPlant = plantGroup.idPlantGroup, Plant = plantGroup.Name, data = PlantData, tableNotesClosed, tableNotesPendingDelay, tableNotesPendingOnTime });

            }


            

            return list;

        }

       
    }
}
