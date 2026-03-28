using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface IPlantDataRepositorie
    {

        List<PlantData> GetPlantData(int idIndicator, int idBudget, bool cdPredictive);

        List<PlantData> GetPlantDataByPlants(int idIndicator, int idBudget, bool cdPredictive, List<string> plants);

        List<PlantData> GetPlantDataByDateRange(string Plant, DateTime startDate, DateTime endDate);
        

        object InsertPlantData(List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate);

        List<PlantData> GetPlantDataFailured(int idIndicator, int idBudget);
    }
}
