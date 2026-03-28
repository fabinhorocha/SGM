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
    [RoutePrefix("api/Type")]
    public class TypeController : ApiController
    {
        
        IMapper _Mapper;
        ITypeService _TypeServ;

        public TypeController(IMapper Mapper, ITypeService TypeServ)
        {
            _Mapper = Mapper;
            _TypeServ = TypeServ;
        }


        [Route("GetTypes")]
        public List<TypeViewModel> GetTypes(){

            var types = _TypeServ.GetTypes();

            return _Mapper.Map<List<TypeEquipment>, List<TypeViewModel>>(types);

        }


    }
}
