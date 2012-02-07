var MANUAL_TRANSPORT_NAME = "Manual";

var Manager = parent.Manager;

//
// Management Object
// Author: John Morgan
// Created: 7/7/2009
//
var UserTransportManagement = {
	UserTransports: null,
	Transports: null,
	SelectedUserTransport: null,
	UpdateFunctionCallback: null,
	CreateFunctionCallback: null
};

//
// Initialize Management Object
// Author: John Morgan
// Created: 7/7/2009
UserTransportManagement.Initialize = function(updateCallback, createCallback) {
	Manager.AuthenticatedUser.Transports = Manager.AuthenticatedUser.Transports;
	UserTransportManagement.UpdateFunctionCallback = updateCallback;
	UserTransportManagement.CreateFunctionCallback = createCallback;
	UserTransportManagement.User = parent.Manager.AuthenticatedUser;
	//WebClient.initialize();

	UserTransportManagement.BuildDetailTable();

	Manager.WebClient.getAllTransports(UserTransportManagement.getAllTransportsSuccess);
}



//
// Builds detail table and inserts it on the page
// Author: John Morgan
// Created: 7/7/2009
UserTransportManagement.BuildDetailTable = function() {
	var detailTable = '<div id="personalTransportDetail" class="hidden"><table id="personalTransportDetailTable"><thead id="personalTransportDetailTableHeader">'
                    + '<tr><th colspan="2" id="userTransportsDetailsTitle">Create Contact Method</th></tr></thead>'
					+ '<tbody id="userTransportsDetailsBody"><tr><td align="right">Contact Type:</td>'
					+ '<td align="left"><select id="transportNameDropDown" onchange="UserTransportManagement.updateAvailableLevels(this)"></select></td></tr>'
                    + '<tr><td align="right">Address:</td><td align="left"><input type="text" value="" id="transportAddressTextBox" style="width: 250px;" /></td></tr>'
                    + '<tr><td valign="top" align="right">Alert Level(s):</td><td align="left" id="transportLevelsCell"></td></tr></tbody>'
					+ '<tfoot id="personalTransportDetailTableFooter"><tr><td>&nbsp;</td><td align="left">'
                    + '<input type="button" id="submitButton" onclick="UserTransportManagement.Submit();return false;" value="Submit" />'
                    + '<input type="button" id="cancelButton" onclick="UserTransportManagement.Close();return false;" value="Cancel" />'
                    + '</td></tr></tfoot></table></div>';
	$(detailTable).appendTo('body');

	for (var i = 0; i < Manager.Levels.length; i++) {
		$("#transportLevelsCell").append("<input type='checkbox' id='" + Manager.Levels[i].Uuid + "' value='" + Manager.Levels[i].Name + "'  /><span class='colorSpan' id='" + Manager.Levels[i].ColorValue + "'>" + Manager.Levels[i].Name + "</span><br />");
	}
	$(".colorSpan").each(function(index, item) { Utility.setLevelColor(item, item.id); });

}

UserTransportManagement.getAllTransportsSuccess = function(transportData) {
	$("#transportNameDropDown").html("");
	UserTransportManagement.Transports = transportData;
	$("#transportNameDropDown").append($("<option id='-1' value=''\> </option>"));
	for (var i = 0; i < UserTransportManagement.Transports.length; i++) {
		if (UserTransportManagement.Transports[i].Name != MANUAL_TRANSPORT_NAME) {
			$("#transportNameDropDown").append($("<option id=\"option" + i + 1 + "\" value=\"" + UserTransportManagement.Transports[i].Name + "\">" + UserTransportManagement.Transports[i].FriendlyName + "</option>"));
		}
	}
}

//
// Updates the available levels in the detail table
// Author: John Morgan
// Created: 7/7/2009
UserTransportManagement.updateAvailableLevels = function(dropDownList) {
	var transportName = $(dropDownList).val()
	UserTransportManagement.selectedTransportName = transportName;
	if (transportName != "") {
		$("#transportLevelsCell [type='checkbox']").attr('checked', false);
		$("#transportLevelsCell [type='checkbox']").attr('defaultChecked', false);
		for (var i = 0; i < UserTransportManagement.Transports.length; i++) {
			if (UserTransportManagement.Transports[i].Name == transportName) {
				UserTransportManagement.enableSupportedLevelsCheckboxes(UserTransportManagement.Transports[i]);
			}
		}
	}
	else {
		$("#transportLevelsCell [type='checkbox']").attr('disabled', true);
	}
}

//
// Submits changes to database
// Author: John Morgan
// Created: 7/7/2009
UserTransportManagement.Submit = function() {
	if (UserTransportManagement.Action == "Create") {
		UserTransportManagement.createPersonalTransport();
	}
	else if (UserTransportManagement.Action == "Edit") {
		UserTransportManagement.updatePersonalTransport();
	}
}

