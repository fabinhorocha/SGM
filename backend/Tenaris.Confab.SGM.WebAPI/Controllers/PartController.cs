using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Services;
using Tenaris.Confab.SGM.WebAPI.Models;

namespace Tenaris.Confab.SGM.WebAPI.Controllers
{   
    [RoutePrefix("api/Part")]
    public class PartController : ApiController
    {

        IMapper _Mapper;
        IPartService _PartServ;


        public PartController(IMapper Mapper, IPartService PartServ)
        {
            _Mapper = Mapper;
            _PartServ = PartServ;
        }


        [Route("GetParts")]
        public List<PartViewModel> GetParts()
        {
            var parts = _PartServ.GetParts();

            return _Mapper.Map<List<Part>, List<PartViewModel>>(parts);
        }


    }
}
