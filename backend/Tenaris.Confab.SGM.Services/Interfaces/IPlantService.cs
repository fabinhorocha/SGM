using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IPlantService
    {

        List<Plant> GetPlants();

        List<PlantGroup> GetPlantsGroups();


        PlantGroup GetPlantGroup(int idPlantGroup);

        PlantGroup GetPlantGroupBySheet(string plant);

    }
}
