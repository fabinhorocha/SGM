using System;
using System.Collections.Generic;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface IOilSupplyRepository
    {
        OilSupply GetOilSupply(int idOilSupply);
        List<OilSupply> GetOilSupplyHistory(int idEquipment, int idComponent);
        List<OilSupply> GetOilSupplyHistory(int? idFactory, int? idArea, int? idEquipment, int? idComponent, DateTime startDate, DateTime endDate, bool considerInsDateTime);
        object Insert(OilSupply newOilSupply);
        object Update(OilSupply oilSupply, string cdUser);
    }
}
