using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class ReportFile
    {
        public int idFile { get; set; }
        public int cdReport { get; set; }
        public string Name { get; set; }
        public float Size { get; set; }
        public string Type { get; set; }
        public DateTime InsDateTime { get; set; }
        public DateTime UpdDateTime { get; set; }
        public string cdUser { get; set; }

    }
}
