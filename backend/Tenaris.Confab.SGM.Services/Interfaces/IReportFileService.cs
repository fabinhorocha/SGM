using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IReportFileService
    {
        object DeleteReportFile(int idFile);

        object RemoveFiles(ReportFile file, string pathUpload);

        int InsertReportFile(ReportFile file);

        int UpdateReportFile(ReportFile file);
    }
}