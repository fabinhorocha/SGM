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
    public class IndicatorRepositorie : IIndicatorRepositorie
    {

        public List<Indicator> GetIndicators()
        {
            try {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    return sqlConn.Query<Indicator>(@"
                        Select a.* 
                        from Common.Indicator i").ToList();

                }
            }
            catch(Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }

    }
}
