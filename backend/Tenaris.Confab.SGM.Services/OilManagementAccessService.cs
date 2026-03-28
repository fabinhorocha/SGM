using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;


namespace Tenaris.Confab.SGM.Services
{
    public class OilManagementAccessService : IOilManagementAccessService
    {
        IOilManagementAccessRepository _repo;

        public OilManagementAccessService(IOilManagementAccessRepository repo)
        {
            _repo = repo;
        }

        public List<OilManagementAccess> GetAllOilManagementAccess()
        {
            return _repo.GetAllOilManagementAccess();
        }
    }
}
