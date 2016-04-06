using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using EDF_CommonData;
using Microsoft.SharePoint;

namespace EDF_WEB_Parts_1.Statistics
{
    public partial class StatisticsUserControl : UserControl
    {
        string connectionString = Constants.GetConnectionString();

        EDF_SPUser current = ADSP.CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(current.StatisticsAccess || current.SoStatisticsAccess))
            {
                Response.Redirect(SPContext.Current.Web.Url);
            }

            if (!IsPostBack)
            {
                setCheckBoxes();
                DrowList();
            }

            ScriptManager.RegisterStartupScript(
                        UpdatePanel1,
                        this.GetType(),
                        "MyAction",
                        "picker();",
                        true);
        }


        protected void date_Click(object sender, EventArgs e)
        {
            hf_type.Value = "d";
            SetClass(sender);
            DrowList();
        }
        protected void status_Click(object sender, EventArgs e)
        {
            hf_type.Value = "s";
            SetClass(sender);
            DrowList();
        }
        protected void type_Click(object sender, EventArgs e)
        {
            hf_type.Value = "t";
            SetClass(sender);
            DrowList();
        }
        protected void BtnLoad_Click(object sender, EventArgs e)
        {
            int count = int.Parse(hf_start.Value);

            hf_start.Value = (count + 5).ToString();
            if (string.IsNullOrEmpty(hf_asc.Value))
                hf_asc.Value = "DESC";

            DrowList();
        }
        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            DrowList();
        }
        public void DrowList()
        {
            int count = int.Parse(hf_start.Value);
            string sortType = hf_type.Value;
            string orderBy = "order by ";
            orderBy += sortType == "s" ? "[State] " + hf_asc.Value : "" +
                       sortType == "t" ? "[Type_id] " + hf_asc.Value : "" +
                       sortType == "d" ? "[add_date] " + hf_asc.Value : "DESC";

            DateTime Start_date;
            DateTime End_date;

            bool d1 = DateTime.TryParse(sdate.Value, out Start_date);
            bool d2 = DateTime.TryParse(enddate.Value, out End_date);

            Start_date = d1 ? Start_date : DateTime.Parse("1/1/1900");
            End_date = d2 ? End_date : DateTime.Parse("1/1/9000");

            List<string> Tlist = new List<string>();

            List<string> keywordList = new List<string>();

            keywordList = getKeywordList();

            if (T1.Checked) Tlist.Add("1");
            if (T2.Checked) Tlist.Add("2");
            if (T3.Checked) Tlist.Add("3");
            if (T4.Checked) Tlist.Add("4");
            if (T5.Checked) Tlist.Add("5");
            if (T6.Checked) Tlist.Add("6");

            DataTable tbPend = GetStatistics(1, count, Start_date, End_date, orderBy, keywordList, Tlist,true);
            DataTable tbPast = GetStatistics(1, count, Start_date, End_date, orderBy, keywordList, Tlist, false);

            ulAppRej.InnerHtml = string.Empty;
            ulPend.InnerHtml = string.Empty;
            DrawList(tbPend);
            DrawList(tbPast);
        }

        public DataTable GetStatistics(int start, int count, DateTime startDate, DateTime endDate, string orderBy, List<string> keywords, List<string> type,bool isPend)
        {
            string typeQuery = string.Empty;
            if (type.Count > 0)
            {
                typeQuery = " and (";
                foreach (string t in type)
                {
                    typeQuery += string.Format(" Type_id = '{0}' or", t);
                }
                typeQuery = typeQuery.Substring(0, typeQuery.Length - 2);
                typeQuery += " ) ";
            }
            else
            {
                typeQuery = "and ( '2' = '1' ) ";
            }

            DataTable table = null;
            string comand = string.Empty;
            if (keywords.Count > 0)
            {
                comand = string.Format(@"

                                            DECLARE @data Table(
                                                id int not NULL,
                                                Type_id int not NULL,
                                                Autor_id nvarchar(50) not NULL,
                                                State bit Null,
                                                Add_date datetime not NULL,
                                                TypeName nvarchar(100) not NULL,
                                                TypeImgUrlOrange nvarchar(100) not NULL
                                                );



                                            DECLARE @requests_id Table(
                                                id int not NULL,
                                                Type_id int not NULL,
                                                Autor_id nvarchar(50) not NULL,
                                                State bit Null,
                                                Add_date datetime not NULL,
                                                TypeName nvarchar(100) not NULL,
                                                TypeImgUrlOrange nvarchar(100) not NULL
                                                );


                                            INSERT INTO @data
                                            SELECT 
	                                            Request.Id, 
	                                            Request.Type_id, 
	                                            Request.Autor_id, 
	                                            Request.State , 
	                                            Request.Add_date,
	                                            (Select Request_type.Name FROM Request_type WHERE Request_type.ID = Request.Type_id) as TypeName,
	                                            (Select Request_type.ImgUrlOrange FROM Request_type WHERE Request_type.ID = Request.Type_id) as TypeImgUrlOrange
                                            FROM Request
                                            WHERE   ([Add_date] between convert(datetime,'{0}',101) and convert(datetime,'{1}',101)) {2} {3} ", startDate.ToShortDateString(), //5
                                                           endDate.ToShortDateString(), //6
                                                           typeQuery, //7
                                                           (isPend ? " and State is null " : "and State is not null")
                                                           );
                //  vacation
                #region dict

                //   VAC TYPE
                Dictionary<string, string> Vac_Type = new Dictionary<string, string>
            {
                //vac type
                {"Annual","Annual / Ամենամյա"},

                {"Pregnancy and maternity","Pregnancy and maternity / Հղիություն և ծննդաբերություն"},
                {"Non-paid","Non-paid / Մինչև 3 տարեկան երեխայի խնամքի համար տրամադրվող - չվճարվող"},
                {"Exam (non-paid)","Exam (non-paid) / Քննություն - չվճարվող"},

                {"Marriage (non-paid)","Marriage (non-paid) / Ամուսնություն - չվճարվող"},
                {"Close relative’s death","Close relative’s death (husband, wife, mother, father, child,brother, sister) (paid) Մոտ բարեկամի մահ` ամուսին,կին,մայր,հայր,երեխա,քույր, եղբայր - վճարվող"},
                {"Relative’s death (non-paid)","Relative’s death (non-paid) / Բարեկամի մահ`տատ,պապ - չվճարվող"},
                {"Non-paid vacation","Non-paid vacation / չվճարվող"}       
         
                //  requestor
            };

                string vt = string.Empty;
                foreach (string tmp_str in keywords)
                {


                    vt = string.Format("( ([Vacation_type] like '%{0}%' ", tmp_str);
                    foreach (var s in Vac_Type)
                    {
                        if (s.Value.ToLower().Contains(keywords[0].ToLower()) || s.Key.ToLower().Contains(keywords[0].ToLower()))
                        {
                            vt += string.Format(" or [Vacation_type] like '%{0}%'", s.Key);
                        }
                    }

                    vt += ")";

                #endregion


                    vt += string.Format(@" OR vacation.number  LIKE '%{0}%' OR 
	                    vacation.Days_offs LIKE '%{0}%' OR 
	                    vacation.End_date LIKE '%{0}%' OR 
	                    vacation.Filling_date LIKE '%{0}%' OR 
	                    vacation.[Start_date] LIKE '%{0}%' OR 
	                    vacation.Work_days LIKE '%{0}%') AND", tmp_str);
                }

                vt = vt.TrimEnd("AND".ToCharArray());

                comand += string.Format(@"INSERT INTO @requests_id
                        SELECT REQ.Id,REQ.Type_id,REQ.Autor_id,REQ.State,req.Add_date,REQ.TypeName,REQ.TypeImgUrlOrange
                            FROM [dbo].vacation
                            INNER JOIN @data REQ
	                        ON REQ.Id = vacation.Request_ID
	                        WHERE {0} UNION ", vt);

                //request id autor id

                comand += string.Format(@"SELECT REQ.Id,REQ.Type_id,REQ.Autor_id,REQ.State,req.Add_date,REQ.TypeName,REQ.TypeImgUrlOrange
                            FROM @data as REQ
                    WHERE ");

                foreach (string tmp_str in keywords)
                {

                    List<string> tmp_list = tmp_str.Split(' ').ToList();

                    comand += "((";

                    foreach (string tmp_tmp_str in tmp_list)
                    {
                        comand += string.Format(@" REQ.Autor_id LIKE '%{0}%' AND", tmp_tmp_str);
                    }

                    comand = comand.TrimEnd("AND".ToCharArray());

                    comand += string.Format(@") OR REQ.Id LIKE '%{0}%') AND", tmp_str);

                }

                comand = comand.TrimEnd("AND".ToCharArray());
                comand += " UNION ";

                // rsr

                comand += string.Format(@"SELECT REQ.Id,REQ.Type_id,REQ.Autor_id,REQ.State,req.Add_date,REQ.TypeName,REQ.TypeImgUrlOrange
                          FROM [dbo].RSR rsr
                          INNER JOIN @data REQ
	                        ON REQ.Id = rsr.Request_ID
	                        WHERE ");

                foreach (string tmp_str in keywords)
                {
                    comand += string.Format(@" (rsr.Cboss LIKE '%{0}%' OR
	                        rsr.Department_And_Position LIKE '%{0}%' OR
	                        rsr.Father LIKE '%{0}%' OR
	                        rsr.Filling_Date LIKE '%{0}%' OR
	                        rsr.Last_Work_Day LIKE '%{0}%' OR
	                        rsr.Phone LIKE '%{0}%' OR
	                        rsr.Private_Phone LIKE '%{0}%' OR
	                        rsr.keep_accesses_date LIKE '%{0}%') AND", tmp_str);
                }

                comand = comand.TrimEnd("AND".ToCharArray());
                comand += " UNION ";
                //SOR
                comand += string.Format(@"SELECT REQ.Id,REQ.Type_id,REQ.Autor_id,REQ.State,req.Add_date,REQ.TypeName,REQ.TypeImgUrlOrange
                          FROM [dbo].StockOutRequest sor 
                          INNER JOIN @data REQ
	                        ON REQ.Id = sor.RequestId
	                        WHERE ");

                foreach (string tmp_str in keywords)
                {
                    comand += string.Format(@" (sor.BudgetedAccount LIKE '%{0}%' OR 
	                                  sor.Department LIKE '%{0}%' OR 
	                                  sor.DueDate LIKE '%{0}%' OR 
	                                  sor.FillingDate LIKE '%{0}%' OR 
	                                  sor.OrderNumber LIKE '%{0}%' OR 
	                                  sor.Position LIKE '%{0}%' OR 
	                                  sor.Purpose LIKE '%{0}%' OR 
                                      sor.OtherPurpose LIKE '%{0}%' OR 
                                      sor.Comments LIKE '%{0}%' OR 
	                                  sor.RequestId LIKE '%{0}%' OR 
	                                  sor.RequestType LIKE '%{0}%' OR 
	                                  sor.RequestorName LIKE '%{0}%') AND", tmp_str);
                }

                comand = comand.TrimEnd("AND".ToCharArray());
                comand += " UNION ";


                // dar
                comand += string.Format(@"SELECT REQ.Id,REQ.Type_id,REQ.Autor_id,REQ.State,req.Add_date,REQ.TypeName,REQ.TypeImgUrlOrange
                          FROM [dbo].DAR dar
                          INNER JOIN @data REQ
	                        ON REQ.Id = dar.Request_ID
	                    WHERE ");

                foreach (string tmp_str in keywords)
                {
                    comand += string.Format(@" (dar.Country LIKE '%{0}%' OR
	                    dar.Department_And_Position LIKE '%{0}%' OR
	                    dar.Filling_Date LIKE '%{0}%' OR
	                    dar.Requestor LIKE '%{0}%' OR
	                    dar.Equipment LIKE '%{0}%' OR
	                    dar.Email LIKE '%{0}%' OR
	                    dar.Internet_Access LIKE '%{0}%' OR
	                    dar.Workstation_MAC_Address LIKE '%{0}%' OR
	                    dar.[Description] LIKE '%{0}%' OR
	                    dar.Access_Period_Start LIKE '%{0}%' OR
	                    dar.Access_Period_End LIKE '%{0}%' OR
	                    dar.Beneficiary LIKE '%{0}%' OR
	                    dar.[EXEC] LIKE '%{0}%' OR
	                    dar.Name LIKE '%{0}%' OR
	                    dar.Department LIKE '%{0}%' OR
	                    dar.Position LIKE '%{0}%' OR
	                    dar.Name2 LIKE '%{0}%' OR
	                    dar.Organization LIKE '%{0}%' OR
	                    dar.Ass_Dep LIKE '%{0}%' OR
	                    dar.Team LIKE '%{0}%' ) AND", tmp_str);
                }

                comand = comand.TrimEnd("AND".ToCharArray());
                comand += " UNION ";

                // LTR

                comand += string.Format(@"SELECT REQ.Id,REQ.Type_id,REQ.Autor_id,REQ.State,req.Add_date,REQ.TypeName,REQ.TypeImgUrlOrange
                              FROM [dbo].LTR ltr
                              INNER JOIN @data REQ
	                            ON REQ.Id = ltr.Request_ID
	                            WHERE ");


                foreach (string tmp_str in keywords)
                {
                    comand += string.Format(@" (ltr.Number LIKE '%{0}%' OR
	                            ltr.Filling_Date LIKE '%{0}%' OR
	                            ltr.Department_And_Position LIKE '%{0}%' OR
	                            ltr.City LIKE '%{0}%' OR
	                            ltr.[Start_Date] LIKE '%{0}%' OR
	                            ltr.[End_Date] LIKE '%{0}%' OR
	                            ltr.Purpose LIKE '%{0}%' OR
	                            ltr.Car_Time LIKE '%{0}%' OR
	                            ltr.Hotel LIKE '%{0}%') AND", tmp_str);
                }

                comand = comand.TrimEnd("AND".ToCharArray());
                comand += " UNION ";

                // ITO
                comand += string.Format(@"SELECT REQ.Id,REQ.Type_id,REQ.Autor_id,REQ.State,req.Add_date,REQ.TypeName,REQ.TypeImgUrlOrange
                              FROM [dbo].ITO ito
                              INNER JOIN @data REQ
	                            ON REQ.Id = ito.Request_ID
	                            WHERE ");

                foreach (string tmp_str in keywords)
                {
                    comand += string.Format(@" (ito.Number LIKE '%{0}%' OR
	                            ito.Filling_Date LIKE '%{0}%' OR
	                            ito.Department_And_Position LIKE '%{0}%' OR
	                            ito.City LIKE '%{0}%' OR
	                            ito.Organization LIKE '%{0}%' OR
	                            ito.Start_Date LIKE '%{0}%' OR
	                            ito.End_Date LIKE '%{0}%' OR
	                            ito.Purpose LIKE '%{0}%' OR
	                            ito.Amount LIKE '%{0}%' OR
	                            ito.Replacement_Id LIKE '%{0}%' OR
	                            ito.Daily LIKE '%{0}%' OR
	                            ito.Hotel_Dates LIKE '%{0}%' OR
	                            ito.Hotel_Location LIKE '%{0}%' OR
	                            ito.Hotel_Phone LIKE '%{0}%' OR
	                            ito.Hotel_Payment LIKE '%{0}%' OR
	                            ito.Fly_Date LIKE '%{0}%' OR
	                            ito.Fly_Airline LIKE '%{0}%' OR
	                            ito.Fly_Number LIKE '%{0}%' OR
	                            ito.Fly_Departure_City LIKE '%{0}%' OR
	                            ito.Fly_Destination_City LIKE '%{0}%' ) AND", tmp_str);
                }
                comand = comand.TrimEnd("AND".ToCharArray());

                comand += string.Format(@" select * from ( 
               SELECT REQ.Id,REQ.Type_id,REQ.Autor_id,REQ.State,req.Add_date,REQ.TypeName,REQ.TypeImgUrlOrange,
               ROW_NUMBER() OVER ({0}) as RowNum 
                FROM @requests_id REQ
               ) AS MyDerivedTable WHERE MyDerivedTable.RowNum BETWEEN {1} AND {2}",

                               orderBy, //0
                               start.ToString(), //1
                               (start + count).ToString() //2  
                               );

                table = new DataTable("Statistics");

                
            }
            else
            {
                comand = string.Format("select * from ( " +
               "SELECT Request.Id, Request.Type_id, Request.Autor_id, Request.State , Request.Add_date , " +
               "ROW_NUMBER() OVER ({0}) as RowNum, " +
                   "(Select Request_type.Name FROM Request_type WHERE Request_type.ID = Request.Type_id) as TypeName, " +
                   "(Select Request_type.ImgUrlOrange FROM Request_type WHERE Request_type.ID = Request.Type_id) as TypeImgUrlOrange, " +
                   "(Select Request_type.ImgUrlDark FROM Request_type WHERE Request_type.ID = Request.Type_id) as TypeImgUrlDark " +

                   "FROM Request " +
                "WHERE {6} {5}" +

                "and ([Add_date] between convert(datetime,'{3}',101) and convert(datetime,'{4}',101)) " +
               ") AS MyDerivedTable WHERE MyDerivedTable.RowNum BETWEEN {1} AND {2}",
               orderBy, //0
               start.ToString(), //1
               (start + count).ToString(), //2
               startDate.ToShortDateString(), //3
               endDate.ToShortDateString(), //4
               typeQuery, //5
               (isPend ? " State is null " : "State is not null") //6
               );

                table = new DataTable("Statistics");
            }

            try
            {
                SqlConnection con = new SqlConnection(connectionString);

                SqlDataAdapter adapter = new SqlDataAdapter(comand, con);

                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetStatistics -  </br> " + ex.Message);
            }

            return table;
        }

        public void DrawList(DataTable table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                string autor_id = table.Rows[i]["Autor_id"].ToString();
                EDF_SPUser autor = AD.GetUserBySPLogin(autor_id);
                string avatat = autor.PictureUrl;
                string UserName = autor.FullName;
                string Dep = autor.Department;
                string t1 = table.Rows[i]["TypeName"].ToString();
                string t2 = table.Rows[i]["TypeImgUrlOrange"].ToString();
                string t4 = ((DateTime)table.Rows[i]["Add_date"]).ToString("dd/MM/yyyy");
                string t5 = GetUppers(t1) + " " + table.Rows[i]["Id"].ToString();
                string t6 = table.Rows[i]["State"].ToString();
                string t7 = table.Rows[i]["Type_id"].ToString();

                string l = string.Empty;
                switch (t7)
                {
                    case "1":
                        l = "ViewReplacement";
                        break;
                    case "2":
                        l = "ViewLTR";
                        break;
                    case "3":
                        l = "ViewITO";
                        break;
                    case "4":
                        l = "ViewRSR";
                        break;
                    case "5":
                        l = "ViewDAR";
                        break;
                    case "6":
                        l = "InternalStockOutRequestView";
                        break;
                }
                string link = SPContext.Current.Web.Url + "/SitePages/" + l + ".aspx?rid=" + table.Rows[i]["Id"].ToString();
                if (t6 != "")
                {
                    ulAppRej.InnerHtml += string.Format("<a href='{0}'><li class='hitem cont color clearfix'  style='cursor:pointer;' ><div style='background:url({1}) no-repeat 28px;' class='img_logo fleft'><img class='avatar' src='{2}' /></div><p class='text4 fleft'><span style='font-size:17px'><b>{3}</b> by <b>{4}</b><br/> from {5}<br />ID:  {6}<br /><h9>{7}</h9></span></p><div class='timer fright'><img src='/_catalogs/masterpage/images/{8}.png' /></div></li></a>", link, t2, avatat, t1, UserName, Dep, t5, t4, (t6 == "True" ? "ok" : "x"));
                }
                else
                {
                    ulPend.InnerHtml += string.Format("<a href='{0}'><li class='hitem cont clearfix'  style='cursor:pointer;' ><div style='background:url({1}) no-repeat 28px;' class='img_logo my_img_logo fleft'><img class='avatar' src='{2}' /></div><p class='text4 fleft'><span style='font-size:17px'><b>{3}</b> by <b>{4}</b><br/> from {5}<br />ID:  {6}<br /><h9>{7}</h9></span></p><div class='timer fright'><img src='/_catalogs/masterpage/images/timer.png' /></div></li></a>", link, t2, avatat, t1, UserName, Dep, t5, t4);
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        void SetClass(object sender)
        {
            LinkButton btn = sender as LinkButton;

            switch (btn.ID)
            {
                case "status":
                    st_type.Attributes["class"] = "step1 fright";
                    st_date.Attributes["class"] = "step1 fright";
                    if (st_status.Attributes["class"].Contains("step1"))
                    {
                        st_status.Attributes["class"] = "step2 fright";
                        hf_asc.Value = "asc";
                    }
                    else
                    {
                        st_status.Attributes["class"] = "step1 fright";
                        hf_asc.Value = "desc";
                    }
                    break;
                case "type":
                    st_date.Attributes["class"] = "step1 fright";
                    st_status.Attributes["class"] = "step1 fright";
                    if (st_type.Attributes["class"].Contains("step1"))
                    {
                        st_type.Attributes["class"] = "step2 fright";
                        hf_asc.Value = "asc";
                    }
                    else
                    {
                        st_type.Attributes["class"] = "step1 fright";
                        hf_asc.Value = "desc";
                    }
                    break;
                case "date":
                    st_status.Attributes["class"] = "step1 fright";
                    st_type.Attributes["class"] = "step1 fright";
                    if (st_date.Attributes["class"].Contains("step1"))
                    {
                        st_date.Attributes["class"] = "step2 fright";
                        hf_asc.Value = "asc";
                    }
                    else
                    {
                        st_date.Attributes["class"] = "step1 fright";
                        hf_asc.Value = "desc";
                    }
                    break;
            }
        }
        void SearchBox(TextBox tb, string DefaulText)
        {
            tb.Attributes.Add("value", DefaulText);
            tb.Attributes.Add("onFocus", @"if(this.value == '" + DefaulText + "') {this.value = '';}");
            tb.Attributes.Add("onBlur", @"if (this.value == '') {this.value = '" + DefaulText + "';}");
        }
        public string GetUppers(string s)
        {
            string ss = string.Empty;
            foreach (string w in s.Split(' '))
            {
                ss += w.Substring(0, 1).ToUpper();
            }
            return (ss);
        }
        private List<string> getKeywordList()
        {
            List<string> keys = KeyWords.Text.Trim().Split(new char[] { ';'},StringSplitOptions.RemoveEmptyEntries).ToList();
            return keys;
        }
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            List<string> keywordList = getKeywordList();

            string command = string.Empty;

            command = @"SELECT RequestId FROM [dbo].StockOutRequest sor WHERE ";

            foreach (string tmp_str in keywordList)
            {
                command += string.Format(@" (sor.BudgetedAccount LIKE '%{0}%' OR 
	                                  sor.Department LIKE '%{0}%' OR 
	                                  sor.DueDate LIKE '%{0}%' OR 
	                                  sor.FillingDate LIKE '%{0}%' OR 
	                                  sor.OrderNumber LIKE '%{0}%' OR 
	                                  sor.Position LIKE '%{0}%' OR 
	                                  sor.Purpose LIKE '%{0}%' OR 
	                                  sor.RequestId LIKE '%{0}%' OR 
                                      sor.OtherPurpose LIKE '%{0}%' OR 
                                      sor.Comments LIKE '%{0}%' OR 
	                                  sor.RequestType LIKE '%{0}%' OR 
	                                  sor.RequestorName LIKE '%{0}%') AND", tmp_str);
            }
            DateTime Start_date;
            DateTime End_date;

            bool d1 = DateTime.TryParse(sdate.Value, out Start_date);
            bool d2 = DateTime.TryParse(enddate.Value, out End_date);

            Start_date = d1 ? Start_date : DateTime.Parse("1/1/1900");
            End_date = d2 ? End_date : DateTime.Parse("1/1/9000");

            command += string.Format("(FillingDate between convert(datetime,'{0}',101) and convert(datetime,'{1}',101))", Start_date.ToShortDateString(), End_date.ToShortDateString());

            DataTable table = new DataTable("SORRequests");

            try
            {
                SqlConnection con = new SqlConnection(connectionString);

                SqlDataAdapter adapter = new SqlDataAdapter(command, con);

                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetStatistics -  </br> " + ex.Message);
            }

            if (table.Rows.Count > 0)
            {
                #region SORDataTable
                DataTable SORDataTable = new DataTable();
                DataColumn column;

                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = "Order N               ";
                SORDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = "Date               ";
                SORDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = "Requester's name               ";
                SORDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = "Item Code               ";
                SORDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = "Item Description               ";
                SORDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = "Quantity               ";
                SORDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = "DueDate               ";
                SORDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = "Purpose               ";
                SORDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = "Status               ";
                SORDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = "Provided               ";
                SORDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = "Returned            ";
                SORDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = "Comments               ";
                SORDataTable.Columns.Add(column);

                StockOutRequestModel request = new StockOutRequestModel();
                foreach (DataRow currequest in table.Rows)
                {
                    request = StockOutRequestDAO.GetRequestById(Convert.ToInt32(currequest[0].ToString()));

                    foreach (StockOutRequestItemsModel item in request.Items)
                    {
                        DataRow row = SORDataTable.NewRow();

                        row[0] = request.OrderNumber;
                        row[1] = request.FillingDate.ToShortDateString();
                        row[2] = request.RequestorName;
                        row[3] = item.ItemCode;
                        row[4] = item.ItemDescription;
                        row[5] = item.Quantity;
                        row[6] = (request.Purpose == Purpose.Temporary) ? request.DueDate.ToShortDateString() : "";
                        row[7] = request.Purpose.ToString();

                        if (request.Approved == true)
                            row[8] = "Approved";
                        else if (request.Approved == false)
                            row[8] = "Rejected";
                        else
                            row[8] = "Pending";

                        if (request.Provided == true)
                            row[9] = "Yes";
                        else if (request.Provided == false)
                            row[9] = "No";
                        else
                            row[9] = "Pending";

                        if (request.RecievedBack == true)
                            row[10] = "Yes";
                        else if (request.RecievedBack == false)
                            row[10] = "No";
                        else
                            row[10] = "   ";

                        row[11] = request.Comments;

                        SORDataTable.Rows.Add(row);
                    }
                }
                #endregion SORDataTable
                StringWriter stw = new StringWriter();
                HtmlTextWriter htextw = new HtmlTextWriter(stw);
                GridView gv = new GridView();
                gv.DataSource = SORDataTable;
                gv.DataBind();
                gv.RenderControl(htextw);

                string attachment = "attachment; filename=ExportSOR.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", attachment);
                Response.ContentType = "application/ms-excel";
                Response.Write(stw.ToString());
                Response.End();
            }
        }
        public void setCheckBoxes()
        {
            T1.Enabled = T2.Enabled = T3.Enabled = T4.Enabled = T5.Enabled = T6.Enabled = false;
            List<string> Tlist = new List<string>();

            Tlist = current.RequestAccess;

            if (current.SoStatisticsAccess)
            {
                Tlist.Add("6");

                btnExcel.Visible = true;
            }

            foreach (string t in Tlist)
            {
                switch (t)
                {
                    case "1":
                        T1.Enabled = true;
                        T1.Checked = true;
                        break;
                    case "2":
                        T2.Enabled = true;
                        T2.Checked = true;
                        break;
                    case "3":
                        T3.Enabled = true;
                        T3.Checked = true;
                        break;
                    case "4":
                        T4.Enabled = true;
                        T4.Checked = true;
                        break;
                    case "5":
                        T5.Enabled = true;
                        T5.Checked = true;
                        break;
                    case "6":
                        T6.Enabled = true;
                        T6.Checked = true;
                        break;
                }
            }
        }
    }
}
