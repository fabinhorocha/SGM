using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain;
using Tenaris.Confab.SGM.Domain.Entities;
using Dapper;
using System.Data.SqlClient;

namespace Tenaris.Confab.SGM.Repositories
{
    public class StoppageTypeRepository : IStoppageTypeRepository
    {
        public List<StoppageType> GetStoppagesTypes()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    return sqlConn.Query<StoppageType>(@"
                        SELECT st.* 
                        FROM [Common].[StoppageType] st WITH(NOLOCK)"
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
