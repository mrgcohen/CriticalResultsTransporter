var AcknowledgeResult = {
	close: null,
	result: null,
	callback: null
}

AcknowledgeResult.initialize = function(result) {
	AcknowledgeResult.result = result;
}

AcknowledgeResult.Submit = function(close, notes, onSuccessCallback) {
	AcknowledgeResult.close = close;
	AcknowledgeResult.callback = onSuccessCallback;
	if (close === true) {
		WebClient.createAcknowledgment(AcknowledgeResult.result.Uuid, Manager.AuthenticatedUser.UserName, notes, AcknowledgeResult.onAcknowledgeSuccess, null);
	}
	else {
		WebClient.createAcknowledgment(AcknowledgeResult.result.Uuid, Manager.AuthenticatedUser.UserName, notes, AcknowledgeResult.onAcknowledgeSuccess, null);
	}
}

AcknowledgeResult.onAcknowledgeSuccess = function(data) {
	if (AcknowledgeResult.callback != null) {
		AcknowledgeResult.callback(data);
	}
	$(Manager.ListResults).each(function(i, result) {
		if (result.ResultUuid == AcknowledgeResult.result.Uuid) {
			result.AcknowledgmentTime = new Date();
		}
	});
	if (AcknowledgeResult.close === 'true') {
		Manager.closeAlert();
		Manager.closeModal(null, true);
	}
}

AcknowledgeResult.userIsReceiver = function() {
	if (Manager.AuthenticatedUser.UserName == AcknowledgeResult.result.Receiver.UserName) {
		return true;
	}
	return false;
}

AcknowledgeResult.confirmCloseWithoutAck = function(event, ui) {
	var a = 1;
	a++;
	if (AcknowledgeResult.userIsReceiver() == true) {
		if (Manager.ActiveRole == Manager.ROLE_RECEIVER) {
			var resultAck = true;
			$(Manager.ListResults).each(function(i, result) {
				if (result.ResultUuid == AcknowledgeResult.result.Uuid) {
					if (result.AcknowledgmentTime == null) {
						resultAck = false;
					}
				}
			});
			if (resultAck == false) {
				Manager.showAlert("<center>The result has not been acknowledged.<br />Would you like to acknowledge the result?<br /><input type='button' value='Acknowledge and Close' onclick='Manager.AcknowledgeResult.ackFromConfirm();' />&nbsp;&nbsp;&nbsp;<input type='button' value='No' onclick='Manager.modalConfirmCallback(false);' /></center>");
				return false;
			}

		}
		else {
			return true;
		}
	}
	else {
		return true;
	}
}

AcknowledgeResult.ackFromConfirm = function() {
	AcknowledgeResult.Submit('true', "I have reviewed and understand the results.");
}