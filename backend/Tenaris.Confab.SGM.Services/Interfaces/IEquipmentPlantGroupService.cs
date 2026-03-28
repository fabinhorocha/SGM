using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IEquipmentPlantGroupService
    {

        EquipmentPlantGroup GetEquipmentPlantGroup(string sheet);
        object InsertEquipmentsPlantGroup(List<string> planPlants, List<string> plantGroups);

        List<EquipmentPlantGroup> GetEquipmentsPlantGroup(int index, string sheedMan);


    }
}
