using System;
using System.Collections.Generic;
using System.Linq;
using Tenaris.Confab.SGM.Domain;
using Tenaris.Confab.SGM.Domain.Entities;
using Dapper;
using System.Data.SqlClient;

namespace Tenaris.Confab.SGM.Repositories
{
    public class OilTypeRepository : IOilTypeRepository
    {
        public List<OilType> GetOilTypes()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    return sqlConn.Query<OilType>(@"
                        SELECT ot.* 
                        FROM [OilManagement].[OilType] ot WITH(NOLOCK)"
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