using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class ReportService : IReportService
    {

        private IReportRepositorie _ReportRepo;

        public ReportService(IReportRepositorie ReportRepo)
        {
            _ReportRepo = ReportRepo;
        }


        public ReportService()
        {

            _ReportRepo = new ReportRepositorie();

        }
        public Report GetReport(int id)
        {
            return _ReportRepo.GetReport(id);
        }

        public List<Report> GetReportsPendentsSAP()
        {
            return _ReportRepo.GetReportsPendentsSAP();
        }

        public List<Report> GetReportsOTs(DateTime startDate, DateTime endDate)
        {
            return _ReportRepo.GetReportsOTs(startDate, endDate);
        }
        public List<object> GetReportsByDateRange(DateTime startDate, DateTime endDate, int idEquipment, int? idType)
        {
           return _ReportRepo.GetReportsByDateRange(startDate, endDate, idEquipment, idType);
        }

        public List<object> GetMeasurementsByDateRange(DateTime startDate, DateTime endDate, int idEquipment, int ? idType)
        {
            return _ReportRepo.GetMeasurementsByDateRange(startDate, endDate, idEquipment, idType);
        }

        public List<object> GetAnalysisValues(DateTime startDate, DateTime endDate, int idEquipment, int idType)
        {
            return _ReportRepo.GetAnalysisValues(startDate, endDate, idEquipment, idType);
        }

        public object InsertReport(Report report, string pathUpload)
        {
            return _ReportRepo.InsertReport(report, pathUpload);
        }

        public object UpdateReport(Report report)
        {
            return _ReportRepo.UpdateReport(report);
        }

        public object DeleteReport(Report report)
        {
            return _ReportRepo.DeleteReport(report);
        }


        public object CreateNoteSAP(Report report, string pathUpload)
        {
            return _ReportRepo.CreateNoteSAP(report, pathUpload);
        }
    }
}
