<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WindowsLogin.aspx.cs" Inherits="WindowsAuthExt.WindowsLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Windows Login</title>

	<link href="../CSS/StyleSheet.css" rel="stylesheet" type="text/css" />
	<script src="../javascript/jquery/jquery-1.3.2.js" type="text/javascript"></script>
	<script src="../javascript/jquery/jquery.json-1.3.min.js" type="text/javascript"></script>
		
	<style type="text/css">
	body
	{
		text-align:center;
	}
	</style>
	<script type="text/javascript">
		if (parent.Manager) {
			var CRManager = parent.Manager;
		}
		
		var Manager = {
			initialize : null
		};		

		Manager.initialize = function() {
			if (!CRManager) {
				alert("Not invoked inside Critical Results Application.  Nothing will happen.");
				return;
			}

			//WebClient.initialize();

			if ($("#UserName").text() && $('#TokenGuid').text()) {
				parent.AuthManager.setBasicAuthHeader($("#UserName").text(), $('#TokenGuid').text());
				CRManager.WebClient.getUser($("#UserName").text(), Manager.getUserSuccessful, Manager.getUserFailure);
			}
			else if ($("#PageMessage").text() != "") {
				alert($("#PageMessage").text());
				Manager.redirect();
			}
			else {
				alert("No account exists for your Windows credentials:\r\n" + $("#WindowsUser").text() + "\r\n\r\nYou will now be redirected to the main login screen.");
				Manager.redirect();
			}
		}
		Manager.redirect = function() {
			parent.window.location = '../';
		}

		Manager.getUserSuccessful = function(response) {
			if (response.Enabled === false) {
				alert("Login Failed: ANCR Account Disabled\r\nPlease contact your System Adminstrator.");
				Manager.redirect();
			}
			CRManager.setAuthenticatedUser(response);
		}
		Manager.getUserFailure = function(xhr) {
			debugger;
			alert('getUser() failed.');
		}

		$(document).ready(function() {   Manager.initialize(); });
		
	</script>
</head>
<body>
<div style="visibility: hidden">
	<div id="WindowsUser"><%=WindowsUser%></div>
	<div id="UserName"><%=UserName%></div>
	<div id="TokenGuid"><%=TokenGuid %></div>
	<div id="PageMessage"><%=PageMessage %></div>
</div>
</body>
</html>
