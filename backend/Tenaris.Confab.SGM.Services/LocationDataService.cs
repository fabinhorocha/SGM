using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class LocationDataService: ILocationDataService
    {
        ILocationDataRepositorie _LocationDataRepo;

        public LocationDataService(ILocationDataRepositorie LocationDataRepo)
        {

            _LocationDataRepo = LocationDataRepo;

        }

        public LocationDataService()
        {

            _LocationDataRepo = new LocationDataRepositorie();

        }

        public List<LocationData> GetLocationData(int idIndicator, int idBudget)
        {
            return _LocationDataRepo.GetLocationData(idIndicator, idBudget);
        }

        public List<LocationData> GetLocationDataByDateRange(string Plant, DateTime startDate, DateTime endDate)
        {
            return _LocationDataRepo.GetLocationDataByDateRange(Plant, startDate, endDate);
        }


        public object InsertLocationsData(List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate)
        {
            return _LocationDataRepo.InsertLocationData(plants, typeDocs, plantsGroups, startdate, endDate);
        }

       

    }
}
