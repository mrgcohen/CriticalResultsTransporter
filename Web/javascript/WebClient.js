var WebClient = {
	resultServiceUri: RootUrl + "/Services/WebServices/Results.svc",
	timeout:60000,
	authHeaderValue: null,
	timeoutAlert: false
};

WebClient.sessionTimeout = function()
{
	if (!WebClient.timeoutAlert)
	{
		WebClient.timeoutAlert = true;
		alert("Your login has timed out due to inactivity.  Please login again.");
		parent.window.location = RootUrl + '/';
	}
}

WebClient.ajaxCall = function(uri, type, data, callbackSuccess, callbackError, callId)
{
	AuthManager.ajax(uri, type, data, callbackSuccess, callbackError, callId, false, WebClient.sessionTimeout);
}

WebClient.syncronousAjaxCall = function(uri, type, data, callbackSuccess, callbackError)
{
	AuthManager.ajax(uri, type, data, callbackSuccess, callbackError, null, true, WebClient.sessionTimeout);
}

WebClient.getAllLevels = function(callbackSuccess, callbackError){
	var uri = WebClient.resultServiceUri + "/level/json";
	WebClient.ajaxCall(uri, "GET", null, callbackSuccess, callbackError);
}

WebClient.createLevel = function(data, callbackSuccess, callbackError) {
    var uri = WebClient.resultServiceUri + "/level/create/json";
    WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError);
}

WebClient.updateLevel = function(data, callbackSuccess, callbackError) {
    var uri = WebClient.resultServiceUri + "/level/update/json";
    WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError);
}

WebClient.auditEvent = function(eventName, eventDescription, callbackSuccess, callbackError) {
	var data = { eventName:eventName, eventDescription: eventDescription};
	var json = $.toJSON(data);
	var uri = WebClient.resultServiceUri + "/audit/json";
	WebClient.ajaxCall(uri, "POST", json, callbackSuccess, callbackError);
}

//Description:
//	Modify the TransportLevel.
//Parameters:
//	levelUuid - the UUID of the level
//	transportName - the name of the transport
//	mandatory - bool - whether the transport is mandatory for the level
//Comments:
//	Jeremy R: refactor to a PUT to be more RESTFul.
//Created:
//Modified:
//	2009-05-31, Jeremy R.
//		Based on service updates: modified parameter list.  changed data parameter to transportName, changed Uri to be RESTFul
WebClient.addTransportToLevel = function(levelUuid, transportName, mandatory, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/level/" + levelUuid + "/transport/" + transportName + "/" + mandatory + "/json";
	WebClient.ajaxCall(uri, "POST", "", callbackSuccess, callbackError);
}


//Description:
//	Modify the TransportLevel.
//Parameters:
//	levelUuid - the UUID of the level
//	transportName - the name of the transport
//	mandatory - bool - whether the transport is mandatory for the level
//Created: 2009-05-31, Jeremy R
//Modified: 
WebClient.modifyTransportLevel = function(levelUuid, transportName, mandatory, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/level/" + levelUuid + "/" + transportName + "/" + mandatory + "/json";
	WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError);
}

WebClient.removeTransportFromLevel = function(levelUuid, data, callbackSuccess, callbackError) {
    var uri = WebClient.resultServiceUri + "/level/removetransport/" + levelUuid + "/json";
    WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError);
}

WebClient.getAllTransports = function(callbackSuccess, callbackError) {
    var uri = WebClient.resultServiceUri + "/transport/json";
    WebClient.syncronousAjaxCall(uri, "GET", null, callbackSuccess, callbackError);
}

WebClient.createTransport = function(name, data, callbackSuccess, callbackError) {
    var uri = WebClient.resultServiceUri + "/transport/" + name + "/create/json";
    WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError);
}

WebClient.updateTransport = function(data, callbackSuccess, callbackError) {
    var uri = WebClient.resultServiceUri + "/transport/update/json";
    WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError);
}

WebClient.deleteTransport = function(name, data, callbackSuccess, callbackError) {
    var uri = WebClient.resultServiceUri + "/transport/" + name + "/delete/json";
    WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError);
}

