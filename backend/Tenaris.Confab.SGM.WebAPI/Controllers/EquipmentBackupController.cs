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
    [AuthorizeRolesMaintance]
    [RoutePrefix("api/EquipmentBackup")]    
    public class EquipmentBackupController : ApiController
    {
        private IEquipmentBackupService _EquipBkpServ;
        private IMapper _Mapper;


        [Route("GetBackups")]
        public List<EquipmentBackupViewModel> GetBackups()
        {
            var backups = _EquipBkpServ.GetBackups();

            return  _Mapper.Map<List<EquipmentBackup> ,List<EquipmentBackupViewModel>>(backups);

            
        }

        public EquipmentBackupController(IMapper Mapper, IEquipmentBackupService EquipBkpServ)
        {
            _Mapper = Mapper;
            _EquipBkpServ = EquipBkpServ;

        }

        

        [HttpPost]
        [Route("Backup")]
        public object Backup()
        {
          
           return _EquipBkpServ.Backup();            
        }

        [HttpPost]
        [Route("Restore")]
        public object Restore([FromBody] int idBackup)
        {
            
            return _EquipBkpServ.Restore(idBackup);            
        }



    }
}