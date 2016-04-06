using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;


namespace EDF_CommonData
{
    public class EDF_SPUser
    {
        DirectoryEntry de;

        public EDF_SPUser(Principal p)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                de = p.GetUnderlyingObject() as DirectoryEntry;
                Id = p.Sid.ToString();
            });
        }
        public EDF_SPUser(DirectoryEntry de)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                this.de = de;
                SecurityIdentifier sid = new SecurityIdentifier(de.Properties["objectSid"][0] as byte[], 0);
                Id = sid.Value;
            });
        }
        public EDF_SPUser() { }
        public EDF_SPUser(DataRow dr) { this.isDeleted = true; this.UserDataRow = dr; }

        public string Id { get; set; }
        public string FirtsName { get { return GetInfo("givenName"); } }
        public string LastName { get { return GetInfo("sn"); } }
        public string FullName { get { if (FirtsName + " " + LastName == " ")return Login; return FirtsName + " " + LastName; } }
        public string Login { get { return GetInfo("samAccountName"); } }
        public string LoginDomain { get { return GetInfo("userPrincipalName"); } }
        public string E_Mail { get { return GetInfo("mail"); } }
        public string Initials { get { return GetInfo("initials"); } }
        public string Descr { get { return GetInfo("description"); } }
        public string JobTitle { get { return GetInfo("title"); } }
        public string Department { get { return GetInfo("department"); } }
        public string ManagerName
        {
            get
            {
                try
                {
                    string man = "";
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        if (de.Properties["manager"].Value != null)
                        {
                            man = ((string)(de.Properties["manager"][0])).Split(',')[0].Split('=')[1];
                        }
                    });
                    return man;
                }
                catch { return null; }
            }
        }
        public string Office { get { return GetInfo("physicalDeliveryOfficeName"); } }
        public string TelephoneNumber { get { return GetInfo("telephoneNumber"); } }
        public string TelephoneOther { get { return GetInfo("otherTelephone"); } }
        public string Mobile { get { return GetInfo("mobile"); } }
        public string WebPage { get { return GetInfo("wwwHomePage"); } }
        public string WebPageOther { get { return GetInfo("url"); } }

        public bool HasManager { get { return (ManagerName == null || ManagerName == "") ? false : true; } }

        string ImagePath { get { return GetInfo("givenName"); } }

        public List<string> RequestAccess
        {
            get
            {
                List<string> RA = new List<string>();

                foreach (SPListItem li in AD.GetSPListByName("Access"))
                {
                    string gu = li["Groups Users"].ToString();
                    List<string> lgu = gu.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

                    foreach (string u in lgu)
                    {
                        if (u.ToLower() == Login.ToLower())
                        {
                            string RT = li["Request Type"].ToString();
                            RA.AddRange(RT.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList());
                        }
                        else
                        {
                            List<EDF_SPUser> gusers;
                            try
                            {
                                gusers = AD.FindGroup(u).AllUsers;
                            }
                            catch { continue; }

                            foreach (EDF_SPUser us in gusers)
                            {
                                if (us.Login == Login)
                                {
                                    string RT = li["Request Type"].ToString();
                                    RA.AddRange(RT.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList());
                                }
                            }
                        }
                    }
                }
                return RA;
            }
        }

        public bool GetDocAccess(int RequestId)
        {
            switch (RequestId)
            {
                case 1:
                    return IsMemberOf(AD.DocVR);
                case 2:
                    return IsMemberOf(AD.DocLTR);
                case 3:
                    return IsMemberOf(AD.DocITO);
                case 4:
                    return IsMemberOf(AD.DocRSR);
                case 5:
                    return IsMemberOf(AD.DocDAR);
            }
            return false;
        }

        public bool StatisticsAccess
        {
            get
            {
                List<EDF_SPUser> gusers = new List<EDF_SPUser>();
                try
                {
                    gusers.AddRange(AD.FindGroup("EDF_stats_DBR").AllUsers);
                    gusers.AddRange(AD.FindGroup("EDF_stats_ITR").AllUsers);
                    gusers.AddRange(AD.FindGroup("EDF_stats_LTR").AllUsers);
                    gusers.AddRange(AD.FindGroup("EDF_stats_RSR").AllUsers);
                    gusers.AddRange(AD.FindGroup("EDF_stats_VR").AllUsers);

                }
                catch { return false; }

                foreach (EDF_SPUser us in gusers)
                {
                    if (us.Login == Login)
                    {
                        return true;
                    }
                }
                return false;

                //foreach (SPListItem li in AD.GetSPListByName("Statistics"))
                //{
                //    string gu = li["group"].ToString();
                //    List<string> lgu = gu.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

                //    foreach (string u in lgu)
                //    {
                //        if (u.ToLower() == Login.ToLower())
                //        {
                //            return true;
                //        }
                //        else
                //        {
                //            List<EDF_SPUser> gusers;
                //            try
                //            {
                //                gusers = AD.FindGroup(u).AllUsers;
                //            }
                //            catch { continue; }

                //            foreach (EDF_SPUser us in gusers)
                //            {
                //                if (us.Login == Login)
                //                {
                //                    return true;
                //                }
                //            }
                //        }
                //    }
                //}
                //return false;
            }
        }

        public List<EDF_Group> Groups
        {
            get
            {
                return AD.GetUserGroups(Login);
            }
        }

        public EDF_Group TeamGroup
        {
            get
            {
                return EDF.GetUserTeam(this);
            }
        }

        public EDF_User GetGroupResult(string RequestId)
        {
            return EDF.GetRSRGroupResult(RequestId, this);
        }


        public bool IsMemberOf(string GroupName)
        {
            foreach (EDF_SPUser u in AD.GetGroupUsers(GroupName))
            {
                if (Login == u.Login)
                    return true;
            }
            return false;
        }

        public bool IsMemberOf(EDF_Group Group)
        {
            try
            {
                foreach (EDF_SPUser u in Group.AllUsers)
                    if (u.Login == Login)
                        return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return false;
        }


        public bool IsMemberOf(List<EDF_SPUser> Users)
        {
            foreach (EDF_SPUser u in Users)
                if (u.Login == Login)
                    return true;

            return false;
        }

        string GetInfo(string key)
        {
            if (this.isDeleted)
            {
                return this.UserDataRow[key].ToString();
            }
            else
            {
                string info = null;
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    info = (string)de.Properties[key].Value;
                });
                return info;
            }
        }

        public EDF_SPUser Manager
        {
            get
            {
                object managerDN = de.Properties["manager"][0];
                EDF_SPUser us;
                if (ManagerName == null || ManagerName == "")
                    return null;
                us = AD.GetUserByFullName(ManagerName);
                if (us == null)
                    us = AD.GetUserByLogin(ManagerName);
                // fro delete
                //else
                //    if (us == null)
                //        us = AD.GetUserBySPLogin(ManagerName);

                return us;
            }
        }

        public bool HasDirector
        {
            get
            {
                return Director != null;
            }
        }

        public EDF_SPUser Director
        {
            get
            {
                string group = "";
                foreach (SPListItem li in AD.GetSPListByName("Groups"))
                {
                    if (li["Title"].ToString().ToLower() == "director group")
                    {
                        group = li["GroupName"].ToString();
                        break;
                    }
                }

                EDF_SPUser us = AD.GetUserBySPLogin(Login);

                foreach (EDF_SPUser u in AD.GetGroupUsers(group))
                {
                    if (us.Department == u.Department)
                        return u;
                }
                return null;
                throw new Exception(string.Format("{0} has not Director", AD.GetUserBySPLogin(Login).FullName));
            }
        }

        public EDF_SPUser BigManager(string Group)
        {
            EDF_SPUser bm = this;
            try
            {
                while (bm.Manager != null && bm.Manager.IsMemberOf(Group))
                {
                    bm = bm.Manager;
                }
            }
            catch { }
            return bm;
        }

        private string RepStart;
        private string RepEnd;
        public string ReplacementStart
        {
            get
            {
                if (HasReplacement)
                    return RepStart;
                return null;
            }
        }
        public string ReplacementEnd
        {
            get
            {
                if (HasReplacement)
                    return RepEnd;
                return null;
            }
        }

        public List<EDF_SPUser> IsReplacements
        {
            get
            {
                List<EDF_SPUser> lUsers = new List<EDF_SPUser> { };
                SPList reps = SPContext.Current.Web.Lists["Replacements"];

                foreach (SPListItem li in reps.Items)
                {
                    string username = li["Replacement people"].ToString();

                    if (username == Login && DateTime.Parse(li["Start"].ToString()) < DateTime.Parse(li["End"].ToString()).AddDays(1))
                    {
                        string replacement = li["People"].ToString();
                        RepStart = li["Start"].ToString().Split(' ')[0];
                        RepEnd = li["End"].ToString().Split(' ')[0];

                        if (DateTime.Parse(RepEnd).AddDays(1) > DateTime.Now)
                            lUsers.Add(AD.GetUserBySPLogin(replacement));
                        else
                        {
                            RemoveReplacement();
                        }
                    }
                }
                return lUsers;
            }
        }

        public EDF_SPUser Replacement
        {
            get
            {
                SPList reps = SPContext.Current.Web.Lists["Replacements"];

                foreach (SPListItem li in reps.Items)
                {
                    string username = li["People"].ToString();

                    if (username == Login && DateTime.Parse(li["Start"].ToString()) < DateTime.Parse(li["End"].ToString()).AddDays(1))
                    {
                        string replacement = li["Replacement people"].ToString();
                        RepStart = li["Start"].ToString().Split(' ')[0];
                        RepEnd = li["End"].ToString().Split(' ')[0];

                        if (DateTime.Parse(RepEnd).AddDays(1) > DateTime.Now)
                            return AD.GetUserBySPLogin(replacement);
                        else
                        {
                            RemoveReplacement();
                            return this;
                        }
                    }
                }
                return this;
            }
        }

        public List<EDF_SPUser> ParentReplacement
        {
            get
            {
                SPList reps = SPContext.Current.Web.Lists["Replacements"];
                List<EDF_SPUser> Rep0 = new List<EDF_SPUser>();
                foreach (SPListItem li in reps.Items)
                {
                    string username = li["Replacement people"].ToString();

                    if (username == Login && DateTime.Now >= DateTime.Parse(li["Start"].ToString()) && DateTime.Now < DateTime.Parse(li["End"].ToString()).AddDays(1))
                    {
                        string replacement = li["People"].ToString();
                        Rep0.Add(AD.GetUserBySPLogin(replacement));
                    }
                }
                return Rep0;
            }
        }

        public bool HasReplacement
        {
            get
            {
                return this.Login != Replacement.Login;
            }
        }

        public bool IsReplacement
        {
            get
            {
                return (IsReplacements.Count > 0);
            }
        }

        public void RemoveReplacement()
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
               {
                   SPSite site = new SPSite(SPContext.Current.Web.Url);
                   SPWeb oSPWeb = site.OpenWeb();
                   oSPWeb.AllowUnsafeUpdates = true;

                   int i = 0; ;
                   foreach (SPListItem it in oSPWeb.Lists["Replacements"].Items)
                   {
                       if (it["People"].ToString() == this.Login)
                       {
                           oSPWeb.Lists["Replacements"].Items[i].Delete();
                           oSPWeb.Lists["Replacements"].Update();
                       }
                       i++;
                   }
               });
            }
            catch
            { }

        }

        public void SetReplacement(string Login, DateTime StartDate, DateTime EndDate)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {

                    SPSite site = new SPSite(SPContext.Current.Web.Url);
                    SPWeb oSPWeb = site.OpenWeb();
                    oSPWeb.AllowUnsafeUpdates = true;


                    int i = 0;
                    foreach (SPListItem it in oSPWeb.Lists["Replacements"].Items)
                    {
                        if (it["People"].ToString() == this.Login)
                        {
                            oSPWeb.Lists["Replacements"].Items[i].Delete();
                            oSPWeb.Lists["Replacements"].Update();
                        }
                        i++;
                    }

                    SPListItem item = oSPWeb.Lists["Replacements"].Items.Add();

                    item["Title"] = this.Login;
                    item["People"] = this.Login;
                    item["Replacement people"] = Login;
                    item["Start"] = StartDate;
                    item["End"] = EndDate;
                    item.Update();
                });
            }
            catch
            { }
        }

        public List<EDF_SPUser> DirectReports
        {
            get
            {
                List<EDF_SPUser> users = new List<EDF_SPUser>();

                foreach (var u in de.Properties["directReports"])
                {
                    users.Add(AD.GetUserByFullName(((string)u).Split(',')[0].Split('=')[1]));
                }
                return users;
            }
        }

        public bool HasDirectReports
        {
            get
            {
                return DirectReports.Count > 0;
            }
        }

        public bool IsManager
        {
            get
            {
                if (DirectReports.Count == 0)
                    return false;
                return true;
            }
        }

        public string Status
        {
            get
            {
                if (IsCEO)
                    return "ceo";
                if (HasManager && !HasDirectReports && Login != this.Director.Login)
                    return "user";

                if (HasDirector && Login == this.Director.Login)
                    return "director";

                if (IsManager)
                    return "manager";

                return null;
            }
        }

        public bool IsDirector
        {
            get
            {
                return (HasDirector && Login == this.Director.Login);
            }
        }

        public bool IsCEO
        {
            get
            {
                return Login == AD.CEO.Login;
            }
        }

        public bool IsStockController
        {
            get
            {
                return Login == AD.StockController.Login || (this.ParentReplacement.Count > 0 && this.ParentReplacement[0].Login == AD.StockController.Login);
            }
        }
        public bool IsSalesDirector
        {
            get
            {
                return Login == AD.SalesDirector.Login || (this.ParentReplacement.Count > 0 && this.ParentReplacement[0].Login == AD.SalesDirector.Login);
            }
        }
        public bool IsAdministrativeSupervisor
        {
            get
            {
                return Login == AD.AdministrativeSupervisor.Login || (this.ParentReplacement.Count > 0 && this.ParentReplacement[0].Login == AD.AdministrativeSupervisor.Login);
            }
        }

        public bool IsAdminStockkeeper
        {
            get
            {
                foreach (EDF_SPUser item in AD.AdminStockkeeper.AllUsers)
                {
                    if (item.Login == Login)
                        return true;
                }
                return false;
            }
        }
        public bool IsRaomarsStockkeeper
        {
            get
            {
                foreach (EDF_SPUser item in AD.RaomarsStockkeeper.AllUsers)
                {
                    if (item.Login == Login)
                        return true;
                }
                return false;
            }
        }
        public bool SoStatisticsAccess
        {
            get
            {
                foreach (EDF_SPUser item in AD.SoStatistics.AllUsers)
                {
                    if (item.Login == Login)
                        return true;
                }
                return false;
            }
        }


        public bool IsHR
        {
            get
            {
                return Login == AD.HR.Login;
            }
        }

        public bool IsCFO
        {
            get
            {
                return Login == AD.CFO.Login;
            }
        }



        public string PictureUrl
        {
            get
            {

                string picUrl = SPContext.Current.Web.Url + "/_catalogs/masterpage/images/default_avatar_gray.jpg";
                if (AD.Domain.Contains("pele"))
                    return picUrl;

                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        SPServiceContext ctx = SPServiceContext.GetContext(SPContext.Current.Site);
                        UserProfileManager upm = new UserProfileManager(ctx);
                        UserProfile up = upm.GetUserProfile(@"FTA\" + Login);
                        picUrl = PictureUrl1(up, "Large");
                    });
                    return picUrl;
                }
                catch { return picUrl; }
            }
        }

        public string PictureUrl1(UserProfile user, string size)
        {
            string PictureUrl0 = "emptyyyyyyy";
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    if (size == "Large")
                    {
                        if (!String.IsNullOrEmpty(Convert.ToString(user[PropertyConstants.PictureUrl].Value)))
                        { PictureUrl0 = user[PropertyConstants.PictureUrl].ToString().Replace("MThumb.jpg", "LThumb.jpg").ToString(); }
                        else
                        { PictureUrl0 = SPContext.Current.Web.Url + "/_layouts/images/edf/O14_person_placeHolder_192.png"; }
                    }
                    if (size == "Medium")
                    {
                        if (!String.IsNullOrEmpty(Convert.ToString(user[PropertyConstants.PictureUrl].Value)))
                        { PictureUrl0 = user[PropertyConstants.PictureUrl].ToString(); }
                        else
                        { PictureUrl0 = SPContext.Current.Web.Url + "/_layouts/images/edf/O14_person_placeHolder_96.png"; }
                    }
                    if (size == "Small")
                    {
                        if (!String.IsNullOrEmpty(Convert.ToString(user[PropertyConstants.PictureUrl].Value)))
                        { PictureUrl0 = user[PropertyConstants.PictureUrl].ToString().Replace("MThumb.jpg", "SThumb.jpg").ToString(); }
                        else
                        { PictureUrl0 = SPContext.Current.Web.Url + "/_layouts/images/edf/O14_person_placeHolder_32.png"; }
                    }
                });
            }
            catch { }
            return PictureUrl0.Replace("http://intra:80", "http://intranet");
        }

        public bool isDeleted { get; set; }

        public DataRow UserDataRow { get; set; }
    }
}
