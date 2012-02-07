ChangePassword = {
	html: "",
	onCompleteCallback: null,
	onErrorCallback: null,
	isInitialized: false,
	userName: null
}

ChangePassword.Initialize = function(userName, onComplete, onError) {
	ChangePassword.userName = userName;
	ChangePassword.onCompleteCallback = onComplete;
	ChangePassword.onErrorCallback = onError;
	ChangePassword.html = "<div id='changePasswordDiv'><table style='border-style:none'><tr><td>Current Password</td><td><input type='password' id='cPasswordTxt' /></td></tr>";
	ChangePassword.html += "<tr><td>New Password</td><td><input type='password' id='nPasswordTxt' /></td></tr>";
	ChangePassword.html += "<tr><td>Confirm Password</td><td><input type='password' id='cnPasswordTxt' /></td></tr>";
	ChangePassword.html += "<tr><td colspan='2'><input type='button' onclick='ChangePassword.Submit()' value='Submit' /></td></tr></table></div>";
	ChangePassword.isInitialized = true;
	$(top.document.body).append(ChangePassword.html);
	$('#changePasswordDiv').dialog({ height: 130, maxHeight: 400, width: 330, zIndex: 4999, stack: true, title: 'Change Password', close: ChangePassword.closeDialog });

}

ChangePassword.Submit = function() {
	var nPassword = $('#nPasswordTxt').val();
	var cnPassword = $('#cnPasswordTxt').val();
	if (nPassword != cnPassword) {
		alert("Passwords do not match!");
		$('#nPasswordTxt').val('');
		$('#cnPasswordTxt').val('');
	}
	else {
		var data = { 'currentPassword':$('#cPasswordTxt').val(),'newPassword':nPassword };
		WebClient.setPassword(ChangePassword.userName, data, ChangePassword.onSuccess, ChangePassword.onFailure);
	}
}

ChangePassword.onSuccess = function(data) {
	if (data.SetPassword_JsonResult === true) {
		alert("Password successfully changed.");
		if (ChangePassword.onCompleteCallback)
			ChangePassword.onCompleteCallback(data);
		$('#changePasswordDiv').dialog('close');
	}
	else {
		alert("Password change failed. Current password incorrect.");
		$('#cPasswordTxt').val("");
		if (ChangePassword.onErrorCallback)
			ChangePassword.onErrorCallback(data);
	}
}

ChangePassword.onError = function(data) {
	alert("Change password failed!");
	if (data.responseText) {
		alert(data.responseText);
	}
	else {
		alert("Unknown server error.");
	}
}

ChangePassword.closeDialog = function() {
	$('#changePasswordDiv').remove();
}