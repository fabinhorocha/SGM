using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class PlantService: IPlantService
    {
        IPlantRepositorie _PlantRepo;

        public PlantService(IPlantRepositorie PlantRepo)
        {

            _PlantRepo = PlantRepo;

        }
        public PlantService()
        {

            _PlantRepo = new PlantRepositorie();

        }


        public List<Plant> GetPlants()
        {
            return _PlantRepo.GetPlants();
        } 


        public List<PlantGroup> GetPlantsGroups()
        {
            return _PlantRepo.GetPlantsGroups();
        }

        public PlantGroup GetPlantGroup(int idPlantGroup)
        {

            return _PlantRepo.GetPlantGroup(idPlantGroup);
        }

        public PlantGroup GetPlantGroupBySheet (string plant)
        {
            return _PlantRepo.GetPlantGroupBySheet(plant);
        }

    }
}