// Description:	
//	Gets the transports for a level
//	Created: April 2009, John Morgan
//	Comments:
//		Future: 2009-05-04, Jeremy R.: *** Refactor *** This appears to be a redundant call.  Getting the level returns the associated transports.  This is also true for GetAllLevels.
WebClient.getTransportsForLevel = function(levelUuid, callbackSuccess, callbackError) {
    var uri = WebClient.resultServiceUri + "/level/" + levelUuid + "/transport/json";
    WebClient.ajaxCall(uri, "GET", null, callbackSuccess, callbackError);
}

WebClient.getAllUsers = function(callbackSuccess, callbackError){
	var uri = WebClient.resultServiceUri + "/user/json";
	WebClient.ajaxCall(uri, "GET", null, callbackSuccess, callbackError);
}
WebClient.getUser = function(userName, callbackSuccess, callbackError){
	var uri = WebClient.resultServiceUri + "/user/"+ userName + "/json";
	data = userName;
	WebClient.ajaxCall(uri, "GET", data, callbackSuccess, callbackError);
}
WebClient.queryUser = function(queryString, pageSize, pageNumber, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/user/json";
	data = { queryString: queryString, pageSize: pageSize, pageNumber: pageNumber };
	WebClient.ajaxCall(uri, "POST", $.toJSON(data), callbackSuccess, callbackError);
}
WebClient.getAllContextTypes = function(callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/contextType/json";
	WebClient.ajaxCall(uri, "GET", null, callbackSuccess, callbackError);
}
WebClient.createResult = function(data, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/result/json";
	WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError);
	//	$.ajax({
	//		url: uri,
	//		type: "POST",
	//		data: data,
	//		contentType: "application/json",
	//		dataType: "json",
	//		timeout: WebClient.timeout,
	//		success: function(msg) {
	//			if (callbackSuccess) {  //want to confirm callback error is not null and not undefined
	//				callbackSuccess(msg);
	//			}
	//		},
	//		error: function(xhr) {
	//			if (callbackError) {	//want to confirm callback error is not null and not undefined	//callbackError !== null, callbackError != null
	//				callbackError(xhr);
	//			} else if (xhr.responseText) {
	//				alert(xhr.responseText);
	//			} else {
	//				alert("Unknown server error.");
	//			}
	//		}
	//	});
}

//  TODO: /updatereceiver/ is not a RESTFul URI.
WebClient.updateResultReciever = function(uuid, userName, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/result/" + uuid + "/updatereceiver/" + userName + "/json";
	WebClient.ajaxCall(uri, "POST", null, callbackSuccess, callbackError);
}
WebClient.createAcknowledgment = function(resultUuid, userName, data, callbackSuccess, callbackError) {
	data = $.toJSON({ notes: data });
	var uri = WebClient.resultServiceUri + "/result/" + resultUuid + "/acknowledgment/" + userName + "/json";
	WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError);
}
WebClient.getResult = function(uuid, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/result/" + uuid + "/json";
	WebClient.ajaxCall(uri, "GET", {}, callbackSuccess, callbackError);
}
WebClient.getResultAcknowledgment = function(uuid, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/result/" + uuid + "/acknowledgment/json";
	WebClient.ajaxCall(uri, "GET", {}, callbackSuccess, callbackError);
}
WebClient.getResultNotifications = function(uuid, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/result/" + uuid + "/notifications/json";
	WebClient.ajaxCall(uri, "GET", {}, callbackSuccess, callbackError);
}
WebClient.queryResultList = function(queryString, pageSize, pageNumber, callbackSuccess, callbackError, callId) {

	var uri = WebClient.resultServiceUri + "/resultlist/json";
	var data = { queryString: queryString, pageSize: pageSize, pageNumber: pageNumber };
	data = $.toJSON(data);
	WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError, callId);
}

WebClient.createResultNotification = function(notification, selectedTransport, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/result/notification/" + selectedTransport + "/json";
	var data = $.toJSON({ "notification": notification, "selectedTransport": selectedTransport });
	WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError);
}

