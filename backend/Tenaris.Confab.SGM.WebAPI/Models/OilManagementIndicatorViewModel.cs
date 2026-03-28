using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class OilManagementIndicatorViewModel
    {
        public List<OilType> OilTypes { get; set; }
        public List<OilSupplyType> OilSupplyTypes { get; set; }
        public List<StoppageType> StoppageTypes { get; set; }
    }
}