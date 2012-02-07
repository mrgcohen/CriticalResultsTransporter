<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CentricityLogin.aspx.cs" Inherits="CentricityAuthExt._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    
	<link href="../CSS/StyleSheet.css" rel="stylesheet" type="text/css" />
	<script src="../javascript/jquery/jquery-1.3.2.js" type="text/javascript"></script>
	<script src="../javascript/jquery/jquery.json-1.3.min.js" type="text/javascript"></script>
	
    <script type="text/javascript">
    	var CRManager = null;
    	
    	$(document).ready(function() {
    		Manager.initialize();
    	});

    	var Manager = {}
    	Manager.initialize = function() {
    		if (parent.Manager) {
    			CRManager = parent.Manager;
    		}
    		else {
    			alert("Not invoked inside Critical Results Application.  Nothing will happen.");
    			return;
    		}
    		if ($("#UserName").text() != "" && $('#Token').text()) {
    			parent.AuthManager.setBasicAuthHeader($("#UserName").text(), $('#Token').text());
    			CRManager.WebClient.getUser($("#UserName").text(), Manager.getUserSuccessful, Manager.getUserFailure);
    		}
    		else {
    			alert("No account exists for your Windows credentials:\r\n" + $("#CentricityUserName").text() + "\r\n\r\nYou will now be redirected to the main login screen.");
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
    		alert('getUser() failed.');
    	}
    </script>
</head>
<body style="text-align:center;">
    <form id="form1" runat="server" style="visibility: hidden">
    <div id="CentricityId"><%=CentricityUserName%></div>
    <div id="UserName"><%=UserName %></div>
    <div id="Token"><%=Token %></div>
    
    </form>
</body>
</html>
