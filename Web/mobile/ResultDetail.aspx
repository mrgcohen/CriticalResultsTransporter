<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResultDetail.aspx.cs" Inherits="Mobile.ResultDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<meta name="viewport" content="width=device-width; initial-scale=1.0; maximum-scale=1.0;" /> 
<meta name="format-detection" content="telephone=no" />  
<meta http-equiv="Pragma" content="no-cache" />
    <title>ANCR Mobile</title>
    <style type="text/css">
            body
        {
            font-family:verdana,sans-serif;
            font-size:smaller;
            text-align:left;
            padding: 0 0 0 0;
            margin: 0px 2px 0px 2px;
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
        .ack
		{
			font-family: Verdana;
			width: 100%;
		}
		input.ack
		{
			font-family: Verdana;
			height: 75px;
			font-size:medium;
			width: 100%;
		}
		div.ack
		{
			font-family: Verdana;
			font-weight: bold;
			width: 95%;
			padding: 8px 5px 8px 5px;
			text-align: left;
		}
		div.mask
		{
			z-index: 500;
			position: absolute;
			width: 98%;
			height: 800px;
			float: left;
			filter:alpha(opacity=80);
			opacitiy:0.8;
			background-color: #EDEDED;
			vertical-align: middle;
			text-align: center;
		}
		div.demask
		{
			z-index: 1000;
			float: left;
			width: 85%;
			position:absolute;
			background-color: #EDEDED;
			vertical-align: middle;
			text-align: center;
			border: outset 2px;
			vertical-align: middle;
			margin: 20px 20px 20px 20px;
		}
		
		div.header
		{
			width: 100%;
			text-align:left;
			font-family:Verdana;
			font-weight: bold;
			background-color: #305481;
			color:White;
			border-bottom: inset 2px;
			padding: 5px 5px 5px 5px;
		}
		
		div.innerDiv
		{
			margin: 10px 10px 10px 10px;
		}

		.hidden
		{
			display: none;
		} 
		div.sectionHead
		{
			font-family: Verdana;
			font-weight:600;
			margin-bottom: 5px;
			margin-top: 3px;
			width: 100%;
		}
		.message
		{
			font-family: Verdana;
			font-style: italic;
			width: 95%;
			padding: 8px 5px 8px 5px;
			text-align: left;
		}
		td.value
		{
			font-weight: bold;
			border: solid 1px #909090;
			padding: 2px 2px 2px 2px;
		}
		td.display
		{
			border: solid 1px #909090;
			padding: 2px 2px 2px 2px;
		}
		td
		{
			font-family: Verdana;
		}
		table
		{
		    border: 0 0 #C0C0C0;
		}
		table.context
		{
			width: 100%;
			background-color: #C0C0C0;
		}
		a.back
		{
		    font-size:medium;
		}
		td.context
		{
		    background-color:  rgb(177,195,218);
		    padding: 3px 5px 3px 5px;
		}
    </style>
    
    <script type="text/javascript">
      
      function updateOrientation(){   
        var contentType = "show_";   
        switch(window.orientation){   
            case 0:   
            contentType += "normal";   
            break;   
      
            case -90:   
            contentType += "right";   
            break;   
      
            case 90:   
            contentType += "left";   
            break;   
      
            case 180:   
            contentType += "flipped";   
            break;   
        }   
        document.getElementById("page_wrapper").setAttribute("class", contentType);   
    }  
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div runat="server" id="mask" class="mask" visible="false">

    </div>
    <div id="ackDiv" runat="server" class="demask" visible="false">
		<div class="header">&nbsp;Acknowledge Alert</div>
		<div class="innerDiv">
    	<asp:TextBox ID="AckNotes" runat="server" Rows="4" Width="95%" TextMode="MultiLine" Text="I have reviewed and understand the results." Font-Names="Verdana" Enabled=false ></asp:TextBox>
		<asp:PlaceHolder ID="AckButtonPH" runat="server"></asp:PlaceHolder><br /><br />
		<asp:Button ID="AckCancel" runat="server" CssClass="ack" Text="Cancel" onclick="AckCancel_Click" />
		</div>
	</div>
	<div id="rerouteDiv" runat="server" class="demask" visible="false">
	<div class="header">Reroute Alert</div>
	<div class="innerDiv">
	<div style="text-align:center">Send Result to Clinical Administrator and Remove From Your List?</div>
	<asp:Button ID="RerouteConfirm" runat="server" Text="Ok" CssClass="ack" 
            onclick="RerouteConfirm_Click" /><br />
	<asp:Button ID="RerountCancel" runat="server" Text="Cancel" CssClass="ack" 
            onclick="RerountCancel_Click" />
	</div>
	</div>
    <div>
    <a class="back" style="float: left" href="ResultList.aspx">&lt;&lt; Back to list</a><a class="back" style="float: right" href="Help.aspx">Help</a>
    <br /><br />
	<table width="100%" cellpadding="0" cellspacing="0">
		<tr>
			<td class="context">
				<div class="sectionHead">Patient & Exam Info</div>
				<asp:PlaceHolder ID="ContextPH" runat="server"></asp:PlaceHolder>
			</td>
		</tr>
		<tr>
			<td class="context">
				<div class="sectionHead">Critical Result Description</div>
				<div style="width: 100%;">
				<center>
				<div id="AckPH" runat="server" class="ack"></div>
				<div ID="MessagePH" runat="server" class="message"></div>
				</center>
				</div>
			</td>
		</tr>
		<tr><td class="context">&nbsp;</td></tr>
		<tr>
			<td><asp:PlaceHolder ID="AcknowledgementPH" runat="server"></asp:PlaceHolder></td>
		</tr>
		<tr><td>&nbsp;</td></tr>
		<tr>
			<td>
			<asp:Button ID="ack" runat="server" Text="Acknowledge" CssClass="ack" 
                    onclick="ack_Click" />
			</td>
		</tr>
		<tr><td>&nbsp;</td></tr>
		<tr>
		    <td>
                <asp:Button ID="notMyPatient" runat="server" Text="Not My Patient" CssClass="ack" OnClick="notMyPatient_Click"  />
            </td>
		</tr>
    </table>
    <br />
    </div>
    </form>
    <form id="alertForm" runat="server" visible="false">
        <asp:TextBox ID="MessageText" Rows="4" Width="95%" runat="server" 
            TextMode="MultiLine"></asp:TextBox><br />
        <asp:Button ID="Ok" Text="Ok" runat="server" CssClass="ack"
            onclick="Ok_Click" />
    </form>
</body>
</html>
