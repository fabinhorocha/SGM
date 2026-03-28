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
    public class LocationRepositorie : ILocationRepositorie
    {

        public Location GetLocation(int id)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, Location>();

                    sqlConn.Query<Location, LocationPlant, Plant, Location>(@"
                        select l.*, lp.*, p.*
                        from [Plant].[Location] l
                        inner join [Plant].[LocationPlant] lp on lp.cdLocation = l.idLocation
                        inner join [Plant].[Plant] p on p.idPlant = lp.cdPlant                                                
                        where l.idLocation = @idLocation and l.Active = 1 and lp.Active = 1 and p.Active = 1",
                        (l, lp, p) =>
                        {
                            Location local;

                            if (!lookup.TryGetValue(l.idLocation, out local))
                            {
                                lookup.Add(l.idLocation, local = l);

                                                                                              
                            }

                            if (local.LocationPlants == null)
                                local.LocationPlants = new List<LocationPlant>();

                            lp.Plant = p;

                            local.LocationPlants.Add(lp);

                            return local;

                        },
                        new { idLocation = id },
                        splitOn: "idLocationPlant,idPlant"
                        ).AsQueryable();


                    return lookup.Values.Cast<Location>().SingleOrDefault();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public List<Location> GetLocations()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, Location>();

                    sqlConn.Query<Location, LocationPlant, Plant, Location>(@"
                        select l.*, lp.*, p.*
                        from [Plant].[Location] l
                        inner join [Plant].[LocationPlant] lp on lp.cdLocation = l.idLocation
                        inner join [Plant].[Plant] p on p.idPlant = lp.cdPlant                                                
                        where l.Active = 1 and lp.Active = 1 and p.Active = 1",
                         (l, lp, p) =>
                         {
                             Location local;

                             if (!lookup.TryGetValue(l.idLocation, out local))
                             {
                                 lookup.Add(l.idLocation, local = l);


                             }

                             if (local.LocationPlants == null)
                                 local.LocationPlants = new List<LocationPlant>();

                             lp.Plant = p;

                             local.LocationPlants.Add(lp);

                             return local;

                         },                         
                         splitOn: "idLocationPlant,idPlant"
                         ).AsQueryable();


                    return lookup.Values.Cast<Location>().ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }
        

    }
}
