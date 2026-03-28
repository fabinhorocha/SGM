using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain;
using Tenaris.Confab.SGM.Domain.Entities;
using Dapper;
using System.Data.SqlClient;
using System.IO;

namespace Tenaris.Confab.SGM.Repositories
{
    public class ReportFileRepositorie : IReportFileRepositorie
    {

        public ReportFile GetReportFile(int id)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    return sqlConn.Query<ReportFile>(@"
                        select f.* 
                        from [Report].[File] f                                        
                        where f.idFile = @idFile", new { idFile = id }).SingleOrDefault();


                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public void InsertReportFile(SqlConnection sqlConn, ReportFile file, string pathUpload)
        {

            try
            {

                sqlConn.Execute(@"
                        Insert into [Report].[File] (cdReport, Name, Size, [Type], cdUser, InsDateTime) values (@cdReport, @Name, @Size, @Type, @cdUser, @InsDateTime) ",
                    new
                    {
                        cdReport = file.cdReport,
                        Name = file.Name,
                        Size = Math.Round(file.Size, 1),
                        Type = file.Type,
                        cdUser = file.cdUser,
                        InsDateTime = file.InsDateTime

                    });

                MoveFile(file, pathUpload);


            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }


        }

        public void UpdateReportFile(SqlConnection sqlConn, ReportFile file, string pathUpload)
        {

            try
            {


                if (file.idFile == 0)
                {
                    InsertReportFile(sqlConn, file, pathUpload);
                }
                else
                {

                    var _file = GetReportFile(file.idFile);

                    if ((_file.Name != file.Name) || (_file.Size != file.Size) || (_file.Type != file.Type))
                        MoveFile(file, pathUpload);

                    sqlConn.Execute(@"
                        update [Report].[File] set  Name = @Name, Size = @Size, [Type] = @Type, UpdDateTime = @UpdDateTime where idFile = @idFile",
                         new
                         {
                             idFile = file.idFile,
                             Name = file.Name,
                             Size = Math.Round(file.Size, 1),
                             Type = file.Type,
                             UpdDateTime = file.UpdDateTime
                         });


                }



            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }

        public int InsertReportFile(ReportFile file)
        {

            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    var id = sqlConn.Query <int>(@"
                        Insert into [Report].[File] (cdReport, Name, Size, [Type], cdUser, InsDateTime) values (@cdReport, @Name, @Size, @Type, @cdUser, @InsDateTime);
                        select cast(SCOPE_IDENTITY() as int)",
                    new
                    {
                        cdReport = file.cdReport,
                        Name = file.Name,
                        Size = Math.Round(file.Size, 1),
                        Type = file.Type,
                        cdUser = file.cdUser,
                        InsDateTime = file.InsDateTime

                    }).Single();


                    return id;
                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }


        }

        public int UpdateReportFile(ReportFile file)
        {

            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    sqlConn.Execute(@"
                        update [Report].[File] set  Name = @Name, Size = @Size, [Type] = @Type, UpdDateTime = @UpdDateTime  where idFile = @idFile",
                         new
                         {
                             idFile = file.idFile,
                             Name = file.Name,
                             Size = Math.Round(file.Size, 1),
                             Type = file.Type,
                             UpdDateTime = file.UpdDateTime

                         });

                    return file.idFile;
                }


            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }

        public object DeleteReportFile(int idFile)
        {

            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Execute(@"Delete [Report].[File] where idFile = @idFile", new { idFile = idFile });
                }

                return new { id = idFile, status = true, message = "Dados excluídos com sucesso !" };
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }

        private void MoveFile(ReportFile file, string pathUpload)
        {
            var pathTemp = Path.Combine(pathUpload, file.Name);
            var pathDirectory = Path.Combine(pathUpload, file.cdReport.ToString());

            if (!Directory.Exists(pathDirectory))
            {
                Directory.CreateDirectory(pathDirectory);
            }

            if (File.Exists(pathTemp))
            {
                pathDirectory = Path.Combine(pathDirectory, file.Name);
                File.Move(pathTemp, pathDirectory);
            }
        }

    }
}
