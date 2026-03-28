using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class NoteFailure
    {
        public int idNoteM2{ get; set; }

        public int ? idNoteFa { get; set; }

        public string PlantGroup { get; set; }

        public string NamePlantGroup { get; set; }

        public string SheetFactory { get; set; }

        public string SheetArea { get; set; }

        public string SheetEquip { get; set; }

        public string NameEquip { get; set; }

        public string NameFactory { get; set; }

        public string NameArea { get; set; }

        public string NameEquipGroup { get; set; }

        public DateTime ? dateStart { get; set; }

        public DateTime ? dateEnd { get; set; }

        public DateTime ? dateRef  { get; set; }

        public DateTime ? InsDateTime { get; set; }

        public string CenterCost { get; set; }

        public int cdIndicator { get; set; }

        public string Type { get; set; }

        public int ? cdTypeOperation { get; set; }

        public int cdPendingFA { get; set; }

        public int cdOpenFA { get; set; }

        public int cdAnalyzedFA { get; set; }

        public int cdClosedNotEndFA { get; set; }

        public int cdClosedOnTimeFA { get; set; }

        public int cdClosedDelayFA { get; set; }

        public int cdClosedNotAnalyzedFA { get; set; }

        public string statusM2 { get; set; }

        public string statusFA { get; set; }
    }
}
