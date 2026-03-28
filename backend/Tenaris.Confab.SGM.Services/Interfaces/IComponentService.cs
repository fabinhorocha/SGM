using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IComponentService
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