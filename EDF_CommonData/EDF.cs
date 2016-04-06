using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;

namespace EDF_CommonData
{
    public class Hotel
    {
        public string Name { get; set; }
        public string Dates { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public string Payment { get; set; }
    }

    public class Fly
    {
        public string Date { get; set; }
        public string Airline { get; set; }
        public string Number { get; set; }
        public string DepartureCity { get; set; }
        public string DestinationCity { get; set; }
    }

    public class OAMuser
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
    }

    public class OAMuserNot
    {
        public string Name { get; set; }
        public string Organization { get; set; }
        public string Country { get; set; }
        public string Ass_Dep { get; set; }
        public string Team { get; set; }
        public bool Intern { get; set; }
    }

    public class EDF
    {
        static string ConString = Constants.GetConnectionString();

        public static DataTable GetRequestType(int id)
        {
            SqlConnection con = new SqlConnection(ConString);
            string comand = string.Format("select * from Request_type where ID='{0}'", id);

            SqlDataAdapter da = new SqlDataAdapter(comand, con);
            DataTable tb = new DataTable();

            da.Fill(tb);
            return tb;
        }

        public static bool isExist(EDF_SPUser u)
        {
            SqlConnection con = new SqlConnection(ConString);

            string com = string.Format("SELECT * FROM del_users WHERE samAccountName='{0}'", u.Login);

            SqlDataAdapter da = new SqlDataAdapter(com, con);
            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool addUserToDB(EDF_SPUser u)
        {
            if (isExist(u))
            {
                return false;
            }

            SqlConnection con = new SqlConnection(ConString);

            String command = string.Format(@"INSERT INTO [del_users]
           ([givenName]
           ,[sn]
           ,[samAccountName]
           ,[userPrincipalName]
           ,[mail]
           ,[initials]
           ,[description]
           ,[title]
           ,[department]
           ,[physicalDeliveryOfficeName]
           ,[telephoneNumber]
           ,[otherTelephone]
           ,[mobile]
           ,[wwwHomePage]
           ,[url]
           ,[delete_date])
     VALUES
           (
            '{0}',
            '{1}',
            '{2}',
            '{3}',
            '{4}',
            '{5}',
            '{6}',
            '{7}',
            '{8}',
            '{9}',
            '{10}',
            '{11}',
            '{12}',
            '{13}',
            '{14}',
            GETDATE()
            )", u.FirtsName,//0
              u.LastName,//1
              u.Login,//2
              u.LoginDomain,//3
              u.E_Mail,//4
              u.Initials,//5
              u.Descr,//6
              u.JobTitle,//7
              u.Department,//8
              u.Office,//9
              u.TelephoneNumber,//10
              u.TelephoneOther,//11
              u.Mobile,//12
              u.WebPage,//13
              u.WebPageOther//14
              );

            SqlCommand com = new SqlCommand(command, con);
            int result = 0;

            try
            {
                con.Open();
                result = com.ExecuteNonQuery();
            }
            catch
            {

                throw new Exception("insert Deleted user error" + command);
            }
            finally { con.Close(); }

            return (result == 1);
        }


        public static EDF_SPUser getDeletedUser(string Login)
        {
            SqlConnection con = new SqlConnection(ConString);

            string com = string.Format("SELECT * FROM del_users WHERE samAccountName='{0}'", Login);

            SqlDataAdapter da = new SqlDataAdapter(com, con);
            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                return new EDF_SPUser(dt.Rows[0]);
            }
            else
            {
                return null;
            }
        }

        public static DataTable GetRequest(string AutorId, int start, int end, bool finished)
        {
            SqlConnection con = new SqlConnection(ConString);
            string cm = string.Format("select * from (select *, ROW_NUMBER() OVER (order by [Add_date] desc) as RowNum from Request {0}) as tb0 where tb0.RowNum BETWEEN {1} AND {2}",
               "where [Autor_id] = '" + AutorId + "' " + (finished ? "and [State] is not null " : "and [State] is null "),
               start.ToString(),
             end.ToString()
               );

            SqlDataAdapter da = new SqlDataAdapter(cm, con);
            DataTable dt = new DataTable();

            da.Fill(dt);
            return dt;
        }

        public static DataTable GetRequestById(int Id)
        {
            SqlConnection con = new SqlConnection(ConString);
            string cm = string.Format("select * from Request where [Id]='{0}'", Id.ToString());

            SqlDataAdapter da = new SqlDataAdapter(cm, con);
            DataTable dt = new DataTable();

            da.Fill(dt);
            return dt;
        }


        public static DataTable Search(string Autor_id, int start, int count, bool allRequest, DateTime startDate, DateTime endDate, string sortByStatus, string sortByType, string sortByDate, string keywords)
        {
            string Cur = ADSP.CurrentUser.Login;
            string comand = string.Empty;

            comand += @"DECLARE @userData Table(
                        id int not NULL,
                        type_name nvarchar(100) not NULL,
                        Autor_id nvarchar(50) not NULL,
                        Add_date datetime not NULL,
                        Icon nvarchar(100) not NULL,    
                        Status bit Null 
                        );
                        DECLARE @requests_id Table(
                        id int not NULL,
                        type_name nvarchar(100) not NULL,
                        Autor_id nvarchar(50) not NULL,
                        Add_date datetime not NULL,
                        Icon nvarchar(100) not NULL,   
                        Status bit Null
                        ); ";

