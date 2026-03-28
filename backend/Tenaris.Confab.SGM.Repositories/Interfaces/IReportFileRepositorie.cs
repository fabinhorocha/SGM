using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface IReportFileRepositorie
    {

        ReportFile GetReportFile(int id);
        void InsertReportFile(SqlConnection sqlConn, ReportFile file, string pathUpload);
        void UpdateReportFile(SqlConnection sqlConn, ReportFile file, string pathUpload);
        int InsertReportFile(ReportFile file);
        int UpdateReportFile(ReportFile file);
        object DeleteReportFile(int idFile);
    }
}
