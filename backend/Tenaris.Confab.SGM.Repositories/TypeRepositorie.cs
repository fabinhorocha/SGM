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
    public class TypeRepositorie : ITypeRepositorie
    {
        public List<TypeEquipment> GetTypes()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    return sqlConn.Query<TypeEquipment>(@"
                        select t.* 
                        from [Common].[Type] t").ToList(); 
                                      
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

    }
}
