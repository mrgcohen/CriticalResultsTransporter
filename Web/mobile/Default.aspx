<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Mobile.WindowsLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<meta name="viewport" content="width=device-width; initial-scale=1.0; maximum-scale=1.0;" /> 
<meta name="format-detection" content="telephone=no" />  
<meta http-equiv="Pragma" content="no-cache" />
<meta name="apple-mobile-web-app-capable" content="yes" />
    <title></title>
    <style type="text/css"> 
        body
        {
            margin: 0px 2px 0px 2px;
            font-family:verdana,sans-serif;
            font-size:smaller;
            text-align:left;
        }
        
        .l
        {
            padding:5px
        }
        .t
        {
            table-layout:fixed;
            padding:5px
        }
        .t .t1
        {
            padding-right:4px
        }
        .t .t2
        {
        }
        .d
        {
            font-size:x-small;
            color:#8a8a8a
        }
        .p
        {
            color:#8a8a8a
        }
        .u
        {
            padding-top:3px;
            padding-bottom:3px
        }
        .f
        {
            color:#999;padding-top:5px
        }
        .h
        {
            font-weight:bold;
            background-color:#E8EEF7;
            margin-top:3px;
            padding:1px 1px 1px 3px
        }
        .b
        {
            padding-top:3px;
            padding-bottom:5px
        }
        .s
        {
            margin-top:5px
        }
        .n
        {
            padding-bottom:6px
        }
        .o
        {
            padding-bottom:7px
        }
        .e
        {
            padding-bottom:5px
        }
        p
        {
            margin:0;
            margin-bottom:3px
        }
        .g
        {
            padding:3px;
            background-color:#FC6;
            margin-top:2px
        }
        .header
		{
			text-align:center;
			padding: 5px;
			background-color:#305481;
			color:White;
			font: Arial;
			margin-bottom:1px;
		}
		span.header
		{
			font-family: Arial;
			color: #888888;
			background-color: #FFFFFF;
		}
		tr.odd
		{
			background-color: #F2F2F2;
			padding: 2px 5px 2px 5px;
		}
		tr.even
		{
			background-color: #D8D8D8;
			padding: 2px 5px 2px 5px;
		}
		td.level
		{
			width: 30px;
		}
		td.bottom
		{
			border-bottom: solid 1px #FFFFFF;
		}
		td
		{
		    padding: 2px 2px 2px 3px;
		}
		.dates
		{
		    text-align: center;
		    font-size: x-small;
		}
		td.dates
		{
		    border-left: solid 1px #FFFFFF;
		    border-right: solid 1px #FFFFFF;
		    padding: 0px 5px 0px 5px;
		    width: 100px;
		}
		table
		{
		    width: 100%;
		}
		
		a
		{
		    font-size: medium;
		}
		
		.viewButton
		{
		    height: 50px;
		}
		
		.version
		{
		    font-size: xx-small;
		}
    </style>

</head>
<body>
    <form id="form1" runat="server">
    <div>
            <div class="header">Alert Notification or Critical Radiology Results</div>
        <br />
        <center>
            <div><h3>The ANCR Mobile application has been tested to work on Blackberry devices that run OS 6.0 or newer, iPhones, iPads, and Android devices. ANCR may not work on other mobile devices or on older software versions.</h3></div>
			<div id="message" runat="server"></div>
        </center>
    
    </div>
    </form>
</body>
</html>