WebClient.getHtml = function(uri, callbackSuccess, callbackError) {
	
	$.ajax({
		url: uri,
		type: "GET",
		dataType: "html",
		timeout: WebClient.timeout,
		success: function(msg) {
			if (callbackSuccess) {  //want to confirm callback error is not null and not undefined
				callbackSuccess(msg);
			}
		},
		error: function(xhr) {
			if (callbackError) {	//want to confirm callback error is not null and not undefined	//callbackError !== null, callbackError != null
				callbackError(msg);
			} else if (xhr.responseText) {
				alert(xhr.responseText);
			} else {
				alert("Unknown server error.");
			}
		}
	});
}

WebClient.getToken = function(userName, passwordHash, callbackSuccess, callbackError) {
    uri = WebClient.resultServiceUri + "/user/" + userName + "/token/json";
    var data = { 'passwordHash': passwordHash };
    data = $.toJSON(data);
    $.ajax({
        url: uri,
        type: "POST",
        data: data,
        contentType: "application/json",
        dataType: "json",
        timeout: WebClient.timeout,
        success: function(msg) {
            if (callbackSuccess) {  //want to confirm callback error is not null and not undefined
                    callbackSuccess(msg);
            }
        },
        error: function(xhr) {
            if (callbackError) {	//want to confirm callback error is not null and not undefined	//callbackError !== null, callbackError != null
                    callbackError(xhr);
            } else if (xhr.responseText) {
                alert(xhr.responseText);
            } else {
                alert("Unknown server error.");
            }
        }
    });
}

//Modified: 2009-10-07, John Morgan - Targeting new CreateUpdateUserEntry_Json Service Call
//TODO: make this consistent with JSONification of data blob.  Should this me an object or a JSON string?
WebClient.createUserEntry = function(userName, data, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/user/" + userName + "/entry/json";
	WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError);
}

WebClient.createUpdateUserEntry = function(userName, data, callbackSuccess, callbackError, callId) {
    var uri = WebClient.resultServiceUri + "/user/" + userName + "/userentry/json";
    WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError, callId);
}

WebClient.deleteUserEntry = function(userName, data, callbackSuccess, callbackError, callId) {
	var uri = WebClient.resultServiceUri + "/user/" + userName + "/entry/delete/json";
	WebClient.ajaxCall(uri, "POST", $.toJSON(data), callbackSuccess, callbackError, callId);
}

WebClient.queryUserEntry = function(queryString, pageSize, pageNumber, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/user/entry/json";
	data = { queryString: queryString, pageSize: pageSize, pageNumber: pageNumber };
	data = $.toJSON(data);
	WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError);
}

//Description:
//
//Parameters:
//
//Created:
//Modified:
//	2009-05-31: Modified Uri to be RESTFul
WebClient.getAllRoles = function(callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/role/json";
	WebClient.ajaxCall(uri, "GET", "", callbackSuccess, callbackError);
}

WebClient.createUser = function(data, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/user/create/json";
	WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError);
}

WebClient.sendAccountConfirmation = function(username, callbackSuccess, callbackError) {
    var uri = WebClient.resultServiceUri + "/user/" + username + "/confirmation/json";
    WebClient.ajaxCall(uri, "POST", null, callbackSuccess, callbackError);
}

WebClient.updateUser = function(data, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/user/update/json";
	WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError);
}
WebClient.updateUserRoles = function(userName, roles, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/user/" + userName + "/roles/json";
	WebClient.ajaxCall(uri, "POST", $.toJSON(roles), callbackSuccess, callbackError);
}
WebClient.addUserRole = function(userName, role, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/user/" + userName + "/role/json";
	var data = { "roleName": role };
	WebClient.ajaxCall(uri, "POST", $.toJSON(data), callbackSuccess, callbackError);
}

