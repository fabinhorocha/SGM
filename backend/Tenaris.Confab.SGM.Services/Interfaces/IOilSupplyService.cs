using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IOilSupplyService
    {
        OilSupply GetOilSupply(int idOilSupply);
        List<OilSupply> GetOilSupplyHistory(int idEquipment, int idComponent);
        List<OilSupply> GetOilSupplyHistory(int? idFactory, int? idArea, int? idEquipment, int? idComponent, DateTime startDate, DateTime endDate, bool considerInsDateTime);
        object Insert(OilSupply newOilSupply);
        object Update(OilSupply oilSupply, string cdUser);
    }
}