            comand += string.Format(@"INSERT INTO @userData
                        SELECT DISTINCT REQ.Id,(SELECT Request_type.Name FROM [dbo].Request_type WHERE Request_type.ID = REQ.Type_id),REQ.Autor_id,REQ.Add_date,(SELECT Request_type.ImgUrlOrange FROM [dbo].Request_type WHERE Request_type.ID =  REQ.Type_id),REQ.State
                          FROM [dbo].[Request] REQ 
                          INNER JOIN [dbo].Approve_reject APP_REJ
	                        ON REQ.Id = APP_REJ.Request_ID
                        WHERE REQ.Autor_id = '{0}' OR APP_REJ.User_ID='{0}' ", Cur);

            comand += string.Format(@"INSERT INTO @requests_id
                    
                                      SELECT REQ.Id,REQ.type_name,REQ.Autor_id,REQ.Add_date,req.Icon,REQ.Status
                                      FROM [dbo].vacation INNER JOIN @userData REQ  ON REQ.Id = vacation.Request_ID
	                                  WHERE vacation.number  LIKE '%{0}%'  UNION ", keywords);
            // rsr

            comand += string.Format(@"SELECT REQ.Id,REQ.type_name,REQ.Autor_id,REQ.Add_date,req.Icon,REQ.Status
                                      FROM [dbo].RSR rsr  INNER JOIN @userData REQ  ON REQ.Id = rsr.Request_ID
	                                  WHERE rsr.Number LIKE '%{0}%' UNION ", keywords);
            // dar
            comand += string.Format(@"SELECT REQ.Id,REQ.type_name,REQ.Autor_id,REQ.Add_date,req.Icon,REQ.Status
                                      FROM [dbo].DAR dar INNER JOIN @userData REQ ON REQ.Id = dar.Request_ID
	                                  WHERE dar.Number LIKE '%{0}%'  UNION ", keywords);
            // LTR

            comand += string.Format(@"SELECT REQ.Id,REQ.type_name,REQ.Autor_id,REQ.Add_date,req.Icon,REQ.Status
                                      FROM [dbo].LTR ltr  INNER JOIN @userData REQ  ON REQ.Id = ltr.Request_ID
	                                  WHERE ltr.Number LIKE '%{0}%' UNION ", keywords);
            // ITO
            comand += string.Format(@"SELECT REQ.Id,REQ.type_name,REQ.Autor_id,REQ.Add_date,req.Icon,REQ.Status
                                      FROM [dbo].ITO ito INNER JOIN @userData REQ  ON REQ.Id = ito.Request_ID
	                                  WHERE ito.Number LIKE '%{0}%' UNION ", keywords);
            /// Stock Out Request
            comand += string.Format(@" SELECT   REQ.Id,
                                        (SELECT Request_type.Name FROM [dbo].Request_type WHERE Request_type.ID = REQ.Type_id),
                                        REQ.Autor_id,REQ.Add_date,
                                        (SELECT Request_type.ImgUrlOrange FROM [dbo].Request_type WHERE Request_type.ID =  REQ.Type_id),
                                        REQ.State
                                       FROM  [dbo].StockOutRequest sor INNER JOIN dbo.Request REQ  ON REQ.Id = sor.RequestId 
	                                   WHERE sor.OrderNumber LIKE '%{0}%' UNION ", keywords);

            //request ID or utor id

            List<string> tmp_list = keywords.Split(' ').ToList();

            string tmp_comand = "( ";

            foreach (string tmp_tmp_str in tmp_list)
            {
                tmp_comand += string.Format(@" REQ.Autor_id LIKE '%{0}%' AND", tmp_tmp_str);
            }

            tmp_comand = tmp_comand.TrimEnd("AND".ToCharArray());
            tmp_comand += " )";


            comand += string.Format(@"SELECT REQ.Id,REQ.type_name,REQ.Autor_id,REQ.Add_date,req.Icon,REQ.Status
                                      FROM @userData REQ
                                      WHERE {1} OR REQ.Id LIKE '%{0}%' ", keywords, tmp_comand);

            comand += string.Format(@"select * from (SELECT *, ROW_NUMBER() OVER ({0}) as RowNum FROM @requests_id) as tb 
                                    where (RowNum between {1} and {2})
                                    and   ([Add_date] between convert(datetime,'{3}',101) and convert(datetime,'{4}',101))",

                ("order by " +
                (sortByStatus != "" ? "[Status] " + sortByStatus + ", " : "") +
                (sortByType != "" ? "[Type_name] " + sortByType + ", " : "") +
                "[Add_date] " + sortByDate),
                start.ToString(),
                (start + count - 1).ToString(),
                startDate.ToShortDateString(),
                endDate.ToShortDateString()
                );

            SqlConnection con = new SqlConnection(ConString);
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(comand, con);

            try
            {
                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                throw new Exception(" - Saerch -  </br> " + "Can not Get Search Result From Datebase: " + ex.Message);
            }
            return table;
        }

        public static DataTable Get_Approve_reject(EDF_SPUser user, int start, int count, bool isPend, string orderBy)
        {
            SqlConnection con = new SqlConnection(ConString);
            string comand = string.Format("select * from(select * , ROW_NUMBER() OVER (order by {0}) as RowNum from ( " +
                "SELECT Approve_reject.User_ID,Approve_reject.Request_ID,Approve_reject.App_rej,Approve_reject.Date_add,Approve_reject.App_rej_Date, Request.Type_id, Request.Autor_id, Request.State , Approve_reject.Status, " +
                    "(Select Request_type.Name FROM Request_type WHERE Request_type.ID = Request.Type_id) as TypeName, " +
                    "(Select Request_type.ImgUrlOrange FROM Request_type WHERE Request_type.ID = Request.Type_id) as TypeImgUrlOrange,  " +
                    "(Select Request_type.ImgUrlDark FROM Request_type WHERE Request_type.ID = Request.Type_id) as TypeImgUrlDark " +
                    "FROM Approve_reject INNER JOIN Request ON Approve_reject.Request_ID = Request.Id " +
                     "WHERE (User_ID = '{3}' or Rep_Id = '{3}')  " +
                //   (user.HasReplacement ? " or User_ID = '" + user.Replacement.Login + "')" : ")") +
                (isPend ? " and App_rej is null " : "and App_rej is not null") +

             " UNION " +

             "SELECT SORApprove.RecievedUserId,SORApprove.RequestId,SORApprove.RequestStatus,SORApprove.RequestReceiveDate,SORApprove.ApproveDate," +
                " Request.Type_id,Request.Autor_id,Request.State,SORApprove.RecievedTo," +
                "(Select Request_type.Name FROM Request_type WHERE Request_type.ID = Request.Type_id) as TypeName, " +
                "(Select Request_type.ImgUrlOrange FROM Request_type WHERE Request_type.ID = Request.Type_id) as TypeImgUrlOrange,  " +
                "(Select Request_type.ImgUrlDark FROM Request_type WHERE Request_type.ID = Request.Type_id) as TypeImgUrlDark " +
                "FROM   SORApprove INNER JOIN Request ON SORApprove.RequestId = request.Id" +
                " WHERE (SORApprove.RecievedUserId  = '{3}' " +
                 (user.ParentReplacement.Count > 0 ? " or SORApprove.RecievedUserId = '" + user.ParentReplacement[0].Login + "')" : ")") +
               (isPend ? " and Request.State is null" : " and Request.State is not null") +
                ") as requests) AS MyDerivedTable WHERE MyDerivedTable.RowNum BETWEEN {1} AND {2}",
               orderBy,
                start.ToString(),
                count.ToString(),
                user.Login
                );

            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(comand, con);

            adapter.Fill(table);
            return table;
        }

        public static string GetUppers(string s)
        {
            string ss = s;
            if (!string.IsNullOrEmpty(ss))
            {
                List<string> words = s.Split(' ').ToList();


                foreach (string w in words)
                {
                    ss += w.Substring(0, 1).ToUpper();
                }
            }
            return (ss);
        }

        public static DataTable GetAssociations(string RequestId)
        {
            SqlConnection con = new SqlConnection(ConString);
            string selcom = string.Format("select " +
                                            "Request_Substitute.REQUEST_ID, " +
                                            "Request_Substitute.User_id, " +
                                            "Request_Substitute.Is_ok, " +
                                            "Request_Substitute.Is_ok_date " +
                                            "from Request_Substitute where REQUEST_ID = '{0}' ",
                                            RequestId);

            SqlDataAdapter da = new SqlDataAdapter(selcom, con);
            DataTable dt = new DataTable();

            da.Fill(dt);
            return dt;
        }

        public static List<EDF_User> AssociationUsers(string RequestId)
        {
            string sss = "pox: 0";
            DataTable dt = GetAssociations(RequestId);
            List<EDF_User> users = new List<EDF_User>();
            sss += "1";
            #region POXARINOXNER
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    EDF_User u = new EDF_User();
                    sss += "2";
                    EDF_SPUser Autor = AD.GetUserBySPLogin(dt.Rows[i]["User_id"].ToString());
                    sss += "2";
                    if (Autor != null)
                    {
                        sss += "3";
                        u.FullName = Autor.FullName;
                        u.Department = Autor.Department;
                        u.PictureUrl = Autor.PictureUrl;
                        switch (dt.Rows[i]["Is_ok"].ToString())
                        {
                            case null: u.IsOk = null; break;
                            case "True": u.IsOk = true; break;
                            case "False": u.IsOk = false; break;

                        }
                        sss += "4";
                        DateTime d;
                        DateTime.TryParse(dt.Rows[i]["Is_ok_date"].ToString(), out d);
                        u.ActionDate = d;
                        u.IsSubtitute = true;
                        users.Add(u);
                        sss += "5";
                    }
                }
            }
            catch
            {
                throw new Exception(sss);
            }
            #endregion
            sss += "6";

            DataTable rt = EDF.GetRequestById(int.Parse(RequestId));

            if (rt.Rows.Count > 0 && rt.Rows[0]["State"].ToString() == "True")
            {
                users.AddRange(EDF.GetAproveRejectMusers(RequestId));
            }
            else
            {

                try
                {
                    EDF_SPUser us = EDF.GetUserByRequestId(RequestId);
                    string Status = us.Status;
                    sss += "7";
                    switch (Status)
                    {
                        case "user":
                            users.Add(EDF.ManagerAppRej(RequestId, us.Manager));
                            users.Add(EDF.ManagerAppRej(RequestId, us.Director));
                            //users.Add(EDF.ManagerAppRej(RequestId, AD.CEO));
                            break;
                        case "manager":
                            if (!us.Manager.IsDirector)
                            {
                                users.Add(EDF.ManagerAppRej(RequestId, us.Manager));
                                users.Add(EDF.ManagerAppRej(RequestId, us.Director));
                            }
                            else
                            {
                                users.Add(EDF.ManagerAppRej(RequestId, us.Director));
                            }
                            //users.Add(EDF.ManagerAppRej(RequestId, AD.CEO));
                            break;
                        case "director":
                            users.Add(EDF.ManagerAppRej(RequestId, AD.CEO));
                            break;
                    }

                    users.Add(EDF.ManagerAppRej(RequestId, AD.HR));
                }
                catch
                {
                    throw new Exception(sss);
                }
            }

            return users;
        }

        public static List<EDF_User> AssociationUsersLTR(string RequestId)
        {
            List<EDF_User> users = new List<EDF_User>();
            DataTable rt = EDF.GetRequestById(int.Parse(RequestId));

            if (rt.Rows.Count > 0 && rt.Rows[0]["State"].ToString() == "True")
            {
                users.AddRange(EDF.GetAproveRejectMusers(RequestId));
            }
            else
            {

                DataTable dt = GetAssociations(RequestId);



                #region MANAGERS
                EDF_SPUser dir = null;
                EDF_SPUser us = null;
                try
                {
                    EDF_SPUser cur = ADSP.CurrentUser;
                    us = EDF.GetUserByRequestId(RequestId);

                    if (us.HasDirector)
                        dir = AD.GetUserDirector(us.Login);
                    //                if(dir == null)
                    //                    ER.GoToErrorPage("Director Not Found "+us.Login);

                    if (us.IsCEO)
                    {
                        // CFO
                    }
                    else if (dir != null && dir.Login == us.Login)
                    {
                        users.Add(EDF.ManagerAppRej(RequestId, us.Login));
                        // CEO
                    }
                    else if (dir != null && us.DirectReports.Count > 0)
                    {
                        users.Add(EDF.ManagerAppRej(RequestId, dir.Login));
                        //Director
                    }
                    else if (dir != null)
                    {
                        users.Add(EDF.ManagerAppRej(RequestId, us.Manager.Login));
                        users.Add(EDF.ManagerAppRej(RequestId, dir.Login));
                        // Manager Director CEO 

                    }

                }
                catch
                {
                    throw new Exception(string.Format("Associations users error in man"));
                }
                #endregion


                if (users.Count == 0)
                    throw new Exception(string.Format("Associations users count = 0"));

            }
            return users;
        }

        public static List<EDF_User> AssociationUsersITO(string RequestId)
        {
            DataTable dt = GetAssociations(RequestId);
            List<EDF_User> users = new List<EDF_User>();

            #region POXARINOXNER
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    EDF_User u = new EDF_User();
                    EDF_SPUser Autor = AD.GetUserBySPLogin(dt.Rows[i]["User_id"].ToString());
                    if (Autor != null)
                    {
                        u.FullName = Autor.FullName;
                        u.Department = Autor.Department;
                        u.PictureUrl = Autor.PictureUrl;
                        switch (dt.Rows[i]["Is_ok"].ToString())
                        {
                            case null: u.IsOk = null; break;
                            case "True": u.IsOk = true; break;
                            case "False": u.IsOk = false; break;

                        }
                        DateTime d;
                        DateTime.TryParse(dt.Rows[i]["Is_ok_date"].ToString(), out d);
                        u.ActionDate = d;
                        u.IsSubtitute = true;
                        users.Add(u);
                    }
                }
            }
            catch
            {
                throw new Exception(string.Format("Associations users error in pox"));
            }
            #endregion

            DataTable rt = EDF.GetRequestById(int.Parse(RequestId));

            if (rt.Rows.Count > 0 && rt.Rows[0]["State"].ToString() == "True")
            {
                users.AddRange(EDF.GetAproveRejectMusers(RequestId));
            }
            else
            {

                #region MANAGERS
                EDF_SPUser dir = null;
                EDF_SPUser us = null;
                try
                {
                    EDF_SPUser cur = ADSP.CurrentUser;
                    us = EDF.GetUserByRequestId(RequestId);

                    if (us.HasDirector)
                        dir = AD.GetUserDirector(us.Login);

                    if (us.IsCEO)
                    {
                        // CFO
                    }
                    else if (dir != null && dir.Login == us.Login)
                    {
                        // CEO CFO
                    }
                    else if (dir != null && us.Manager.IsDirector)
                    {
                        users.Add(EDF.ManagerAppRej(RequestId, dir.Login));

                    }
                    else if (dir != null)
                    {
                        users.Add(EDF.ManagerAppRej(RequestId, us.Manager.Login));
                        users.Add(EDF.ManagerAppRej(RequestId, dir.Login));
                    }

                }
                catch
                {
                    throw new Exception(string.Format("Associations users error in man"));
                }
                #endregion

                string str = string.Empty;

                #region CEO_CFO
                try
                {
                    string CFOgroup = "EDF_CFO";
                    string CEOgroup = "EDF_CEO";
                    if (!us.IsCEO)
                    {
                        string login = AD.GetGroupUsers(CEOgroup)[0].Login;
                        users.Add(EDF.ManagerAppRej(RequestId, login));

                        login = AD.GetGroupUsers(CFOgroup)[0].Login;
                        users.Add(EDF.ManagerAppRej(RequestId, login));
                    }
                    else
                    {
                        string login = AD.GetGroupUsers(CFOgroup)[0].Login;
                        users.Add(EDF.ManagerAppRej(RequestId, login));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Associations users error in ceo,hr" + ex.Message + str));
                }

                if (users.Count == 0)
                    throw new Exception(string.Format("Associations users count = 0"));

                return users;
                #endregion

            }
            return users;
        }

        public static bool GetRSRKeepAccess(string RequestId)
        {
            SqlConnection con = new SqlConnection(ConString);
            string selcom = string.Format("select top 1 * from RSR where Request_Id = '{0}'", RequestId);
            SqlCommand com = new SqlCommand(selcom, con);

            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();

            da.Fill(dt);

            return dt.Rows[0]["keep_accesses"].ToString() == "True";
        }

        public static EDF_User GetRSRGroupResult(string RequestId, EDF_Group Group)
        {
            EDF_User us = new EDF_User();
            us.IsGroup = true;
            us.Login = string.Empty;
            us.FullName = AD.TeamGroups[Group.Name];
            us.GroupName = AD.TeamGroups[Group.Name];
            us.IsOk = null;
            us.PictureUrl = SPContext.Current.Web.Url + "/_catalogs/masterpage/images/default_group.jpg";

            foreach (EDF_User u in EDF.ManagerAppRej(RequestId, Group.AllUsers))
            {
                if (u.IsOk != null)
                {
                    //us.FullName = string.Format("{0} from {1}", u.FullName, Group.Name);
                    us.Login = u.Login;
                    us.FullName = string.Format("{0} <span> from </span> {1}", u.FullName, us.GroupName);
                    us.PictureUrl = u.PictureUrl;
                    us.ActionDate = u.ActionDate;
                    us.IsOk = u.IsOk;
                    us.Department = u.Department;
                    break;
                }
            }
            return us;
        }

        public static EDF_Group GetUserTeam(EDF_SPUser User)
        {
            if (User.IsMemberOf(AD.Administration))
                return AD.Administration;
            if (User.IsMemberOf(AD.RSRAdminOffice))
                return AD.RSRAdminOffice;
            if (User.IsMemberOf(AD.RSRAdminCar))
                return AD.RSRAdminCar;
            if (User.IsMemberOf(AD.RSRITBilling))
                return AD.RSRITBilling;
            if (User.IsMemberOf(AD.RSRITReporting))
                return AD.RSRITReporting;
            if (User.IsMemberOf(AD.RSRitdbse))
                return AD.RSRitdbse;
            if (User.IsMemberOf(AD.RSRITNetwork))
                return AD.RSRITNetwork;
            if (User.IsMemberOf(AD.RSRitse))
                return AD.RSRitse;
            if (User.IsMemberOf(AD.RSRitpcsupport))
                return AD.RSRitpcsupport;
            if (User.IsMemberOf(AD.RSRitavaya))
                return AD.RSRitavaya;
            if (User.IsMemberOf(AD.RSRitdomainadmins))
                return AD.RSRitdomainadmins;
            if (User.IsMemberOf(AD.RSRVas))
                return AD.RSRVas;

            return null;
        }

        public static EDF_User GetRSRGroupResult(string RequestId, EDF_SPUser User)
        {
            EDF_Group Group = User.TeamGroup;

            return GetRSRGroupResult(RequestId, Group);
        }

        public static List<EDF_User> AssociationUsersRSR(string RequestId)
        {
            List<EDF_User> users = new List<EDF_User>();
            DataTable rt = EDF.GetRequestById(int.Parse(RequestId));

            if (rt.Rows.Count > 0 && rt.Rows[0]["State"].ToString() == "True")
            {
                users.AddRange(EDF.GetAproveRejectMusersIsApproved(RequestId));
            }
            else
            {

                EDF_SPUser us = null;

                string ss = "History RSR: ";

                bool Keep = GetRSRKeepAccess(RequestId);

                if (Keep)
                    #region CHECKED
                    try
                    {
                        us = EDF.GetUserByRequestId(RequestId);

                        if (us.Status == "director")
                            users.Add(EDF.ManagerAppRej(RequestId, AD.CEO));

                        if (us.Status == "user")
                            users.Add(EDF.ManagerAppRej(RequestId, us.Manager));

                        users.Add(EDF.ManagerAppRej(RequestId, us.Director));

                        users.Add(EDF.ManagerAppRej(RequestId, AD.ITDirector.AllUsers[0], 2));

                        ss = "ERROR in Step3Users group";

                        users.Add(GetRSRGroupResult(RequestId, AD.RSRAdministration));
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRAdminOffice));
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRAdminCar));

                        ss = "ERROR in information Security group";
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRInformationSecurity));

                        ss = "ERROR in corporate Security group";
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRCorporateSecurity));

