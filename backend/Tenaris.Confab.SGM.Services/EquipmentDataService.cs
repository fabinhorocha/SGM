using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class EquipmentDataService: IEquipmentDataService
    {
        IEquipmentDataRepositorie _EquipmentDataRepo;

        public EquipmentDataService(IEquipmentDataRepositorie EquipmentDataRepo)
        {

            _EquipmentDataRepo = EquipmentDataRepo;

        }

        public EquipmentDataService()
        {

            _EquipmentDataRepo = new EquipmentDataRepositorie();

        }

        public List<EquipmentData> GetEquipmentData(int idIndicator, int idBudget)
        {
            return _EquipmentDataRepo.GetEquipmentData(idIndicator, idBudget);
        }

        public List<EquipmentData> GetEquipmentDataByDateRange(string Equipment, DateTime startDate, DateTime endDate)
        {
            return _EquipmentDataRepo.GetEquipmentDataByDateRange(Equipment, startDate, endDate);
        }
       

    }
}
