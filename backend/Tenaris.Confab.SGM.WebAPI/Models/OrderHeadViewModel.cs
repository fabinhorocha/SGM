using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class OrderHeadViewModel
    {
        public int idOrderHead { get; set; }

        public string idOrder { get; set; }

        public string Type { get; set; }

        public string Plant { get; set; }

        public string PlantGroup { get; set; }

        public string CenterCost { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public string SheetLocation { get; set; }

        public string Location { get; set; }

        public string TypeActivity { get; set; }

        public DateTime dateStart { get; set; }

        public DateTime dateEnd { get; set; }

        public DateTime dateRef { get; set; }

        public DateTime InsDateTime { get; set; }

        public DateTime UpdDateTime { get; set; }

        public string cdUser { get; set; }

        public List<string> planPlants { get; set; }

        public List<string> typeDocs { get; set; }

        public int? cdPriority { get; set; }

        public float? hoursWorked { get; set; }


    }
}