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
    public class LocationPlantRepositorie : ILocationPlantRepositorie
    {

      
        public List<LocationPlant> GetLocationsPlants()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, LocationPlant>();

                    sqlConn.Query<LocationPlant, Location, Plant, LocationPlant>(@"
                        select lp.*, l.*, p.*
                        from [Plant].[LocationPlant] lp
                        inner join [Plant].[Location] l on l.idLocation = lp.cdLocation
                        inner join [Plant].[Plant] p on p.idPlant = lp.cdPlant                                                
                        where l.Active = 1 and lp.Active = 1 and p.Active = 1",
                         (lp, l, p) =>
                         {
                             LocationPlant local;

                             if (!lookup.TryGetValue(lp.idLocationPlant, out local))
                             {
                                 lookup.Add(lp.idLocationPlant, local = lp);


                             }

                             local.Plant = p;
                             local.Location = l;

                           

                             return local;

                         },                         
                         splitOn: "idLocationPlant,idLocation,idPlant"
                         ).AsQueryable();


                    return lookup.Values.Cast<LocationPlant>().ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }
        

    }
}
