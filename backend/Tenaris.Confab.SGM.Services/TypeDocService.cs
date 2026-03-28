using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class TypeDocService: ITypeDocService
    {
        ITypeDocRepositorie _TypeDocRepo;

        public TypeDocService(ITypeDocRepositorie TypeDocRepo)
        {

            _TypeDocRepo = TypeDocRepo;

        }
        public TypeDocService()
        {

            _TypeDocRepo = new TypeDocRepositorie();

        }


        public List<TypeDoc> GetTypeDocs()
        {
            return _TypeDocRepo.GetTypeDocs();
        }


        public List<TypeDoc> GetTypeDocs(ScheduledRangeType scheduledType)
        {
            return _TypeDocRepo.GetTypeDocs(scheduledType);
        }


    }
}
