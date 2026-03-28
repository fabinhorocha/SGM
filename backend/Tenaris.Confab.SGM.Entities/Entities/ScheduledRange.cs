using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class ScheduledRange
    {
        public int idScheduledRange { get; set; }

        public DateTime startdate { get; set; }

        public DateTime endDate { get; set; }

        public int cdStatus { get; set; }

        public string Message { get; set; }

        public DateTime InsDateTime { get; set; }

        public DateTime UpdDateTime { get; set; }

        public Status Status {get; set;}

        public string PlantGroups { get; set; }

        public string TypeDocs { get; set; }

        public int cdScheduledType { get; set; }
    }
}
