using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;

namespace Tenaris.Confab.Common.Util
{
    public static class Account
    {
        public static List<string[]> GetAllUser()
        {
            string[] RetProps = new string[] { "SamAccountName", "DisplayName", "Mail" };
            List<string[]> users = new List<string[]>();
            foreach (SearchResult User in GetAllUsers("CONFAB", RetProps))
            {
                DirectoryEntry DE = User.GetDirectoryEntry();
                try
                {
                    users.Add(new string[] {
                        DE.Properties["SamAccountName"][0].ToString(),
                        DE.Properties["DisplayName"][0].ToString(),
                        DE.Properties["Mail"][0].ToString()
                    });
                }
                catch
                {
                }
            }

            return users;
        }

        internal static SearchResultCollection GetAllUsers(string DomainName, string[] Properties)
        {
            DirectoryEntry DE = new DirectoryEntry("LDAP://" + DomainName);
            string Filter = "(&(objectCategory=organizationalPerson)(objectClass=User))";
            DirectorySearcher DS = new DirectorySearcher(DE);
            DS.PageSize = 10000;
            DS.SizeLimit = 10000;
            DS.SearchScope = SearchScope.Subtree;
            DS.PropertiesToLoad.AddRange(Properties); DS.Filter = Filter;
            SearchResultCollection RetObjects = DS.FindAll();
            return RetObjects;
        }
    }
}
