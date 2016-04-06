using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

namespace EDF_CommonData
{
    public class EDF_Group
    {
        public EDF_Group(string GroupName)
        {
            Name = GroupName;
        }

        public EDF_Group(string GroupName, bool Update)
        {
            Name = GroupName;

            if(Update)
                allusers=AD.GetGroupUsers(Name);
        }

        public string Name { get; private set; }

        public List<EDF_Group> Groups
        {
            get
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
                GroupPrincipal grp = GroupPrincipal.FindByIdentity(ctx, IdentityType.Name, Name);

                List<EDF_Group> groups = new List<EDF_Group>();
                if (grp != null)
                {
                    foreach (Principal p in grp.GetMembers())
                    {
                        if(p.GetType().ToString().Split('.')[p.GetType().ToString().Split('.').Length-1]=="GroupPrincipal")
                            groups.Add(new EDF_Group(p.Name));
                    }

                    grp.Dispose();
                    ctx.Dispose();
                }
                return groups;
            }
        }

        public List<EDF_SPUser> Users
        {
            get
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
                GroupPrincipal grp = GroupPrincipal.FindByIdentity(ctx, IdentityType.Name, Name);

                List<EDF_SPUser> users = new List<EDF_SPUser>();
                if (grp != null)
                {
                    foreach (Principal p in grp.GetMembers())
                    {
                        if (p.GetType().ToString().Split('.')[p.GetType().ToString().Split('.').Length - 1] == "UserPrincipal")
                            users.Add(AD.GetUserByFullName(p.Name));
                    }

                    grp.Dispose();
                    ctx.Dispose();
                }
                return users;
            }
        }

        public List<EDF_SPUser> AllUsers 
        {
            get
            {
                if (allusers == null || allusers.Count == 0)
                    return AD.GetGroupUsers(Name);
                else
                    return allusers;
            }
        }

        List<EDF_SPUser> allusers;

        public EDF_Group FindGroup(string Name)
        {
            foreach (EDF_Group g in Groups)
                if (g.Name == Name)
                    return g;
            return null;
         
        }

        public List<EDF_Request> Requests { get; set; }

        public EDF_Request FindRequest(string RequestId)
        {
            foreach (EDF_Request r in Requests)
                if(r.Id == RequestId)
                    return r;
            
            return null;
        }
    }
}
