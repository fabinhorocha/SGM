using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class FailureDataService: IFailureDataService
    {
        IFailureDataRepositorie _FailureDataRepo;

        public FailureDataService(IFailureDataRepositorie FailureDataRepo)
        {

            _FailureDataRepo = FailureDataRepo;

        }

        public FailureDataService()
        {

            _FailureDataRepo = new FailureDataRepositorie();

        }

        public List<FailureData> GetFailureData(int idIndicator, int idBudget)
        {
            return _FailureDataRepo.GetFailureData(idIndicator, idBudget);
        }

        public List<FailureData> GetFailureDataByPlants(int idIndicator, int idBudget, List<string> plants)
        {
            return _FailureDataRepo.GetFailureDataByPlants(idIndicator, idBudget, plants);
        }

        public List<FailureData> GetFailureDataByDateRange(string Plant, DateTime startDate, DateTime endDate)
        {
            return _FailureDataRepo.GetFailureDataByDateRange(Plant, startDate, endDate);
        }


        public object InsertFailureData(List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate)
        {
            return _FailureDataRepo.InsertFailureData(plants, typeDocs, plantsGroups, startdate, endDate);
        }

        public List<FailureData> GetFailureDataFailured(int idIndicator, int idBudget)
        {
            return _FailureDataRepo.GetFailureDataFailured(idIndicator, idBudget);
        }

    }
}
