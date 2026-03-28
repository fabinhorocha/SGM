using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class FailureDataViewModel
    {
        public int idFailureData { get; set; }

        public int cdbudget { get; set; }

        public int cdPlant { get; set; }

        public int dtMonth { get; set; }

        public int dtYear { get; set; }

        public float? vlCountRequestedM2ByFA { get; set; }

        public float? vlCountRequestedM2ByFAOpen { get; set; }

        public float? vlCountPendingFA { get; set; }

        public float? vlCountAnalyzedFA { get; set; }

        public float? vlCountClosedMeasuresFA { get; set; }

        public int cdIndicator { get; set; }

        public DateTime InsDateTime { get; set; }

        public DateTime UpdDateTime { get; set; }

        public string cdUser { get; set; }

        public Budget Budget { get; set; }

        public Plant Plant { get; set; }

        public Indicator Indicator { get; set; }

        public string monthYear { get; set; }


    }
}