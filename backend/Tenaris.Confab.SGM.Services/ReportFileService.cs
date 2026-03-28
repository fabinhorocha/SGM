using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class ReportFileService : IReportFileService
    {

        private IReportFileRepositorie _RptFileRepo;
        

        public ReportFileService(IReportFileRepositorie RptFileRepo)
        {
            _RptFileRepo = RptFileRepo;            
        }


        public object DeleteReportFile(int idFile)
        {
            return _RptFileRepo.DeleteReportFile(idFile);
        }

        public int InsertReportFile(ReportFile file)
        {
            return _RptFileRepo.InsertReportFile(file);
        }

        public int UpdateReportFile(ReportFile file)
        {
            return _RptFileRepo.UpdateReportFile(file);
        }

        public object RemoveFiles(ReportFile file, string pathUpload)
        {
            try
            {
                var pathFile = pathUpload;

                if (file.idFile == 0)
                {
                    pathFile = Path.Combine(pathFile, file.Name);
                    if (File.Exists(pathFile))
                        File.Delete(pathFile);

                }
                else
                {
                    pathFile = Path.Combine(pathFile, file.cdReport.ToString(), file.Name);
                    if (File.Exists(pathFile))
                        File.Delete(pathFile);

                    //Verifica se existem arquivos na pasta, caso não exista deleta a pasta
                    var dir = new DirectoryInfo(Path.Combine(pathUpload, file.cdReport.ToString()));
                    if (dir.GetFiles().Count() == 0)
                        Directory.Delete(Path.Combine(pathUpload, file.cdReport.ToString()));

                    _RptFileRepo.DeleteReportFile(file.idFile);

                }

                return new { status = true, message = "Arquivo removido com sucesso !" };
            }
            catch (Exception ex)
            {
                return new { status = false, message = ex.Message };
            }

        }

    }
}
