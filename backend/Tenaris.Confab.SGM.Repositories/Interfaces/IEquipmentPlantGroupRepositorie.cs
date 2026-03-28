using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface IEquipmentPlantGroupRepositorie
    {

        EquipmentPlantGroup GetEquipmentPlantGroup(string sheet);
        object InsertEquipmentPlantGroup(List<string> planPlants, List<string> plantGroups);

        List<EquipmentPlantGroup> GetEquipmentsPlantGroupSAP(List<string> planPlants, List<string> plantGroups);

        List<EquipmentPlantGroup> GetEquipmentsPlantGroup(int index, string sheedMan);




    }
}
