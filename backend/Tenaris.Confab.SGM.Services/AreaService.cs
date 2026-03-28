using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class AreaService: IAreaService
    {
        IAreaRepositorie _AreaRepo;

        public AreaService(IAreaRepositorie AreaRepo)
        {

            _AreaRepo = AreaRepo;

        }

        public List<Area> GetAreas()
        {
            return _AreaRepo.GetAreas();
        } 


    }
}
