using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface IEquipmentRepositorie
    {
        Equipment GetEquipment(int id);

        List<Equipment> GetEquipments();

        List<Equipment> GetEquipmentsWithReports(DateTime startDate, DateTime endDate, int id);

        List<Equipment> GetAllEquipments(bool? status);

        List<Equipment> GetNewsEquipments(SqlConnection sqlConn, int idBackup);

        object InsertEquipment(Equipment equipment);

        object UpdateEquipment(Equipment equipment);

        void UpdateEquipment(SqlConnection sqlConn, int idBackup);

        void DeleteEquipment(int idEquipment);

        void DeleteEquipment(SqlConnection sqlConn, int idEquipment);

        void DisableEquipment(SqlConnection sqlConn, int idEquipment);


    }
}
