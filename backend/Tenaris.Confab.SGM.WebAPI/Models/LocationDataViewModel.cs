using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class LocationDataViewModel
    {
        public int idLocationData { get; set; }

        public int cdbudget { get; set; }

        public int cdLocation { get; set; }

        public int dtMonth { get; set; }

        public int dtYear { get; set; }

        public float? vlCount { get; set; }

        public float? vlExec { get; set; }

        public float? vlNExec { get; set; }

        public int cdIndicator { get; set; }

        public DateTime InsDateTime { get; set; }

        public DateTime UpdDateTime { get; set; }

        public string cdUser { get; set; }

        public Budget Budget { get; set; }

        public LocationPlant LocationPlant { get; set; }

        public Indicator Indicator { get; set; }

        public string monthYear { get; set; }

        public int ? cdSpeciality { get; set; }

        public Speciality speciality { get; set; }

        public int ? cdLocationLocal { get; set; }
    }
}