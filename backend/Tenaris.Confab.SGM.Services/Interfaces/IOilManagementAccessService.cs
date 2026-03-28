using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;


namespace Tenaris.Confab.SGM.Services
{
    public interface IOilManagementAccessService
    {
        List<OilManagementAccess> GetAllOilManagementAccess();
    }
}
