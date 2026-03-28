using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain;
using Tenaris.Confab.SGM.Domain.Entities;
using Dapper;
using System.Data.SqlClient;
using System.Dynamic;
using System.Transactions;
using Tenaris.Confab.SAP.Connector;
using System.IO;

namespace Tenaris.Confab.SGM.Repositories
{
    public class TypeNoteRepositorie : ITypeNoteRepositorie
    {

      
        public List<TypeNote> GetTypeNotes()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    return sqlConn.Query<TypeNote>(@"
                       select n.*
                        from [Common].[TypeNote] n                                                       
                        where n.Active = 1"                                            
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
