using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface IEquipmentBackupRepositorie
    {
      
        List<EquipmentBackup> GetBackups();

        List<EquipmentBackupDetails> GetBackupsDetails(int idBackup);

        object Backup();

        object Restore(int idBackup);

     


    }
}
