using System.Collections.Generic;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IOilManagementReportService
    {
        List<ReportLayout> GetReportLayout(int idReport);
    }
}
