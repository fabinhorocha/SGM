using System.Collections.Generic;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IReportTypeService
    {
        ReportType GetReportType(int id);
        List<ReportType> GetReportsTypes();

    }
}