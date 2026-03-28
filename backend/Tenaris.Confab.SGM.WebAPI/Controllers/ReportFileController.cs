using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Services;
using AutoMapper;
using Tenaris.Confab.SGM.WebAPI.Models;
using System.Web.Http.Cors;
using System.Dynamic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.IO;
using Tenaris.Confab.SGM.WebAPI.Filter;

namespace Tenaris.Confab.SGM.WebAPI.Controllers
{
    [AuthorizeRolesMaintance]
    [RoutePrefix("api/ReportFile")]    
    public class ReportFileController : ApiController
    {
        private IReportFileService _ReportFileServ;
        private IReportService _ReportServ;
        private IMapper _Mapper;

        public ReportFileController(IMapper Mapper, IReportFileService ReportFileServ, IReportService ReportServ)
        {
            _Mapper = Mapper;
            _ReportFileServ = ReportFileServ;
            _ReportServ = ReportServ;

        }
       

        [HttpPost]
        [Route("DeleteReportFile")]
        public object DeleteFileReport(int id)
        {
           
           return _ReportFileServ.DeleteReportFile(id);            
        }


        [HttpPost]
        [Route("UploadFiles")]
        public object UploadFiles(string cdUser, int ? idReport)
        {

            // Verifica se a requisição contem multipart/form-data.  
            if (!Request.Content.IsMimeMultipartContent())
            {
                return new { status = false, message = new HttpResponseException(HttpStatusCode.UnsupportedMediaType).Message };
            }


            HttpPostedFile file = HttpContext.Current.Request.Files[0];

            var fileName = Path.GetFileName(file.FileName);
            var path = "";

            int? id = null;

            
            if (idReport == null)
            {

                path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads"), fileName);
                file.SaveAs(path);
            }
            else
            {
                var report = _ReportServ.GetReport(Convert.ToInt32(idReport));
                var rptFiles = report.Files == null ? null : report.Files.Where(w => w.Name == fileName).ToList();

                if (report.Files != null && rptFiles.Count > 0)
                    id = _ReportFileServ.UpdateReportFile(new ReportFile { idFile = rptFiles[0].idFile, cdReport = report.idReport, Name = fileName, Size = file.ContentLength/1024, Type = file.ContentType, UpdDateTime = DateTime.Now, cdUser = cdUser});
                else
                    id = _ReportFileServ.InsertReportFile(new ReportFile { idFile = 0, cdReport = report.idReport, Name = fileName, Size = file.ContentLength / 1024, Type = file.ContentType, InsDateTime = DateTime.Now, cdUser = cdUser });


                if (!Directory.Exists(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads"), idReport.ToString())))
                {
                    Directory.CreateDirectory(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads"), idReport.ToString()));
                }

                path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads"), idReport.ToString(), fileName);
                file.SaveAs(path);
            }


            return new { id = id, status = true, message = "Upload realizado com sucesso !"};
        }

        

        [HttpPost]
        [Route("RemoveFiles")]
        public object RemoveFiles(ReportFileViewModel file)
        {
            var rptFile = _Mapper.Map<ReportFileViewModel, ReportFile>(file);
            return _ReportFileServ.RemoveFiles(rptFile, HttpContext.Current.Server.MapPath("~/Uploads"));
                       
        }

    }
}