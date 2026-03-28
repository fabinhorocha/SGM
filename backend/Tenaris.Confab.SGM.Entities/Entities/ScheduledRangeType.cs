using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public enum ScheduledRangeType
    {
        Location = 0,
        OrderSAP = 1,        
        NoteSAP = 2,
        OtSAP = 3,
        PlantData = 4,
        LocationData = 5,
        FailureData = 6,
        FailureSAP = 7,

    }
}
