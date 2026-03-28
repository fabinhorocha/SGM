using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tenaris.Confab.SGM.Domain
{
    public class BusinessException : Exception
    {

        public BusinessException(string message) : base ( message)
        {
            
        }

    }
}