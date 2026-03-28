using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class PartService : IPartService
    {

        IPartRepositorie _PartRepo;


        public PartService(IPartRepositorie PartRepo)
        {
            _PartRepo = PartRepo;
        }

        public List<Part> GetParts()
        {
            return _PartRepo.GetParts();
        }

    }
}
