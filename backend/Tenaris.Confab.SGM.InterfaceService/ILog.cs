using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.InterfaceService
{
    public interface ILog
    {

        void Info(string message);


        void Warn(string message);


        void Error(Exception exception);
        
    }
}
