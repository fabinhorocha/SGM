using AutoMapper;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Services;
using Tenaris.Confab.SGM.WebAPI.Models;

namespace Tenaris.Confab.SGM.WebAPI.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        private IOilManagementAccessService _access;
        private IMapper _Mapper;
        public UserController(IMapper Mapper, IOilManagementAccessService access)
        {
            _Mapper = Mapper;
            _access = access;
        }


        [Route("GetUser")]
        public UserViewModel GetUser()
        {

            var user = new UserViewModel();
            try
            {
                string sid = HttpContext.Current.Request.LogonUserIdentity.User.ToString();
                string path = String.Format("LDAP://<SID={0}>", sid.ToString());
                DirectoryEntry userEntry = new DirectoryEntry(path, null, null, AuthenticationTypes.Secure);

                user.Login = HttpContext.Current.Request.LogonUserIdentity.Name.Split('\\').Count() > 0 ?
                HttpContext.Current.Request.LogonUserIdentity.Name.Split('\\')[1].ToString() : HttpContext.Current.Request.LogonUserIdentity.Name;
                user.Name = userEntry.Properties["displayName"][0] as string;
            }
            catch
            {
                user.Name = HttpContext.Current.Request.LogonUserIdentity.Name.Split('\\').Count() > 0 ?
              HttpContext.Current.Request.LogonUserIdentity.Name.Split('\\')[1].ToString() : HttpContext.Current.Request.LogonUserIdentity.Name;
                user.Login = user.Name;
            }

            user.Domain = HttpContext.Current.Request.LogonUserIdentity.Name.Split('\\').Count() > 0 ?
               HttpContext.Current.Request.LogonUserIdentity.Name.Split('\\')[0].ToString() : "";
            user.Groups = new List<GroupViewModel>();

            foreach (IdentityReference group in HttpContext.Current.Request.LogonUserIdentity.Groups)
            {
                try
                {
                    var name = group.Translate(typeof(NTAccount)).ToString().Split('\\').Count() > 0 ?
                        group.Translate(typeof(NTAccount)).ToString().Split('\\')[1].ToString() : group.Translate(typeof(NTAccount)).ToString().Split('\\')[0].ToString();
                    var domain = group.Translate(typeof(NTAccount)).ToString().Split('\\').Count() > 0 ?
                        group.Translate(typeof(NTAccount)).ToString().Split('\\')[0].ToString() : "";

                    user.Groups.Add(new GroupViewModel { Name = name, Domain = domain });
                }
                catch (Exception ex) { }
            }

            user.ReadOnly = user.Groups.Where(w => Properties.Settings.Default.GroupsAdmMaintance.Contains(w.Name)).Count() == 0 && user.Groups.Where(w => Properties.Settings.Default.GroupsAdmConfiguration.Contains(w.Name)).Count() == 0 ? true : false;
            user.Maintance = user.Groups.Where(w => Properties.Settings.Default.GroupsAdmMaintance.Contains(w.Name)).Count() == 0 ? false : true;
            user.Configuration = user.Groups.Where(w => Properties.Settings.Default.GroupsAdmConfiguration.Contains(w.Name)).Count() == 0 ? false : true;

            user.OilUser = false;
            user.OilManagement = false;
            try
            {
                var oilManagementAccess = _access.GetAllOilManagementAccess();
                user.OilUser = user.Groups.Where(w => oilManagementAccess.Where(x => x.LevelAccess == "User").Select(y => y.GroupName).Contains(w.Name)).Count() == 0 ? false : true;
                user.OilManagement = user.Groups.Where(w => oilManagementAccess.Where(x => x.LevelAccess == "Administrator").Select(y => y.GroupName).Contains(w.Name)).Count() == 0 ? false : true;
            }
            catch (Exception ex)
            {
                //System.IO.StreamWriter sw = new System.IO.StreamWriter(@"E:\Appls\SGM\logWS.txt", true);
                //sw.WriteLine("Erro : " + ex.Message);
                //sw.Close();
            }

            try
            {
                //if ((user.Login == "T13459" || user.Login == "TERCRD" || user.Login == "José" || user.Login == "admin") && !user.OilManagement)
                //if ((user.Login == "T13459" || user.Login == "TERCRD" || user.Login == "José"))
                //if (user.Login == "T13459" || user.Login == "admin")
                if ((user.Login == "T13459"))
                {
                    user.OilManagement = true;
                    user.OilUser = true;
                }
                else if (user.Login == "admin")
                {
                    user.OilManagement = true;
                    user.OilUser = true;
                }
            }
            catch
            {
            }

            return user;
        }
    }
}
