using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IScheduledRangeService
    {

        List<ScheduledRange> GetScheduledRangeByStatus(ScheduledRangeStatus status, ScheduledRangeType type);


        List<ScheduledRange> GetScheduledRangeByDate(DateTime startDate, DateTime endDate, ScheduledRangeType type);


        object InsertScheduledRange(ScheduledRange scheduledRange);


        object UpdateScheduledRange(ScheduledRange scheduledRange);

    }
}
