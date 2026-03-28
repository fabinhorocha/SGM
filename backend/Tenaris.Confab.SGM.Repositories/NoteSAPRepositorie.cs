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
    public class NoteSAPRepositorie : INoteSAPRepositorie
    {
        public NoteSAP GetNoteSAPByReport(int cdReport)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var result = sqlConn.Query<NoteSAP>(@"
                        select n.* 
                        from [Report].[NoteSAP] n where n.cdReport = @cdReport",
                    new
                    {
                        cdReport = cdReport                       
                    }).SingleOrDefault();


                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }
        public object InsertNoteSAP(int ? cdSAP, int cdReport, bool cdStatus, string statusMessage, string cdUser, bool ? cdAttachDoc)
        {

            try
            {
                
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    
                    var id = sqlConn.Query<int>(@"
                        Insert into [Report].[NoteSAP] (cdSAP, cdReport, cdStatus, statusMessage, InsDateTime, UpdDateTime, cdUser, cdAttachDoc) 
                            values (@cdSAP, @cdReport, @cdStatus, @statusMessage, @InsDateTime, @UpdDateTime, @cdUser, @cdAttachDoc);                        
                        select cast(SCOPE_IDENTITY() as int)",
                    new
                    {
                        cdSAP = cdSAP,
                        cdReport = cdReport,
                        cdStatus = cdStatus,
                        statusMessage = statusMessage,
                        InsDateTime = DateTime.Now,
                        UpdDateTime = DateTime.Now,
                        cdUser = cdUser,
                        cdAttachDoc = cdAttachDoc

                    }).Single();

                   

                    return new { id = id, status = true, message = "Dados inseridos com sucesso !" };

                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }


        }

        public object UpdateNoteSAP(int idNoteSAP, int? cdSAP, int cdReport, bool cdStatus, string statusMessage, string cdUser, bool ? cdAttachDoc)
        {

            try
            {                
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Execute(@"
                        update [Report].[NoteSAP] set  
                            cdSAP = @cdSAP,                             
                            cdStatus = @cdStatus, 
                            cdReport = @cdReport, 
                            statusMessage = @statusMessage, 
                            UpdDateTime = @UpdDateTime,                             
                            cdUser = @cdUser,
                            cdAttachDoc = @cdAttachDoc
                        where idNoteSAP = @idNoteSAP",
                    new
                    {
                        idNoteSAP = idNoteSAP,
                        cdReport = cdReport,
                        cdSAP = cdSAP,                        
                        cdStatus = cdStatus,
                        statusMessage = statusMessage,
                        cdUser = cdUser,
                        UpdDateTime = DateTime.Now,
                        cdAttachDoc = cdAttachDoc
                    });
                    


                    return new { id = idNoteSAP, status = true, message = "Dados atualizados com sucesso !" };

                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }
    }
}
