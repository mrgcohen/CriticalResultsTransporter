<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Help.aspx.cs" Inherits="Mobile.Help" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<meta name="viewport" content="width=device-width; initial-scale=1.0; maximum-scale=1.0;" /> 
    <title></title>
    <style type="text/css">
            body
        {
            font-family:verdana,sans-serif;
            font-size:smaller;
            text-align:left;
            padding: 0 0 0 0;
        }
        
        .l
        {
            padding:5px
        }
        .t
        {            padding:5px
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
			margin: 1px 1px 1px 1px;
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
		span.sectionHead
		{
			font-family: Verdana;
			font-weight:600;
		}
		.message
		{
			font-family: Verdana;
			font-style: italic;
			width: 95%;
			padding: 8px 5px 8px 5px;
			margin: 1px 1px 1px 1px;
			text-align: left;
		}
		td.value
		{
			font-weight: bold;
			border: solid 1px black;
			padding: 2px 2px 2px 2px;
		}
		td.display
		{
			border: solid 1px black;
			padding: 2px 2px 2px 2px;
		}
		td
		{
			font-family: Verdana;
		}
		table.context
		{
			width: 100%;
			background-color: #C0C0C0;
		}
		a
		{
		    font-size:medium;
		}
    </style>
    
    <script type="text/javascript">
        function onload() {
        	var image = document.getElementById("imgHelp");
        	if (image != null) {
        		image.style.width = screen.availWidth - 4 + 'px';
        		image.style.height = 'auto';
        	}
        }

        function goback() {
            history.go(-1);
        }
    </script>
</head>
<body onload="onload()">
    <form id="form1" runat="server">
    <a href="javascript:goback()" >&lt;&lt;Back</a>
    <div style="text-align:center">    
    <h3>ANCR Mobile Help</h3>
    <br /><br />
    <input type="button" class="ack" value="View" onclick="" />
    <br />
    <h4>Click to view alert details</h4>
    <br /><br />
    <input type="button" class="ack" value="Acknowledge" onclick="" />
    <br />
    <h4>Click to acknowledge alert</h4>
    <br /><br />
    <input type="button" class="ack" value="Not my Patient" onclick="" />
    <br />
    <h4>Click if this is not for a patient of yours, this will send alert to system administrator.</h4>
    </div>
    </form>
</body>
</html>
