using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class OilSupplyService : IOilSupplyService
    {
        IOilSupplyRepository _repo;
        public OilSupplyService()
        {
            _repo = new OilSupplyRepository();
        }

        public OilSupplyService(IOilSupplyRepository repo)
        {
            _repo = repo;
        }
        public OilSupply GetOilSupply(int idOilSupply)
        {
            return _repo.GetOilSupply(idOilSupply);
        }
        public List<OilSupply> GetOilSupplyHistory(int idEquipment, int idComponent)
        {
            return _repo.GetOilSupplyHistory(idEquipment, idComponent);
        }

        public List<OilSupply> GetOilSupplyHistory(int? idFactory, int? idArea, int? idEquipment, int? idComponent, DateTime startDate, DateTime endDate, bool considerInsDateTime)
        {
            return _repo.GetOilSupplyHistory(idFactory, idArea, idEquipment, idComponent, startDate, endDate, considerInsDateTime);
        }

        public object Insert(OilSupply newOilSupply)
        {
            return _repo.Insert(newOilSupply);
        }

        public object Update(OilSupply oilSupply, string cdUser)
        {
            return _repo.Update(oilSupply, cdUser);
        }
    }
}
