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
    [RoutePrefix("api/LocationData")]
    public class LocationDataController : ApiController
    {
        private ILocationDataService _LocationDataServ;
        private ILocationLocalService _LocationLocalServ;
        private IOrderHeadService _OrderHeadService;

        private IMapper _Mapper;


        public LocationDataController(IMapper Mapper, ILocationDataService LocationDataServ, IOrderHeadService OrderHeadService, ILocationLocalService LocationLocalServ)
        {

            _Mapper = Mapper;
            _LocationDataServ = LocationDataServ;
            _OrderHeadService = OrderHeadService;
            _LocationLocalServ = LocationLocalServ;


        }

        [Route("GetLocationData")]
        public List<object> GetLocationData(int idIndicator, int idBudget)
        {
            var list = new List<object>();
            var locationsData = _LocationDataServ.GetLocationData(idIndicator, idBudget);

            var locationsPlants = locationsData.Select(s => new { s.LocationPlant.idLocationPlant, s.LocationPlant.Location.idLocation, Location = s.LocationPlant.Location.Name, s.LocationPlant.Plant.idPlant, Plant = s.LocationPlant.Plant.Sheet, PlantName = s.LocationPlant.Plant.Name }).Distinct().ToList();

           
            foreach (var locationPlant in locationsPlants.OrderBy(o => o.idLocationPlant))
            {

                


                var locationData = locationsData.Where(w => w.cdLocationPlant == locationPlant.idLocationPlant && w.cdbudget == idBudget).OrderBy(O => O.dtYear).ThenBy(t => t.dtMonth).GroupBy(g => new { g.monthYear, g.dtMonth, g.dtYear }).Select(s => new { s.Key.monthYear, s.Key.dtMonth, s.Key.dtYear, vlReal = (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value) <= 0 ? 0 : Math.Round((s.Sum(ts => ts.vlExec).Value / (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value)) * 100, 1), vlCount = s.Sum(ts => ts.vlCount).Value, vlNExec = s.Sum(ts => ts.vlNExec).Value }).ToList();
                var locationDataAccActual = locationsData.Where(w => w.cdLocationPlant == locationPlant.idLocationPlant && w.cdbudget == idBudget).GroupBy(g => new { yearStart = g.Budget.dateStart.Year, yearEnd = g.Budget.dateEnd.Year, g.vlGoal }).Select(s => new { yearToYear = s.Key.yearStart.ToString().Substring(2) + "/" + s.Key.yearEnd.ToString().Substring(2), vlGoal = s.Key.vlGoal, vlRealAcc = (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value) <= 0 ? 0 : Math.Round((s.Sum(ts => ts.vlExec).Value / (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value)) * 100, 1) }).SingleOrDefault();
                var locationDataAcc = locationsData.Where(w => w.cdLocationPlant == locationPlant.idLocationPlant && w.cdbudget != idBudget).OrderBy(O => O.Budget.dateStart.Year).ThenBy(t => t.Budget.dateEnd.Year).GroupBy(g => new { yearStart = g.Budget.dateStart.Year, yearEnd = g.Budget.dateEnd.Year, g.vlGoal }).Select(s => new { yearToYear = s.Key.yearStart.ToString().Substring(2) + "/" + s.Key.yearEnd.ToString().Substring(2), vlGoal = s.Key.vlGoal, vlRealAcc = (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value) <= 0 ? 0 : Math.Round((s.Sum(ts => ts.vlExec).Value / (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value)) * 100, 1) }).ToList();

                var ordershead = (locationData.Count > 0 ? _OrderHeadService.GetOrderHead(locationPlant.Plant, locationPlant.idLocation, idIndicator, locationData.Last().dtMonth, locationData.Last().dtYear) : null);


                
                var specialitys = locationsData.Where(w => w.cdLocationPlant == locationPlant.idLocationPlant && w.cdSpeciality != null).Select(s => new { s.speciality.idSpeciality, s.speciality.Name }).Distinct().ToList();

                var dataSpecialitys = new List<object>();
                foreach (var speciality in specialitys)
                {
                    var specialityData = locationsData.Where(w => w.cdLocationPlant == locationPlant.idLocationPlant && w.cdbudget == idBudget && w.cdSpeciality == speciality.idSpeciality).OrderBy(O => O.dtYear).ThenBy(t => t.dtMonth).GroupBy(g => new { g.monthYear, g.dtMonth, g.dtYear }).Select(s => new { s.Key.monthYear, s.Key.dtMonth, s.Key.dtYear, vlReal = (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value) <= 0 ? 0 : Math.Round((s.Sum(ts => ts.vlExec).Value / (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value)) * 100, 1), vlCount = s.Sum(ts => ts.vlCount), vlNExec = s.Sum(ts => ts.vlNExec) }).ToList();
                    var specialityDataAccActual = locationsData.Where(w => w.cdLocationPlant == locationPlant.idLocationPlant && w.cdbudget == idBudget && w.cdSpeciality == speciality.idSpeciality).GroupBy(g => new { yearStart = g.Budget.dateStart.Year, yearEnd = g.Budget.dateEnd.Year, g.vlGoal }).Select(s => new { yearToYear = s.Key.yearStart.ToString().Substring(2) + "/" + s.Key.yearEnd.ToString().Substring(2), vlGoal = s.Key.vlGoal, vlRealAcc = (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value) <= 0 ? 0 : Math.Round((s.Sum(ts => ts.vlExec).Value / (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value)) * 100, 1) }).SingleOrDefault();
                    var specialityAcc = locationsData.Where(w => w.cdLocationPlant == locationPlant.idLocationPlant && w.cdbudget != idBudget && w.cdSpeciality == speciality.idSpeciality).OrderBy(O => O.Budget.dateStart.Year).ThenBy(t => t.Budget.dateEnd.Year).GroupBy(g => new { yearStart = g.Budget.dateStart.Year, yearEnd = g.Budget.dateEnd.Year, g.vlGoal }).Select(s => new { yearToYear = s.Key.yearStart.ToString().Substring(2) + "/" + s.Key.yearEnd.ToString().Substring(2), vlGoal = s.Key.vlGoal, vlRealAcc = (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value) <= 0 ? 0 : Math.Round((s.Sum(ts => ts.vlExec).Value / (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value)) * 100, 1) }).ToList();

                    if(specialityData.Count > 0)
                        dataSpecialitys.Add(new { id = speciality.idSpeciality, name = speciality.Name, data = specialityData, dataAccActual = specialityDataAccActual, dataAcc = specialityAcc });
                }


                var locationLocals = _LocationLocalServ.GetLocationLocal(locationPlant.idLocation);

                var dataLocals = new List<object>();
                foreach (var local in locationLocals)
                {
                    var localData = locationsData.Where(w => w.cdLocationPlant == locationPlant.idLocationPlant && w.cdbudget == idBudget && w.cdLocationLocal == local.idLocationLocal).OrderBy(O => O.dtYear).ThenBy(t => t.dtMonth).GroupBy(g => new { g.monthYear, g.dtMonth, g.dtYear }).Select(s => new { s.Key.monthYear, s.Key.dtMonth, s.Key.dtYear, vlReal = (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value) <= 0 ? 0 : Math.Round((s.Sum(ts => ts.vlExec).Value / (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value)) * 100, 1), vlCount = s.Sum(ts => ts.vlCount), vlNExec = s.Sum(ts => ts.vlNExec) }).ToList();
                    var localDataAccActual = locationsData.Where(w => w.cdLocationPlant == locationPlant.idLocationPlant && w.cdbudget == idBudget && w.cdLocationLocal == local.idLocationLocal).GroupBy(g => new { yearStart = g.Budget.dateStart.Year, yearEnd = g.Budget.dateEnd.Year, g.vlGoal }).Select(s => new { yearToYear = s.Key.yearStart.ToString().Substring(2) + "/" + s.Key.yearEnd.ToString().Substring(2), vlGoal = s.Key.vlGoal, vlRealAcc = (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value) <= 0 ? 0 : Math.Round((s.Sum(ts => ts.vlExec).Value / (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value)) * 100, 1) }).SingleOrDefault();
                    var localAcc = locationsData.Where(w => w.cdLocationPlant == locationPlant.idLocationPlant && w.cdbudget != idBudget && w.cdLocationLocal == local.idLocationLocal).OrderBy(O => O.Budget.dateStart.Year).ThenBy(t => t.Budget.dateEnd.Year).GroupBy(g => new { yearStart = g.Budget.dateStart.Year, yearEnd = g.Budget.dateEnd.Year, g.vlGoal }).Select(s => new { yearToYear = s.Key.yearStart.ToString().Substring(2) + "/" + s.Key.yearEnd.ToString().Substring(2), vlGoal = s.Key.vlGoal, vlRealAcc = (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value) <= 0 ? 0 : Math.Round((s.Sum(ts => ts.vlExec).Value / (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value)) * 100, 1) }).ToList();

                    if(localData.Count > 0)
                        dataLocals.Add(new { id = local.cdLocal, name = local.Local.Description, data = localData, dataAccActual = localDataAccActual, dataAcc = localAcc });

                }

                if(locationData.Count > 0)
                    list.Add(new { locationPlant.idLocationPlant, idLocation = locationPlant.idLocation, location = locationPlant.Location, plant = locationPlant.Plant, data = locationData, dataAccActual = locationDataAccActual, dataAcc = locationDataAcc, dataDetails = ordershead, specialitys = dataSpecialitys, locals = dataLocals });

            }


            var plantsGroups = locationsPlants
                                       .GroupBy(g => new { g.idPlant, g.Plant, g.PlantName })
                                       .Where(w => w.Count() > 1)
                                       .Select(s => new { s.Key.idPlant, Sheet = s.Key.Plant , Name = s.Key.PlantName })
                                       .ToList();


            foreach (var plant in plantsGroups.OrderBy(o => o.idPlant))
            {


                var idlocationsPlants = locationsPlants.Where(w => w.idPlant == plant.idPlant).Select(s=>s.idLocationPlant).ToList();



                var locationData = locationsData.Where(w => idlocationsPlants.Contains(w.cdLocationPlant) && w.cdbudget == idBudget).OrderBy(O => O.dtYear).ThenBy(t => t.dtMonth).GroupBy(g => new { g.monthYear, g.dtMonth, g.dtYear }).Select(s => new { s.Key.monthYear, s.Key.dtMonth, s.Key.dtYear, vlReal = (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value) <= 0 ? 0 : Math.Round((s.Sum(ts => ts.vlExec).Value / (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value)) * 100, 1), vlCount = s.Sum(ts => ts.vlCount).Value, vlNExec = s.Sum(ts => ts.vlNExec).Value }).ToList();
                var locationDataAccActual = locationsData.Where(w => idlocationsPlants.Contains(w.cdLocationPlant) && w.cdbudget == idBudget).GroupBy(g => new { yearStart = g.Budget.dateStart.Year, yearEnd = g.Budget.dateEnd.Year, g.vlGoal }).Select(s => new { yearToYear = s.Key.yearStart.ToString().Substring(2) + "/" + s.Key.yearEnd.ToString().Substring(2), vlGoal = s.Key.vlGoal, vlRealAcc = (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value) <= 0 ? 0 : Math.Round((s.Sum(ts => ts.vlExec).Value / (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value)) * 100, 1) }).SingleOrDefault();
                var locationDataAcc = locationsData.Where(w => idlocationsPlants.Contains(w.cdLocationPlant) && w.cdbudget != idBudget).OrderBy(O => O.Budget.dateStart.Year).ThenBy(t => t.Budget.dateEnd.Year).GroupBy(g => new { yearStart = g.Budget.dateStart.Year, yearEnd = g.Budget.dateEnd.Year, g.vlGoal }).Select(s => new { yearToYear = s.Key.yearStart.ToString().Substring(2) + "/" + s.Key.yearEnd.ToString().Substring(2), vlGoal = s.Key.vlGoal, vlRealAcc = (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value) <= 0 ? 0 : Math.Round((s.Sum(ts => ts.vlExec).Value / (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value)) * 100, 1) }).ToList();

                var ordershead = (locationData.Count > 0 ? _OrderHeadService.GetOrderHead(plant.Sheet, null, idIndicator, locationData.Last().dtMonth, locationData.Last().dtYear) : null);


                var specialitys = locationsData.Where(w => idlocationsPlants.Contains(w.cdLocationPlant) && w.cdSpeciality != null).Select(s => new { s.speciality.idSpeciality, s.speciality.Name }).Distinct().ToList();

                var dataSpecialitys = new List<object>();
                foreach (var speciality in specialitys)
                {
                    var specialityData = locationsData.Where(w => idlocationsPlants.Contains(w.cdLocationPlant) && w.cdbudget == idBudget && w.cdSpeciality == speciality.idSpeciality).OrderBy(O => O.dtYear).ThenBy(t => t.dtMonth).GroupBy(g => new { g.monthYear, g.dtMonth, g.dtYear }).Select(s => new { s.Key.monthYear, s.Key.dtMonth, s.Key.dtYear, vlReal = (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value) <= 0 ? 0 : Math.Round((s.Sum(ts => ts.vlExec).Value / (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value)) * 100, 1), vlCount = s.Sum(ts => ts.vlCount), vlNExec = s.Sum(ts => ts.vlNExec) }).ToList();
                    var specialityDataAccActual = locationsData.Where(w => idlocationsPlants.Contains(w.cdLocationPlant) && w.cdbudget == idBudget && w.cdSpeciality == speciality.idSpeciality).GroupBy(g => new { yearStart = g.Budget.dateStart.Year, yearEnd = g.Budget.dateEnd.Year, g.vlGoal }).Select(s => new { yearToYear = s.Key.yearStart.ToString().Substring(2) + "/" + s.Key.yearEnd.ToString().Substring(2), vlGoal = s.Key.vlGoal, vlRealAcc = (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value) <= 0 ? 0 : Math.Round((s.Sum(ts => ts.vlExec).Value / (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value)) * 100, 1) }).SingleOrDefault();
                    var specialityAcc = locationsData.Where(w => idlocationsPlants.Contains(w.cdLocationPlant) && w.cdbudget != idBudget && w.cdSpeciality == speciality.idSpeciality).OrderBy(O => O.Budget.dateStart.Year).ThenBy(t => t.Budget.dateEnd.Year).GroupBy(g => new { yearStart = g.Budget.dateStart.Year, yearEnd = g.Budget.dateEnd.Year, g.vlGoal }).Select(s => new { yearToYear = s.Key.yearStart.ToString().Substring(2) + "/" + s.Key.yearEnd.ToString().Substring(2), vlGoal = s.Key.vlGoal, vlRealAcc = (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value) <= 0 ? 0 : Math.Round((s.Sum(ts => ts.vlExec).Value / (s.Sum(ts => ts.vlCount).Value - s.Sum(ts => ts.vlNExec).Value)) * 100, 1) }).ToList();

                    if (specialityData.Count > 0)
                        dataSpecialitys.Add(new { id = speciality.idSpeciality, name = speciality.Name, data = specialityData, dataAccActual = specialityDataAccActual, dataAcc = specialityAcc });
                }


               

                var dataLocals = new List<object>();
                

                if (locationData.Count > 0)
                    list.Add(new { idLocationPlant = plant.idPlant * -1, idLocation = plant.idPlant * -1, location = plant.Name, plant = plant.Sheet , data = locationData, dataAccActual = locationDataAccActual, dataAcc = locationDataAcc, dataDetails = ordershead, specialitys = dataSpecialitys, locals = dataLocals });

            }


            return list;

        }


    }
}
