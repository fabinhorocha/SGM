using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class ReportStatusService : IReportStatusService
    {

        IReportStatusRepositorie _StatusRepo;


        public ReportStatusService(IReportStatusRepositorie StatusRepo)
        {
            _StatusRepo = StatusRepo;
        }


        public ReportStatus GetReportStatus(int id)
        {
            return _StatusRepo.GetReportStatus(id);
        }

        public List<ReportStatus> GetReportsStatus()
        {
            return _StatusRepo.GetReportsStatus();
        }

    }
}
