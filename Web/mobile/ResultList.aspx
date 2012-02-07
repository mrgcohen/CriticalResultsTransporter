<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResultList.aspx.cs" Inherits="Mobile.ResultList" %>

<!DOCTYPE html PUBLIC "-//WAPFORUM//DTD XHTML Mobile 1.0//EN" "http://www.wapforum.org/DTD/xhtml-mobile10.dtd">


<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<meta name="apple-mobile-web-app-capable" content="yes" />
<meta name="format-detection" content="telephone=no" />
<meta name="viewport" content="width=device-width; initial-scale=1.0; maximum-scale=1.0;" /> 
<meta http-equiv="Pragma" content="no-cache" />
    <title>ANCR Mobile - List</title>
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
    <a style="float: right" href="Help.aspx">Help</a>
    <br /><br />
    <div class="header">Alert Notification of Critical Radiology Results<br /><span class="version">Pilot Version 1.0</span></div>
    <div>
		<table border="0" cellpadding="0" cellspacing="0">
    	<asp:Repeater ID="Results" runat="server" OnItemCommand="showDetail_Click">
    	<HeaderTemplate>		
    		
    	</HeaderTemplate>
    	<ItemTemplate>
    			<tr class="t odd">
    				<td rowspan="3" class="level bottom" bgcolor='<%# getLevelColor(DataBinder.Eval(Container, "DataItem.LevelUuid")) %>'>&nbsp;</td>
					<td style="padding-top:3px"><asp:Label ID="Name" CssClass="name" runat="server" Text='<%# getPatientName(DataBinder.Eval(Container, "DataItem.ResultContextJson")) %>'></asp:Label></td>
					<td rowspan="3" class="level bottom dates"><asp:Label CssClass="dates" ID="DueTime" runat="server" Text='<%# getDueTime(DataBinder.Eval(Container, "DataItem"))  %>'></asp:Label></td> 				
					<td rowspan="3" class="search bottom"><asp:Button CssClass="viewButton" ID="search" Text="View" runat="server" CommandName="<% # getCommandName() %>" CommandArgument='<%#DataBinder.Eval(Container, "DataItem.ResultUuid") %>' /></td>
    			</tr>
 			    <tr class="t odd">
    				<td><asp:Label ID="mrn" runat="server" CssClass="mrn" Text='<%# getPatientId(DataBinder.Eval(Container, "DataItem.ResultContextJson")) %>'></asp:Label></td>
    			</tr>
    			<tr class="t odd">
    			    <td class="bottom"><asp:Label ID="dob" runat="server" CssClass="dob" Text='<%# getDOB(DataBinder.Eval(Container, "DataItem.ResultContextJson")) %>'></asp:Label></td>
    			</tr>
    	</ItemTemplate>
    	<AlternatingItemTemplate>
    			<tr class="t even">
					<td rowspan="3" class="level bottom" bgcolor='<%# getLevelColor(DataBinder.Eval(Container, "DataItem.LevelUuid")) %>'>&nbsp;</td>	    				
					<td style="padding-top:3px"><asp:Label ID="Name" CssClass="name" runat="server" Text='<%# getPatientName(DataBinder.Eval(Container, "DataItem.ResultContextJson")) %>'></asp:Label></td>
					<td rowspan="3" class="level bottom dates"><asp:Label ID="DueTime" CssClass="dates" runat="server" Text='<%# getDueTime(DataBinder.Eval(Container, "DataItem"))  %>'></asp:Label></td> 
					<td rowspan="3" class="bottom"><asp:Button CssClass="viewButton" ID="search" Text="View"  runat="server" CommandName="<% # getCommandName() %>" CommandArgument='<%#DataBinder.Eval(Container, "DataItem.ResultUuid") %>' /></td>
    			</tr>
    			<tr class="t even">
    				<td><asp:Label ID="mrn" runat="server" CssClass="mrn" Text='<%# getPatientId(DataBinder.Eval(Container, "DataItem.ResultContextJson")) %>'></asp:Label></td>
    			</tr>
    			<tr class="t even">
    			    <td class="bottom"><asp:Label ID="dob" runat="server" CssClass="dob" Text='<%# getDOB(DataBinder.Eval(Container, "DataItem.ResultContextJson")) %>'></asp:Label></td>
    			</tr>
    	</AlternatingItemTemplate>
    	<FooterTemplate>
    		<tr><td><asp:Label ID="lblNoResults" Visible='<%# getVisible() %>' runat="server">No open alerts.</asp:Label></td></tr>
    	</FooterTemplate>
		</asp:Repeater>
		</table>
    </div>
    <br />
    <asp:HyperLink runat="server" ID="lnkFeedback" >ANCR Application Suggestions</asp:HyperLink>
    </form>
</body>
</html>
