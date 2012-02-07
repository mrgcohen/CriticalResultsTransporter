ADWebClient = {
	ADWebServiceUri: RootUrl + "/Services/DirectoryServices/DirectoryService.svc"
}


ADWebClient.ajaxCall = function(uri, type, data, callbackSuccess, callbackError)
{
	AuthManager.ajax(uri, type, data, callbackSuccess, callbackError, null, false, WebClient.sessionTimeout);
}

ADWebClient.createUser = function(activeDirectoryUser, callbackSuccess, callbackError) {
	var uri = ADWebClient.ADWebServiceUri + "/User";
	var data = $.toJSON(activeDirectoryUser);
	ADWebClient.ajaxCall(uri, "POST", data, callbackSuccess, callbackError);
}

ADWebClient.getCreatableUsers = function(lastName, firstName, callbackSuccess, callbackError) {
	var uri = ADWebClient.ADWebServiceUri + "/Person/Creatable/" + lastName + "," + firstName;
	ADWebClient.ajaxCall(uri, "POST", null, callbackSuccess, callbackError);
}

ADWebClient.lookupUser = function(firstName, lastName, callbackSuccess, callbackError) {
	var uri = ADWebClient.ADWebServiceUri + "/User/Name/" + lastName + "/" + firstName;
	ADWebClient.ajaxCall(uri, "GET", {}, callbackSuccess, callbackError);
}

ADWebClient.getUserByEmail = function(emailAddress, callbackSuccess, callbackError) {
	var uri = ADWebClient.ADWebServiceUri + "/User/Mail/" + emailAddress;
	ADWebClient.ajaxCall(uri, "GET", {}, callbackSuccess, callbackError);
}

ADWebClient.getUserByCanonicalName = function(cn, callbackSuccess, callbackError) {
	var uri = ADWebClient.ADWebServiceUri + "/User/CN/" + cn;
	ADWebClient.ajaxCall(uri, "GET", {}, callbackSuccess, callbackError);
}