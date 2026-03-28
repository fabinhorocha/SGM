using System.Collections.Generic;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface IPlantRepositorie
    {
        List<Plant> GetPlants();

        List<PlantGroup> GetPlantsGroups();

        PlantGroup GetPlantGroup(int idPlantGroup);

        PlantGroup GetPlantGroupBySheet(string plant);

    }
}