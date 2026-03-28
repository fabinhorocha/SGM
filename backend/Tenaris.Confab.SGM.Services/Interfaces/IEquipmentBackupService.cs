using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Services
{
    public interface IEquipmentBackupService
    {        

        List<EquipmentBackup> GetBackups();        

        object Backup();

        object Restore(int id);
   


    }
}
