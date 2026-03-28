using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenaris.Confab.SGM.InterfaceService
{
    public static class HelperFunctions
    {
        

        #region OTHER PUBLIC METHODS

        public static string GetExceptionDetail(Exception exception)
        {
            string result;

            if (exception != null)
            {
                int level = 1;
                StringBuilder stringBuilder =
                    new StringBuilder(String.Format(CultureInfo.InvariantCulture, "Level: {0}\n{1}\n", level, exception));

                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    level++;
                    stringBuilder.Append(String.Format(CultureInfo.InvariantCulture, "Level: {0}\n{1}\n", level,
                        exception));
                }

                result = stringBuilder.ToString();
            }
            else
            {
                result = "NO EXCEPTION";
            }

            return result;
        }

        #endregion

        
    }
}
