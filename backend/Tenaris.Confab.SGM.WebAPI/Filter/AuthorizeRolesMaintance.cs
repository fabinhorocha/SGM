using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Tenaris.Confab.SGM.WebAPI.Models;

namespace Tenaris.Confab.SGM.WebAPI.Filter
{


    [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public sealed class AuthorizeRolesMaintanceAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {


            
            var requiredRoles = Properties.Settings.Default.GroupsAdmMaintance.Cast<string>().ToList();


           
            var groups = new List<GroupViewModel>();
            foreach (IdentityReference group in HttpContext.Current.Request.LogonUserIdentity.Groups)
            {
                try
                {
                    var name = group.Translate(typeof(NTAccount)).ToString().Split('\\').Count() > 0 ?
                        group.Translate(typeof(NTAccount)).ToString().Split('\\')[1].ToString() : group.Translate(typeof(NTAccount)).ToString().Split('\\')[0].ToString();
                    var domain = group.Translate(typeof(NTAccount)).ToString().Split('\\').Count() > 0 ?
                        group.Translate(typeof(NTAccount)).ToString().Split('\\')[0].ToString() : "";

                    groups.Add(new GroupViewModel { Name = name, Domain = domain });
                }
                catch (Exception ex) { }
            }


            if (groups.Where(w => requiredRoles.Contains(w.Name)).Count() == 0)                
                filterContext.Response = 
                    filterContext.Request.CreateResponse( 
                        HttpStatusCode.Unauthorized,
                        new  { status = false, Message = "Usuário não autorizado !" } , 
                        filterContext.ControllerContext.Configuration.Formatters.JsonFormatter);
            

        }
    }
}
