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
    public class TypeDocRepositorie : ITypeDocRepositorie
    {

      
        public List<TypeDoc> GetTypeDocs()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    return sqlConn.Query<TypeDoc>(@"
                       select d.*
                        from [Common].[TypeDoc] d                                                       
                        where d.Active = 1"                                            
                        ).ToList();                 
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }


        public List<TypeDoc> GetTypeDocs(ScheduledRangeType scheduledType)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    switch (scheduledType)
                    {
                        case ScheduledRangeType.OrderSAP:
                            return sqlConn.Query<TypeDoc>(@"
                               select d.*
                                from [Common].[TypeDoc] d                                                       
                                where d.Active = 1 and d.cdIndicator in (1,2)"
                               ).ToList();                       
                        case ScheduledRangeType.OtSAP:
                            return sqlConn.Query<TypeDoc>(@"
                               select d.*
                                from [Common].[TypeDoc] d                                                       
                                where d.Active = 1 and d.cdIndicator in (3)"
                               ).ToList();                      
                        case ScheduledRangeType.PlantData:
                            return sqlConn.Query<TypeDoc>(@"
                               select d.*
                                from [Common].[TypeDoc] d                                                       
                                where d.Active = 1 and d.cdIndicator in (3)"
                               ).ToList();
                        case ScheduledRangeType.LocationData:
                            return sqlConn.Query<TypeDoc>(@"
                               select d.*
                                from [Common].[TypeDoc] d                                                       
                                where d.Active = 1 and d.cdIndicator in (1,2)"
                               ).ToList();
                        default:
                            return null;

                    }
                   
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }


        public List<TypeDoc> GetTypeDocs(List<string> docs, List<int> indicators)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    return sqlConn.Query<TypeDoc>(@"
                       select d.*
                        from [Common].[TypeDoc] d                                                       
                        where d.Active = 1 and d.Sheet in @docs and d.cdIndicator in @indicators",
                        new { docs = docs, indicators = indicators }
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
