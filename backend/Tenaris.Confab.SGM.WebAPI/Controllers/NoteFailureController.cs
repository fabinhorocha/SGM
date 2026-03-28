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
    [RoutePrefix("api/NoteFailure")]
    public class NoteFailureController : ApiController
    {
        private INoteFailureService _NoteFailureServ;          
        private IMapper _Mapper;
        
        public NoteFailureController(IMapper Mapper, INoteFailureService NoteFailureServ)
        {
            _Mapper = Mapper;
            _NoteFailureServ = NoteFailureServ;                        
        }



        [Route("GetNotesFailure")]
        public List<NoteFailureViewModel> GetNotesFailure(DateTime dateStart, DateTime dateEnd, string location = null)
        {

            var notes = _NoteFailureServ.GetNotesFailure(dateStart, dateEnd, location);
            
            return _Mapper.Map<List<NoteFailure>, List<NoteFailureViewModel>>(notes);
            
        }

    }
}