using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class StoppageTypeService : IStoppageTypeService
    {
        IStoppageTypeRepository _repo;

        public StoppageTypeService(IStoppageTypeRepository repo)
        {
            _repo = repo;
        }

        public List<StoppageType> GetStoppageTypes()
        {
            return _repo.GetStoppagesTypes();
        }
    }
}
