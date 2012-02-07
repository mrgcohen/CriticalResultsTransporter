//This js file will check and update a ANCR User against Active Directory and
//Partners Paging Directory and update the account in ANCR
//
//IF an updateCompleteCallback is provided, the userobject will be returned PRIOR to database update
//Then data can be verified or modified, then passed back in a call to updateUser to perform data
//send.  if updateCompleteCallback is null; the data will be written to the database as soon as the
//buildUpdatedUser function is complete.
//
//callbackSuccess is required and will be called after the update is complete.

var UpdateUser = {
    SelectedUser: null,
    UpdatedUser: null,
    ADUserInfo: null,
    isInitialized: false,
    lookupCompleteCallback: null,
    updateCompleteCallback: null,
    callbackSuccess: null,
    callbackError: null,
    ADLookupComplete: false,
    updateUserEntry: false,
    updateTransports: false
}

var ANCRCONSTANTS = {
    EMAIL_TRANSPORT: "SMTP Transport",
    PAGER_TRANSPORT: "Pager Transport",
    MANUAL_TRANSPORT: "Manual"
}

var AUTHEXTCONSTANTS = {
    TYPE: "AuthExt",
    WINDOWS: "Windows",
    LOGONDOMAIN: DefaultDomain
}

var ADCONSTANTS = {
	PAGER: "pager",
    MAIL: "mail",
    MIDDLENAME: "middleName",
    FIRSTNAME: "givenName",
    LASTNAME: "sn",
    TITLE: "title",
    USERNAME: "cn"
}

UpdateUser.lookupUser = function(userName, lookupCompleteCallback, updateCompleteCallback, callbackError) {
    if (updateCompleteCallback) {
        UpdateUser.updateCompleteCallback = updateCompleteCallback;
    }
    else {
        alert("You must provide an updateCompleteCallback function.");
    }
    if (lookupCompleteCallback) {
        UpdateUser.lookupCompleteCallback = lookupCompleteCallback;
    }
    if (callbackError) {
        UpdateUser.callbackError = callbackError;
    }
    else {
        UpdateUser.callbackError = UpdateUser.defaultErrorCallback;
    }

    UpdateUser.getANCRUser(userName);
}

UpdateUser.getANCRUser = function(username) {
    CRManager.WebClient.getUser(username, UpdateUser.getANCRUserSuccess, UpdateUser.callbackError);
}

UpdateUser.getANCRUserSuccess = function(data) {
    if (data.length == 0) {
        alert("User not found in ANCR Database.");
    }
    else {
        var emailAddress = null;
        UpdateUser.SelectedUser = data;
        $(UpdateUser.SelectedUser.Transports).each(function(i, transport) {
            if (transport.Transport.Name == ANCRCONSTANTS.EMAIL_TRANSPORT) {
                emailAddress = transport.Address;
            }
        });
        if (emailAddress) {
            UpdateUser.getADUserInfo(emailAddress);
        }
        else {
            alert("User does not have email address assigned.  Email address is required for lookup.");
        }
    }
}

UpdateUser.getADUserInfo = function(email) {
    ADWebClient.getUserByEmail(email, UpdateUser.getADUserInfoSuccess, UpdateUser.adCallbackError);
}

UpdateUser.getADUserInfoSuccess = function(data) {
    UpdateUser.ADUserInfo = data;
    UpdateUser.ADLookupComplete = true;
    
    UpdateUser.buildUpdatedAD();
   }

UpdateUser.adCallbackError = function() {
   	UpdateUser.ADLookupComplete = true;
   	var message = "Error finding user in AD: ";
   	if (data.responseText) {
   		message += data.responseText;
   	}
   	else {
   		message += "Unknown Server Error.";
   	}
   	alert(message);
   	UpdateUser.buildUpdatedUser();
}

UpdateUser.buildUpdatedAD = function()
{
	var ntID = "";
	if (UpdateUser.ADLookupComplete == true)
	{
		UpdateUser.UpdatedUser = jQuery.extend(true, {}, UpdateUser.SelectedUser);
		UpdateUser.UpdatedUser.UserEntries = new Array();
		if (UpdateUser.ADUserInfo.length > 0)
		{
			$.each(UpdateUser.ADUserInfo[0].Properties, function(index, kvp)
			{
				if (kvp.value != null && kvp != "")
				{
					switch (kvp.key)
					{
						case ADCONSTANTS.FIRSTNAME:
							UpdateUser.UpdatedUser.FirstName = kvp.value;
							break;
						case ADCONSTANTS.LASTNAME:
							UpdateUser.UpdatedUser.LastName = kvp.value;
							break;
						case ADCONSTANTS.MIDDLENAME:
							UpdateUser.UpdatedUser.MiddleName = kvp.value;
							break;
						case ADCONSTANTS.MAIL:
							break;
						case ADCONSTANTS.TITLE:
							UpdateUser.UpdatedUser.Title = kvp.value;
							break;
						case ADCONSTANTS.USERNAME:
							var authExtFound = false;
							ntID = kvp.value;
							$(UpdateUser.SelectedUser.UserEntries).each(function(i, entry)
							{
								var a = 1;
								a++;
								if (entry.Type == AUTHEXTCONSTANTS.TYPE && entry.Key == AUTHEXTCONSTANTS.WINDOWS)
								{
									authExtFound = true;
									if (AUTHEXTCONSTANTS.LOGONDOMAIN + "\\" + kvp.value != entry.Value)
									{
										UpdateUser.UpdatedUser.UserEntries[UpdateUser.UpdatedUser.UserEntries.length] = {
											type: AUTHEXTCONSTANTS.TYPE,
											key: AUTHEXTCONSTANTS.WINDOWS,
											value: AUTHEXTCONSTANTS.LOGONDOMAIN + "\\" + kvp.value,
											xmlvalue: ""
										}
										UpdateUser.updateUserEntry = true;
									}
									else
									{
										entry = null;
									}
								}
							});
							if (authExtFound == false)
							{
								var entry = {
									type: AUTHEXTCONSTANTS.TYPE,
									key: AUTHEXTCONSTANTS.WINDOWS,
									value: AUTHEXTCONSTANTS.LOGONDOMAIN + "\\" + kvp.value,
									xmlvalue: ""
								}
								UpdateUser.updateUserEntry = true;
								UpdateUser.UpdatedUser.UserEntries[UpdateUser.UpdatedUser.UserEntries.length] = entry;
							}
							break;
						default:
							break;
					}
				}
			});
		}
		Extension.extendedUserUpdate(ntID, UpdateUser.lookupComplete);
	}
}
UpdateUser.lookupComplete = function() {
	if (UpdateUser.lookupCompleteCallback) {
		UpdateUser.lookupCompleteCallback(UpdateUser.UpdatedUser);
	}
	else {
		UpdateUser.updateUser(UpdateUser.UpdatedUser);
	}
}

