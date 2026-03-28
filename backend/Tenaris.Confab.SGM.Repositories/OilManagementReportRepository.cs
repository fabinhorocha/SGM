using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data.SqlClient;

namespace Tenaris.Confab.SGM.Repositories
{
    public class OilManagementReportRepository : IOilManagementReportRepository
    {
        public List<dynamic> GetReportLayout(int idReport)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    return sqlConn.Query<dynamic>(@"
                            SELECT	r.idReport,
		                            r.Title,
                                    cc.ColumnOrder,
		                            cc.ColumnName id,
		                            cc.ColumnDescription name,
		                            cc.ColumnFilterEnabled haveFilterFunctionality,
		                            container.ItemLayoutName container,
		                            il.ItemLayoutName attribute,
		                            cl.Value
                            FROM [OilManagement].[Report] r WITH(NOLOCK)
                            JOIN [OilManagement].[ColumnConfig] cc WITH(NOLOCK) ON cc.idReport = r.idReport AND cc.Active = 1
                            JOIN [OilManagement].[ColumnLayoutConfig] cl WITH(NOLOCK) ON cl.idColumnConfig = cc.idColumnConfig
                            JOIN [OilManagement].[ItemLayout] il WITH(NOLOCK) ON il.idItemLayout = cl.idItemLayout
                            LEFT JOIN [OilManagement].[ItemLayout] container WITH(NOLOCK) ON il.idItemLayoutParent = container.idItemLayout
                            WHERE r.idReport = @idReport
                            ORDER BY cc.ColumnOrder, il.idItemLayoutParent",
                            new
                            {
                                idReport = idReport
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
