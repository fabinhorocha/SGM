using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Tenaris.Confab.SGM.Domain.Entities;
using System.Data.SqlClient;
using Tenaris.Confab.SGM.Domain;
using Tenaris.Confab.SAP.Connector;
using System.Data;
using System.Transactions;

namespace Tenaris.Confab.SGM.Repositories
{
    public class NoteFailureRepositorie : INoteFailureRepositorie
    {

        public List<NoteFailure> GetNotesFailure(DateTime dateStart, DateTime dateEnd, string location)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    return sqlConn.Query<NoteFailure>(@"
                            select *
                                from [Sap].[VwNotesFailure] l                                                                
                                    where dateStart between @dateStart and @dateEnd and (@location is null or SheetEquip like @location+'%') order by dateStart",
                          new { dateStart = dateStart, dateEnd = dateEnd, location = location }, null, true, 10000, null).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }     

    }
}
