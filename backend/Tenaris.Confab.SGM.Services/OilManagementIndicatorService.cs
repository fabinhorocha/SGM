using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public class OilManagementIndicatorService : IOilManagementIndicatorService
    {
        private OilSupplyService _repo = new OilSupplyService();
        public List<object> GetOilManagementData(int idIndicator, int idGroupBy, int? idFactory, int? idArea, int? idEquipment, int? idComponent, int? idOilType, int? idOilSupplyType, int? idStoppageType, DateTime startDate, DateTime endDate)
        {
            var indicatorData = new List<object>();
            if (idIndicator == 1) // Abastecimentos
            {
                switch (idGroupBy)
                {
                    case 1://Periodo
                        indicatorData = GetOilSupplyIndicatorByPeriod(idFactory, idArea, idEquipment, idComponent, idOilType, idOilSupplyType, idStoppageType, startDate, endDate);
                        break;
                    case 2://Mensal
                        indicatorData = GetOilSupplyIndicatorByMonth(idFactory, idArea, idEquipment, idComponent, idOilType, idOilSupplyType, idStoppageType, startDate, endDate);
                        break;
                    case 3://Diario
                        indicatorData = GetOilSupplyIndicatorByDay(idFactory, idArea, idEquipment, idComponent, idOilType, idOilSupplyType, idStoppageType, startDate, endDate);
                        break;
                    default:
                        break;
                }
            }
            else if (idIndicator == 2) // Paradas NO
            {
                switch (idGroupBy)
                {
                    case 1://Periodo
                        indicatorData = GetStoppageIndicatorByPeriod(idFactory, idArea, idEquipment, idComponent, idOilType, idOilSupplyType, idStoppageType, startDate, endDate);
                        break;
                    case 2://Mensal
                        indicatorData = GetStoppageIndicatorByMonth(idFactory, idArea, idEquipment, idComponent, idOilType, idOilSupplyType, idStoppageType, startDate, endDate);
                        break;
                    case 3://Diario
                        indicatorData = GetStoppageIndicatorByDay(idFactory, idArea, idEquipment, idComponent, idOilType, idOilSupplyType, idStoppageType, startDate, endDate);
                        break;
                    default:
                        break;
                }
            }

            return indicatorData;
        }

        private List<object> GetOilSupplyIndicatorByPeriod(int? idFactory, int? idArea, int? idEquipment, int? idComponent, int? idOilType, int? idOilSupplyType, int? idStoppageType, DateTime startDate, DateTime endDate)
        {
            var oilSupplies = _repo.GetOilSupplyHistory(idFactory, idArea, idEquipment, idComponent, startDate, endDate, false);

            oilSupplies = oilSupplies
                .Where(x => 
                    (!idOilType.HasValue || (idOilType.HasValue && idOilType.Value == x.OilType.idOilType))
                    && (!idOilSupplyType.HasValue || (idOilSupplyType.HasValue && idOilSupplyType.Value == x.OilSupplyType.idOilSupplyType))
                    && (!idStoppageType.HasValue || x.StoppageType == null || (idStoppageType.HasValue && x.StoppageType != null && idStoppageType.Value == x.StoppageType.idStoppageType))
                ).ToList();

            var indicatorData = new List<object>();
            if (idFactory == null)
            {
                indicatorData = (from r in oilSupplies
                                 group r by new
                                 {
                                     r.Component.Equipment.Area.Factory.Name
                                 }
                        into groupByFactory
                                 select new
                                 {
                                     Label = groupByFactory.Key.Name,
                                     Data = groupByFactory.Sum(x => x.Quantity)
                                 }).OrderBy(x => x.Label).ToList<object>();
            }
            else if (idArea == null)
            {
                indicatorData = (from r in oilSupplies
                                 group r by new { r.Component.Equipment.Area.Name }
                        into groupByArea
                                 select new
                                 {
                                     Label = groupByArea.Key.Name,
                                     Data = groupByArea.Sum(x => x.Quantity)
                                 }).OrderBy(x => x.Label).ToList<object>();
            }
            else if (idEquipment == null)
            {
                indicatorData = (from r in oilSupplies
                                 group r by new { r.Component.Equipment.Name }
                        into groupByEquipment
                                 select new
                                 {
                                     Label = groupByEquipment.Key.Name,
                                     Data = groupByEquipment.Sum(x => x.Quantity)
                                 }).OrderBy(x => x.Label)
                                 .ToList<object>();
                //.Select(x => new { Label = x.Label != null && x.Label.Length > 20 ? x.Label.Substring(0, 20) : x.Label, Data = x.Data }).ToList<object>();
            }
            else
            {
                indicatorData = (from r in oilSupplies
                                 group r by new { r.Component.Name }
                        into groupByComponent
                                 select new
                                 {
                                     Label = groupByComponent.Key.Name,
                                     Data = groupByComponent.Sum(x => x.Quantity)
                                 }).OrderBy(x => x.Label)
                                 .ToList<object>();
                //.Select(x => new { Label = x.Label != null && x.Label.Length > 20 ? x.Label.Substring(0, 20) : x.Label, Data = x.Data }).ToList<object>();
            }

            return indicatorData;
        }

        private List<object> GetStoppageIndicatorByPeriod(int? idFactory, int? idArea, int? idEquipment, int? idComponent, int? idOilType, int? idOilSupplyType, int? idStoppageType, DateTime startDate, DateTime endDate)
        {
            var oilSupplies = _repo.GetOilSupplyHistory(idFactory, idArea, idEquipment, idComponent, startDate, endDate, false);

            oilSupplies = oilSupplies
                .Where(x =>
                    (!idOilType.HasValue || (idOilType.HasValue && idOilType.Value == x.OilType.idOilType))
                    && (!idOilSupplyType.HasValue || (idOilSupplyType.HasValue && idOilSupplyType.Value == x.OilSupplyType.idOilSupplyType))
                    && (!idStoppageType.HasValue || x.StoppageType == null || (idStoppageType.HasValue && x.StoppageType != null && idStoppageType.Value == x.StoppageType.idStoppageType))
                ).ToList();

            var indicatorData = new List<object>();
            if (idFactory == null)
            {
                indicatorData = (from r in oilSupplies
                                 where r.StoppageTime > 0
                                 group r by new { r.Component.Equipment.Area.Factory.Name }
                        into groupByFactory
                                 select new
                                 {
                                     Label = groupByFactory.Key.Name,
                                     Data = groupByFactory.Sum(x => x.StoppageTime)
                                 }).OrderBy(x => x.Label).ToList<object>();
            }
            else if (idArea == null)
            {
                indicatorData = (from r in oilSupplies
                                 where r.StoppageTime > 0
                                 group r by new { r.Component.Equipment.Area.Name }
                        into groupByArea
                                 select new
                                 {
                                     Label = groupByArea.Key.Name,
                                     Data = groupByArea.Sum(x => x.StoppageTime)
                                 }).OrderBy(x => x.Label).ToList<object>();
            }
            else if (idEquipment == null)
            {
                indicatorData = (from r in oilSupplies
                                 where r.StoppageTime > 0
                                 group r by new { r.Component.Equipment.Name }
                        into groupByEquipment
                                 select new
                                 {
                                     Label = groupByEquipment.Key.Name,
                                     Data = groupByEquipment.Sum(x => x.StoppageTime)
                                 }).OrderBy(x => x.Label)
                                 .ToList<object>();
                //.Select(x => new { Label = x.Label != null && x.Label.Length > 20 ? x.Label.Substring(0, 20) : x.Label, Data = x.Data }).ToList<object>();
            }
            else
            {
                indicatorData = (from r in oilSupplies
                                 where r.StoppageTime > 0
                                 group r by new { r.Component.Name }
                        into groupByComponent
                                 select new
                                 {
                                     Label = groupByComponent.Key.Name,
                                     Data = groupByComponent.Sum(x => x.StoppageTime)
                                 }).OrderBy(x => x.Label)
                                 .ToList<object>();
                //.Select(x => new { Label = x.Label != null && x.Label.Length > 20 ? x.Label.Substring(0, 20) : x.Label, Data = x.Data }).ToList<object>();
            }

            return indicatorData;
        }

        private List<object> GetOilSupplyIndicatorByMonth(int? idFactory, int? idArea, int? idEquipment, int? idComponent, int? idOilType, int? idOilSupplyType, int? idStoppageType, DateTime startDate, DateTime endDate)
        {
            var oilSupplies = _repo.GetOilSupplyHistory(idFactory, idArea, idEquipment, idComponent, startDate, endDate, false);

            oilSupplies = oilSupplies
                .Where(x =>
                    (!idOilType.HasValue || (idOilType.HasValue && idOilType.Value == x.OilType.idOilType))
                    && (!idOilSupplyType.HasValue || (idOilSupplyType.HasValue && idOilSupplyType.Value == x.OilSupplyType.idOilSupplyType))
                    && (!idStoppageType.HasValue || x.StoppageType == null || (idStoppageType.HasValue && x.StoppageType != null && idStoppageType.Value == x.StoppageType.idStoppageType))
                ).ToList();

            var indicatorData = new List<object>();
            var indicatorDataAux = new List<object>();
            if (idFactory == null)
            {
                indicatorDataAux = oilSupplies
                    .GroupBy(x => new
                    {
                        Period = x.SupplyDateTime.ToString("yyyy/MM"),
                        Name = x.Component.Equipment.Area.Factory.Name
                    })
                    .Select(y => new
                    {
                        Label = y.Key.Period,
                        Serie = y.Key.Name,
                        Data = y.Sum(z => z.Quantity)
                    })
                    .OrderBy(z => z.Label)
                    .ToList<object>();

            }
            else if (idArea == null)
            {
                indicatorDataAux = oilSupplies
                    .GroupBy(x => new
                    {
                        Period = x.SupplyDateTime.ToString("yyyy/MM"),
                        Name = x.Component.Equipment.Area.Name
                    })
                    .Select(y => new
                    {
                        Label = y.Key.Period,
                        Serie = y.Key.Name,
                        Data = y.Sum(z => z.Quantity)
                    })
                    .OrderBy(z => z.Label)
                    .ToList<object>();
            }
            else if (idEquipment == null)
            {
                indicatorDataAux = oilSupplies
                    .GroupBy(x => new
                    {
                        Period = x.SupplyDateTime.ToString("yyyy/MM"),
                        Name = x.Component.Equipment.Name
                    })
                    .Select(y => new
                    {
                        Label = y.Key.Period,
                        Serie = y.Key.Name,
                        Data = y.Sum(z => z.Quantity)
                    })
                    .OrderBy(z => z.Label)
                    .ToList<object>();
            }
            else
            {
                indicatorDataAux = oilSupplies
                    .GroupBy(x => new
                    {
                        Period = x.SupplyDateTime.ToString("yyyy/MM"),
                        Name = x.Component.Name
                    })
                    .Select(y => new
                    {
                        Label = y.Key.Period,
                        Serie = y.Key.Name,
                        Data = y.Sum(z => z.Quantity)
                    })
                    .OrderBy(z => z.Label)
                    .ToList<object>();
            }

            var dataList = new List<List<int>>();
            //for each factory/area/statio/equipment
            foreach (var serie in indicatorDataAux.Select(x => (string)((dynamic)x).Serie).Distinct().OrderBy(y => y).ToList())
            {
                //add all periods/months/days with default value
                var data = indicatorDataAux.Select(x => (string)((dynamic)x).Label).Distinct().Select(y => (int)0).ToList();
                int idx = 0;
                foreach(var label in indicatorDataAux.Select(x => (string)((dynamic)x).Label).Distinct().OrderBy(y => y).ToList())
                {
                    var dataLabel = indicatorDataAux.Where(x => ((string)((dynamic)x).Serie) == serie && ((string)((dynamic)x).Label) == label).FirstOrDefault();
                    if (dataLabel != null)
                    {
                        data[idx] = (int)((dynamic)dataLabel).Data;
                    }
                    idx++;
                }
                dataList.Add(data);
            }


            indicatorData = new List<object> {
                    new {
                        Labels = indicatorDataAux.Select(x => (string)((dynamic)x).Label).Distinct().ToList(),
                        Series = indicatorDataAux.Select(x => (string)((dynamic)x).Serie).Distinct().ToList(),
                        Data = dataList
                    }
                };

            return indicatorData;
        }

        private List<object> GetStoppageIndicatorByMonth(int? idFactory, int? idArea, int? idEquipment, int? idComponent, int? idOilType, int? idOilSupplyType, int? idStoppageType, DateTime startDate, DateTime endDate)
        {
            var oilSupplies = _repo.GetOilSupplyHistory(idFactory, idArea, idEquipment, idComponent, startDate, endDate, false);

            oilSupplies = oilSupplies
                .Where(x =>
                    (!idOilType.HasValue || (idOilType.HasValue && idOilType.Value == x.OilType.idOilType))
                    && (!idOilSupplyType.HasValue || (idOilSupplyType.HasValue && idOilSupplyType.Value == x.OilSupplyType.idOilSupplyType))
                    && (!idStoppageType.HasValue || x.StoppageType == null || (idStoppageType.HasValue && x.StoppageType != null && idStoppageType.Value == x.StoppageType.idStoppageType))
                ).ToList();

            var indicatorData = new List<object>();
            var indicatorDataAux = new List<object>();
            if (idFactory == null)
            {
                indicatorDataAux = oilSupplies
                    .Where(y => y.StoppageTime > 0)
                    .GroupBy(x => new
                    {
                        Period = x.SupplyDateTime.ToString("yyyy/MM"),
                        Name = x.Component.Equipment.Area.Factory.Name
                    })
                    .Select(y => new
                    {
                        Label = y.Key.Period,
                        Serie = y.Key.Name,
                        Data = y.Sum(z => z.StoppageTime)
                    })
                    .OrderBy(z => z.Label)
                    .ToList<object>();
            }
            else if (idArea == null)
            {
                indicatorDataAux = oilSupplies
                    .Where(y => y.StoppageTime > 0)
                    .GroupBy(x => new
                    {
                        Period = x.SupplyDateTime.ToString("yyyy/MM"),
                        Name = x.Component.Equipment.Area.Name
                    })
                    .Select(y => new
                    {
                        Label = y.Key.Period,
                        Serie = y.Key.Name,
                        Data = y.Sum(z => z.StoppageTime)
                    })
                    .OrderBy(z => z.Label)
                    .ToList<object>();
            }
            else if (idEquipment == null)
            {
                indicatorDataAux = oilSupplies
                    .Where(y => y.StoppageTime > 0)
                    .GroupBy(x => new
                    {
                        Period = x.SupplyDateTime.ToString("yyyy/MM"),
                        Name = x.Component.Equipment.Name
                    })
                    .Select(y => new
                    {
                        Label = y.Key.Period,
                        Serie = y.Key.Name,
                        Data = y.Sum(z => z.StoppageTime)
                    })
                    .OrderBy(z => z.Label)
                    .ToList<object>();
            }
            else
            {
                indicatorDataAux = oilSupplies
                    .Where(y => y.StoppageTime > 0)
                    .GroupBy(x => new
                    {
                        Period = x.SupplyDateTime.ToString("yyyy/MM"),
                        Name = x.Component.Name
                    })
                    .Select(y => new
                    {
                        Label = y.Key.Period,
                        Serie = y.Key.Name,
                        Data = y.Sum(z => z.StoppageTime)
                    })
                    .OrderBy(z => z.Label)
                    .ToList<object>();
            }


            var dataList = new List<List<int>>();
            //for each factory/area/statio/equipment
            foreach (var serie in indicatorDataAux.Select(x => (string)((dynamic)x).Serie).Distinct().OrderBy(y => y).ToList())
            {
                //add all periods/months/days with default value
                var data = indicatorDataAux.Select(x => (string)((dynamic)x).Label).Distinct().Select(y => (int)0).ToList();
                int idx = 0;
                foreach (var label in indicatorDataAux.Select(x => (string)((dynamic)x).Label).Distinct().OrderBy(y => y).ToList())
                {
                    var dataLabel = indicatorDataAux.Where(x => ((string)((dynamic)x).Serie) == serie && ((string)((dynamic)x).Label) == label).FirstOrDefault();
                    if (dataLabel != null)
                    {
                        data[idx] = (int)((dynamic)dataLabel).Data;
                    }
                    idx++;
                }
                dataList.Add(data);
            }


            indicatorData = new List<object> {
                    new {
                        Labels = indicatorDataAux.Select(x => (string)((dynamic)x).Label).Distinct().ToList(),
                        Series = indicatorDataAux.Select(x => (string)((dynamic)x).Serie).Distinct().ToList(),
                        Data = dataList
                    }
                };

            return indicatorData;
        }

        private List<object> GetOilSupplyIndicatorByDay(int? idFactory, int? idArea, int? idEquipment, int? idComponent, int? idOilType, int? idOilSupplyType, int? idStoppageType, DateTime startDate, DateTime endDate)
        {
            var oilSupplies = _repo.GetOilSupplyHistory(idFactory, idArea, idEquipment, idComponent, startDate, endDate, false);

            oilSupplies = oilSupplies
                .Where(x =>
                    (!idOilType.HasValue || (idOilType.HasValue && idOilType.Value == x.OilType.idOilType))
                    && (!idOilSupplyType.HasValue || (idOilSupplyType.HasValue && idOilSupplyType.Value == x.OilSupplyType.idOilSupplyType))
                    && (!idStoppageType.HasValue || x.StoppageType == null || (idStoppageType.HasValue && x.StoppageType != null && idStoppageType.Value == x.StoppageType.idStoppageType))
                ).ToList();

            var indicatorData = new List<object>();
            var indicatorDataAux = new List<object>();
            if (idFactory == null)
            {
                indicatorDataAux = oilSupplies
                    .GroupBy(x => new
                    {
                        Period = x.SupplyDateTime.ToString("yyyy/MM/dd"),
                        Name = x.Component.Equipment.Area.Factory.Name
                    })
                    .Select(y => new
                    {
                        Label = y.Key.Period,
                        Serie = y.Key.Name,
                        Data = y.Sum(z => z.Quantity)
                    })
                    .OrderBy(z => z.Label)
                    .ToList<object>();
            }
            else if (idArea == null)
            {
                indicatorDataAux = oilSupplies
                    .GroupBy(x => new
                    {
                        Period = x.SupplyDateTime.ToString("yyyy/MM/dd"),
                        Name = x.Component.Equipment.Area.Name
                    })
                    .Select(y => new
                    {
                        Label = y.Key.Period,
                        Serie = y.Key.Name,
                        Data = y.Sum(z => z.Quantity)
                    })
                    .OrderBy(z => z.Label)
                    .ToList<object>();
            }
            else if (idEquipment == null)
            {
                indicatorDataAux = oilSupplies
                    .GroupBy(x => new
                    {
                        Period = x.SupplyDateTime.ToString("yyyy/MM/dd"),
                        Name = x.Component.Equipment.Name
                    })
                    .Select(y => new
                    {
                        Label = y.Key.Period,
                        Serie = y.Key.Name,
                        Data = y.Sum(z => z.Quantity)
                    })
                    .OrderBy(z => z.Label)
                    .ToList<object>();
            }
            else
            {
                indicatorDataAux = oilSupplies
                    .GroupBy(x => new
                    {
                        Period = x.SupplyDateTime.ToString("yyyy/MM/dd"),
                        Name = x.Component.Name
                    })
                    .Select(y => new
                    {
                        Label = y.Key.Period,
                        Serie = y.Key.Name,
                        Data = y.Sum(z => z.Quantity)
                    })
                    .OrderBy(z => z.Label)
                    .ToList<object>();
            }

            var dataList = new List<List<int>>();
            //for each factory/area/statio/equipment
            foreach (var serie in indicatorDataAux.Select(x => (string)((dynamic)x).Serie).Distinct().OrderBy(y => y).ToList())
            {
                //add all periods/months/days with default value
                var data = indicatorDataAux.Select(x => (string)((dynamic)x).Label).Distinct().Select(y => (int)0).ToList();
                int idx = 0;
                foreach (var label in indicatorDataAux.Select(x => (string)((dynamic)x).Label).Distinct().OrderBy(y => y).ToList())
                {
                    var dataLabel = indicatorDataAux.Where(x => ((string)((dynamic)x).Serie) == serie && ((string)((dynamic)x).Label) == label).FirstOrDefault();
                    if (dataLabel != null)
                    {
                        data[idx] = (int)((dynamic)dataLabel).Data;
                    }
                    idx++;
                }
                dataList.Add(data);
            }


            indicatorData = new List<object> {
                    new {
                        Labels = indicatorDataAux.Select(x => (string)((dynamic)x).Label).Distinct().ToList(),
                        Series = indicatorDataAux.Select(x => (string)((dynamic)x).Serie).Distinct().ToList(),
                        Data = dataList
                    }
                };


            return indicatorData;
        }

        private List<object> GetStoppageIndicatorByDay(int? idFactory, int? idArea, int? idEquipment, int? idComponent, int? idOilType, int? idOilSupplyType, int? idStoppageType, DateTime startDate, DateTime endDate)
        {
            var oilSupplies = _repo.GetOilSupplyHistory(idFactory, idArea, idEquipment, idComponent, startDate, endDate, false);

            oilSupplies = oilSupplies
                .Where(x =>
                    (!idOilType.HasValue || (idOilType.HasValue && idOilType.Value == x.OilType.idOilType))
                    && (!idOilSupplyType.HasValue || (idOilSupplyType.HasValue && idOilSupplyType.Value == x.OilSupplyType.idOilSupplyType))
                    && (!idStoppageType.HasValue || x.StoppageType == null || (idStoppageType.HasValue && x.StoppageType != null && idStoppageType.Value == x.StoppageType.idStoppageType))
                ).ToList();

            var indicatorData = new List<object>();
            var indicatorDataAux = new List<object>();
            if (idFactory == null)
            {
                indicatorDataAux = oilSupplies
                    .Where(y => y.StoppageTime > 0)
                    .GroupBy(x => new
                    {
                        Period = x.SupplyDateTime.ToString("yyyy/MM/dd"),
                        Name = x.Component.Equipment.Area.Factory.Name
                    })
                    .Select(y => new
                    {
                        Label = y.Key.Period,
                        Serie = y.Key.Name,
                        Data = y.Sum(z => z.StoppageTime)
                    })
                    .OrderBy(z => z.Label)
                    .ToList<object>();
            }
            else if (idArea == null)
            {
                indicatorDataAux = oilSupplies
                    .Where(y => y.StoppageTime > 0)
                    .GroupBy(x => new
                    {
                        Period = x.SupplyDateTime.ToString("yyyy/MM/dd"),
                        Name = x.Component.Equipment.Area.Name
                    })
                    .Select(y => new
                    {
                        Label = y.Key.Period,
                        Serie = y.Key.Name,
                        Data = y.Sum(z => z.StoppageTime)
                    })
                    .OrderBy(z => z.Label)
                    .ToList<object>();
            }
            else if (idEquipment == null)
            {
                indicatorDataAux = oilSupplies
                    .Where(y => y.StoppageTime > 0)
                    .GroupBy(x => new
                    {
                        Period = x.SupplyDateTime.ToString("yyyy/MM/dd"),
                        Name = x.Component.Equipment.Name
                    })
                    .Select(y => new
                    {
                        Label = y.Key.Period,
                        Serie = y.Key.Name,
                        Data = y.Sum(z => z.StoppageTime)
                    })
                    .OrderBy(z => z.Label)
                    .ToList<object>();
            }
            else
            {
                indicatorDataAux = oilSupplies
                    .Where(y => y.StoppageTime > 0)
                    .GroupBy(x => new
                    {
                        Period = x.SupplyDateTime.ToString("yyyy/MM/dd"),
                        Name = x.Component.Name
                    })
                    .Select(y => new
                    {
                        Label = y.Key.Period,
                        Serie = y.Key.Name,
                        Data = y.Sum(z => z.StoppageTime)
                    })
                    .OrderBy(z => z.Label)
                    .ToList<object>();
            }

            var dataList = new List<List<int>>();
            //for each factory/area/statio/equipment
            foreach (var serie in indicatorDataAux.Select(x => (string)((dynamic)x).Serie).Distinct().OrderBy(y => y).ToList())
            {
                //add all periods/months/days with default value
                var data = indicatorDataAux.Select(x => (string)((dynamic)x).Label).Distinct().Select(y => (int)0).ToList();
                int idx = 0;
                foreach (var label in indicatorDataAux.Select(x => (string)((dynamic)x).Label).Distinct().OrderBy(y => y).ToList())
                {
                    var dataLabel = indicatorDataAux.Where(x => ((string)((dynamic)x).Serie) == serie && ((string)((dynamic)x).Label) == label).FirstOrDefault();
                    if (dataLabel != null)
                    {
                        data[idx] = (int)((dynamic)dataLabel).Data;
                    }
                    idx++;
                }
                dataList.Add(data);
            }


            indicatorData = new List<object> {
                    new {
                        Labels = indicatorDataAux.Select(x => (string)((dynamic)x).Label).Distinct().ToList(),
                        Series = indicatorDataAux.Select(x => (string)((dynamic)x).Serie).Distinct().ToList(),
                        Data = dataList
                    }
                };

            return indicatorData;
        }

    }
}
