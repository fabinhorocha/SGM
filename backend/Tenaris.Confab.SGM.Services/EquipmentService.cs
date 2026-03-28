using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class EquipmentService : IEquipmentService
    {

        private IEquipmentRepositorie _EquipRepo;

        public EquipmentService(IEquipmentRepositorie EquipRepo)
        {
            _EquipRepo = EquipRepo;
        }

        public Equipment GetEquipment(int id)
        {
            return _EquipRepo.GetEquipment(id);
        }

        public List<Equipment> GetEquipments()
        {
           return _EquipRepo.GetEquipments();
        }

        public List<Equipment> GetAllEquipments(bool? status = null)
        {
            return _EquipRepo.GetAllEquipments(status);
        }

        public object InsertEquipment(Equipment equipment)
        {
            return _EquipRepo.InsertEquipment(equipment);
        }

        public object UpdateEquipment(Equipment equipment)
        {
            return _EquipRepo.UpdateEquipment(equipment);
        }

        public void DeleteEquipment(int idEquipment)
        {
            _EquipRepo.DeleteEquipment(idEquipment);
        }

    }
}
