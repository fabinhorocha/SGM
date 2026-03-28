using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class EquipmentBackupService : IEquipmentBackupService
    {

        private IEquipmentBackupRepositorie _EquipBkpRepo;
        private IEquipmentRepositorie _EquipRepo;

        public EquipmentBackupService(IEquipmentBackupRepositorie EquipBkpRepo, IEquipmentRepositorie EquipRepo)
        {
            _EquipBkpRepo = EquipBkpRepo;
            _EquipRepo = EquipRepo;
        }
        

        public List<EquipmentBackup> GetBackups()
        {
           return _EquipBkpRepo.GetBackups();
        }

       
        public object Backup()
        {
            return _EquipBkpRepo.Backup();
        }

        public object Restore(int id)
        {            

            return _EquipBkpRepo.Restore(id);
        }

       

    }
}
