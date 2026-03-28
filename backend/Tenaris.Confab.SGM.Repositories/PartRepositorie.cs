using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Domain;


namespace Tenaris.Confab.SGM.Repositories
{
    public class PartRepositorie : IPartRepositorie
    {


        public List<Part> GetParts() {
            try {

                using(var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    return sqlConn.Query<Part>(@"
                            Select p.*
                            from [Equipment].[Part] p
                            where p.Active = 1 order by p.[Description]").ToList();                        
                }

            }

            catch(Exception ex){
                throw new BusinessException(ex.Message);
            }
        }

        

    }
}
