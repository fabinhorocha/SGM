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
    public class LocationLocalRepositorie : ILocationLocalRepositorie
    {

        public List<LocationLocal> GetLocationLocal(int idLocation)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var lookup = new Dictionary<int, LocationLocal>();

                    sqlConn.Query<LocationLocal, Location, Local, LocationLocal>(@"
                         select ll.*, lt.*,l.*
                            from [Plant].[LocationLocal] ll
                            inner join [Plant].[Location] lt on lt.idLocation = ll.cdLocation
                            inner join [Common].[Local] l on l.idlocal = ll.cdLocal                                                
                            where lt.idLocation = @idlocation and lt.Active = 1 and ll.Active = 1 and l.Active = 1",
                        (ll, lt, l) =>
                        {
                            LocationLocal local;

                            if (!lookup.TryGetValue(ll.idLocationLocal, out local))
                            {
                                lookup.Add(ll.idLocationLocal, local = ll);

                                                                                              
                            }

                            local.Local = l;
                            local.Location = lt;
                          

                            return local;

                        },
                        new { idLocation = idLocation },
                        splitOn: "idLocationLocal,idLocation,idLocal"
                        ).AsQueryable();


                    return lookup.Values.Cast<LocationLocal>().ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

    
        

    }
}
