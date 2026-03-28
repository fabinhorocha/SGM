using System.Collections.Generic;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface ITypeService
    {
        List<TypeEquipment> GetTypes();

    }
}