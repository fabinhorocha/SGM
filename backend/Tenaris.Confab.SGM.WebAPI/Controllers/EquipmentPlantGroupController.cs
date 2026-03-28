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
    [RoutePrefix("api/EquipmentPlantGroup")]
    public class EquipmentPlantGroupController : ApiController
    {
        private IEquipmentPlantGroupService _EquipmentPlantGroupServ;
        private IMapper _Mapper;


        public EquipmentPlantGroupController(IMapper Mapper, IEquipmentPlantGroupService EquipmentPlantGroupServ)
        {

            _Mapper = Mapper;
            _EquipmentPlantGroupServ = EquipmentPlantGroupServ;

        }

        [Route("GetEquipmentsPlantGroup")]
        public List<EquipmentPlantGroupViewModel> GetEquipmentsPlantGroup(int index, string sheedMan = null)
        {

            var equipPlantGroup = _EquipmentPlantGroupServ.GetEquipmentsPlantGroup(index, sheedMan);

            equipPlantGroup.Insert(0, new EquipmentPlantGroup { Name = "TODOS" });

            return _Mapper.Map<List<EquipmentPlantGroup>, List<EquipmentPlantGroupViewModel>>(equipPlantGroup);

        }


    }
}
