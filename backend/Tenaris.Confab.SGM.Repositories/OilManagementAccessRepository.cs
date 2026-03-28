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
    public class OilManagementAccessRepository : IOilManagementAccessRepository
    {
        public List<OilManagementAccess> GetAllOilManagementAccess()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    return sqlConn.Query<OilManagementAccess>(@"
                        SELECT	ag.Name [GroupName],
		                        al.Name [LevelAccess],
		                        al.[Description]
                        FROM [OilManagement].[AccessGroup] ag WITH(NOLOCK)
                        JOIN [OilManagement].[AccessLevel] al WITH(NOLOCK) ON al.idAccessLevel = ag.idAccessLevel
                        WHERE ag.Active = 1
                        ").ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
    }
}
