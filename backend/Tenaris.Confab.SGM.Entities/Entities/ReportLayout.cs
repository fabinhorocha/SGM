using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class ReportLayout
    {
        public int idReport { get; set; }
        public string Title { get; set; }
        public int ColumnOrder { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public bool haveFilterFunctionality { get; set; }
        public LayoutContainer css { get; set; }
    }

    public class LayoutContainer
    {
        public string min_width { get; set; }
        public string font_size { get; set; }
        public string min_height { get; set; }

    }
}
