using System;
using System.Collections.Generic;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface IScheduledRangeRepositorie
    {
        List<ScheduledRange> GetScheduledRangeByStatus(int idStatus, int idScheduledType);

        List<ScheduledRange> GetScheduledRangeByDate(DateTime startDate, DateTime endDate, int idScheduledType);

        object InsertScheduledRange(ScheduledRange scheduledRange);


        object UpdateScheduledRange(ScheduledRange scheduledRange);
    }
}