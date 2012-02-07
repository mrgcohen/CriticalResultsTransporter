var CreateUser = {
	Lookup: {
		firstName: null,
		lastName: null
	},
	UsersFound: null,
	User: null,
	UserName: null,
	onCreateComplete: null,
	isInitialized: false,
	RECEIVER_ROLE: "receiver",
	EMAIL_TRANSPORT: "SMTP Transport",
	EMAIL_TRANSPORT_LEVELS: null,
	PAGER_TRANSPORT: "Pager Transport",
	ConfirmationMessage: null,
	UserAlreadyExists: false,
	WindowsAuthExtDomain: DefaultDomain,
	CurrentSearch: ""
}

var CRManager = parent.Manager;

CreateUser.initialize = function(onUserCreated, emailTransportLevels, manager, confirmationMessage, currentSearch) {
	CRManager = manager;
	CreateUser.ConfirmationMessage = confirmationMessage;
	CreateUser.onCreateComplete = onUserCreated;
	CreateUser.EMAIL_TRANSPORT_LEVELS = emailTransportLevels;
	CreateUser.isInitialized = true;
	CreateUser.CurrentSearch = currentSearch;
	return true;
}

CreateUser.showCreateUserDialog = function() {
	var html = "<div id='createUserDialog'>";
	html += "<span id='lastNameLabel' name='label'>Last Name:</span><input type='text' id='textLastName' value='" + CreateUser.CurrentSearch + "' /><br />";
	html += "<span id='firstNameLabel' name='label'>First Name:</span><input type='text' id='textFirstName' /><br />";
	html += "<input type='button' id='btnSearch' value='Search' onclick='CreateUser.search(event)' /><br /><hr />";
	html += "<br/><div><table id='tblResults' class='noBorder'><thead id='createUserResultHead'></thead><tbody id='createUserResultBody'></tbody></table></div></div>";
	$(top.document.body).append(html);
	$('#createUserDialog').dialog({ height: 400, maxHeight: 400, width: 715, zIndex: 4999, stack: true, title: 'Add Receiver from Directory', close: CreateUser.closeDialog });
}

CreateUser.search = function(event) {
	if ($('#textLastName').val() != "") {
		ADWebClient.getCreatableUsers($('#textLastName').val(), $('#textFirstName').val(), CreateUser.onSearchComplete);
	}
	else {
		alert("Please provide at least the last name.");
	}
}


CreateUser.onSearchComplete = function(data) {
	CreateUser.UsersFound = data;
	$('#createUserResultBody').html("");
	if (data.length == 0) {
		$('#createUserResultBody').append('<tr><td>No matches found</td></tr>');
	}
	$('#createUserResultHead').html('<tr><th colspan="3">Users Found</th></tr>');

	$(data).each(function(index, user) {
		var row = CreateUser.buildReturnRow(index, user);
	});
}

//If user has no email address configured in AD user cannot be created and is not shown in list.
CreateUser.buildReturnRow = function(index, person) {
	if ((person.mail == null || person.mail == "") && (person.ANCRAccountUserName == null || person.ANCRAccountUserName == "")) {
		return;
	}
	var tr = "<tr class='";
	if (index % 2 == 0) {
		tr += "evenRow";
	}
	else {
		tr += "oddRow";
	}
	tr += "'></tr>";
	tr = $(tr);
	var name = "<td>" + person.displayName + "</td>";
	name = $(name);
	var emailAddress = "No email address found";
	if (person.mail != null && person.mail != "") {
		emailAddress = person.mail;
	}
	var email = "<td>" + emailAddress + "</tD>";
	email = $(email);
	var buttonCell = "<td></td>";
	buttonCell = $(buttonCell);
	var button = "";
	if (person.ANCRAccountUserName == null || person.ANCRAccountUserName == "") {
		button = "<input type='button' style='width:110px;' value='Add User' title='Create an account for this user in ANCR.' />";
		button = $(button);
		button.bind("click", function(e) { CreateUser.Create(person); });
	}
	else {
		if (person.Enabled == true) {
			button = "<input type='button' style='width:110px;' value='Select User' title='User exists in ANCR, click to select this user to receive this alert.' />";
		}
		if (person.Enabled == false) {
			button = "<input type='button' style='width:110px;' value='Select User' disabled='disabled' title='User exists in ANCR but is disabled.  Please contact the system administrator.' />";
		}
		button = $(button);
		button.bind("click", function(e) { CreateUser.SelectUser(person.ANCRAccountUserName); });
	}


	$(tr).append(name);
	$(tr).append(email);
	$(buttonCell).append(button);
	$(tr).append(buttonCell);
	$('#createUserResultBody').append(tr);
	return;
}

CreateUser.SelectUser = function(ANCRUserName) {
	WebClient.getUser(ANCRUserName, CreateUser.onCreateComplete);
	CreateUser.closeDialog();
}

CreateUser.Create = function(person) {
	ADWebClient.createUser(person, CreateUser.CreateComplete, CreateUser.onCreateError);
}

CreateUser.onCreateError = function(msg){
	CRManager.log(msg.ResponseText);
	alert("An error occurred creating the user.  Please contact your System Administrator.");
}

CreateUser.CreateComplete = function(msg) {
	CRManager.log("User Created: " + msg.UserName);
	CreateUser.SelectUser(msg.UserName);
}

CreateUser.FormatName = function(name) {
	name = name.replace('.', '');
	name = name.replace(',', '');
	name = CreateUser.Trim(name);
	return name;
}

CreateUser.Trim = function(stringToTrim) {
	return stringToTrim.replace(/^\s+|\s+$/g, "");
}

CreateUser.closeDialog = function(event, ui) {
	$('#createUserDialog').remove();
}