//
// Alters the detail table for edits
// Author: John Morgan
// Created: 7/7/2009
UserTransportManagement.buildEditDetailsPopup = function() {
	editTransportName = UserTransportManagement.SelectedUserTransport.Transport.Name;
	UserTransportManagement.Action = "Edit";
	$('#personalTransportDetailTableHeader').html("Edit Personal Transport");

	$('#transportNameDropDown').attr("disabled", "disabled");
	$('#transportNameDropDown').val(editTransportName);


	UserTransportManagement.displayEditTransportData(UserTransportManagement.SelectedUserTransport);
	UserTransportManagement.enableSupportedLevelsCheckboxes();
}

//
// Enables and disables checkboxes for allowed levels
// Author: John Morgan
// Created: 7/7/2009
UserTransportManagement.enableSupportedLevelsCheckboxes = function() {
	$("#transportLevelsCell [type='checkbox']").removeAttr("disabled", "disabled");
	var bRequired = false;

	$(UserTransportManagement.Transports).each(function(k, transport) {
			if (transport.Name == UserTransportManagement.SelectedUserTransport.Transport.Name) {
				$(transport.MandatoryLevels).each(function(j, level) {
						$("#" + level.Uuid).attr("disabled", "disabled");
						$("#" + level.Uuid).attr("checked", "checked");
						$("#" + level.Uuid).attr("defaultChecked", "checked");
				});
			}
		});
}

//
//Initiates editing
//Author: John Morgan
//Created: 7/7/2009
UserTransportManagement.editTransport = function(address, name) {
	for (var i = 0; i < Manager.AuthenticatedUser.Transports.length; i++) {
		if (Manager.AuthenticatedUser.Transports[i].Transport.Name == name &&
			Manager.AuthenticatedUser.Transports[i].Address == address) {
			UserTransportManagement.SelectedUserTransport = Manager.AuthenticatedUser.Transports[i];
		}
	}

	UserTransportManagement.buildEditDetailsPopup();
//	UserTransportManagement.updateAvailableLevels();
}

//
//Displays modal popup
//Author: John Morgan
//Created: 7/7/2009
UserTransportManagement.displayEditTransportData = function(userTransportDataJSONStr) {
	var selectedLevels = null;
	for (var i = 0; i < Manager.AuthenticatedUser.Transports.length; i++) {
		if (Manager.AuthenticatedUser.Transports[i].Address == UserTransportManagement.SelectedUserTransport.Address) {
			selectedLevels = Manager.AuthenticatedUser.Transports[i].Levels;
		}
	}
	var userTransportData = userTransportDataJSONStr;
	$('#transportAddressTextBox').val(UserTransportManagement.SelectedUserTransport.Address);
	$("#transportLevelsCell [type='checkbox']").attr('checked', false);
	$("#transportLevelsCell [type='checkbox']").attr('defaultChecked', false);
	for (var i = 0; i < selectedLevels.length; i++) {
		$("#" + selectedLevels[i].Uuid).attr("checked", true);
		$("#" + selectedLevels[i].Uuid).attr("defaultChecked", true);
	}
	//        	$("#modal").show();
	var h = 400;
	var w = 450;

	$("#personalTransportDetail").dialog({ draggable: true, resizable: false, modal: true,
		zIndex: 4999, title: "Edit " + UserTransportManagement.SelectedUserTransport.Transport.FriendlyName, position: 'center',
		height: h, width: w, close: function(event, ui) { $("#personalTransportDetail").dialog('destroy'); }
	});
}

//
// Rsets the detail table
// Author: John Morgan
// Created: 7/7/2009
UserTransportManagement.ResetDetailData = function() {
	$('#transportNameDropDown').attr("disabled", false);
	$('#transportNameDropDown option:first').attr('selected', 'selected');
	$("#transportLevelsCell [type='checkbox']").attr('checked', false);
	$("#transportLevelsCell [type='checkbox']").attr('defaultChecked', false);
	$("#transportLevelsCell [type='checkbox']").attr('disabled', true);
	$("#transportAddressTextBox").val("");
	//$('#transportLevelDropDown >option').remove();
}

//Sets the levels for the newly created transport.
UserTransportManagement.createPersonalTransportCallback = function() {
	var transportName = $("#transportNameDropDown").val();
	var levels = $("#transportLevelsCell [type='checkbox']");
	var address = $("#transportAddressTextBox").val();
	var levelNames = new Array();
	for (var i = 0; i < levels.length; i++) {
		if (levels[i].checked) {
			levelNames[levelNames.length] = levels[i].value;
		}
	}
	Manager.WebClient.addLevelsToUserTransport(UserTransportManagement.User.UserName, transportName, address, levelNames, UserTransportManagement.addLevelsToUserTransportCallback);

}

UserTransportManagement.addLevelsToUserTransportCallback = function() {
	if (UserTransportManagement.Action == "Edit") {
		UserTransportManagement.UpdateFunctionCallback();
		UserTransportManagement.Close();
	}
}

UserTransportManagement.updatePersonalTransport = function(userID) {
	var transportName = $("#transportNameDropDown").val();
	var address = $("#transportAddressTextBox").val();
	Manager.WebClient.updateUserTransport(UserTransportManagement.User.UserName, transportName, UserTransportManagement.SelectedUserTransport.Address, address, UserTransportManagement.createPersonalTransportCallback)
}



