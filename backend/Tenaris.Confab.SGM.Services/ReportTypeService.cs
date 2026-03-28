using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class ReportTypeService : IReportTypeService
    {

        IReportTypeRepositorie _TypeRepo;


        public ReportTypeService(IReportTypeRepositorie TypeRepo)
        {
            _TypeRepo = TypeRepo;
        }


        public ReportType GetReportType(int id)
        {
            return _TypeRepo.GetReportType(id);
        }

        public List<ReportType> GetReportsTypes()
        {
            return _TypeRepo.GetReportsTypes();
        }

    }
}
