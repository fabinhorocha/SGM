using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class ComponentService : IComponentService
    {
        IComponentRepository _repo;

        public ComponentService(IComponentRepository repo)
        {
            _repo = repo;
        }

        public List<Component> GetComponents()
        {
            return _repo.GetComponents();
        }

        public Component GetComponent(int idComponent)
        {
            return _repo.GetComponent(idComponent);
        }

        public List<Component> GetComponents(int? idFactory, int? idArea, int? idEquipment, int? idComponent)
        {
            return _repo.GetComponents(idFactory, idArea, idEquipment, idComponent);
        }
        public List<OilManagementFactory> GetAllFactories()
        {
            return _repo.GetAllFactories();
        }
        public List<OilManagementArea> GetAllAreas()
        {
            return _repo.GetAllAreas();
        }
        public List<OilManagementEquipment> GetAllEquipments()
        {
            return _repo.GetAllEquipments();
        }
        public object Insert(Component newComponent)
        {
            return _repo.Insert(newComponent);
        }

        public object Update(Component editComponet, string cdUser)
        {
            return _repo.Update(editComponet, cdUser);
        }
    }
}