                        ss = "ERROR in FinancePayroll group";
                        users.Add(GetRSRGroupResult(RequestId, AD.FinancePayroll));

                        ss = "ERROR in Finance group";
                        users.Add(GetRSRGroupResult(RequestId, AD.Finance));

                        ss = "ERROR in HR group";
                        users.Add(EDF.ManagerAppRej(RequestId, AD.HR));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ss + " : " + ex.Message);
                    }
                    #endregion
                else
                    #region NON CHECKED
                    try
                    {
                        us = EDF.GetUserByRequestId(RequestId);

                        if (us.Status == "director")
                            users.Add(EDF.ManagerAppRej(RequestId, AD.CEO));


                        if (us.Status == "user")
                            users.Add(EDF.ManagerAppRej(RequestId, us.Manager));


                        users.Add(EDF.ManagerAppRej(RequestId, us.Director));

                        ss = "ERROR in Step3Users group";
                        //  STEP 3
                        users.Add(GetRSRGroupResult(RequestId, AD.Administration));
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRAdminOffice));
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRAdminCar));
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRITBilling));
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRITReporting));
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRitdbse));
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRITNetwork));
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRitse));
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRitpcsupport));
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRitavaya));
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRitdomainadmins));
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRVas));

                        users.Add(EDF.ManagerAppRej(RequestId, AD.ITDirector.AllUsers[0], 2));

                        ss = "ERROR in information Security group";
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRInformationSecurity));

                        ss = "ERROR in corporate Security group";
                        users.Add(GetRSRGroupResult(RequestId, AD.RSRCorporateSecurity));


                        ss = "ERROR in FinancePayroll group";
                        users.Add(GetRSRGroupResult(RequestId, AD.FinancePayroll));

                        ss = "ERROR in Finance group";
                        users.Add(GetRSRGroupResult(RequestId, AD.Finance));

                        ss = "ERROR in HR group";
                        users.Add(EDF.ManagerAppRej(RequestId, AD.HR));


                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ss + " : " + ex.Message);
                    }
                    #endregion
                if (users.Count == 0)
                    throw new Exception(string.Format("Associations users count = 0"));
            }

            return users;
        }

        public static List<EDF_User> AssociationUsersDAR(string RequestId)
        {
            List<EDF_SPUser> rem = new List<EDF_SPUser>();
            return PartFromAppRej(RequestId);
        }

        public static bool TeamApprove(string RequestId, string Team, string Sender)
        {
            SqlConnection con = new SqlConnection(ConString);
            string selcom = string.Format("select COUNT(*) from (select *,(select tr.parent from TR where tr.Ids=Approve_reject.ID) as sender from Approve_reject where Request_ID={0}) as tb where tb.sender='{2}' and tb.Status='{1}' and tb.App_rej='True'", RequestId, Team, Sender);

            SqlCommand com = new SqlCommand(selcom, con);

            try
            {
                con.Open();

                return ((int)com.ExecuteScalar()) == 0 ? false : true;
            }
            catch { return false; }
        }

        static List<EDF_User> PartFromAppRej(string RequestId)
        {
            List<EDF_User> users = new List<EDF_User>();

            SqlConnection con = new SqlConnection(ConString);
            string selstr = string.Format("select *,(select tr.parent from TR where tr.Ids=Approve_reject.ID) as sender from Approve_reject where [Request_ID] = '{0}' ", RequestId);

            string us = string.Empty;

            selstr += us + " order by [App_rej_date]";


            SqlDataAdapter da = new SqlDataAdapter(selstr, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                EDF_SPUser u = AD.GetUserByLogin(dt.Rows[i]["User_ID"].ToString());
                string apprej = dt.Rows[i]["App_rej"].ToString().ToLower();


                if (apprej == "true" || apprej == "false")
                {
                    EDF_User mu = new EDF_User();

                    mu.IsOk = dt.Rows[i]["App_rej"].ToString() == "True";
                    mu.HasRep = dt.Rows[i]["Rep_Id"].ToString() != "";

                    if (mu.HasRep)
                        u = u.Replacement;

                    mu.Login = u.Login;
                    mu.FullName = u.FullName;
                    mu.Department = u.Department;
                    mu.PictureUrl = u.PictureUrl;
                    mu.Team = dt.Rows[i]["Status"].ToString();
                    mu.Sender = dt.Rows[i]["sender"].ToString();
                    DateTime date;
                    DateTime.TryParse(dt.Rows[i]["App_rej_Date"].ToString(), out date);
                    mu.ActionDate = date;
                    users.Add(mu);
                }
                else
                {
                    EDF_User mu = new EDF_User();
                    mu.Login = u.Login;
                    mu.FullName = u.FullName;
                    mu.Department = u.Department;
                    mu.PictureUrl = u.PictureUrl;
                    mu.Team = dt.Rows[i]["Status"].ToString();
                    mu.Sender = dt.Rows[i]["sender"].ToString();


                    mu.IsOk = null;

                    users.Add(mu);
                }
            }
            EDF_SPUser autor = EDF.GetUserByRequestId(RequestId);
            string Status = autor.Status;

            if (Status == "user" && autor.Manager.IsDirector)
                Status = "manager";

            string ss = string.Empty;

            try
            {
                switch (users.Count)
                {
                    case 1:
                        switch (Status)
                        {
                            case "user":
                                ss = "in autor director";
                                users.Add(EDF.ManagerAppRej(RequestId, autor.Director));
                                ss = "in security";
                                users.Add(EDF.ManagerAppRej(RequestId, AD.DBRInformationSecurity));
                                ss = "in Director";
                                users.Add(EDF.ManagerAppRej(RequestId, AD.ITDirector.AllUsers[0], 2));
                                break;
                            case "manager":
                                ss = string.Empty;
                                users.Add(EDF.ManagerAppRej(RequestId, AD.DBRInformationSecurity));
                                ss = string.Empty;
                                users.Add(EDF.ManagerAppRej(RequestId, AD.ITDirector.AllUsers[0], 2));
                                break;

                        }
                        if (autor.IsDirector || autor.IsCEO)
                        {
                            ss = string.Empty;
                            users.Add(EDF.ManagerAppRej(RequestId, AD.ITDirector.AllUsers[0]));
                        }
                        break;
                    case 2:
                        switch (Status)
                        {
                            case "user":
                                users.Add(EDF.ManagerAppRej(RequestId, AD.DBRInformationSecurity));
                                users.Add(EDF.ManagerAppRej(RequestId, AD.ITDirector.AllUsers[0], 2));
                                break;
                            case "manager":
                                users.Add(EDF.ManagerAppRej(RequestId, AD.ITDirector.AllUsers[0], 2));
                                break;
                        }
                        break;
                    case 3:
                        switch (Status)
                        {
                            case "user":
                                users.Add(EDF.ManagerAppRej(RequestId, AD.ITDirector.AllUsers[0], 2));
                                break;
                        }
                        break;
                }
                string itdirectorlogin = AD.ITDirector.AllUsers[0].Login;
                if (users.Where(x => x.Login == itdirectorlogin).Count() == 2 && users.Where(x => x.Login == AD.DBRInformationSecurity.Login).First().IsOk != true)
                {
                    users.Where(x => x.Login == itdirectorlogin).Last().IsOk = null;
                }
            }
            catch { throw new Exception(ss); }

            return users;
        }

        public static bool GetGroupState(string RequestId, string Status)
        {
            List<EDF_SPUser> users = new List<EDF_SPUser>();

            SqlConnection con = new SqlConnection(ConString);
            string selstr = string.Format("Select count(*) FROM [Approve_reject] WHERE [Request_ID]='{0}' and [Status]='{1}' and [App_rej]='True'", RequestId, Status);

            SqlCommand com = new SqlCommand(selstr, con);

            try
            {
                con.Open();

                int count = (int)com.ExecuteScalar();
                if (count == 0)
                    return false;
                return true;
            }
            catch { return false; }
        }

        static EDF_User ManagerAppRej(string RequestId, string Login)
        {
            SqlConnection con = new SqlConnection(ConString);
            string selstr = string.Format("select top 1 * from Approve_reject where [Request_ID] = '{0}' and [User_ID] = '{1}' and [App_rej] is not null", RequestId, Login);

            SqlDataAdapter da = new SqlDataAdapter(selstr, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                EDF_SPUser man = AD.GetUserBySPLogin(Login);
                EDF_User u = new EDF_User();
                u.FullName = man.FullName;
                u.Department = man.Department;
                u.PictureUrl = man.PictureUrl;
                u.IsOk = null;

                return u;
            }
            else
            {
                EDF_SPUser man = AD.GetUserBySPLogin(dt.Rows[0]["User_ID"].ToString());
                EDF_User u = new EDF_User();

                u.IsOk = dt.Rows[0]["App_rej"].ToString() == "True";
                u.HasRep = dt.Rows[0]["Rep_Id"].ToString() != "";

                if (u.HasRep)
                    man = man.Replacement;

                u.FullName = man.FullName;
                u.Department = man.Department;
                u.PictureUrl = man.PictureUrl;
                DateTime date;
                DateTime.TryParse(dt.Rows[0]["App_rej_Date"].ToString(), out date);
                u.ActionDate = date;
                return u;
            }
        }

       static EDF_User ManagerAppRej(string RequestId, EDF_SPUser user)
        {
            string Login = user.Login;
            SqlConnection con = new SqlConnection(ConString);
            string selstr = string.Format("select top 1 * from Approve_reject where [Request_ID] = '{0}' and [User_ID] = '{1}' and [App_rej] is not null", RequestId, Login);

            SqlDataAdapter da = new SqlDataAdapter(selstr, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                EDF_SPUser man = AD.GetUserBySPLogin(Login);
                EDF_User u = new EDF_User();
                u.Login = man.Login;
                u.FullName = man.FullName;
                u.Department = man.Department;
                u.PictureUrl = man.PictureUrl;
                u.Team = string.Empty;

                u.IsOk = null;

                return u;
            }
            else
            {
                EDF_SPUser man = AD.GetUserBySPLogin(dt.Rows[0]["User_ID"].ToString());
                EDF_User u = new EDF_User();

                u.IsOk = dt.Rows[0]["App_rej"].ToString() == "True";
                u.HasRep = dt.Rows[0]["Rep_Id"].ToString() != "";


                if (u.HasRep)
                    man = man.Replacement;

                u.Login = man.Login;
                u.FullName = man.FullName;
                u.Department = man.Department;
                u.PictureUrl = man.PictureUrl;
                u.Team = dt.Rows[0]["Status"].ToString();

                DateTime date;
                DateTime.TryParse(dt.Rows[0]["App_rej_Date"].ToString(), out date);
                u.ActionDate = date;
                return u;
            }
        }

        static int CountInAppRej(string RequestId, EDF_SPUser user)
        {
            string Login = user.Login;
            SqlConnection con = new SqlConnection(ConString);
            string selstr = string.Format("select count(*) from Approve_reject where [Request_ID] = '{0}' and [User_ID] = '{1}' and [App_rej] = 'True'", RequestId, Login);
            //if(Scan)
            //string count = string.Format("select top 1 * from Approve_reject where [Request_ID] = '{0}' and [User_ID] = '{1}' and [App_rej] is not null", RequestId, Login);
            SqlCommand com = new SqlCommand(selstr, con);

            try
            {
                con.Open();
                return (int)com.ExecuteScalar();
            }
            catch { throw new Exception("count in man"); }
        }

        static EDF_User ManagerAppRej(string RequestId, EDF_SPUser user, bool Scan)
        {
            string Login = user.Login;
            SqlConnection con = new SqlConnection(ConString);
            string selstr = string.Format("select top 1 * from Approve_reject where [Request_ID] = '{0}' and [User_ID] = '{1}' and [App_rej] is not null", RequestId, Login);

            SqlDataAdapter da = new SqlDataAdapter(selstr, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            int count = dt.Rows.Count;

            if (Scan && CountInAppRej(RequestId, user) == 1)
                count = 0;
            if (count == 0)
            {
                EDF_SPUser man = AD.GetUserBySPLogin(Login);
                EDF_User u = new EDF_User();
                u.Login = man.Login;
                u.FullName = man.FullName;
                u.Department = man.Department;
                u.PictureUrl = man.PictureUrl;
                u.Team = string.Empty;

                u.IsOk = dt.Rows[0]["App_rej"].ToString() == "True";


                DateTime date;
                DateTime.TryParse(dt.Rows[0]["App_rej_Date"].ToString(), out date);
                u.ActionDate = date;

                return u;
            }
            else
            {
                EDF_SPUser man = AD.GetUserBySPLogin(dt.Rows[0]["User_ID"].ToString());
                EDF_User u = new EDF_User();

                u.IsOk = dt.Rows[0]["App_rej"].ToString() == "True";
                u.HasRep = dt.Rows[0]["Rep_Id"].ToString() != "";


                if (u.HasRep)
                    man = man.Replacement;

                u.Login = man.Login;
                u.FullName = man.FullName;
                u.Department = man.Department;
                u.PictureUrl = man.PictureUrl;
                u.Team = dt.Rows[0]["Status"].ToString();

                DateTime date;
                DateTime.TryParse(dt.Rows[0]["App_rej_Date"].ToString(), out date);
                u.ActionDate = date;
                return u;
            }
        }

        static EDF_User ManagerAppRej(string RequestId, EDF_SPUser user, int Number)
        {
            string Login = user.Login;

            if (Number == 2 && CountInAppRej(RequestId, user) == 1)
                return ManagerAppRej(RequestId, user, true);

            SqlConnection con = new SqlConnection(ConString);
            string selstr = string.Format("select top 1 * from Approve_reject where [Request_ID] = '{0}' and [User_ID] = '{1}' and [App_rej] is not null {2}", RequestId, Login, Number == 1 ? "" : "order by id desc");

            SqlDataAdapter da = new SqlDataAdapter(selstr, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            int count = dt.Rows.Count;


            if (count == 0)
            {
                EDF_SPUser man = AD.GetUserBySPLogin(Login);
                EDF_User u = new EDF_User();
                u.Login = man.Login;
                u.FullName = man.FullName;
                u.Department = man.Department;
                u.PictureUrl = man.PictureUrl;

                u.Team = string.Empty;

                u.IsOk = null;

                return u;
            }
            else
            {
                EDF_SPUser man = AD.GetUserBySPLogin(dt.Rows[0]["User_ID"].ToString());
                EDF_User u = new EDF_User();

                u.IsOk = dt.Rows[0]["App_rej"].ToString() == "True";
                u.HasRep = dt.Rows[0]["Rep_Id"].ToString() != "";


                if (u.HasRep)
                    man = man.Replacement;

                u.Login = man.Login;
                u.FullName = man.FullName;
                u.Department = man.Department;
                u.PictureUrl = man.PictureUrl;
                u.Team = dt.Rows[0]["Status"].ToString();

                DateTime date;
                DateTime.TryParse(dt.Rows[0]["App_rej_Date"].ToString(), out date);
                u.ActionDate = date;
                return u;
            }
        }

        static List<EDF_User> ManagerAppRej(string RequestId, List<EDF_SPUser> Users)
        {
            List<EDF_User> musers = new List<EDF_User>();
            foreach (EDF_SPUser u in Users)
            {
                musers.Add(ManagerAppRej(RequestId, u.Login));
            }
            return musers;
        }

        static EDF_SPUser GetUserByRequestId(string RequestId)
        {
            SqlConnection con = new SqlConnection(ConString);
            string selstr = string.Format("select * from Request where [Id] = '{0}'", RequestId);

            SqlDataAdapter da = new SqlDataAdapter(selstr, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0)
                return null;

            string Login = dt.Rows[0]["Autor_id"].ToString();

            return AD.GetUserBySPLogin(Login);
        }

        public static int GetAproveRejectState(string RequestId, string User_id)
        {
            SqlConnection con = new SqlConnection(ConString);
            string selstr = string.Format("SELECT * FROM Approve_reject WHERE User_ID = '{0}' AND Request_ID = '{1}'", User_id, RequestId);

            SqlDataAdapter da = new SqlDataAdapter(selstr, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0)
                return -2;

            string state = dt.Rows[0]["App_rej"].ToString().ToLower();

            if (state == "true")
            {
                return 1;
            }
            else if (state == "false")
            {
                return 0;

            }
            else
                return -1;
        }

        public static int GetAproveRejectStateCFOITO(string RequestId, string User_id)
        {
            SqlConnection con = new SqlConnection(ConString);
            string selstr = string.Format("SELECT * FROM Approve_reject WHERE User_ID = '{0}' AND Request_ID = '{1}' AND App_rej is NULL ", User_id, RequestId);

            SqlDataAdapter da = new SqlDataAdapter(selstr, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0)
                return 0;
            else return 1;
        }

        public static List<EDF_User> GetAproveRejectMusers(string RequestId)
        {
            List<EDF_User> users = new List<EDF_User>();

            SqlConnection con = new SqlConnection(ConString);
            string selstr = string.Format("SELECT * FROM Approve_reject WHERE Request_ID = '{0}'", RequestId);

            SqlDataAdapter da = new SqlDataAdapter(selstr, con);
            DataTable dt = new DataTable();
            da.Fill(dt);


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                EDF_SPUser u = AD.GetUserByLogin(dt.Rows[i]["User_ID"].ToString());
                string apprej = dt.Rows[i]["App_rej"].ToString().ToLower();


                if (apprej == "true" || apprej == "false")
                {
                    EDF_User mu = new EDF_User();

                    mu.IsOk = dt.Rows[i]["App_rej"].ToString() == "True";
                    mu.HasRep = dt.Rows[i]["Rep_Id"].ToString() != "";

                    if (mu.HasRep)
                        u = u.Replacement;

                    mu.Login = u.Login;
                    mu.FullName = u.FullName;
                    mu.Department = u.Department;
                    mu.PictureUrl = u.PictureUrl;
                    mu.Team = dt.Rows[i]["Status"].ToString();
                    DateTime date;
                    DateTime.TryParse(dt.Rows[i]["App_rej_Date"].ToString(), out date);
                    mu.ActionDate = date;
                    users.Add(mu);
                }
                else
                {
                    EDF_User mu = new EDF_User();
                    mu.Login = u.Login;
                    mu.FullName = u.FullName;
                    mu.Department = u.Department;
                    mu.PictureUrl = u.PictureUrl;
                    mu.Team = dt.Rows[i]["Status"].ToString();


                    mu.IsOk = null;

                    users.Add(mu);
                }
            }
            return users;
        }

        public static List<EDF_User> GetAproveRejectMusersIsApproved(string RequestId)
        {
            List<EDF_User> users = new List<EDF_User>();

            SqlConnection con = new SqlConnection(ConString);
            string selstr = string.Format("SELECT * FROM Approve_reject WHERE Request_ID = '{0}' AND App_rej = 'True'", RequestId);

            SqlDataAdapter da = new SqlDataAdapter(selstr, con);
            DataTable dt = new DataTable();
            da.Fill(dt);


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                EDF_SPUser u = AD.GetUserByLogin(dt.Rows[i]["User_ID"].ToString());
                string apprej = dt.Rows[i]["App_rej"].ToString().ToLower();


                if (apprej == "true" || apprej == "false")
                {
                    EDF_User mu = new EDF_User();

                    mu.IsOk = dt.Rows[i]["App_rej"].ToString() == "True";
                    mu.HasRep = dt.Rows[i]["Rep_Id"].ToString() != "";

                    if (mu.HasRep)
                        u = u.Replacement;

                    mu.Login = u.Login;
                    mu.FullName = u.FullName;
                    mu.Department = u.Department;
                    mu.PictureUrl = u.PictureUrl;
                    mu.Team = dt.Rows[i]["Status"].ToString();
                    DateTime date;
                    DateTime.TryParse(dt.Rows[i]["App_rej_Date"].ToString(), out date);
                    mu.ActionDate = date;
                    users.Add(mu);
                }
                else
                {
                    EDF_User mu = new EDF_User();
                    mu.Login = u.Login;
                    mu.FullName = u.FullName;
                    mu.Department = u.Department;
                    mu.PictureUrl = u.PictureUrl;
                    mu.Team = dt.Rows[i]["Status"].ToString();

                    mu.IsOk = null;

                    users.Add(mu);
                }
            }
            return users;
        }

        public static bool AddITO(string RequestId, string Number, string DepartmentAndPosition, string City, string Organization, DateTime StartDate, DateTime EndDate, string Purpose, bool Budgeted, decimal Amount, string ReplacementId, bool Daily, Hotel Hotel, Fly Flying)
        {
            SqlConnection con = new SqlConnection(ConString);
            string Ins = string.Format(@"insert into [ITO] (Request_Id,
                                                            Number,
                                                            Filling_Date,
                                                            Department_And_Position, 
                                                            City,
                                                            Organization,
                                                            Start_Date,
                                                            End_Date, 
                                                            Purpose,
                                                            Budgeted,
                                                            Amount,
                                                            Replacement_Id,
                                                            Daily,                                                            
                                                            Hotel,
                                                            Hotel_Name,
                                                            Hotel_Dates,
                                                            Hotel_Location,
                                                            Hotel_Phone,
                                                            Hotel_Payment,
                                                            Fly_Date,
                                                            Fly_Airline,
                                                            Fly_Number,
                                                            Fly_Departure_City,
                                                            Fly_Destination_City) values ('{0}','{1}','{2}','{3}',N'{4}',{5},'{6}','{7}',N'{8}','{9}',{10},{11},'{12}','{13}',{14},{15},{16},{17},{18},'{19}',N'{20}',N'{21}',N'{22}',N'{23}')",
                                            RequestId.ToString(),
                                            Number.ToString(),
                                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                            DepartmentAndPosition,
                                            City.Replace("'", "''"),
                                            (Organization == null ? "NULL" : "N'" + Organization.Replace("'", "''") + "'"),
                                            StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                            EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                            Purpose.Replace("'", "''"),
                                            Budgeted.ToString().Replace("'", "''"),
                                            (Amount == decimal.Zero || Amount == 0 ? "NULL" : "'" + Amount.ToString().Replace("'", "''") + "'"),
                                            (ReplacementId == null ? "NULL" : "'" + ReplacementId + "'"),
                                            Daily.ToString().Replace("'", "''"),
                                            (Hotel == null ? "False" : "True"),

                                            (Hotel == null ? "NULL" : "N'" + Hotel.Name.Replace("'", "''") + "'"),
                                            (Hotel == null ? "NULL" : "N'" + Hotel.Dates.Replace("'", "''") + "'"),
                                            (Hotel == null ? "NULL" : "N'" + Hotel.Location.Replace("'", "''") + "'"),
                                            (Hotel == null ? "NULL" : "N'" + Hotel.Phone.Replace("'", "''") + "'"),
                                            (Hotel == null ? "NULL" : "N'" + Hotel.Payment.Replace("'", "''") + "'"),

                                            Flying.Date.Replace("'", "''"),
                                            Flying.Airline.Replace("'", "''"),
                                            Flying.Number.Replace("'", "''"),
                                            Flying.DepartureCity.Replace("'", "''"),
                                            Flying.DestinationCity.Replace("'", "''")
                                            );

            SqlCommand InsCom = new SqlCommand(Ins, con);
            try
            {
                con.Open();
                InsCom.ExecuteNonQuery();
            }
            catch { return false; }
            finally { con.Close(); }

            return true;
        }

        public static bool AddLTR(string RequestId, string Number, string DepartmentAndPosition, string City, DateTime StartDate, DateTime EndDate, string Purpose, bool Daily, string CarTime, bool Hotel)
        {
            SqlConnection con = new SqlConnection(ConString);
            string Ins = string.Format(@"insert into [LTR] (Request_Id,
                                                            Number,
                                                            Filling_Date,
                                                            Department_And_Position, 
                                                            City, 
                                                            Start_Date, 
                                                            End_Date, 
                                                            Purpose, 
                                                            Daily, 
                                                            Car_Time, 
                                                            Hotel) values ('{0}','{1}','{2}','{3}',N'{4}','{5}','{6}',N'{7}','{8}',{9},'{10}')",
                                            RequestId.ToString(),
                                            Number.ToString(),
                                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                            DepartmentAndPosition,
                                            City,
                                            StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                            EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                            Purpose,
                                            Daily.ToString(),
                                            (CarTime == null ? "NULL" : "'" + CarTime.ToString() + "'"),
                                            Hotel.ToString()
                                            );

            SqlCommand InsCom = new SqlCommand(Ins, con);
            try
            {
                con.Open();
                InsCom.ExecuteNonQuery();
            }
            catch { return false; }
            finally { con.Close(); }

            return true;
        }

        public static bool AddRSR(string RequestId, string Number, string DepartmentAndPosition, string AutorId, string Father, string Cboss, string Phone, string PrivatePhone, DateTime LastWorkingDay, bool keep_accesses, DateTime keep_accesses_date)
        {
            SqlConnection con = new SqlConnection(ConString);
            string Ins = string.Format(@"insert into [RSR] (Request_Id,
                                                            Number,
                                                            Filling_Date,
                                                            Department_And_Position, 
                                                            Autor_Id,
                                                            Father,
                                                            Cboss,
                                                            Phone, 
                                                            Private_Phone,
                                                            Last_Work_Day,
                                                            keep_accesses,
                                                            keep_accesses_date) values ('{0}','{1}','{2}','{3}','{4}',N'{5}',{6},'{7}','{8}','{9}','{10}',{11})",
                                            RequestId.ToString(),
                                            Number.ToString(),
                                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                            DepartmentAndPosition.Replace("'", "''"),
                                            AutorId,
                                            Father.Replace("'", "''"),
                                            (Cboss == null || Cboss.Contains("Write Cboss") ? "NULL" : "N'" + Cboss.Replace("'", "''") + "'"),
                                            Phone.Replace("'", "''"),
                                            PrivatePhone.Replace("'", "''"),
                                            LastWorkingDay.ToShortDateString(),//("yyyy-MM-dd HH:mm:ss")
                                            keep_accesses,
                                            keep_accesses ? "'" + keep_accesses_date + "'" : "NULL"
                                            );

            SqlCommand InsCom = new SqlCommand(Ins, con);
            try
            {
                con.Open();
                InsCom.ExecuteNonQuery();

            }
            catch { return false; }
            finally { con.Close(); }

            return true;
        }

        public static bool AddDAR(string RequestId, string Number, string DepartmentAndPosition, string AutorId, string Requestor, string Equipment, string Email, string InternetAccess, string WorkstationMACAddress, string Description, string Attachment, string AccessPeriodStart, string AccessPeriodEnd, string Beneficiary, OAMuser oam, OAMuserNot noam)
        {
            SqlConnection con = new SqlConnection(ConString);
            string Ins = string.Format(@"insert into [DAR] (Request_Id,
                                                            Number,
                                                            Filling_Date,
                                                            Department_And_Position, 
                                                            Autor_Id,
                                                            Requestor,
                                                            Equipment,
                                                            Email,
                                                            Internet_Access,
                                                            Workstation_MAC_Address,
                                                            [Description],
                                                            Attachment,
                                                            Access_Period_Start,
                                                            Access_Period_End,
                                                            Beneficiary,

                                                            Name,
                                                            Department,
                                                            Position,

                                                            Name2,
                                                            Organization,
                                                            Country,
                                                            Ass_Dep,
                                                            Team,
                                                            Intern
                                                            ) values ('{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}','{8}','{9}',{10},{11},{12},{13},{14},   {15},{16},{17},  {18},{19},{20},{21},{22},{23})",
                                            RequestId.ToString(),
                                            Number.ToString(),
                                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                            DepartmentAndPosition.Replace("'", "''"),
                                            AutorId,
                                            Requestor,
                                            (Equipment == null ? "NULL" : "'" + Equipment.Replace("'", "''") + "'"),
                                            Email.Replace("'", "''"),
                                            InternetAccess.Replace("'", "''"),
                                            WorkstationMACAddress.Replace("'", "''"),
                                            (Description == null || Description.Contains("Write") ? "NULL" : "N'" + Description.Replace("'", "''") + "'"),
                                            (Attachment == null ? "NULL" : "'" + Attachment.Replace("'", "''") + "'"),
                                            (AccessPeriodStart == null ? "NULL" : "'" + AccessPeriodStart.Replace("'", "''") + "'"),
                                            (AccessPeriodEnd == null ? "NULL" : "'" + AccessPeriodEnd.Replace("'", "''") + "'"),
                                            (Beneficiary == null ? "NULL" : "'" + Beneficiary.Replace("'", "''") + "'"),

                                            (Beneficiary == null ? "NULL" : !Beneficiary.Contains("Non") ? "N'" + oam.Name.Replace("'", "''") + "'" : "NULL"),
                                            (Beneficiary == null ? "NULL" : !Beneficiary.Contains("Non") ? "N'" + oam.Department.Replace("'", "''") + "'" : "NULL"),
                                            (Beneficiary == null ? "NULL" : !Beneficiary.Contains("Non") ? "N'" + oam.Position.Replace("'", "''") + "'" : "NULL"),

                                            (Beneficiary == null ? "NULL" : Beneficiary.Contains("Non") ? "N'" + noam.Name.Replace("'", "''") + "'" : "NULL"),
                                            (Beneficiary == null ? "NULL" : Beneficiary.Contains("Non") && !noam.Organization.Contains("Write organization:") ? "N'" + noam.Organization.Replace("'", "''") + "'" : "NULL"),
                                            (Beneficiary == null ? "NULL" : Beneficiary.Contains("Non") && !noam.Country.Contains("Write country:") ? "N'" + noam.Country.Replace("'", "''") + "'" : "NULL"),
                                            (Beneficiary == null ? "NULL" : Beneficiary.Contains("Non") ? "N'" + noam.Ass_Dep.Replace("'", "''") + "'" : "NULL"),
                                            (Beneficiary == null ? "NULL" : Beneficiary.Contains("Non") && !noam.Team.Contains("Write team:") ? "N'" + noam.Team.Replace("'", "''") + "'" : "NULL"),
                                            (Beneficiary == null ? "NULL" : Beneficiary.Contains("Non") ? "N'" + noam.Intern.ToString().Replace("'", "''") + "'" : "NULL")
                                            );

            SqlCommand InsCom = new SqlCommand(Ins, con);
            try
            {
                con.Open();
                InsCom.ExecuteNonQuery();
            }
            catch { return false; }
            finally { con.Close(); }

            return true;
        }

        public static DataTable GetLTR(string RequestId)
        {
            SqlConnection con = new SqlConnection(ConString);
            string Sel = string.Format("select * from [LTR] where Request_Id = '{0}'", RequestId);

            SqlDataAdapter da = new SqlDataAdapter(Sel, con);
            DataTable dt = new DataTable();

            da.Fill(dt);
            return dt;
        }

        public static DataTable GetITO(string RequestId)
        {
            SqlConnection con = new SqlConnection(ConString);
            string Sel = string.Format("select * from [ITO] where Request_Id = '{0}'", RequestId);

            SqlDataAdapter da = new SqlDataAdapter(Sel, con);
            DataTable dt = new DataTable();

            da.Fill(dt);
            return dt;
        }

        public static DataTable GetRSR(string RequestId)
        {
            SqlConnection con = new SqlConnection(ConString);
            string Sel = string.Format("select * from [RSR] where Request_Id = '{0}'", RequestId);

            SqlDataAdapter da = new SqlDataAdapter(Sel, con);
            DataTable dt = new DataTable();

            da.Fill(dt);
            return dt;
        }

        public static DataTable GetDAR(string RequestId)
        {
            SqlConnection con = new SqlConnection(ConString);
            string Sel = string.Format("select * from [DAR] where Request_Id = '{0}'", RequestId);

            SqlDataAdapter da = new SqlDataAdapter(Sel, con);
            DataTable dt = new DataTable();

            da.Fill(dt);
            return dt;
        }
    }
}
