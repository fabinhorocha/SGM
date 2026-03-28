using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IOilManagementIndicatorService
    {
        List<object> GetOilManagementData(int idIndicator, int idGroupBy, int? idFactory, int? idArea, int? idEquipment, int? idComponent, int? idOilType, int? idOilSupplyType, int? idStoppageType, DateTime startDate, DateTime endDate);
    }
}
