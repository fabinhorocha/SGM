using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface IOilManagementReportRepository
    {
        List<dynamic> GetReportLayout(int idReport);
    }
}
