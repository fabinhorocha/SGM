using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.Domain.Entities
{
    public class OilManagementUser
    {
        public int idUser { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public bool selected { get; set; }
    }
}
