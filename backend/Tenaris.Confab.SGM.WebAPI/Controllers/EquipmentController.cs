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
using Tenaris.Confab.SGM.WebAPI.Filter;

namespace Tenaris.Confab.SGM.WebAPI.Controllers
{
   
    [RoutePrefix("api/Equipment")]    
    public class EquipmentController : ApiController
    {
        private IEquipmentService _EquipServ;
        private IMapper _Mapper;

      
        [Route("GetEquipments")]
        public List<EquipmentViewModel> GetEquipments()
        {
            var equips = _EquipServ.GetEquipments();

            return  _Mapper.Map<List<Equipment> ,List<EquipmentViewModel>>(equips);

            
        }

        public EquipmentController(IMapper Mapper, IEquipmentService EquipServ)
        {
            _Mapper = Mapper;            
            _EquipServ = EquipServ;

        }

      
        [Route("GetAllEquipments")]
        public List<EquipmentViewModel> GetAllEquipments(bool? status = null)
        {
            var equips = _EquipServ.GetAllEquipments(status);

            return _Mapper.Map<List<Equipment>, List<EquipmentViewModel>>(equips);


        }


        [AuthorizeRolesMaintance]
        [HttpPost]
        [Route("InsertEquipment")]
        public object InsertEquipment(EquipmentViewModel equipment)
        {
          
           var equip = _Mapper.Map<EquipmentViewModel, Equipment>(equipment);
           return _EquipServ.InsertEquipment(equip);            
        }

        [AuthorizeRolesMaintance]
        [HttpPost]
        [Route("UpdateEquipment")]
        public object UpdateEquipment(EquipmentViewModel equipment)
        {
            var equip = _Mapper.Map<EquipmentViewModel, Equipment>(equipment);
            return _EquipServ.UpdateEquipment(equip);            
        }



    }
}