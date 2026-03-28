using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class PlantDataService: IPlantDataService
    {
        IPlantDataRepositorie _PlantDataRepo;

        public PlantDataService(IPlantDataRepositorie PlantDataRepo)
        {

            _PlantDataRepo = PlantDataRepo;

        }

        public PlantDataService()
        {

            _PlantDataRepo = new PlantDataRepositorie();

        }

        public List<PlantData> GetPlantData(int idIndicator, int idBudget, bool cdPredictive)
        {
            return _PlantDataRepo.GetPlantData(idIndicator, idBudget, cdPredictive);
        }

        public List<PlantData> GetPlantDataByPlants(int idIndicator, int idBudget, bool cdPredictive, List<string> plants)
        {
            return _PlantDataRepo.GetPlantDataByPlants(idIndicator, idBudget, cdPredictive, plants);
        }

        public List<PlantData> GetPlantDataByDateRange(string Plant, DateTime startDate, DateTime endDate)
        {
            return _PlantDataRepo.GetPlantDataByDateRange(Plant, startDate, endDate);
        }


        public object InsertPlantData(List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate)
        {
            return _PlantDataRepo.InsertPlantData(plants, typeDocs, plantsGroups, startdate, endDate);
        }

        public List<PlantData> GetPlantDataFailured(int idIndicator, int idBudget)
        {
            return _PlantDataRepo.GetPlantDataFailured(idIndicator, idBudget);
        }

    }
}
