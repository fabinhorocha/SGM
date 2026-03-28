using System;
using System.Collections.Generic;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface IComponentRepository
    {
        List<Component> GetComponents();
        Component GetComponent(int idComponent);
        List<Component> GetComponents(int? idFactory, int? idArea, int? idEquipment, int? idComponent);
        List<OilManagementFactory> GetAllFactories();
        List<OilManagementArea> GetAllAreas();
        List<OilManagementEquipment> GetAllEquipments();
        object Insert(Component newComponent);
        object Update(Component currentComponent, string cdUser);
    }
}