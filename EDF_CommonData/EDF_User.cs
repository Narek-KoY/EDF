using System;
using System.Collections.Generic;

namespace EDF_CommonData
{
    public class EDF_User
    {
        public int id { get; set; }
        public string Login { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string PictureUrl { get; set; }
        public bool? IsOk { get; set; }
        public DateTime ActionDate { get; set; }
        public bool IsSubtitute { get; set; }
        public string Date
        {
            get
            {
                if (ActionDate.ToString() == "01/01/0001 12:00:00 AM" || ActionDate.ToString() == "1/1/0001 12:00:00 AM")
                    return "";
                else
                    return ActionDate.ToString("dd/MM/yyyy HH:mm:ss").Split(' ')[0] + " | ";
            }
        }
        public string Time
        {
            get
            {
                if (ActionDate.ToString() == "01/01/0001 12:00:00 AM" || ActionDate.ToString() == "1/1/0001 12:00:00 AM")
                    return "";
                else
                    return ActionDate.ToString().Split(' ')[1] + " " + ActionDate.ToString().Split(' ')[2];
            }
        }
        public string Icon
        {
            get
            {
                switch (IsOk)
                {
                    case null: return "/_catalogs/masterpage/images/timer.png";
                    case true: return "/_catalogs/masterpage/images/ok.png";
                    case false: return "/_catalogs/masterpage/images/x.png";
                }
                return "/_catalogs/masterpage/images/timer.png";
            }
        }
        public bool HasRep { get; set; }
        public string R
        {
            get
            {
                return (HasRep ? "(R) " : "");
            }
        }

        public bool IsMemberOf(EDF_Group Group)
        {
            return AD.GetUserBySPLogin(Login).IsMemberOf(Group);
        }

        public bool IsMemberOf(List<EDF_Group> Group, bool And)
        {
            return false;// AD.GetUserBySPLogin(Login).IsMemberOf(Group);
        }

        public string Team { get; set; }

        public string Sender { get; set; }

        public bool TeamAprove(string RequestID)
        {            
            return EDF.TeamApprove(RequestID, Team, Sender);
        }

        public string TeamIcon(string RequsetId)
        {
            return TeamAprove(RequsetId) ? "/_catalogs/masterpage/images/ok.png" : "/_catalogs/masterpage/images/timer.png";
        }

        public bool IsGroup { get; set; }
        public string GroupName { get; set; }
    }
}
