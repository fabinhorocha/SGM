using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface IEquipmentDataRepositorie
    {

        List<EquipmentData> GetEquipmentData(int idIndicator, int idBudget);

        List<EquipmentData> GetEquipmentDataByDateRange(string Plant, DateTime startDate, DateTime endDate);

        bool InsertEquipmentsData(SqlConnection sqlConn, List<string> plants, List<string> typeDocs, List<string> plantsGroups, DateTime startdate, DateTime endDate, ref string message);

        void DeleteEquipmentData(SqlConnection sqlConn, List<string> plants, List<string> typeDocs, DateTime dateStart, DateTime dateEnd);
    }
}
