using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface ILocationDataRepositorie
    {

        List<LocationData> GetLocationData(int idIndicator, int idBudget);

        List<LocationData> GetLocationDataByDateRange(string Plant, DateTime startDate, DateTime endDate);
        

        object InsertLocationData(List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate);


        
    }
}
