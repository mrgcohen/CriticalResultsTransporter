using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CriticalResults;
using System.Reflection;



namespace Mobile
{
	public partial class ResultDetail : System.Web.UI.Page
	{
		Guid _Token = Guid.Empty;
		User _LoggedInUser = null;
		Guid _ResultUuid = Guid.Empty;
		ResultsService _Service = new ResultsService();
		Result _Result;
		Acknowledgment _Acknowledgement;
        Setting[] _SystemSettings;

		protected void Page_Load(object sender, EventArgs e)
		{
            string userAgent = Request.ServerVariables.GetValues("HTTP_USER_AGENT")[0].ToString();
            string[] userAgentSections = userAgent.Split(new char[] { '/' });
            if (userAgentSections[0].ToLowerInvariant().Contains("blackberry"))
            {
                int bbMajVer = Int32.Parse(userAgentSections[1].ToCharArray()[0].ToString());
                if (bbMajVer < 5)
                {
                    //notMyPatient.Enabled = false;
                }
            }

            try
            {
                _Token = new Guid((string)Session["Token"]);
                _LoggedInUser = (User)Session["User"];
            }
            catch (ArgumentNullException)
            {
                Response.Redirect("~/Default.aspx");
            }
            _ResultUuid = new Guid(Request.QueryString["ResultUuid"]);
            string ip = Utilities.GetIP4Address();
            HTTPCheckRoles roles;
            if (CheckTokenUtil.CheckToken(_LoggedInUser.UserName, _Token.ToString(), ip, Request.HttpMethod, true, out roles))
			{
                _SystemSettings = _Service.GetSettings("System");
				_Result = _Service.GetResult(_ResultUuid.ToString());
				_Acknowledgement = _Service.GetResultAcknowledgment(_ResultUuid.ToString());
				BuildDetail();
			}
			else
			{
				Response.Write("You are not logged into ANCR.  Please login to view this page.");
			}
		}

		protected void BuildDetail()
		{
			BuildContext();
			BuildMessage();
			BuildAcknowledgement();
			if (_Acknowledgement.CreationTime != DateTime.MinValue)
			{
				notMyPatient.Visible = false;
                ack.Visible = false;
			}
		}

		protected void BuildAcknowledgement()
		{
			AcknowledgementPH.Controls.Clear();
			if (_Acknowledgement.CreationTime == DateTime.MinValue)
			{
				Button ackButton = new Button();
				ackButton.Text = "Acknowledge";
				ackButton.CssClass = "ack";
				ackButton.Click += new EventHandler(ackButton_Click);
				AckButtonPH.Controls.Add(ackButton);
			}
			else
			{
				AckButtonPH.Visible = false;
				//clientAckBtn.Visible = false;
			}
		}

		void ackButton_Click(object sender, EventArgs e)
		{
			if (AckNotes.Text != "")
			{
                string notes = AckNotes.Text;
                string user = _LoggedInUser.FirstName + " " + _LoggedInUser.LastName;
                string time = DateTime.Now.ToString("MM-DD-YYYY hh:mm zz");
                AckNotes.Text = String.Format("User: {0}\n\rTime: {1}\n\rNotes: {2}", user, time, notes);
				_Acknowledgement = _Service.CreateResultAcknowledgment(_Result.Uuid.ToString(), _LoggedInUser.UserName, AckNotes.Text);
                ackDiv.Visible = false;
                mask.Visible = false;
                Response.Redirect("~/ResultList.aspx");
			}
		}

		protected void BuildMessage()
		{
			AckPH.Controls.Clear();
			if (_Acknowledgement.CreationTime == DateTime.MinValue)
			{
				Label label = new Label();
				label.Text = "Result has not been acknowledged.";
				label.CssClass = "ack";
				AckPH.Controls.Add(label);
			}
			else
			{	
				Label label = new Label();
				label.CssClass = "ack";
				label.Text = String.Format("Result was acknowledged on {0} at {1} by {2} {3}.", _Acknowledgement.CreationTime.ToString(@"M/d/yyyy"), _Acknowledgement.CreationTime.ToString("h:mm tt"), _Acknowledgement.User.FirstName, _Acknowledgement.User.LastName);
				AckPH.Controls.Add(label);
			}
			string message = _Result.Message;
			string colorHex = _Result.Level.ColorValue;
			MessagePH.InnerText = message;
			MessagePH.Attributes.Add("class", "message");
			MessagePH.Attributes.Add("style", "background-color:" + colorHex);
			AckPH.Attributes.Add("style", "background-color:" + colorHex);
		}

