var AuthManager = {
	BASIC_AUTH_PREFIX: "Token ",
	BASIC_AUTH_HEADER_NAME: "ANCR",

	timeout: 60000,
	isInitialized: false,
	authHeaderValue: null,
	setBasicAuthHeader: function(username, token)
	{
		var authHeaderValue = username + ":" + token;
		this.authHeaderValue = this.BASIC_AUTH_PREFIX + authHeaderValue.encodeBase64();
	},
	setHeader: function(xhr)
	{
		if (this.authHeaderValue != null)
			xhr.setRequestHeader(this.BASIC_AUTH_HEADER_NAME, this.authHeaderValue);
	},
	ajax: function(uri, type, data, callbackSuccess, callbackError, callId, sync, sessionTimeout)
	{
		if (!AuthManager.isInitialized)
		{
			$.ajaxSetup({
				contentType: "application/json",
				dataType: "json",
				timeout: AuthManager.timeout
			});
			AuthManager.isInitialized = true;
		}

		$.ajax(
		{
			async: !sync,
			url: uri,
			type: type,
			data: data,
			cache: false,
			beforeSend: function(xhr)
			{
				AuthManager.setHeader(xhr);
			},
			success: function(msg)
			{
				var ok = true;
				if (typeof (msg.exception) != "undefined" && msg.exception.lastIndexOf("SecurityException") != -1)
				{
					ok = false;
					if (sessionTimeout)
					{
						sessionTimeout();
					}
				}

				if (ok && callbackSuccess)
				{  //want to confirm callback error is not null and not undefined
					if (callId)
					{
						callbackSuccess(msg, callId);
					}
					else
					{
						callbackSuccess(msg);
					}
				}
			},
			error: function(xhr)
			{
				if (callbackError)
				{	//want to confirm callback error is not null and not undefined	//callbackError !== null, callbackError != null
					if (callId)
					{
						callbackError(xhr, callId);
					}
					else
					{
						callbackError(xhr);
					}
				} else if (xhr.responseText)
				{
					alert(xhr.responseText);
				} else
				{
					alert("Unknown server error.");
				}
			}
		});
	}
};