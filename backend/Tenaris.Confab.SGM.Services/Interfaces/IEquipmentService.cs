using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IEquipmentService
    {
        Equipment GetEquipment(int id);

        List<Equipment> GetEquipments();

        List<Equipment> GetAllEquipments(bool? status);

        object InsertEquipment(Equipment equipment);

        object UpdateEquipment(Equipment equipment);

        void DeleteEquipment(int idEquipment);


    }
}
