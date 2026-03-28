using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class LocationService: ILocationService
    {
        ILocationRepositorie _LocationRepo;

        public LocationService(ILocationRepositorie locationRepo)
        {

            _LocationRepo = locationRepo;

        }

        public List<Location> GetLocations()
        {
            return _LocationRepo.GetLocations();
        }

        public Location GetLocation(int id)
        {
            return _LocationRepo.GetLocation(id);
        }


    }
}
