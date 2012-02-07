<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="_Default.aspx.cs" Inherits="Mobile._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <style type="text/css">
    		.header
		{
			text-align:center;
			padding: 3px;
			background-color:#305481;
			color:White;
			margin:4px;
			font: 15px Arial;
		}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">Alert Notification or Critical Radiology Results</div>
        <br />
        <center>
    <div>
		<a href="WindowsLogin.aspx">Click here to login with your Partners Username and Password</a>
		<asp:Login ID="mobileLogin" runat="server" BackColor="#F7F6F3" 
			BorderColor="#E6E2D8" BorderPadding="4" BorderStyle="Solid" BorderWidth="1px" 
			Font-Names="Verdana" Font-Size="0.8em" ForeColor="#0E0E0E" 
			onauthenticate="mobileLogin_Authenticate">
			<TextBoxStyle Font-Size="0.8em" />
			<LoginButtonStyle BackColor="#FFFBFF" BorderColor="#CCCCCC" BorderStyle="Solid" 
				BorderWidth="1px" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#284775" />
			<InstructionTextStyle Font-Italic="True" ForeColor="Black" />
			<TitleTextStyle BackColor="#305481" Font-Bold="True" Font-Size="0.9em" 
				ForeColor="White" />
		</asp:Login>
		
    </div>
    </center>
    </form>
</body>
</html>
