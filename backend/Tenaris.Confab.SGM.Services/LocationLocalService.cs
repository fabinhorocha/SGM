using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class LocationLocalService: ILocationLocalService
    {
        ILocationLocalRepositorie _LocationRepo;

        public LocationLocalService(ILocationLocalRepositorie locationRepo)
        {

            _LocationRepo = locationRepo;

        }

        public List<LocationLocal> GetLocationLocal(int idLocation)
        {
            return _LocationRepo.GetLocationLocal(idLocation);
        }

       

    }
}
