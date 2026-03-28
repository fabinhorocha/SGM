using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class EquipmentData
    {

        public int idEquipmentData { get; set; }

        public int cdbudget { get; set; }        

        public int dtMonth { get; set; }

        public int dtYear { get; set; }                  

        public float? vlCountNotesPending { get; set; }

        public float? vlCountOTsPending { get; set; }

        public float? vlCountNotesOpen { get; set; }

        public float? vlCountOTsOpen { get; set; }

        public float? vlCountOTsClosed { get; set; }

        public float? vlCountOTsVeryHigh { get; set; }

        public float? vlCountOTsHigh { get; set; }

        public float? vlCountNotesVeryHigh { get; set; }

        public float? vlCountNotesHigh { get; set; }

        public int? cdEquipmentPlantGroup { get; set; }

        public float? vlGoal { get; set; }

        public int cdIndicator { get; set; }

        public DateTime InsDateTime { get; set; }

        public DateTime UpdDateTime { get; set; }

        public string cdUser { get; set; }

        public Budget Budget { get; set; }        

        public Indicator Indicator { get; set; }

        public string monthYear { get; set; }
               
        public EquipmentPlantGroup EquipmentPlantGroup { get; set; }

    }
}
