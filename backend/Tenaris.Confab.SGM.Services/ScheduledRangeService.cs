using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class ScheduledRangeService: IScheduledRangeService
    {
        IScheduledRangeRepositorie _ScheduledRangeRepo;

        public ScheduledRangeService(IScheduledRangeRepositorie ScheduledRangeRepo)
        {

            _ScheduledRangeRepo = ScheduledRangeRepo;

        }

        public ScheduledRangeService()
        {

            _ScheduledRangeRepo = new ScheduledRangeRepositorie();

        }

        public List<ScheduledRange> GetScheduledRangeByStatus(ScheduledRangeStatus status, ScheduledRangeType type)
        {
            return _ScheduledRangeRepo.GetScheduledRangeByStatus(Convert.ToInt32(status), Convert.ToInt32(type));
        }

        public List<ScheduledRange> GetScheduledRangeByDate(DateTime startDate, DateTime endDate, ScheduledRangeType type)
        {
            return _ScheduledRangeRepo.GetScheduledRangeByDate(startDate, endDate, Convert.ToInt32(type));
        }

        public object InsertScheduledRange(ScheduledRange scheduledRange)
        {
            return _ScheduledRangeRepo.InsertScheduledRange(scheduledRange);
        }


        public object UpdateScheduledRange(ScheduledRange scheduledRange)
        {
            return _ScheduledRangeRepo.UpdateScheduledRange(scheduledRange);
        }
    }
}
