using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class LocationPlantService: ILocationPlantService
    {
        ILocationPlantRepositorie _LocationPlantRepo;

        public LocationPlantService(ILocationPlantRepositorie locationPlantRepo)
        {

            _LocationPlantRepo = locationPlantRepo;

        }

        public List<LocationPlant> GetLocationsPlants()
        {
            return _LocationPlantRepo.GetLocationsPlants();
        }

     


    }
}