UpdateUser.updateUser = function(updatedUser) {
	if (UpdateUser.updateUserEntry == true) {
		UpdateUser.updateUserEntries();
	}
	else if (UpdateUser.updateTransports == true) {
		UpdateUser.updateUserTransports();
	}
	else {
		UpdateUser.updateUserInfo();
	}
}

var userEntriesCounter = new Array();

UpdateUser.updateUserEntries = function() {
	$(UpdateUser.UpdatedUser.UserEntries).each(function(i, entry) {
		if (entry != null) {
			userEntriesCounter[userEntriesCounter.length] = entry.EntryKey;
			CRManager.WebClient.createUpdateUserEntry(UpdateUser.UpdatedUser.UserName, $.toJSON(entry), UpdateUser.updateUserEntrySuccess, entry.EntryKey);
		}
	});
}

UpdateUser.updateUserEntrySuccess = function(data, callId) {
    var tempArray = new Array();
    $(userEntriesCounter).each(function(index, item) {
        if (item != callId) {
            tempArray[tempArray.length] = item;
        }
    });
    userEntriesCounter = tempArray;
    if (userEntriesCounter.length == 0) {
        UpdateUser.updateUserTransports();
    }
}
var transportUpdateCounter = new Array();

UpdateUser.updateUserTransports = function() {
	var transportUpdated = false;
	transportUpdateCounter = new Array();
	if (UpdateUser.updateTransports == true) {
		$(UpdateUser.UpdatedUser.Transports).each(function(i, transport) {
			var origAddress = "";
			$(UpdateUser.SelectedUser.Transports).each(function(j, oTransport) {
				if (transport.Transport.Name == oTransport.Transport.Name) {
					origAddress = oTransport.Address;
				}
			});
			if (origAddress != transport.Address) {
				transportUpdated = true;
				transportUpdateCounter[transportUpdateCounter.length] = transport.Transport.Name;
				CRManager.WebClient.updateUserTransport(
                UpdateUser.UpdatedUser.UserName,
                transport.Transport.Name,
                origAddress,
                transport.Address,
                UpdateUser.updateUserTransportsSuccess,
                UpdateUser.updateUserTransportsError,
                transport.Transport.Name
                );
			}
		});
	}
	else {
		UpdateUser.updateUserInfo();
	}
	if (transportUpdated == false) {
		UpdateUser.updateUserInfo();
	}
}
UpdateUser.updateUserTransportsError = function(data, callId) {
	var tempArray = new Array();
	$(transportUpdateCounter).each(function(i, item) {
		if (item != callId) {
			tempArray[tempArray.length] = item;
		}
	});
	transportUpdateCounter = tempArray;
	if (transportUpdateCounter.length == 0) {
		UpdateUser.updateUserInfo();
	}

}
UpdateUser.updateUserTransportsSuccess = function(data, callId) {
    var tempArray = new Array();
    $(transportUpdateCounter).each(function(i, item) {
        if (item != callId) {
            tempArray[tempArray.length] = item;
        }
    });
    transportUpdateCounter = tempArray;
    if (transportUpdateCounter.length == 0) {
        UpdateUser.updateUserInfo();
    }
}

UpdateUser.updateUserInfo = function() {
    CRManager.WebClient.updateUser($.toJSON(UpdateUser.UpdatedUser), UpdateUser.updateUserInfoSuccess);
}

UpdateUser.updateUserInfoSuccess = function(data) {
    if (UpdateUser.updateCompleteCallback) {
        UpdateUser.updateCompleteCallback(data);
    }
}





UpdateUser.defaultErrorCallback = function(data) {
    if (data) {
        if (data.responseText) {
            alert(data.responseText);
        }
        else if (data.message) {
            alert(data.message);
        }
        else {
            alert("An unknown server error occured.");
        }
    }
    else {
        alert("An unknown error occured");
    }
}