UserTransportManagement.Close = function() {
	UserTransportManagement.ResetDetailData();
	$('#personalTransportDetail').dialog('destroy');
}








/*
UserTransportManagement.getUserTransportsCallback = function(userTransportData) {
var userTransports = eval('(' + userTransportData + ')');
for (var i = 0; i < userTransports.length; i++) {
var address = userTransports[i].Address;
var levelArray = userTransports[i].Levels;
var transportInfo = userTransports[i].Transport;  //Name, TransportUri
}
UserTransportManagement.DisplayUserTransportsTable(userTransports)
}
*/



UserTransportManagement.BuildUserTransportsTable = function() {

	var transporttable = "<div id='personalTransports' class='hidden'><a href='#' onclick='UserTransportManagement.createNewTransportPopup();'>Create New Contact</a><br /><br /><table id='personalTransportsTable'><tr><th>Name</th><th>Address</th><th>Alert Level(s) supported</th><th>Action</th></tr>";
	for (var i = 0; i < Manager.AuthenticatedUser.Transports.length; i++) {
		if (Manager.AuthenticatedUser.Transports[i].Transport.Name != MANUAL_TRANSPORT_NAME) {
			var address = Manager.AuthenticatedUser.Transports[i].Address;
			var levelArray = Manager.AuthenticatedUser.Transports[i].Levels;
			var levelsInfo = "<ul>";
			for (var j = 0; j < levelArray.length; j++) {
				levelsInfo += "<li><span class='colorSpan' id=\"" + levelArray[j].ColorValue + "\">" + levelArray[j].Name + "</span><br /> " + levelArray[j].ShortDescription + "</li>";
			}
			levelsInfo += "</ul>";
			var transportInfo = Manager.AuthenticatedUser.Transports[i].Transport;  //Name, TransportUri

			transporttable += "<tr id=\"transport\"" + i + "><td>" + transportInfo.FriendlyName
                                                        + "</td><td>" + address
                                                        + "</td><td>" + levelsInfo
                                                        + "</td><td>" + "<a href=\"#\" onclick=\"UserTransportManagement.buildEditDetailsPopup('" + transportInfo.Name + "', '" + Manager.AuthenticatedUser.Transports[i].Address + "', '" + Manager.AuthenticatedUser.Transports[i].Transport.FriendlyName + "')\" >Modify</a>"
                                                        + "</td></tr>";
		}
	}
	transporttable += "</table></div>";
	$(transporttable).appendTo("body");
	$('#personalTransportsTableBody tr:nth-child(odd)').addClass("oddRow");
	$('#personalTransportsTableBody tr:nth-child(even)').addClass("evenRow");
	$('.colorSpan').each(function(index, item) { Utility.setLevelColor(item, item.id); });
}





UserTransportManagement.showUserTransportTable = function() {
	$('#personalTransports').removeClass('hidden');
	var h = $('#personalTransports').height() + 50;
	var w = Math.round($(window).width() * .6);
	$('#personalTransports').dialog({ draggable: true, resizable: false, modal: true,
		zIndex: 4999, title: "Edit Contact Info", position: 'center',
		height: h, width: w, close: function(event, ui) { UserTransportManagement.closeUserTransportTable(); }
	});
}

UserTransportManagement.closeUserTransportTable = function() {
	$('#personalTransports').dialog('destroy');
}

UserTransportManagement.updatePersonalTransportCallback = function(userID) {
	alert("update complete");
	var transportName = $("#transportNameDropDown").val();
	var levels = $("#transportLevelsCell [type='checkbox']");
	//We need a remove transport option before we can really do this
	/*
	for (var i = 0; i < levels.length; i++) {
	if (levels[i].checked) {
	WebClient.addTransportToLevel(levels[i].id, transportName)  //levels[i].id is the level Uuid
	}
	}
	*/
	UserTransportManagement.ResetDetailData();
	$("div#modal").hide();
	refreshPage();
}




/***Create Transport Functions **/




/** Edit Transport Functions **/





/** Functions used for both Create and Modify **/







//Although this updates the database, it throws a self referencing loop error
UserTransportManagement.createPersonalTransport = function() {
	var transportName = $("#transportNameDropDown").val();
	var address = $("#transportAddressTextBox").val();
	Manager.WebClient.createUserTransport(UserTransportManagement.User.UserName, transportName, address, UserTransportManagement.createPersonalTransportCallback)
}



UserTransportManagement.createNewTransportPopup = function() {
	UserTransportManagement.Action = "Create";
	UserTransportManagement.ResetDetailData();
	var transportName = $("#transportNameDropDown").val()
	$("#transportNameDropDown").removeAttr("disabled");
	//WebClient.getTransport(transportName, UserTransportManagement.enableSupportedLevelsCheckboxes);
	parent.Manager.addModal("editTransport", $("#personalTransportDetail").html(), "Create New Contact");
}


