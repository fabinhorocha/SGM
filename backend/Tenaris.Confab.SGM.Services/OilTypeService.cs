using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class OilTypeService : IOilTypeService
    {
        IOilTypeRepository _repo;

        public OilTypeService(IOilTypeRepository repo)
        {
            _repo = repo;
        }
        public List<OilType> GetOilTypes()
        {
            return _repo.GetOilTypes();
        }
    }
}
