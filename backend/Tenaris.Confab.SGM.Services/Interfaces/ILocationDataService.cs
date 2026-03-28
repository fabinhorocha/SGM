using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface ILocationDataService
    {

        List<LocationData> GetLocationData(int idIndicator, int idBudget);

        List<LocationData> GetLocationDataByDateRange(string Plant, DateTime startDate, DateTime endDate);

        object InsertLocationsData(List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate);
        


    }
}
