using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class EquipmentPlantGroupService : IEquipmentPlantGroupService
    {

        private IEquipmentPlantGroupRepositorie _EquipRepo;
        
        public EquipmentPlantGroupService(IEquipmentPlantGroupRepositorie EquipRepo)
        {
            _EquipRepo = EquipRepo;
   
        }

        public EquipmentPlantGroupService()
        {

            _EquipRepo = new EquipmentPlantGroupRepositorie();
        }


        public EquipmentPlantGroup GetEquipmentPlantGroup(string sheet)
        {            
            return _EquipRepo.GetEquipmentPlantGroup(sheet);
        }

        public object InsertEquipmentsPlantGroup(List<string> planPlants, List<string> plantGroups)
        {

          
            return _EquipRepo.InsertEquipmentPlantGroup(planPlants, plantGroups);
        }


        public List<EquipmentPlantGroup> GetEquipmentsPlantGroup(int index, string sheedMan)
        {
            return _EquipRepo.GetEquipmentsPlantGroup(index, sheedMan);
        }



    }
}
