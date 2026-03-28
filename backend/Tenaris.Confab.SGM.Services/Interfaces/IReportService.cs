using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IReportService
    {
        Report GetReport(int id);

        List<Report> GetReportsPendentsSAP();

        List<Report> GetReportsOTs(DateTime startDate, DateTime endDate);

        List<object> GetReportsByDateRange(DateTime startDate, DateTime endDate, int idEquipment, int? idType);

        List<object> GetMeasurementsByDateRange(DateTime startDate, DateTime endDate, int idEquipment, int? idType);

        List<object> GetAnalysisValues(DateTime startDate, DateTime endDate, int idEquipment, int idType);
        object InsertReport(Report report, string pathUpload);

        object UpdateReport(Report report);

        object DeleteReport(Report report);

        object CreateNoteSAP(Report report, string pathUpload);


    }
}
