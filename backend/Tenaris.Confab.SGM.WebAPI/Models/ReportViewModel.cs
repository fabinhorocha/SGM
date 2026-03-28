using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.WebAPI.Models
{
    public class ReportViewModel
    {
        public int idReport { get; set; }
        public DateTime dateInput { get; set; }
        public int cdEquipment { get; set; }
        public int cdStatus { get; set; }
        public int cdType { get; set; }
        public DateTime dateInfo { get; set; }
        public DateTime dateMeasure { get; set; }
        public string Diagnostic { get; set; }
        public string cdUser { get; set; }
        public List<ReportFile> Files { get; set; }
        public DateTime InsDateTime { get; set; }
        public DateTime UpdDateTime { get; set; }
        public NoteSAP noteSAP { get; set; }
        public string Title { get; set; }
        public int? cdPriority { get; set; }
        public Priority prioritySAP { get; set; }
        public float? vlVelocity { get; set; }
        public float? vlAcceleration { get; set; }
        public float? vlRotation { get; set; }
        public float? vlPressure { get; set; }
        public float? vlTempMax { get; set; }
        public float? vlTempEnvironment { get; set; }
        public string vlISOMax { get; set; }
        public string vlISOAlarm { get; set; }
        public string vlISO { get; set; }
        public float? vlMore4Micron { get; set; }
        public float? vlMore6Micron { get; set; }
        public float? vlMore14Micron { get; set; }
        public float? vlWaterKarlFischer { get; set; }
        public float? vlViscosity40 { get; set; }
        public float? vlViscosity100 { get; set; }
        public float? vlEmissivity { get; set; }
        public float? vlTAN { get; set; }
        public Equipment Equip { get; set; }
        public ReportStatus Status { get; set; }
        public ReportType Type { get; set; }
        public bool? cdRecurrent { get; set; }
        public bool? cdOTExecuted { get; set; }
        public bool Active { get; set; }

    }
}