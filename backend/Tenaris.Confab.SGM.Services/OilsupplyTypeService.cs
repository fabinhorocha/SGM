using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class OilSupplyTypeService : IOilSupplyTypeService
    {
        IOilSupplyTypeRepository _repo;

        public OilSupplyTypeService(IOilSupplyTypeRepository repo)
        {
            _repo = repo;
        }
        public List<OilSupplyType> GetOilSupplyTypes()
        {
            return _repo.GetOilSupplyTypes();
        }

    }
}
