using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IFailureDataService
    {

        List<FailureData> GetFailureData(int idIndicator, int idBudget);

        List<FailureData> GetFailureDataByPlants(int idIndicator, int idBudget, List<string> plants);

        List<FailureData> GetFailureDataByDateRange(string Plant, DateTime startDate, DateTime endDate);

        object InsertFailureData(List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate);

        List<FailureData> GetFailureDataFailured(int idIndicator, int idBudget);


    }
}
