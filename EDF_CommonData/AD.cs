using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using Microsoft.SharePoint;

namespace EDF_CommonData
{
    public class AD
    {
        #region Private fileds
        static Dictionary<string, string> KeyGroups = new Dictionary<string, string>() 
        {
            {"rsh itbilling"    ,"IT Billing"},
            {"rsh itreporting"  ,"IT Reporting"},
            {"rsh itdbse"       ,"IT DBSE"},
            {"rsh itnetwork"    ,"TD Network"},
            {"rsh itse"         ,"IT SE"},
            {"rsh itpcsupport"  ,"IT PC Support"},
            {"rsh itavaya"      ,"IT AVAYA"},
            {"rsh itdomainadmins","IT Domain Admin"},
            {"rsh administration","Administration"},
            {"rsh admin office" ,"Admin Office"},
            {"rsh admin car"    ,"Admin Car"},
            {"rsh vas"          ,"VAS"},
            {"administration"   ,"Administration"}
        };

        public static string Domain
        {
            get
            {
                string dom = null;
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    dom = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                });
                return dom;
            }
        }

        static string DomainPath
        {
            get
            {
                if (Domain == "ad.orangearmenia.am")
                {
                    if (Domain.Split('.').ToList().Count == 2)
                        return string.Format("LDAP://OU=OR-AM,DC={0},DC={1}", Domain.Split('.')[0], Domain.Split('.')[1]);
                    return string.Format("LDAP://OU=OR-AM,DC={0},DC={1},DC={2}", Domain.Split('.')[0], Domain.Split('.')[1], Domain.Split('.')[2]);
                }
                else
                {
                    if (Domain.Split('.').ToList().Count == 2)
                        return string.Format("LDAP://DC={0},DC={1}", Domain.Split('.')[0], Domain.Split('.')[1]);

                    return string.Format("LDAP://DC={0},DC={1},DC={2}", Domain.Split('.')[0], Domain.Split('.')[1], Domain.Split('.')[2]);
                }
            }
        }

        static Dictionary<string, EDF_SPUser> SUsers = new Dictionary<string, EDF_SPUser>();

        static Dictionary<string, EDF_Group> SGroups = new Dictionary<string, EDF_Group>();

        static DateTime ResetDate = DateTime.Now;

        static List<EDF_Group> Groups
        {
            get
            {
                List<EDF_Group> groups = new List<EDF_Group>();
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

                    GroupPrincipal qbeGroup = new GroupPrincipal(ctx);

                    PrincipalSearcher srch = new PrincipalSearcher(qbeGroup);

                    foreach (var found in srch.FindAll())
                    {
                        groups.Add(new EDF_Group(found.Name));
                    }
                });
                return groups;
            }
        }

        #endregion

        #region Public properties

        public static Dictionary<string, string> TeamGroups = new Dictionary<string, string>();

        #region            StockOut
        public static EDF_SPUser StockController
        {
            get
            {
                return FindGroup("EDF_so_stock-control", true).AllUsers[0];
            }
        }
        public static EDF_SPUser SalesDirector
        {
            get
            {
                return FindGroup("EDF_Sales-director", true).AllUsers[0];
            }
        }
        public static EDF_SPUser AdministrativeSupervisor
        {
            get
            {
                return FindGroup("Edf_so_administration", true).AllUsers[0];
            }
        }
        public static EDF_Group AdminStockkeeper
        {
            get
            {
                return FindGroup("EDF_so_Admin-stock");
            }
        }
        public static EDF_Group RaomarsStockkeeper
        {
            get
            {
                return FindGroup("EDF_so_RAOMARS-stock");
            }
        }
        public static EDF_Group SoStatistics
        {
            get
            {
                return FindGroup("EDF_stats_SO");
            }
        }
        #endregion

        #region DBR
        public static EDF_SPUser DBRInformationSecurity
        {
            get
            {
                return FindGroup("EDF_DBR_InformationSecurity").AllUsers[0];
            }
        }
        public static EDF_Group Agility
        {
            get
            {
                return GetDicGroup("agility");
            }
        }
        public static EDF_Group ITSETeam
        {
            get
            {
                return GetDicGroup("itse team");
            }
        }
        public static EDF_Group ITBillingTeam
        {
            get
            {
                return GetDicGroup("itbilling team");
            }
        }
        public static EDF_Group ITReportingTeam
        {
            get
            {
                return GetDicGroup("itreporting team");
            }
        }
        public static EDF_Group ITDBSETeam
        {
            get
            {
                return GetDicGroup("itdbse team");
            }
        }
        public static EDF_Group BillingDivision
        {
            get
            {
                return GetDicGroup("billing division");
            }
        }
        public static EDF_Group Reporting
        {
            get
            {
                return GetDicGroup("reporting responsible");
            }
        }
        public static EDF_Group ITSystemsEngineering
        {
            get
            {
                return GetDicGroup("edf dbr itnse responsible");
            }
        }
        public static EDF_Group OfficeIT
        {
            get
            {
                return FindGroup("EDF_DBR_OfficeIT");
            }
        }
        public static EDF_Group OfficeITTeam
        {
            get
            {
                return FindGroup("EDF_DBR_OfficeIT_Team");
            }
        }
        
        #endregion
  
        #region            RSR
        public static EDF_Group Finance
        {
            get
            {
                return GetDicGroup("finance responsible");
            }
        }
        public static EDF_Group ITDirector
        {
            get
            {
                return GetDicGroup("it director");
            }
        }
        public static EDF_Group FinancePayroll
        {
            get
            {
                return GetDicGroup("finance payroll");
            }
        }
        public static EDF_Group Administration
        {
            get
            {
                return GetDicGroup("administration");
            }
        }
        public static List<EDF_SPUser> Step3Users
        {
            get
            {
                List<EDF_SPUser> users = new List<EDF_SPUser>();
                string error = string.Empty;
                try
                {
                    error = "ERROR in Administration group";
                    foreach (EDF_SPUser u in Administration.AllUsers)
                        users.Add(u);
                    error = "ERROR in RSRAdminOffice group";
                    foreach (EDF_SPUser u in RSRAdminOffice.AllUsers)
                        users.Add(u);
                    error = "ERROR in RSRAdminCar group";
                    foreach (EDF_SPUser u in RSRAdminCar.AllUsers)
                        users.Add(u);
                    error = "ERROR in RSRITBilling group";
                    foreach (EDF_SPUser u in RSRITBilling.AllUsers)
                        users.Add(u);
                    error = "ERROR in RSRITReporting group";
                    foreach (EDF_SPUser u in RSRITReporting.AllUsers)
                        users.Add(u);
                    error = "ERROR in RSRitdbse group";
                    foreach (EDF_SPUser u in RSRitdbse.AllUsers)
                        users.Add(u);
                    error = "ERROR in RSRITNetwork group";
                    foreach (EDF_SPUser u in RSRITNetwork.AllUsers)
                        users.Add(u);
                    error = "ERROR in RSRitse group";
                    foreach (EDF_SPUser u in RSRitse.AllUsers)
                        users.Add(u);
                    error = "ERROR in RSRitpcsupport group";
                    foreach (EDF_SPUser u in RSRitpcsupport.AllUsers)
                        users.Add(u);
                    error = "ERROR in RSRitavaya group";
                    foreach (EDF_SPUser u in RSRitavaya.AllUsers)
                        users.Add(u);
                    error = "ERROR in RSRitdomainadmins group";
                    foreach (EDF_SPUser u in RSRitdomainadmins.AllUsers)
                        users.Add(u);
                    error = "ERROR in RSRVas group";
                    foreach (EDF_SPUser u in RSRVas.AllUsers)
                        users.Add(u);
                }
                catch (Exception ex) { throw new Exception(error + " err: " + ex.Message); }

                return users;
            }
        }
        public static EDF_Group RSRCorporateSecurity
        {
            get
            {

                EDF_Group g = FindGroup("EDF_RSH_CorporateSecurity");
                if (!TeamGroups.ContainsKey("EDF_RSH_CorporateSecurity"))
                    TeamGroups.Add("EDF_RSH_CorporateSecurity", "Corporate Security");
                return g;
            }
        }
        public static EDF_Group RSRInformationSecurity
        {
            get
            {
                EDF_Group g = FindGroup("EDF_RSH_InformationSecurity");
                if (!TeamGroups.ContainsKey("EDF_RSH_InformationSecurity"))
                    TeamGroups.Add("EDF_RSH_InformationSecurity", "Information Security");
                return g;
            }
        }
        public static EDF_Group RSRITBilling
        {
            get
            {
                return GetDicGroup("rsh itbilling");
            }
        }
        public static EDF_Group RSRITReporting
        {
            get
            {
                return GetDicGroup("rsh itreporting");
            }
        }
        public static EDF_Group RSRitdbse
        {
            get
            {
                return GetDicGroup("rsh itdbse");
            }
        }
        public static EDF_Group RSRITNetwork
        {
            get
            {
                return GetDicGroup("rsh itnetwork");
            }
        }
        public static EDF_Group RSRitse
        {
            get
            {
                return GetDicGroup("rsh itse");
            }
        }
        public static EDF_Group RSRitpcsupport
        {
            get
            {
                return GetDicGroup("rsh itpcsupport");
            }
        }
        public static EDF_Group RSRitavaya
        {
            get
            {
                return GetDicGroup("rsh itavaya");
            }
        }
        public static EDF_Group RSRitdomainadmins
        {
            get
            {
                return GetDicGroup("rsh itdomainadmins");
            }
        }
        public static EDF_Group RSRAdministration
        {
            get
            {
                return GetDicGroup("rsh administration");
            }
        }
        public static EDF_Group RSRAdminOffice
        {
            get
            {
                return GetDicGroup("rsh admin office");
            }
        }
        public static EDF_Group RSRAdminCar
        {
            get
            {
                return GetDicGroup("rsh admin car");
            }
        }
        public static EDF_Group RSRVas
        {
            get
            {
                return GetDicGroup("rsh vas");
            }
        }
        #endregion

        #region other
        public static EDF_SPUser CEO
        {
            get
            {
                return FindGroup("EDF_CEO", true).AllUsers[0];
               // return GetDicUser("ceo");
            }
        }
        public static EDF_SPUser HR
        {
            get
            {
                return FindGroup("EDF_HR", true).AllUsers[0];
               // return GetDicUser("hr");
            }
        }
        public static EDF_SPUser CFO
        {
            get
            {
                return FindGroup("EDF_CFO", true).AllUsers[0];
                // return GetDicUser("cfo");
            }
        }
        public static EDF_Group CCShiftEmployees
        {
            get
            {
                return GetDicGroup("ccshift");
            }
        }
        //public static string CarPool
        //{
        //    get
        //    {
        //        try
        //        {
        //            foreach (SPListItem li in AD.GetSPListByName("emails"))
        //            {
        //                if (li["Title"].ToString().ToLower() == "carpool")
        //                {
        //                    return li["email"].ToString();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception(ex.Message + ": In Carpool");
        //        }
        //        return null;

        //    }
        //}
       
        #endregion

        #region Document PDF

        public static EDF_Group DocVR
        {
            get
            {
                return GetDicGroup("doc_vr");
            }
        }

        public static EDF_Group DocLTR
        {
            get
            {
                return GetDicGroup("doc_ltr");
            }
        }

        public static EDF_Group DBRMemberOff
        {
            get
            {
                return GetDicGroup("dbr memberof");
            }
        }

        public static EDF_Group DocITO
        {
            get
            {
                return GetDicGroup("doc_ito");
            }
        }

        public static EDF_Group DocRSR
        {
            get
            {
                return GetDicGroup("doc_rsr");
            }
        }

        public static EDF_Group DocDAR
        {
            get
            {
                return GetDicGroup("doc_dar");
            }
        }

        #endregion       
    
        #endregion

        #region Private Methods

        //static EDF_SPUser GetDicUser(string key)
        //{
        //    if (SUsers.ContainsKey(key))
        //    {
        //        return SUsers[key];
        //    }
        //    else
        //    {
        //        foreach (SPListItem li in AD.GetSPListByName("Vacation_Approve"))
        //        {
        //            if (li["Title"].ToString().ToLower() == key)
        //            {
        //                string group = li["GroupName"].ToString();
        //                SUsers.Remove(key);
        //                SUsers.Add(key, AD.GetGroupUsers(group)[0]);
        //                return SUsers[key];
        //            }
        //        }
        //        throw new Exception(string.Format("There is not {0} user", key));
        //    }
        //}

        static EDF_Group GetDicGroup(string key)
        {
            if (SGroups.ContainsKey(key))
            {
                return SGroups[key];
            }
            else
            {
                try
                {
                    string group = GetGroupNameByKey(key);
                    SGroups.Remove(key);
                    EDF_Group g = AD.FindGroup(group, true);
                    SGroups.Add(key, g);
                    if (KeyGroups.ContainsKey(key))
                    {
                        if (!TeamGroups.ContainsKey(g.Name))
                            TeamGroups.Add(g.Name, KeyGroups[key]);
                    }
                    else
                    {
                        if (!TeamGroups.ContainsKey(g.Name))
                            TeamGroups.Add(g.Name, key);
                    }
                    return SGroups[key];
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("There is not {0} group : {1}", key, ex.Message));
                }
            }
        }
    
        #endregion

        #region Public Methods

        public static bool ResetDicionary()
        {
            if (DateTime.Now > ResetDate.AddDays(1))
            {
                SUsers.Clear();
                SGroups.Clear();
                ResetDate = DateTime.Now;
                return true;
            }
            return false;
        }

        public static bool Exist(string fullName)
        {
            return !(AD.GetUserByFullName(fullName) == null);
        }

        public static string GetGroupNameByKey(string key)
        {
            string group = string.Empty;
            foreach (SPListItem li in AD.GetSPListByName("Groups"))
            {
                if (li["Title"].ToString().ToLower() == key.ToLower())
                {
                    group = li["GroupName"].ToString();
                    return group;
                }
            }
            throw new Exception(string.Format("There is not {0} group in List", key));
        }

        public static List<EDF_SPUser> GetGroupUsers(string groupName)
        {
            List<EDF_SPUser> users = new List<EDF_SPUser>();
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, Domain);
                GroupPrincipal grp = GroupPrincipal.FindByIdentity(ctx, IdentityType.Name, groupName);

                DirectoryEntry de = new DirectoryEntry();
                if (grp != null)
                {
                    foreach (Principal p in grp.GetMembers(true))
                    {
                        EDF_SPUser u = new EDF_SPUser(p);
                        users.Add(u);
                        de = p.GetUnderlyingObject() as DirectoryEntry;
                    }
                    grp.Dispose();
                    ctx.Dispose();
                }
                else
                {
                    //MessageBox.Show("\nWe did not find that group in that domain, perhaps the group resides in a different domain?");
                }
            });
            return users;
        }

        public static EDF_SPUser GetUserDirector(string login)
        {
            string group = string.Empty;
            foreach (SPListItem li in AD.GetSPListByName("Groups"))
            {
                if (li["Title"].ToString().ToLower() == "director group")
                {
                    group = li["GroupName"].ToString();
                    break;
                }
            }

            EDF_SPUser us = AD.GetUserBySPLogin(login);

            foreach (EDF_SPUser u in AD.GetGroupUsers(group))
            {
                if (us.Department == u.Department)
                    return u;
            }

            return null;
        }

        public static List<EDF_Group> GetUserGroups(string userName)
        {
            List<EDF_Group> group = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

                // find a user
                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, userName);

                if (user != null)
                {
                    group = new List<EDF_Group>();
                    var groups = user.GetGroups();
                    // or there's also:
                    foreach (Principal g in groups)
                    {
                        group.Add(new EDF_Group(g.Name));
                    }
                }
            });
            return group;
        }
        
        public static SPListItemCollection GetSPListByName(string listName)
        {
            try
            {
                return SPContext.Current.Web.Lists[listName].Items;
            }
            catch
            {
                throw new Exception(string.Format("There is not {0} list", listName));
            }
        }

        public static EDF_Group FindGroup(string groupName)
        {
            return new EDF_Group(groupName);
        }

        public static EDF_Group FindGroup(string groupName, bool update)
        {
            return new EDF_Group(groupName, update);
        }

        public static EDF_SPUser GetUserById(string id)
        {
            try
            {
                DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
                DirectorySearcher search = new DirectorySearcher(searchRoot);
                search.Filter = string.Format("(&(objectSID={0})(objectCategory=person)(objectClass=user))", id);
                SearchResult result = search.FindOne();
                DirectoryEntry de = result.GetDirectoryEntry();
                return new EDF_SPUser(de);
            }
            catch
            { return null; }
        }

        public static EDF_SPUser GetUserByLogin(string login)
        {
            try
            {
                DirectoryEntry de = null;
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
                    DirectorySearcher search = new DirectorySearcher(searchRoot);
                    if (login == "system")
                        login = "Administrator";
                    search.Filter = string.Format("(&(SAMAccountname={0})(objectCategory=person)(objectClass=user))", login);
                    SearchResult result = search.FindOne();
                    de = result.GetDirectoryEntry();
                });
                return new EDF_SPUser(de);
            }
            catch
            {
                EDF_SPUser du = EDF.getDeletedUser(login);

                return du;

                throw new Exception(string.Format("There are not {0}", login));
            }
        }

        public static EDF_SPUser GetUserByProperty(string key, string value)
        {
            try
            {
                DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
                DirectorySearcher search = new DirectorySearcher(searchRoot);
                search.Filter = string.Format("(&({0}={1})(objectCategory=person)(objectClass=user))", key, value);
                SearchResult result = search.FindOne();
                DirectoryEntry de = result.GetDirectoryEntry();
                return new EDF_SPUser(de);
            }
            catch
            { return null; }
        }

        public static EDF_SPUser GetUserByFullName(string fullName)
        {
            try
            {
                DirectoryEntry de = null;
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
                    DirectorySearcher search = new DirectorySearcher(searchRoot);
                    if (fullName.Split(' ').ToList().Count == 2)
                        search.Filter = string.Format("(&(givenName={0})(sn={1})(objectCategory=person)(objectClass=user))", fullName.Split(' ')[0], fullName.Split(' ')[1]);
                    else
                        if (fullName.Split(' ').ToList().Count == 3)
                            search.Filter = string.Format("(&(givenName={0})(initials={1})(sn={2})(objectCategory=person)(objectClass=user))", fullName.Split(' ')[0], fullName.Split(' ')[1].Split('.')[0], fullName.Split(' ')[2]);
                        else
                            search.Filter = string.Format("(&(SAMAccountname={0})(objectCategory=person)(objectClass=user))", fullName);
                    /// search.Filter = string.Format("(&(displayName={0})(objectCategory=person)(objectClass=user))", fullName);
                    SearchResult result = search.FindOne();
                    de = result.GetDirectoryEntry();
                });
                return new EDF_SPUser(de);
            }
            catch
            {
                return null;
                throw new Exception(string.Format("There are not {0}", fullName));
            }
        }

        public static EDF_SPUser GetUserBySPLogin(string spLogin)
        {
            string name = spLogin;
            if (name.Contains("\\"))
            {
                name = name.Split('\\')[1];
            }
            return GetUserByLogin(name);
        }
       
        #endregion       

    }
}
