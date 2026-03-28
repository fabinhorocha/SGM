using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class PlantGroupService : IPlantGroupService
    {

        private IPlantGroupRepositorie _PlantGroupRepo;
        
        public PlantGroupService(IPlantGroupRepositorie PlantGroupRepo)
        {
            _PlantGroupRepo = PlantGroupRepo;
   
        }

        public PlantGroup GetPlantGroup(string sheet)
        {            
            return _PlantGroupRepo.GetPlantGroup(sheet);
        }

      
       

    }
}
