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
    public class PlantGroupRepositorie : IPlantGroupRepositorie
    {

        public PlantGroup GetPlantGroup(string sheet)
        {
            try {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))

                    return sqlConn.Query<PlantGroup>(@"
                        Select p.* 
                        from Plant.PlantGroup p where p.Sheet = @Sheet", new { Sheet = sheet }).SingleOrDefault();

                }
            
            catch(Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }

    }
}
