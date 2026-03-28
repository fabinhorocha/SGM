using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public class ReportStatusRepositorie : IReportStatusRepositorie
    {

        public ReportStatus GetReportStatus(int id)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    return sqlConn.Query<ReportStatus>(@"
                        select s.* 
                        from [Report].[Status] s where s.idStatus = @idStatus", new { idStatus = id }).Single();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public List<ReportStatus> GetReportsStatus()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    return sqlConn.Query<ReportStatus>(@"
                        select s.* 
                        from [Report].[Status] s").ToList(); 
                                      
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

    }
}
