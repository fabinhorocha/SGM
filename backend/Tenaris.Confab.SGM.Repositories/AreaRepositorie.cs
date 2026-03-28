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
    public class AreaRepositorie : IAreaRepositorie
    {

        public List<Area> GetAreas()
        {
            try {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    return sqlConn.Query<Area>(@"
                        Select a.* 
                        from Common.Area a").ToList();

                }
            }
            catch(Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }

    }
}
