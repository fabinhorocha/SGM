using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain;
using Tenaris.Confab.SGM.Domain.Entities;
using Dapper;
using System.Data.SqlClient;
using System.IO;

namespace Tenaris.Confab.SGM.Repositories
{
    public class OilSupplyTypeRepository : IOilSupplyTypeRepository
    {
        public List<OilSupplyType> GetOilSupplyTypes()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    return sqlConn.Query<OilSupplyType>(@"
                        SELECT ost.* 
                        FROM [OilManagement].[OilSupplyType] ost WITH(NOLOCK)"
                    ).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
    }
}