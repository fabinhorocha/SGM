using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class PriorityService: IPriorityService
    {
        IPriorityRepositorie _PriorityRepo;

        public PriorityService(IPriorityRepositorie PriorityRepo)
        {

            _PriorityRepo = PriorityRepo;

        }

        public List<Priority> GetPriorities()
        {
            return _PriorityRepo.GetPriorities();
        } 


    }
}
