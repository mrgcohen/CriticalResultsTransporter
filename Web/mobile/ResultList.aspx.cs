using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CriticalResults;

namespace Mobile
{
	public partial class ResultList : System.Web.UI.Page
	{
		CriticalResults.User _LoggedInUser = null;
		Guid _Token = Guid.Empty;
        string _UserName = "";
		Level[] _Levels = null;
		ResultsService _Service = new ResultsService();
		ResultViewEntry[] _Results = null;

		protected void Page_Load(object sender, EventArgs e)
		{
            try
            {
                _Token = new Guid((string)Session["Token"]);
                _UserName = ((string)Session["UserName"]);
            }
            catch (ArgumentNullException)
            {
                Response.Redirect("~/Default.aspx");
            }

            string ip = Utilities.GetIP4Address();

            HTTPCheckRoles roles;
            if (CheckTokenUtil.CheckToken(_UserName, _Token.ToString(), ip, Request.HttpMethod, true, out roles))
			{
                if (!IsPostBack)
                {
                    _LoggedInUser = _Service.GetUser(_UserName);
                    Session["User"] = _LoggedInUser;
                    _Levels = _Service.GetAllLevels();
                    lnkFeedback.NavigateUrl = GetFeedbackUrl();
                    refresh();
                }
			}
			else
			{
				Response.Write("You are not logged into the ANCR system, please log in prior to accessing this page.");
			}
			
		}

		protected void refresh()
		{
			buildList();
		}

		protected void buildList()
		{
			getResults();
			Results.DataSource = _Results;
			Results.DataBind();
		}

		protected void getResults()
		{
			string queryString = "(it.ReceiverUserName='{0}' OR it.SenderUserName='{0}') AND it.AcknowledgmentTime IS NULL";
			queryString = string.Format(queryString, _LoggedInUser.UserName);
			_Results = _Service.QueryResultList(queryString, null, null);
		}

		protected string getPatientName(object json)
		{
			string nameValue = "";
			string jsString = json as string;
			jsString = jsString.Replace("undefined", "\"\"");
			System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
			RadiologyContext context = serializer.Deserialize<RadiologyContext>(jsString);
			nameValue = context.PatientName.displayName + ": " + context.PatientName.value;
            if (string.IsNullOrEmpty(context.PatientName.value))
                nameValue = context.PatientName.displayName + ": Unknown";
			return nameValue;
		}

		protected string getLevelColor(object levelUuid)
		{
			foreach (Level level in _Levels)
			{
				if (level.Uuid == (Guid)levelUuid)
				{
					return level.ColorValue;
				}
			}
			return "";
		}

        protected string getDueTime(object data)
        {
            string times = "";
            ResultViewEntry result = (ResultViewEntry)data;
            DateTime dueDate = (DateTime)result.EscalationTime;
			DateTime creationDate = (DateTime)result.CreationTime;

            TimeSpan ts = DateTime.Now - dueDate;
            string timeString = "";
            if (ts.Days > 0)
               timeString = ts.Days.ToString() + " days";
            else if (ts.Hours > 0)
               timeString = ts.Hours.ToString() + " hours";
            else if (ts.Minutes > 0)
               timeString = ts.Minutes.ToString() + " minutes";

			TimeSpan tsc = DateTime.Now - creationDate;
			string ctimeString = "";
			if (tsc.Days > 0)
				ctimeString = tsc.Days.ToString() + " day(s) ago";
			else if (tsc.Hours > 0)
				ctimeString = tsc.Hours.ToString() + " hour(s) ago";
			else if (tsc.Minutes > 0)
				ctimeString = tsc.Minutes.ToString() + " minute(s) ago";
			else
				ctimeString = "< 1 minute ago";

            if (ts > new TimeSpan(0))
            {
                times = "-" + timeString + "<br />Overdue";
            }
            else
            {
				times = ctimeString + "<br />Pending";
            }
            
            return times;
        }

		protected bool getVisible()
		{
			if (_Results.Length == 0)
			{
				return true;
			}
			return false;
		}

		protected string getPatientId(object json)
		{
			string patientId = "";
			string jsString = json as string;
			jsString = jsString.Replace("undefined", "\"\"");
			System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
			RadiologyContext context = serializer.Deserialize<RadiologyContext>(jsString);
            patientId = context.MRN.displayName + ": " + context.MRN.value;
            if (string.IsNullOrEmpty(context.MRN.value))
                patientId = context.MRN.displayName + ": Unknown";
			return patientId;
		}

		protected string getDOB(object json)
		{
			string patientDOB = "";
			string jsString = json as string;
			jsString = jsString.Replace("undefined", "\"\"");
			System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
			RadiologyContext context = serializer.Deserialize<RadiologyContext>(jsString);
			patientDOB = context.DOB.displayName + ": " + context.DOB.value;
            if (string.IsNullOrEmpty(context.DOB.value))
                patientDOB = context.DOB.displayName + ": Unknown";
			return patientDOB;
		}

		private System.Drawing.Color HexToColor(string hex)
		{
			hex = hex.Replace("#", "");
			int r, g, b;
			r = System.Convert.ToInt32(hex.Substring(0, 2), 16);
			g = System.Convert.ToInt32(hex.Substring(2, 2), 16);
			b = System.Convert.ToInt32(hex.Substring(4, 2), 16);
			return System.Drawing.Color.FromArgb(r, g, b);
		}

		protected void showDetail_Click(object sender, RepeaterCommandEventArgs e)
		{
			string url = "~/ResultDetail.aspx?ResultUuid={0}";
			string resultUuid = e.CommandArgument as string;
			Response.Redirect(String.Format(url, resultUuid));

		}

		protected string getCommandName()
		{
			return "Item_" + Results.Items.Count.ToString();
		}

        protected string GetFeedbackUrl()
        {
            Setting[] settings = _Service.GetSettings("System");
            foreach (Setting setting in settings)
            {
                if (setting.EntryKey == "MasterClinicalAdminAccount")
                {
                    User clinAdmin = _Service.GetUser(setting.Value);
                    clinAdmin.ResolveTransports();
                    foreach (UserTransport t in clinAdmin.Transports)
                    {
                        t.ResolveTransport();
                        if (t.Transport.Name == CriticalResults.Properties.Settings.Default.FeedbackTransportName)
                        {
                            return "mailto:" + t.Address;
                        }
                    }
                }
            }
            return "";
        }
	}
}
