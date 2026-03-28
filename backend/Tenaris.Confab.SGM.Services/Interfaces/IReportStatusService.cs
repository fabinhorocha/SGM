using System.Collections.Generic;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IReportStatusService
    {
        ReportStatus GetReportStatus(int id);
        List<ReportStatus> GetReportsStatus();

    }
}