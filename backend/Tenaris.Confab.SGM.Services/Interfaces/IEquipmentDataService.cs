using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IEquipmentDataService
    {

        List<EquipmentData> GetEquipmentData(int idIndicator, int idBudget);

        List<EquipmentData> GetEquipmentDataByDateRange(string Equipment, DateTime startDate, DateTime endDate);       


    }
}
