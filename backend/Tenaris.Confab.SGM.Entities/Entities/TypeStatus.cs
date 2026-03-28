using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public enum TypeStatus
    {
        
        Pending = 1,        
        Open = 2,
        Closed = 3,
        PendingEnfa = 4,
        OpenEnfa = 5,
        ClosedEnfa = 6,
        PendingOpen = 7,
        PendingReleased = 8,
        ClosedOnTime = 9,
        ClosedDelay = 10,
        PendingDelay = 11,
        PendingOnTime = 12,
        PendingOpenEnfa = 13,
        PendingReleasedEnfa = 14,
        ClosedOnTimeEnfa = 15,
        ClosedDelayEnfa = 16,
        PendingDelayEnfa = 17,
        PendingOnTimeEnfa = 18,
        RequestedM2ByFA = 19,
        RequestedM2ByFAOpen = 20,
        PendingFA = 21,
        AnalyzedFA = 22,
        ClosedMeasuresFA = 23,

    }
}
