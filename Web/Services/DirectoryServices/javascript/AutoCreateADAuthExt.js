AutoCreateADAuthExt = {
	userObj: null,
	email: null,
	adSearchResult: null,
	onComplete: null,
	LogonDomain: DefaultDomain,
	EmailTransportName: "SMTP Transport"
}

AutoCreateADAuthExt.CheckAccountByUserObject = function(Manager, userObj, onComplete) {
	var create = true;
	$(userObj.UserEntries).each(function(index, item) {
		if (item.Type == "AuthExt" && item.Key == "Windows") {
			create = false;
		}
	});
	if (create == true) {
		AutoCreateADAuthExt.userObj = userObj;
		AutoCreateADAuthExt.onComplete = onComplete;
		$(userObj.Transports).each(function(index, item) {
			if (item.Transport.Name == AutoCreateADAuthExt.EmailTransportName) {
				AutoCreateADAuthExt.email = item.Address;
			}
		});
		if (AutoCreateADAuthExt.email != null && AutoCreateADAuthExt.email != "") {
			ADWebClient.getUserByEmail(AutoCreateADAuthExt.email, AutoCreateADAuthExt.onADSearchSuccess, AutoCreateADAuthExt.onADSearchError);
		}
	}
}

AutoCreateADAuthExt.onADSearchError = function(data) { return; }

AutoCreateADAuthExt.onADSearchSuccess = function(data) {
	AutoCreateADAuthExt.adSearchResult = data;
	if (data.length){
		if(data.length > 0) {
			var adUserName = "";
			$(data[0].Properties).each(function(index, item) {
				if (item.key == "cn") {
					adUserName = item.value;
				}
			});
			if (adUserName != "") {
				var AuthExt = {
					type: "AuthExt",
					key: "Windows",
					value: AutoCreateADAuthExt.LogonDomain + "\\" + adUserName,
					xmlValue: ""
				}
				WebClient.createUpdateUserEntry(AutoCreateADAuthExt.email, $.toJSON(AuthExt), AutoCreateADAuthExt.onComplete);
			}
			else {
				if (AutoCreateADAuthExt.onComplete != null) {
					AutoCreateADAuthExt.onComplete();
				}
			} 
		}
	}
}

