using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Filters;
using Tenaris.Confab.SGM.Domain;

namespace Tenaris.Confab.SGM.WebAPI.Filter
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            
            if (actionExecutedContext.Exception is BusinessException)
            {
                var param = Newtonsoft.Json.JsonConvert.SerializeObject(new { id = -1, status = false, message = actionExecutedContext.Exception.Message });
                actionExecutedContext.Response = new System.Net.Http.HttpResponseMessage { Content = new StringContent(param, Encoding.UTF8, "application/json") };
            }            
                
            

        }

    }
}