using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.SharePoint;

namespace EDF_CommonData
{
    public static class Connection
    {
        public static string connectionString = Constants.GetConnectionString();
    }

    public static class request
    {
        public static int count
        {
            get
            {
                string comand = "SELECT count(*) FROM Request ";
                SqlConnection con = new SqlConnection(Connection.connectionString);
                try
                {
                    con.Open();
                    SqlCommand com = new SqlCommand(comand, con);
                    return (int)com.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    ER.GoToErrorPage(" - GetRequest count -  </br> " + "Can not Get Request from database: " + ex.Message);
                }
                finally { con.Close(); }
                return 0;
            }
        }

        public static bool Add(int Type_id, string Autor_id)
        {
            string comand = "INSERT INTO Request (Type_id, Autor_id, Add_date)" +
                                         "VALUES ('" + Type_id.ToString() + "', " +
                                                 "'" + Autor_id.ToString() + "', " +
                                                 "'" + DateTime.Now + "')";

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();

            }
            catch
            {
                return false;
            }
            finally
            {
                con.Close();
            }

            return true;
        }

        public static int GetId(string Autor_id)
        {
            int ID = 0;

            string comand = "SELECT TOP 1 * FROM Request WHERE " +
                "Autor_id='" + Autor_id + "' AND State is null ORDER BY ID DESC";
            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ID = int.Parse(reader["Id"].ToString());
                    }
                }
                else
                {
                    ID = -1;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetRequest_ID -  </br> " + "Can not Get Request from database: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return ID;
        }

        public static string GetAutor_id(string Request_ID)
        {
            string Autor_id = "-1";

            string comand = "SELECT TOP 1 Autor_id FROM Request WHERE " +
                "Id='" + Request_ID + "'";
            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Autor_id = reader["Autor_id"].ToString();
                    }
                }
                else
                {
                    Autor_id = "-1";
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetAutor_id -  </br> " + "Can not Get Request Autor from database: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return Autor_id;
        }

        public static bool IsYour(string id, string Autor_id)
        {
            string comand = "SELECT Id  FROM Request WHERE " +
                "Autor_id='" + Autor_id + "' and Id = '" + id + "'";
            SqlConnection con = new SqlConnection(Connection.connectionString);
            bool l = false;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    l = true;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - IsYour -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
            return l;
        }

        public static DataTable GetAllUsers_ids(string REQUEST_ID)
        {
            string comand = string.Format("select Request.Autor_id from Request where Request.Id='{0}' " +
                                          "union " +
                                          "select Approve_reject.User_ID from Approve_reject where Request_ID='{0}' " +
                                          "union " +
                                          "select Approve_reject.Rep_Id from Approve_reject where Rep_Id is not NULL and Request_ID='{0}' "
                                          , REQUEST_ID);

            DataTable table = new DataTable("users");

            try
            {
                SqlConnection con = new SqlConnection(Connection.connectionString);

                SqlDataAdapter adapter = new SqlDataAdapter(comand, con);

                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetAllUsers_ids -  </br> " + "Can not Get All Users from database: " + ex.Message);
            }

            return table;
        }

        public static void Update(string Request_Id, bool State)
        {
            string comand = string.Format("UPDATE [dbo].[Request] SET [dbo].[Request].State = '{0}' where Id = '{1}'", State.ToString(), Request_Id);

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateRequest -  </br> " + "Can not Change Request fields in database: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
    }

    public static class LTR
    {
        public static bool Add(string RequestId, int Number, string DepartmentAndPosition, string City, DateTime StartDate, DateTime EndDate, string Purpose, bool Daily, string CarTime, bool Hotel)
        {
            string Ins = string.Empty;
            if (CarTime.ToLower() == "null")
            {
                Ins = string.Format("insert into [LTR] (Request_Id,Number,Filling_Date,Department_And_Position, City, Start_Date, End_Date,  Purpose,  Daily,  Hotel) values ('{0}','{1}','{2}','{3}',N'{4}','{5}','{6}',N'{7}','{8}','{9}')",
                                           RequestId,
                                           Number.ToString(),
                                           DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                           DepartmentAndPosition.Replace("'", "''"),
                                           City.Replace("'", "''"),
                                           StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                           EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                           Purpose.Replace("'", "''"),
                                           Daily.ToString().Replace("'", "''"),
                                           Hotel.ToString().Replace("'", "''")
                                           );

            }
            else
            {
                Ins = string.Format("insert into [LTR] (Request_Id,Number,Filling_Date,Department_And_Position, City, Start_Date, End_Date,  Purpose,  Daily,  Car_Time,  Hotel) values ('{0}','{1}','{2}','{3}',N'{4}','{5}','{6}',N'{7}','{8}','{9}','{10}')",
                                               RequestId,
                                               Number.ToString(),
                                               DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                               DepartmentAndPosition.Replace("'", "''"),
                                               City.Replace("'", "''"),
                                               StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                               EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                               Purpose.Replace("'", "''"),
                                               Daily.ToString().Replace("'", "''"),
                                               CarTime.ToString().Replace("'", "''"),
                                               Hotel.ToString().Replace("'", "''")
                                               );
            }

            SqlConnection con = new SqlConnection(Connection.connectionString);


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

        public static void Update(string Request_Id)
        {
            string comand = string.Empty;
            if (DateTime.Now.Year == 2015)
                comand = string.Format("IF (SELECT MAX(Number) FROM [ITO] where Filling_date > Cast('2015-03-01' as date)) IS NOT NULL " +
                                "BEGIN " +
                                    " IF ((SELECT MAX(Number) FROM [LTR] where Filling_date > Cast('2015-04-01' as date)) > (SELECT MAX(Number) FROM [ITO] where Filling_date > Cast('2015-03-01' as date)))" +
                                        " BEGIN " +
                                            " UPDATE [LTR] SET Number=((SELECT MAX(Number) FROM [LTR] where Filling_date > Cast('2015-04-01' as date)) + 1) WHERE Request_Id = '{0}' " +
                                        " END " +
                                    " ELSE " +
                                        " UPDATE [LTR] SET Number=((SELECT MAX(Number) FROM [ITO] where Filling_date > Cast('2015-03-01' as date)) + 1) WHERE Request_Id = '{0}' " +
                                " END " +
                                " ELSE " +
                                    " UPDATE [LTR] SET Number=((SELECT MAX(Number) FROM [LTR] where Filling_date > Cast('2015-04-01' as date)) + 1) WHERE Request_Id = '{0}'", Request_Id);
            else
                comand = string.Format("IF (SELECT MAX(Number) FROM [ITO] where year(Filling_date) = YEAR(getdate())) IS NOT NULL " +
                    "BEGIN " +
                        " IF ((SELECT MAX(Number) FROM [LTR] where year(Filling_date) = YEAR(getdate())) > (SELECT MAX(Number) FROM [ITO] where year(Filling_date) = YEAR(getdate())))" +
                            " BEGIN " +
                                " UPDATE [LTR] SET Number=((SELECT MAX(Number) FROM [LTR] where year(Filling_date) = YEAR(getdate())) + 1) WHERE Request_Id = '{0}' " +
                            " END " +
                        " ELSE " +
                            " UPDATE [LTR] SET Number=((SELECT MAX(Number) FROM [ITO] where year(Filling_date) = YEAR(getdate())) + 1) WHERE Request_Id = '{0}' " +
                    " END " +
                    " ELSE " +
                        " UPDATE [LTR] SET Number=((SELECT MAX(Number) FROM [LTR] where year(Filling_date) = YEAR(getdate())) + 1) WHERE Request_Id = '{0}'", Request_Id);


            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateVacation -  </br> " + "Can not Change Vacation request document in database: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static DataTable Get(string RequestId)
        {
            SqlConnection con = new SqlConnection(Connection.connectionString);
            string Sel = string.Format("select * from [LTR] where Request_Id = '{0}'", RequestId);

            SqlDataAdapter da = new SqlDataAdapter(Sel, con);
            DataTable dt = new DataTable();

            da.Fill(dt);
            return dt;
        }

        public static string GetNumber(string RequestId)
        {
            string Number = "0";

            string comand = string.Format("select Number from LTR WHERE Request_Id = '{0}'", RequestId);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Number = reader["Number"].ToString();
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - Get LTR Number - " + "Can not Get Local Travel Request Number from database: "); }
            finally { con.Close(); }
            return Number;
        }

        public static bool CarIsChecked(string RequestId)
        {
            bool b = false;

            string comand = string.Format("select Car_Time from LTR WHERE Request_Id = '{0}'", RequestId);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        b = true;
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - Get LTR CarIsChecked - " + "Can not Get Local Travel Request CarIsChecked from database: "); }
            finally { con.Close(); }
            return b;
        }

        public static bool CanAppRejLRT(string Request_ID, string User_ID)
        {
            string comand = string.Format("select ID from Approve_reject where Request_ID = '{0}' and User_ID = '{1}' union select Id from Request where Id = '{0}' and Autor_Id = '{1}'", Request_ID, User_ID);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();


                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return true;
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - CanAppRej - "); }
            finally { con.Close(); }
            return false;
        }
    }

    public static class ITO
    {
        public static bool Add(string RequestId, string Number, string DepartmentAndPosition, string City, string Organization, DateTime StartDate, DateTime EndDate, string Purpose, bool Budgeted, decimal Amount, string ReplacementId, bool Daily, Hotel Hotel, Fly Flying)
        {
            try
            {
                EDF.AddITO(RequestId, Number, DepartmentAndPosition, City, Organization, StartDate, EndDate, Purpose, Budgeted, Amount, ReplacementId, Daily, Hotel, Flying);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetNumber(string RequestId)
        {
            string Number = "0";

            string comand = string.Format("select Number from [ITO] WHERE Request_Id = '{0}'", RequestId);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Number = reader["Number"].ToString();
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - Get LRT Number - "); }
            finally { con.Close(); }
            return Number;
        }

        public static bool CanAppRejITO(string Request_ID, string User_ID)
        {

            string comand = string.Format("select ID from Approve_reject where Request_ID = '{0}' and User_ID = '{1}' union select ID from Request_Substitute where REQUEST_ID = '{0}' and User_id = '{1}' AND Is_ok IS NULL union select Id from Request where Id = '{0}'  and Autor_Id = '{1}'", Request_ID, User_ID);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return true;
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - CanAppRej - "); }
            finally { con.Close(); }
            return false;
        }

        public static bool CanAppRejIsITO_Null(string Request_ID, string User_ID)
        {

            string comand = string.Format("select ID from Approve_reject where Request_ID = '{0}' and User_ID = '{1}' AND App_rej IS NULL", Request_ID, User_ID);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return true;
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - CanAppRej - "); }
            finally { con.Close(); }
            return false;
        }

        public static int UpdateWorkDaysCount(string Request_ID, string p)
        {
            string comand = "Update vacation SET Work_days = " + p + " WHERE REQUEST_ID = '" + Request_ID + "'";
            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);
                return com.ExecuteNonQuery();
            }
            catch { }
            return -1;
        }

        public static void Update(string Request_Id)
        {
            string comand = string.Empty;
            if (DateTime.Now.Year == 2015)
                comand = string.Format("IF (SELECT MAX(Number) FROM [LTR] where  Filling_date > Cast('2015-04-01' as date)) IS NOT NULL " +
                                        " BEGIN " +
                                        " IF ((SELECT MAX(Number) FROM [LTR] where Filling_date > Cast('2015-04-01' as date)) > (SELECT MAX(Number) FROM [ITO] where Filling_date > Cast('2015-03-01' as date)))" +
                                            " BEGIN " +
                                            " UPDATE [ITO] SET Number=((SELECT MAX(Number) FROM [LTR] where Filling_date > Cast('2015-04-01' as date)) + 1) WHERE Request_Id = '{0}' " +
                                            " END " +
                                            " ELSE " +
                                            " UPDATE [ITO] SET Number=((SELECT MAX(Number) FROM [ITO] where Filling_date > Cast('2015-03-01' as date)) + 1) WHERE Request_Id = '{0}' " +
                                            " END " +
                                            " ELSE " +
                                            " UPDATE [ITO] SET Number=((SELECT MAX(Number) FROM [ITO] where Filling_date > Cast('2015-03-01' as date)) + 1) WHERE Request_Id = '{0}'", Request_Id);
            else
                comand = string.Format("IF (SELECT MAX(Number) FROM [LTR] where year(Filling_date) = YEAR(getdate())) IS NOT NULL " +
                                        " BEGIN " +
                                        " IF ((SELECT MAX(Number) FROM [LTR] where year(Filling_date) = YEAR(getdate())) > (SELECT MAX(Number) FROM [ITO] where year(Filling_date) = YEAR(getdate())))" +
                                            " BEGIN " +
                                            " UPDATE [ITO] SET Number=((SELECT MAX(Number) FROM [LTR] where year(Filling_date) = YEAR(getdate())) + 1) WHERE Request_Id = '{0}' " +
                                            " END " +
                                            " ELSE " +
                                            " UPDATE [ITO] SET Number=((SELECT MAX(Number) FROM [ITO] where year(Filling_date) = YEAR(getdate())) + 1) WHERE Request_Id = '{0}' " +
                                            " END " +
                                            " ELSE " +
                                            " UPDATE [ITO] SET Number=((SELECT MAX(Number) FROM [ITO] where year(Filling_date) = YEAR(getdate())) + 1) WHERE Request_Id = '{0}'", Request_Id);

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateITO -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
    }

    public static class Approve_reject
    {
        public static List<EDF_SPUser> getRepPeoples(string Request_ID)
        {
            string comand = string.Format("SELECT * FROM Request_Substitute WHERE [REQUEST_ID] = {0}", Request_ID);

            List<EDF_SPUser> users = new List<EDF_SPUser> { };

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                SqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    users.Add(AD.GetUserBySPLogin(dr["User_id"].ToString()));
                }
            }
            catch
            {
                return users;
            }
            finally
            {
                con.Close();
            }

            return users;

        }

        public static bool isApproved(string Request_ID, string Status)
        {
            string command = string.Format(@"IF (SELECT COUNT(*) FROM Approve_reject WHERE Request_ID='{0}' AND Approve_reject.Status = '{1}') > 0
BEGIN
SELECT COUNT(*) as count FROM Approve_reject tmp_app_rej WHERE tmp_app_rej.Request_ID = '{0}' AND tmp_app_rej.Status = '{1}' AND tmp_app_rej.App_rej IS NOT NULL
END
ELSE
BEGIN
SELECT -1 as count
END", Request_ID, Status);




            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(command, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (reader["count"].ToString() == "-1")
                        {
                            return false;
                        }
                        else if (int.Parse(reader["count"].ToString()) > 0)
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - isApproved - "); }
            finally { con.Close(); }
            return false;


        }

        public static DateTime getLastApproveDate(string Request_ID)
        {
            string comand = string.Format("SELECT MAX(App_rej_Date) as app_date FROM Approve_reject WHERE Request_ID = '{0}'", Request_ID);


            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return ((DateTime)reader["app_date"]);
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - CanAppRej - "); }
            finally { con.Close(); }
            return DateTime.Parse("1/1/9000");

        }

        public static bool canView(EDF_SPUser current, string Request_ID)
        {
            string comand = string.Empty;

            if (current.IsReplacement)
            {
                string tmp_str = string.Empty;

                foreach (EDF_SPUser tmpUser in current.IsReplacements)
                {
                    tmp_str += "'" + tmpUser.Login + "',";
                }
                tmp_str += "'" + current.Login + "'";
                comand = string.Format(@" SELECT COUNT(ID) as count FROM (SELECT [Approve_reject].ID as ID FROM [Approve_reject]  WHERE ([Approve_reject].Request_ID = '{0}') AND ([Approve_reject].User_ID IN({1}) OR [Approve_reject].Rep_Id IN({1}))
UNION
SELECT [Id] as ID FROM [Request] WHERE Autor_id IN({1}) AND Id='{0}'
UNION
SELECT [ID] as ID FROM [Request_Substitute] WHERE [REQUEST_ID] = '{0}' AND [User_id] IN({1})) as tmp", Request_ID, tmp_str);

            }
            else
            {
                comand = string.Format(@" SELECT COUNT(ID) as count FROM (SELECT [Approve_reject].ID as ID FROM [Approve_reject]  WHERE ([Approve_reject].Request_ID = '{0}') AND ([Approve_reject].User_ID = '{1}' OR [Approve_reject].Rep_Id = '{1}')
UNION
SELECT [Id] as ID FROM [Request] WHERE Autor_id = '{1}' AND Id='{0}'
UNION
SELECT [ID] as ID FROM [Request_Substitute] WHERE [REQUEST_ID] = '{0}' AND [User_id] = '{1}') as tmp", Request_ID, current.Login);
            }

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();


                reader.Read();

                if (int.Parse(reader["count"].ToString()) > 0)
                {
                    reader.Close();
                    return true;
                }
                else
                {
                    reader.Close();
                   
                    DataTable rt = EDF.GetRequestById(int.Parse(Request_ID));
                    foreach (DataRow dr in rt.Rows)
                    {
                        foreach (string type_id in current.RequestAccess)
                        {
                            if (dr["Type_id"].ToString() == type_id)
                            {
                                return true;
                            }
                        }
                    }
                    string notCommand = string.Format("SELECT COUNT(*) as count FROM [Notificaion] WHERE User_id = '{0}' AND (select item FROM (select item, ROW_NUMBER() over (order by item) as rowNum  from fnSplit(Notificaion.Visit_url, '?rid=')) as temp WHERE temp.rowNum = 1) = '{1}'", current.Login, Request_ID);

                    com = new SqlCommand(notCommand, con);

                    SqlDataReader notreader = com.ExecuteReader();


                    notreader.Read();

                    if (int.Parse(notreader["count"].ToString()) > 0)
                    {
                        notreader.Close();
                        return true;
                    }
                    else
                    {
                        notreader.Close();
                    }



                    return false;
                }
            }
            catch (Exception ex) { ER.GoToErrorPage(" - Can View - " + ex.Message + comand); }
            finally { con.Close(); }
            return false;

        }

        public static bool Add(string User_ID, string Request_ID, string url)
        {
            EDF_SPUser rus = AD.GetUserBySPLogin(User_ID);
            string comand = string.Empty;
            if (rus.HasReplacement)
            {
                comand = string.Format("INSERT INTO Approve_reject (User_ID, Request_ID, Date_add,Rep_Id) VALUES ('{0}','{1}','{2}','{3}')", User_ID, Request_ID, DateTime.Now, rus.Replacement.Login);
            }
            else
            {
                comand = string.Format("INSERT INTO Approve_reject (User_ID, Request_ID, Date_add) VALUES ('{0}','{1}','{2}')", User_ID, Request_ID, DateTime.Now);
            }
            SqlConnection con = new SqlConnection(Connection.connectionString);

            string msg = string.Empty;
            string typeN = string.Empty;
            string Autor_id = request.GetAutor_id(Request_ID);
            EDF_SPUser autor = AD.GetUserBySPLogin(Autor_id);

            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();
            }
            catch
            {
                return false;
            }
            finally
            {
                con.Close();
            }

            typeN = Request_type.GetTypeName(Request_ID);

            msg = string.Format("<b>{0}’s</b> {1} (ID: {2} {3}) is <b>submitted to you</b> for approval", autor.FullName, typeN, Request_type.GetUppers(typeN), Request_ID);

            Notificaion.Add(User_ID, SPContext.Current.Web.Url + "/SitePages/" + url + "?rid=" + Request_ID, msg, autor.PictureUrl, Request_type.GetId(Request_ID));

            if (rus.HasReplacement)
            {
                msg = string.Format("<b>{0}’s</b> {1} (ID: {2} {3}) is <b>submitted to {4}</b> for approval", autor.FullName, typeN, Request_type.GetUppers(typeN), Request_ID, rus.FullName); // YUPE IP

                Notificaion.Add(rus.Replacement.Login, SPContext.Current.Web.Url + "/SitePages/" + url + "?rid=" + Request_ID, msg, autor.PictureUrl, Request_type.GetId(Request_ID));
            }

            return true;
        }

        public static bool Add(string User_ID, string Request_ID, string url, string status)
        {
            EDF_SPUser rus = AD.GetUserBySPLogin(User_ID);
            string comand = string.Empty;
            if (rus.HasReplacement)
            {
                comand = string.Format("INSERT INTO Approve_reject (User_ID, Request_ID, Date_add, Status,Rep_Id) VALUES ('{0}','{1}','{2}','{3}','{4}')", User_ID, Request_ID, DateTime.Now, status, rus.Replacement.Login);
            }
            else
            {
                comand = string.Format("INSERT INTO Approve_reject (User_ID, Request_ID, Date_add, Status) VALUES ('{0}','{1}','{2}','{3}')", User_ID, Request_ID, DateTime.Now, status);
            }
            SqlConnection con = new SqlConnection(Connection.connectionString);

            string msg = string.Empty;
            string typeN = string.Empty;
            string Autor_id = request.GetAutor_id(Request_ID);
            EDF_SPUser autor = AD.GetUserBySPLogin(Autor_id);

            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();
            }
            catch
            {
                return false;
            }
            finally
            {
                con.Close();
            }
            string ss = "0";
            try
            {
                typeN = Request_type.GetTypeName(Request_ID);
                ss = "1";
                msg = string.Format("<b>{0}’s</b> {1} (ID: {2} {3}) is <b>submitted to you</b> for approval", autor.FullName, typeN, Request_type.GetUppers(typeN), Request_ID);

                ss = "2";
                Notificaion.Add(User_ID, SPContext.Current.Web.Url + "/SitePages/" + url + "?rid=" + Request_ID, msg, autor.PictureUrl, Request_type.GetId(Request_ID));
                ss = "3";

                ss = "4";
                if (rus.HasReplacement)
                {
                    ss = "5";
                    msg = string.Format("<b>{0}’s</b> {1} (ID: {2} {3}) is <b>submitted to {4}</b> for approval", autor.FullName, typeN, Request_type.GetUppers(typeN), Request_ID, rus.FullName); // YUPE IP
                    ss = "6";
                    Notificaion.Add(rus.Replacement.Login, SPContext.Current.Web.Url + "/SitePages/" + url + "?rid=" + Request_ID, msg, autor.PictureUrl, Request_type.GetId(Request_ID));
                }
            }
            catch { throw new Exception(ss); }

            return true;
        }

        public static string AddT(string User_ID, string Request_ID, string url, string status, string parent)
        {
            string AppRejID = string.Empty;
            string comand = string.Format("INSERT INTO Approve_reject (User_ID, Request_ID, Date_add, Status) VALUES ('{0}','{1}','{2}','{3}') select SCOPE_IDENTITY()", User_ID, Request_ID, DateTime.Now, status);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            string msg = string.Empty;
            string typeN = string.Empty;
            string Autor_id = request.GetAutor_id(Request_ID);
            EDF_SPUser autor = AD.GetUserBySPLogin(Autor_id);

            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                AppRejID = command.ExecuteScalar().ToString();
            }
            catch
            {
                return "-1";
            }
            finally
            {
                con.Close();
            }

            comand = string.Format("INSERT INTO TR (Request_ID, Ids, parent) VALUES ('{0}','{1}','{2}')", Request_ID, AppRejID, parent);

            con = new SqlConnection(Connection.connectionString);

            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();
            }
            catch
            {
                return "-2";
            }
            finally
            {
                con.Close();
            }

            string ss = "0";
            try
            {
                typeN = Request_type.GetTypeName(Request_ID);
                ss = "1";
                msg = string.Format("<b>{0}’s</b> {1} (ID: {2} {3}) is <b>submitted to you</b> for approval", autor.FullName, typeN, Request_type.GetUppers(typeN), Request_ID);

                ss = "2";
                Notificaion.Add(User_ID, SPContext.Current.Web.Url + "/SitePages/" + url + "?rid=" + Request_ID, msg, autor.PictureUrl, Request_type.GetId(Request_ID));
                ss = "3";
                EDF_SPUser rus = AD.GetUserBySPLogin(User_ID);
                ss = "4";
                if (rus.HasReplacement)
                {
                    ss = "5";
                    msg = string.Format("<b>{0}’s</b> {1} (ID: {2} {3}) is <b>submitted to {4}</b> for approval", autor.FullName, typeN, Request_type.GetUppers(typeN), Request_ID, rus.FullName); // YUPE IP
                    ss = "6";
                    Notificaion.Add(rus.Replacement.Login, SPContext.Current.Web.Url + "/SitePages/" + url + "?rid=" + Request_ID, msg, autor.PictureUrl, Request_type.GetId(Request_ID));
                }
            }
            catch { throw new Exception(ss); }

            return AppRejID;
        }

        public static bool CanAppRejLTR(string Request_ID, string User_ID)
        {

            string comand = string.Format("select ID from Approve_reject where Request_ID = '{0}' and User_ID = '{1}' union select Id from Request where Id = '{0}'  and Autor_Id = '{1}'", Request_ID, User_ID);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return true;
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - CanAppRej - "); }
            finally { con.Close(); }
            return false;
        }

        public static bool CanAppRejLTR_Null(string Request_ID, string User_ID)
        {
            string comand = string.Format("select ID, Status from Approve_reject where Request_ID = '{0}' and User_ID = '{1}' AND App_rej IS NULL", Request_ID, User_ID);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string status = reader["status"].ToString();

                        try
                        {
                            if (string.IsNullOrEmpty(status))
                            {
                                return true;
                            }
                            else
                            {

                                if (!EDF.GetGroupState(Request_ID, status))
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        catch
                        {
                            return true;
                        }
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - CanAppRej - "); }
            finally { con.Close(); }
            return false;
        }

        public static Int16 App_rej(string RequestId, string UserId)
        {
            Int16 App_rej = -1;
            string comand = string.Format("select App_rej from Approve_reject where (Request_ID = '{0}' and User_ID = '{1}')", RequestId, UserId);

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                SqlCommand com = new SqlCommand(comand, con);

                con.Open();

                SqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {

                    if (reader["App_rej"].ToString().ToLower() == "true")
                        App_rej = 1;
                    else if (reader["App_rej"].ToString().ToLower() == "false")
                        App_rej = 2;
                    else
                    {
                        return App_rej = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - App_rej -  </br> " + ex.Message);
            }
            finally { con.Close(); }
            return App_rej;
        }

        public static void UpdateParent(string User_id, string Request_Id, bool App_rej)
        {
            bool hasParent = false;
            string Par = string.Empty;

            EDF_SPUser Cur = AD.GetUserBySPLogin(User_id);

            if (Cur.ParentReplacement.Count > 0)
            {
                foreach (EDF_SPUser p in Cur.ParentReplacement)
                    if (CanAppRejLTR_Null(Request_Id, p.Login))
                    {
                        hasParent = true;
                        Par = p.Login;
                    }
            }
            if (hasParent)
            {
                Update(Par, Request_Id, App_rej, Cur.Login);
            }
            else
            {
                Update(User_id, Request_Id, App_rej);
            }
        }

        public static void Update(string User_id, string Request_Id, bool App_rej)
        {
            string comand = string.Format("UPDATE [dbo].[Approve_reject] SET [dbo].[Approve_reject].App_rej = '{0}', [dbo].[Approve_reject].App_rej_Date = '{1}'  where User_ID = '{2}' and Request_ID = '{3}' and App_rej is null", App_rej.ToString(), DateTime.Now, User_id, Request_Id);

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateApprove_reject -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static void Update(string User_id, string Request_Id, bool App_rej, string RepId)
        {
            string comand = string.Format("UPDATE [dbo].[Approve_reject] SET [dbo].[Approve_reject].App_rej = '{0}', [dbo].[Approve_reject].App_rej_Date = '{1}', [dbo].[Approve_reject].Rep_Id = '{4}'  where User_ID = '{2}' and Request_ID = '{3}' and App_rej is null", App_rej.ToString(), DateTime.Now, User_id, Request_Id, RepId);

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateApprove_reject2 -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // DAR

        public static void UpdateParent(DataTable IDS, string User_id, string Request_Id, bool App_rej)
        {
            bool hasParent = false;
            string Par = string.Empty;

            EDF_SPUser Cur = AD.GetUserBySPLogin(User_id);

            if (Cur.ParentReplacement.Count > 0)
            {
                foreach (EDF_SPUser p in Cur.ParentReplacement)
                    if (CanAppRejLTR(Request_Id, p.Login))
                    {
                        hasParent = true;
                        Par = p.Login;
                    }
            }
            if (hasParent)
            {
                Update(IDS, Par, User_id);
            }
            else
            {
                Update(IDS, User_id);
            }
        }

        public static void Update(DataTable IDS, string user_id)
        {
            string inCMD = string.Empty;
            foreach (DataRow idRow in IDS.Rows)
            {
                inCMD += idRow["Ids"].ToString() + ", ";
            }

            inCMD = inCMD.Trim().TrimEnd(',');


            string comand = string.Format("UPDATE [dbo].[Approve_reject] SET [dbo].[Approve_reject].App_rej = 'true', [dbo].[Approve_reject].App_rej_Date = '{0}'  where ID  IN ({1}) AND User_id='{2}'", DateTime.Now, inCMD, user_id);

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateApprove_reject -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static void Update(DataTable IDS, string RepId, string user_id)
        {
            string inCMD = string.Empty;
            foreach (DataRow idRow in IDS.Rows)
            {
                inCMD += idRow["Ids"].ToString() + ", ";
            }

            inCMD = inCMD.Trim().TrimEnd(',');


            string comand = string.Format("UPDATE [dbo].[Approve_reject] SET [dbo].[Approve_reject].App_rej = 'true', [dbo].[Approve_reject].App_rej_Date = '{0}', [dbo].[Approve_reject].Rep_Id = '{2}'  where ID  IN ({1}) AND User_id='{3}'", DateTime.Now, inCMD, RepId, user_id);

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateApprove_reject2 -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static bool InAppRej(string Request_ID, string User_ID)
        {
            string comand = string.Format("select ID from Approve_reject where Request_ID = '{0}' and User_ID = '{1}'", Request_ID, User_ID);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return true;
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - CanAppRej - "); }
            finally { con.Close(); }
            return false;
        }

        public static string GetStatus(string User_ID, string Request_ID)
        {
            string Name = string.Empty;

            string comand = string.Format("SELECT [Status] FROM Approve_reject WHERE User_ID = '{0}' AND Request_ID = '{1}' AND App_rej IS NULL", User_ID, Request_ID);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);


                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Name = reader["Status"].ToString();
                    }
                }
                else
                {
                    Name = string.Empty;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetStatus DB ERROR -  </br> " + ex.Message);
            }
            finally { con.Close(); }

            return Name;
        }

        public static string GetStatusColumn(string User_ID, string Request_ID)
        {
            string Name = string.Empty;

            string comand = string.Format("SELECT Status FROM Approve_reject WHERE User_ID = '{0}' AND Request_ID = '{1}' AND App_rej IS NOT NULL", User_ID, Request_ID);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);


                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Name = reader["Status"].ToString();
                    }
                }
                else
                {
                    Name = string.Empty;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetStatusColumn DB ERROR -  </br> " + ex.Message);
            }
            finally { con.Close(); }

            return Name;
        }

        public static DataTable GetGroupsStatus(string Request_ID)
        {
            string comand = string.Format("SELECT [Status] FROM [Approve_reject]  WHERE Request_ID = '{0}' AND Status <> 'Agiliti' GROUP BY Status ", Request_ID);

            DataTable table = new DataTable("Status");

            try
            {
                SqlConnection con = new SqlConnection(Connection.connectionString);

                SqlDataAdapter adapter = new SqlDataAdapter(comand, con);

                adapter.Fill(table);
            }
            catch { }

            return table;
        }

        public static void DeleteFreeRequests(string Request_ID)
        {
            string delstr = string.Format("delete from Approve_reject where Request_ID = '{0}' AND App_rej is null ", Request_ID);

            try
            {
                SqlConnection con = new SqlConnection(Connection.connectionString);
                SqlCommand delcom = new SqlCommand(delstr, con);
                con.Open();
                delcom.ExecuteNonQuery();
            }
            catch
            {

            }
        }
    }
    public static class Request_type
    {
        public static string GetTypeName(string Request_ID)
        {
            string Name = string.Empty;

            string comand = string.Format("SELECT Type_id,(SELECT Request_type.Name FROM Request_type WHERE Request.Type_id = Request_type.ID)as Name FROM Request where Request.Id = '{0}'", Request_ID);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);


                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Name = reader["Name"].ToString();
                    }
                }
                else
                {
                    Name = string.Empty;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetType_Name -  </br> " + ex.Message);
            }
            finally { con.Close(); }

            return Name;
        }

        public static int GetId(string Request_ID)
        {
            int Type_id = -1;

            string comand = "select Type_id from Request WHERE " +
                "Id = '" + Request_ID + "'";


            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);


                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Type_id = (int)reader["Type_id"];
                    }
                }
                else
                {
                    Type_id = -1;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetType_id -  </br> " + ex.Message);
            }
            finally { con.Close(); }

            return Type_id;
        }

        public static string GetUppers(string s)
        {
            string ss = string.Empty;
            foreach (string w in s.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                ss += w.Substring(0, 1).ToUpper();
            }
            return (ss);
        }
    }

    public static class Notificaion
    {
        public static void Add(string User_id, string Visit_url, string notification, string avatar, int Type_id)
        {
            string comand = "INSERT INTO [Notificaion] (User_id, Date_Add, visited, Visit_url, notification, avatar, Type_id) " +
                                         "VALUES ('" + User_id + "', " +
                                                 "'" + DateTime.Now + "', " +
                                                 "'False', " +
                                                 "'" + Visit_url + "', " +
                                                 "'" + notification + "', " +
                                                 "'" + avatar + "', " +
                                                 "'" + Type_id.ToString() + "')";

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - AddNotificaion -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
            sendMail("EDF - Notification", AD.GetUserByLogin(User_id).E_Mail, notification + "<p>URL: <a href='" + Visit_url + "'>" + Visit_url + "</a></p>");

            //////////// send Stock Out requests replacement 
            if (Type_id == 6 && AD.GetUserByLogin(User_id).HasReplacement)
            {
                string comand1 = "INSERT INTO [Notificaion] (User_id, Date_Add, visited, Visit_url, notification, avatar, Type_id) " +
                                        "VALUES ('" + AD.GetUserByLogin(User_id).Replacement.Login + "', " +
                                                "'" + DateTime.Now + "', " +
                                                "'False', " +
                                                "'" + Visit_url + "', " +
                                                "'" + notification + "', " +
                                                "'" + avatar + "', " +
                                                "'" + Type_id.ToString() + "')";

                SqlConnection con1 = new SqlConnection(Connection.connectionString);
                try
                {
                    SqlCommand command = new SqlCommand(comand, con);

                    con.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    ER.GoToErrorPage(" - AddNotificaion - Stck Replacement  </br> " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
                sendMail("EDF - Notification", AD.GetUserByLogin(User_id).Replacement.E_Mail, notification + "<p>URL: <a href='" + Visit_url + "'>" + Visit_url + "</a></p>");
            }
        }

        public static void Update(string NId, bool Is_ok)
        {
            string comand = string.Format("UPDATE [dbo].[Notificaion] SET [dbo].[Notificaion].visited = '{0}' where Id = '{1}'", Is_ok.ToString(), NId);

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateNotificaion -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static void sendMail(string subject, string To, string msg)
        {
            try
            {
                if (AD.Domain.Contains("pele"))
                    To = "Narek.Arzumanyan@ucom.am";

                string fromEmail = Constants.EmailFrom;
                string smtp = Constants.EmailHost;

                SmtpClient mySmtpClient = new SmtpClient(smtp);


                MailAddress from = new MailAddress(fromEmail, fromEmail);
                MailAddress to = new MailAddress(To, To);
                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);



                myMail.Subject = string.Format(subject);
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                myMail.Body = msg;
                myMail.BodyEncoding = System.Text.Encoding.UTF8;
                // text or html
                myMail.IsBodyHtml = true;

                mySmtpClient.Send(myMail);
            }
            catch (Exception ex)
            {
                //throw new Exception(" - Could not send the e-mail - error: -  </br> " + ex.Message);
                //Response.Write("- Could not send the e-mail - error: -");
                //ER.GoToErrorPage(" - Could not send the e-mail - error: -  </br> " + ex.Message);
            }
        }
    }

    public static class Request_Substitute
    {
        public static Int16 Is_ok(string RequestId, string UserId)
        {
            Int16 Is_ok = -1;
            string comand = string.Format("select Is_ok from Request_Substitute where REQUEST_ID = '{0}' and User_id = '{1}'", RequestId, UserId);

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                SqlCommand com = new SqlCommand(comand, con);

                con.Open();

                SqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {

                    if (reader["Is_ok"].ToString().ToLower() == "true")
                        Is_ok = 1;
                    else
                        if (reader["Is_ok"].ToString().ToLower() == "false")
                            Is_ok = 2;
                        else
                        {
                            Is_ok = 0;
                        }
                }
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - IsActive -  </br> " + ex.Message);
            }
            finally { con.Close(); }
            return Is_ok;
        }

        public static string Get_User_id_NULL(string REQUEST_ID, string For_user_id)
        {
            string Users_ids = string.Empty;

            string comand = string.Format("select User_id from Request_Substitute where " +
                "REQUEST_ID='{0}' AND For_user_id = '{1}' AND Is_ok IS NULL", REQUEST_ID, For_user_id);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                SqlCommand com = new SqlCommand(comand, con);

                con.Open();

                SqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    Users_ids += reader["User_id"] + ",";
                }
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetRequest_Substitute_User_id -  </br> " + ex.Message);
            }
            finally { con.Close(); }
            return Users_ids;
        }

        public static string Get_User_ids(string REQUEST_ID, string For_user_id)
        {
            string Users_ids = string.Empty;

            string comand = string.Format("select User_id from Request_Substitute where " +
                "REQUEST_ID='{0}' AND For_user_id = '{1}'", REQUEST_ID, For_user_id);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                SqlCommand com = new SqlCommand(comand, con);

                con.Open();

                SqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    Users_ids += reader["User_id"] + ",";
                }
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetRequest_Substitute_User_id -  </br> " + ex.Message);
            }
            finally { con.Close(); }
            return Users_ids;
        }

        public static bool AppRejFromAllSub(string Request_Id)
        {
            string comand = string.Format("select ID from Request_Substitute where REQUEST_ID = '{0}' and Is_ok IS NULL", Request_Id);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - ApprovedFromAllSubstitute -  </br> " + ex.Message);
            }
            finally { con.Close(); }
            return true;
        }

        public static void Update(string User_id, string Request_Id, bool Is_ok)
        {
            string comand = string.Format("UPDATE [dbo].[Request_Substitute]  SET [dbo].[Request_Substitute].Is_ok = '{0}', [dbo].[Request_Substitute].Is_ok_date = '{3}' where User_id = '{1}' and REQUEST_ID = '{2}'", Is_ok, User_id, Request_Id, DateTime.Now);

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateRequest_Substitute -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static List<string> DelateByRId(string RequestId, string Names)
        {
            List<string> users = new List<string>();
            foreach (string s in Names.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                users.Add(s.Split('\\')[1]);
            }
            string selstr = string.Format("select * from Request_Substitute where REQUEST_ID = '{0}'", RequestId);
            string delstr;
            string countstr;
            SqlConnection con = new SqlConnection(Connection.connectionString);
            SqlCommand selcom = new SqlCommand(selstr, con);
            SqlCommand countcom;

            delstr = string.Format("delete from Request_Substitute where REQUEST_ID = '{0}' ", RequestId);
            foreach (string u in users)
            {
                delstr += string.Format("and User_id <> '{0}'", u);
            }

            SqlCommand delcom = new SqlCommand(delstr, con);
            List<string> NewUsers = new List<string>();

            try
            {
                con.Open();
                delcom.ExecuteNonQuery();

                foreach (string u in users)
                {
                    countstr = string.Format("select count(*) from Request_Substitute where REQUEST_ID = '{0}' and User_id = '{1}'", RequestId, u);
                    countcom = new SqlCommand(countstr, con);

                    int k = int.Parse(countcom.ExecuteScalar().ToString());

                    if (k == 0)
                        NewUsers.Add(u);
                }
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - DelFromReqSubByRId -  </br> " + ex.Message);
            }
            finally { con.Close(); }
            return NewUsers;
        }

        public static void Add(string User_id, string REQUEST_ID, string For_user_id, DateTime End_date, DateTime Start_date)
        {
            string comand = string.Format("INSERT INTO [Request_Substitute] (User_id, REQUEST_ID, For_user_id, End_date, Start_date) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                User_id,
                REQUEST_ID,
                For_user_id,
                End_date,
                Start_date
                );

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - Request_Substitute2 -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static void Add2(string User_id, string REQUEST_ID, string For_user_id, DateTime End_date, DateTime Start_date, bool Is_ok)
        {
            string comand = string.Format("INSERT INTO [Request_Substitute] (User_id, REQUEST_ID, For_user_id, End_date, Start_date, Is_ok, Is_ok_date) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')",
                User_id,
                REQUEST_ID,
                For_user_id,
                End_date,
                Start_date,
                Is_ok.ToString(),
                DateTime.Now
                );

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - Request_Substitute2 -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static string Get_User_Names(string REQUEST_ID, string For_user_id)
        {
            string Users_ids = string.Empty;

            string comand = string.Format("select User_id from Request_Substitute where " +
                "REQUEST_ID='{0}' AND For_user_id = '{1}'", REQUEST_ID, For_user_id);


            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                SqlCommand com = new SqlCommand(comand, con);

                con.Open();

                SqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    Users_ids += "<p>" + AD.GetUserByLogin(reader["User_id"].ToString()).FullName + "</p>";
                }
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetRequest_Substitute_User_id -  </br> " + ex.Message);
            }
            finally { con.Close(); }
            return Users_ids;
        }

    }

    public static class Comments
    {
        public static DataTable Get(string Request_ID)
        {
            string comand = "select * from Comments where Request_ID='" + Request_ID + "'";

            DataTable table = new DataTable("Comments");

            try
            {
                SqlConnection con = new SqlConnection(Connection.connectionString);

                SqlDataAdapter adapter = new SqlDataAdapter(comand, con);

                adapter.Fill(table);
            }
            catch { }

            return table;
        }

        public static DataTable Get2(string Request_ID)
        {
            string comand = "select * from ComentDAR where Request_ID='" + Request_ID + "'";

            DataTable table = new DataTable("ComentDAR");

            try
            {
                SqlConnection con = new SqlConnection(Connection.connectionString);

                SqlDataAdapter adapter = new SqlDataAdapter(comand, con);

                adapter.Fill(table);
            }
            catch { }

            return table;
        }

        public static void Add(string Request_ID, string MSG, string User_ID, bool see)
        {
            string comand = string.Format("INSERT INTO Comments (Request_ID, MSG, Add_date, User_ID, see) VALUES ('{0}', N'{1}', '{2}','{3}','{4}')", Request_ID, MSG.Replace("'", "''"), DateTime.Now, User_ID, see.ToString());

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - AddComments -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static void Add(string Request_ID, string MSG, string AutorStatus, string type, string User_ID)
        {
            MSG = MSG == "Write your comment:" ? "No Comment" : MSG;

            string comand = string.Format("INSERT INTO ComentDAR (Request_ID, MSG, Add_date, AutorStatus, type, User_ID) VALUES ('{0}', N'{1}', '{2}','{3}','{4}','{5}')", Request_ID, MSG, DateTime.Now, AutorStatus, type, User_ID);

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - AddDARComments -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static bool CanAddComment(string Request_ID, string AutorStatus)
        {

            string comand = string.Format("select ID from ComentDAR where Request_ID = '{0}' and AutorStatus = '{1}'", Request_ID, AutorStatus);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return false;
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - CanAddComment - "); }
            finally { con.Close(); }
            return true;
        }
    }

    public static class RSR
    {
        public static void Update(string Request_Id)
        {
            string comand = string.Format("IF (SELECT MAX(Number) FROM [RSR]) IS NOT NULL " +
                                            " BEGIN " +
                                                " UPDATE [RSR] SET Number=((SELECT MAX(Number) FROM [RSR]) + 1) WHERE Request_Id = '{0}' " +
                                            "END " +
                                            "ELSE " +
                                                " UPDATE [RSR] SET Number='1' WHERE Request_Id = '{0}'", Request_Id);

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateRSR -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static string GetNumber(string RequestId)
        {
            string Number = "0";

            string comand = string.Format("select Number from [RSR] WHERE Request_Id = '{0}'", RequestId);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Number = reader["Number"].ToString();
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - Get RSR Number - "); }
            finally { con.Close(); }
            return Number;
        }

        public static void Update(string Request_Id, bool access_pending)
        {
            string comand = string.Format("UPDATE [RSR] SET access_pending = '{0}' WHERE Request_Id = '{1}' ", access_pending.ToString(), Request_Id);

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateRSR access_pending -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
    }

    public static class DAR
    {
        public static string GetNumber(string RequestId)
        {
            string Number = "0";

            string comand = string.Format("select Number from DAR WHERE Request_Id = '{0}'", RequestId);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Number = reader["Number"].ToString();
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - Get DAR Number - " + "Can not Get Database Access Request Number from database: "); }
            finally { con.Close(); }
            return Number;
        }

        public static bool CanAppRej(string Request_ID, string User_ID)
        {
            string comand = string.Format("select ID from Approve_reject where Request_ID = '{0}' and User_ID = '{1}' and App_rej is null", Request_ID, User_ID);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return true;
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - DAR.CanAppRej - "); }
            finally { con.Close(); }
            return false;
        }

        public static string GetAutorComent(string RequestId, string Status)
        {
            string Number = string.Empty;

            string comand = string.Format("select [User_ID] from [ComentDAR] WHERE Request_ID = '{0}' and [type] = '{1}'", RequestId, Status);

            SqlConnection con = new SqlConnection(Connection.connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return reader["User_ID"].ToString();
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - Get RSR Number - "); }
            finally { con.Close(); }
            return Number;
        }

        public static void Update(string Request_Id)
        {
            string comand = string.Format("UPDATE [DAR] SET Number=((SELECT MAX(Number) FROM [DAR]) + 1) WHERE Request_Id = '{0}' ", Request_Id);

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - DAR.Update -  </br> " + "Can not Change DAR document in database: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static void Update(string Request_Id, string EXEC)
        {
            string comand = string.Format("UPDATE [DAR] SET [EXEC]='{1}' WHERE Request_Id = '{0}'", Request_Id, EXEC);

            SqlConnection con = new SqlConnection(Connection.connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - DAR.Update -  </br> " + "Can not Change DAR document in database: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

    }

    public static class TR
    {
        public static DataTable getParent(string Request_ID, string User_ID)
        {
            string comand = string.Format("SELECT (select top 1 [parent] from [tr] where ids=[Approve_reject].Id) as parent FROM [Approve_reject] WHERE Request_ID = '{0}' AND User_ID = '{1}'", Request_ID, User_ID);

            DataTable table = new DataTable("ID");

            try
            {
                SqlConnection con = new SqlConnection(Connection.connectionString);

                SqlDataAdapter adapter = new SqlDataAdapter(comand, con);

                adapter.Fill(table);
            }
            catch { }

            return table;
        }

        public static DataTable getIds(string Request_ID, string parent)
        {
            string comand = string.Format("select [Ids] from [tr] where Request_ID = '{0}' AND parent = '{1}'", Request_ID, parent);

            DataTable table = new DataTable("ID");

            try
            {
                SqlConnection con = new SqlConnection(Connection.connectionString);

                SqlDataAdapter adapter = new SqlDataAdapter(comand, con);

                adapter.Fill(table);
            }
            catch { }

            return table;
        }
    }

    public static class PDF_DOC
    {
        public static string Create(string html, string type, string rid)
        {
            string str_tmp = ":1:";
            string path = string.Empty;
            try
            {
                //MemoryStream msOutput = new MemoryStream();
                TextReader reader = new StringReader(html);// step 1: creation of a document-object
                str_tmp += ":1:";
                Document document = new Document(PageSize.A4, 76, 30, 30, 30);
                str_tmp += ":1:";

                // step 2:
                // we create a writer that listens to the document
                // and directs a XML-stream to a file

                path = HttpContext.Current.Request.PhysicalApplicationPath + "\\All_Requests\\" + type + "_" + rid + ".pdf";
                str_tmp += ":1:";
                if (File.Exists(path))
                {
                    str_tmp += ":2:";
                    //File.Delete(path);
                    return path;
                }
                str_tmp += ":1:";
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    str_tmp += ":2:";
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(path, FileMode.Create));
                    str_tmp += ":2:";
                });

                str_tmp += ":1:";
                // step 3: we create a worker parse the document
                HTMLWorker worker = new HTMLWorker(document);
                str_tmp += ":1:";
                // step 4: we open document and start the worker on the document
                document.Open();
                str_tmp += ":1:";
                ////////////
                iTextSharp.text.Image pdfImage = iTextSharp.text.Image.GetInstance(SPContext.Current.Web.Url + "/_catalogs/masterpage/images/docLogo.png");
                str_tmp += ":1:";
                pdfImage.ScaleToFit(66, 62);
                str_tmp += ":1:";
                pdfImage.Alignment = iTextSharp.text.Image.UNDERLYING; pdfImage.SetAbsolutePosition(488, 750);
                str_tmp += ":1:";
                document.Add(pdfImage);
                str_tmp += ":1:";               
                FontFactory.Register("C:\\Windows\\Fonts\\UH.TTF", "arial unicode ms");
                str_tmp += ":1:";
                iTextSharp.text.html.simpleparser.StyleSheet ST = new iTextSharp.text.html.simpleparser.StyleSheet();
                str_tmp += ":1:";
                ST.LoadTagStyle("body", "encoding", "Identity-H");
                str_tmp += ":1:";

                // step 4.3: assign the style sheet to the html parser
                worker.SetStyleSheet(ST);
                str_tmp += ":1:";
                worker.StartDocument();
                str_tmp += ":1:";
                // step 5: parse the html into the document
                worker.Parse(reader);
                str_tmp += ":1:";
                // step 6: close the document and the worker
                worker.EndDocument();
                str_tmp += ":1:";
                worker.Close();
                str_tmp += ":1:";
                document.Close();
                str_tmp += ":1:";

            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(ex.Message + ":" + str_tmp);
            }
            return path;
        }
    }
}