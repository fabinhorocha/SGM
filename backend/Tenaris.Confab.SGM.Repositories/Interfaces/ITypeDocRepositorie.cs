using System.Collections.Generic;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface ITypeDocRepositorie
    {
        List<TypeDoc> GetTypeDocs();

        List<TypeDoc> GetTypeDocs(ScheduledRangeType scheduledType);
    }
}