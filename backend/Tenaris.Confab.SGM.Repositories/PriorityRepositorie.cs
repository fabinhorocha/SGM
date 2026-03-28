using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Tenaris.Confab.SGM.Domain.Entities;
using System.Data.SqlClient;
using Tenaris.Confab.SGM.Domain;


namespace Tenaris.Confab.SGM.Repositories
{
    public class PriorityRepositorie : IPriorityRepositorie
    {

        public List<Priority> GetPriorities()
        {
            try {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    return sqlConn.Query<Priority>(@"
                        Select p.* 
                        from [Common].[Priority] p").ToList();

                }
            }
            catch(Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }

    }
}