//User Transports
WebClient.getUserTransports = function(userName, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/user/" + userName + "/transport/json";
	WebClient.ajaxCall(uri, "GET", "", callbackSuccess, callbackError);
}
WebClient.createUserTransport = function(userName, transportName, address, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/user/createTransport/json";
	var data = { userName: userName, transportName: transportName, address: address };
	WebClient.ajaxCall(uri, "POST", $.toJSON(data), callbackSuccess, callbackError);
}
WebClient.addLevelsToUserTransport = function(userName, transportName, address, levelNames, callbackSuccess, callbackError) {
	var uri = WebClient.resultServiceUri + "/user/addLevelsToTransport/json";
	var data = { userName: userName, transportName: transportName, address: address, levelNames: levelNames };
	WebClient.ajaxCall(uri, "POST", $.toJSON(data), callbackSuccess, callbackError);
}
WebClient.updateUserTransport = function(userName, transportName, originalAddress, address, callbackSuccess, callbackError, callId) {
	var uri = WebClient.resultServiceUri + "/user/updateTransport/json";
	var data = { userName: userName, transportName: transportName, originalAddress: originalAddress, address: address };
	WebClient.ajaxCall(uri, "POST", $.toJSON(data), callbackSuccess, callbackError, callId);
}
//Proxy
WebClient.createProxy = function(userName, proxyUserName, description, callbackSuccess, callbackFailure) {
	var uri = WebClient.resultServiceUri + "/user/" + userName + "/proxy/" + proxyUserName + "/" + description + "/json";
	WebClient.ajaxCall(uri, "POST", null, callbackSuccess, callbackFailure);
}
WebClient.deleteProxy = function(userName, proxyUserName, callbackSuccess, callbackFailure) {
	var uri = WebClient.resultServiceUri + "/user/" + userName + "/proxy/" + proxyUserName + "/json";
	WebClient.ajaxCall(uri, "DELETE", null, callbackSuccess, callbackFailure);
}
WebClient.getUsersForProxy = function(userName, callbackSuccess, callbackFailure) {
	var uri = WebClient.resultServiceUri + "/user/" + userName + "/proxy/json";
	WebClient.ajaxCall(uri, "GET", null, callbackSuccess, callbackFailure);
}

WebClient.getSettings = function(owner, callbackSuccess, callbackFailure) {
	var uri = WebClient.resultServiceUri + "/settings/" + owner + "/json";
	WebClient.ajaxCall(uri, "GET", null, callbackSuccess, callbackFailure);
}

WebClient.getAllSettings = function(callbackSuccess, callbackFailure) {
    var uri = WebClient.resultServiceUri + "/settings/json";
    WebClient.ajaxCall(uri, "GET", null, callbackSuccess, callbackFailure);
}

WebClient.updateSetting = function(owner, data, callbackSuccess, callbackFailure) {
    var uri = WebClient.resultServiceUri + "/settings/" + owner + "/json";
    WebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackFailure);
}

WebClient.generateNewPassword = function(username, callbackSuccess, callbackFailure) {
    var uri = WebClient.resultServiceUri + "/user/" + username + "/newpassword/json";
    WebClient.ajaxCall(uri, "POST", null, callbackSuccess, callbackFailure);
}

WebClient.setPassword = function(userName, data, callbackSuccess, callbackFailure) {
	var uri = WebClient.resultServiceUri + "/user/" + userName + "/password/json";
	WebClient.ajaxCall(uri, "POST", $.toJSON(data), callbackSuccess, callbackFailure);
}

WebClient.leaveFeedback = function(resultUuid, userName, rating, comments, callbackSuccess, callbackFailure) {
    var uri = WebClient.resultServiceUri + "/result/" + resultUuid + "/feedback/json";
    var data = {
        userName: userName,
        rating: rating,
        comments: comments
    }
    WebClient.ajaxCall(uri, "POST", $.toJSON(data), callbackSuccess, callbackFailure);
}

WebClient.getNotifications = function(state, startDate, endDate, callbackSuccess, callbackFailure) {
	var uri = WebClient.resultServiceUri + "/notification/state/" + state;
	var data = {
		startDateRangeString: startDate,
		endDateRangeString: endDate
	}
	WebClient.ajaxCall(uri, "POST", $.toJSON(data), callbackSuccess, callbackFailure);
}

WebClient.getUserByAuthExt = function(authExtKey, authExtValue, callbackSuccess, callbackError, callId) {
var uri = WebClient.resultServiceUri + "/AuthExt/User/json";
	var data = {
		AuthExtKey: authExtKey,
		AuthExtValue: authExtValue
	}
WebClient.ajaxCall(uri, "POST", $.toJSON(data), callbackSuccess, callbackError, callId);
}