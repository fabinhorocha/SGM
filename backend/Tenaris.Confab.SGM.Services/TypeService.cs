using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class TypeService : ITypeService
    {

        ITypeRepositorie _TypeRepo;


        public TypeService(ITypeRepositorie TypeRepo)
        {
            _TypeRepo = TypeRepo;
        }


        public List<TypeEquipment> GetTypes()
        {
            return _TypeRepo.GetTypes();
        }

    }
}