		protected void BuildContext()
		{
			string jsString = _Result.Context.First().JsonValue;
			jsString = jsString.Replace("undefined", "\"\"");
			System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
			RadiologyContext context = serializer.Deserialize<RadiologyContext>(jsString);

			ContextPH.Controls.Clear();

			Table contextTable = new Table();
			contextTable.CssClass = "context";
			contextTable.CellPadding = 0;
			contextTable.CellSpacing = 0;

            string[] skippedContext = CriticalResults.Properties.Settings.Default.ContextNotDisplayed.Split(new char[] {','});

			Type t = context.GetType();
			foreach (PropertyInfo p in t.GetProperties())
			{
				string displayName = "";
				string value = "";
				object o = p.GetValue(context, null);
				Type t2 = o.GetType();
				foreach (PropertyInfo p2 in t2.GetProperties())
				{
					if (p2.Name == "displayName")
					{
						displayName = (string)p2.GetValue(o, null);
					}
					if (p2.Name == "value")
					{
						value = (string)p2.GetValue(o, null);
					}
				}
                bool display = true;
                foreach (string contextName in skippedContext)
                {
                    if (displayName == contextName)
                        display = false;
                }
                if (display)
                {
                    TableRow tableRow = new TableRow();
                    TableCell displayNameCell = new TableCell();
                    displayNameCell.Text = displayName;
                    displayNameCell.CssClass = "display";
                    tableRow.Controls.Add(displayNameCell);
                    TableCell valueCell = new TableCell();
                    valueCell.Text = value;
                    valueCell.CssClass = "value";
                    tableRow.Controls.Add(valueCell);
                    contextTable.Controls.Add(tableRow);
                }
			}
            TableRow creatorRow = new TableRow();
            TableCell creatorDisplay = new TableCell();
            creatorDisplay.Text = "Alert Created By";
            creatorDisplay.CssClass = "display";
            creatorRow.Controls.Add(creatorDisplay);
            TableCell creatorValue = new TableCell();

            HyperLink hl = new HyperLink();
            hl.Text = _Result.Sender.LastName + "," + _Result.Sender.FirstName;
            if (!string.IsNullOrEmpty(Extension.DefaultExtension.GetUserLink()))
            {
                hl.NavigateUrl = Extension.DefaultExtension.GetUserLink();
            }
            creatorValue.CssClass = "value";
            creatorValue.Controls.Add(hl);
            creatorRow.Controls.Add(creatorValue);
            contextTable.Controls.Add(creatorRow);
			ContextPH.Controls.Add(contextTable);
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

        protected void notMyPatient_Click(object sender, EventArgs e)
        {
            mask.Visible = true;
            rerouteDiv.Visible = true; 
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            form1.Visible = true;
            alertForm.Visible = false;
        }

        protected void ack_Click(object sender, EventArgs e)
        {
            ackDiv.Visible = true;
            mask.Visible = true;
        }

        protected void AckCancel_Click(object sender, EventArgs e)
        {
            ackDiv.Visible = false;
            mask.Visible = false;
        }

        protected void RerouteConfirm_Click(object sender, EventArgs e)
        {
            User clinicalAdmin = null;
            foreach (Setting setting in _SystemSettings)
            {
                if (setting.EntryKey == "MasterClinicalAdminAccount")
                {
                    clinicalAdmin = _Service.GetUser(setting.Value);
                }
            }

            if (clinicalAdmin == null)
            {
                MessageText.Text = "An Error Occured Rerouting the alert.  Please contact or system administrator.";
                form1.Visible = false;
                alertForm.Visible = true;
            }
            else
            {
                _Service.UpdateResultReceiver(_Result.Uuid.ToString(), clinicalAdmin.UserName);
                Response.Redirect("~/ResultList.aspx");
            }
        }

        protected void RerountCancel_Click(object sender, EventArgs e)
        {
            rerouteDiv.Visible = false;
            mask.Visible = false;
        }

	}

}
