var UserFilter = {
	FilterData: {
		Senders: null,
		Receivers: null
	},
	Id: null,
	Name: null,
	onOkClick: null,
	onCancelClick: null,
	jQueryObj: null,
	ReceiverAutoCompleteArray: null,
	SenderAutoCompleteArray: null
}
UserFilter.getFilter = function(id, name, okClick, cancelClick) {
	UserFilter.Id = id;
	UserFilter.Name = name;
	UserFilter.onOkClick = okClick;
	UserFilter.onCancelClick = cancelClick;
	var html = "<div id='divUserFilter" + UserFilter.Id + "'>";
	html += "<h5>Senders</h5><br/>";
	html += "<ul class='filterUL' id='senderList" + UserFilter.Id + "'>";
	if (UserFilter.FilterData.Senders) {
		$(UserFilter.FilterData.Senders).each(function(index, item) {
			html += "<li><span>" + item + "</span>&nbsp;<a href='#' onclick='UserFilter.remove(event)'>remove</a></li>";
		});
	}
	html += "</ul>";
	html += "<input type='text' id='userFilterSender" + UserFilter.Id + "' style='display: none;' />&nbsp;<a href='#' id='addSender" + UserFilter.Id + "' onclick='UserFilter.addSender(event)' class='hidden'>submit</a><a id='newSender" + UserFilter.Id + "' href='#' onclick='UserFilter.newSender(event)' >add</a><br />";
	html += "<hr />";
	html += "<h5>Receivers</h5><br/>";
	html += "<ul class='filterUL' id='receiverList" + UserFilter.Id + "'>";
	if (UserFilter.FilterData.Receivers) {
		$(UserFilter.FilterData.Receivers).each(function(index, item) {
			html += "<li><span>" + item + "</span>&nbsp;<a href='#' onclick='UserFilter.remove(event)'>remove</a></li>";
		});
	}
	html += "</ul>";
	html += "<input type='text' id='userFilterReceiver" + UserFilter.Id + "' />&nbsp;<a id='addReceiver" + UserFilter.Id + "' href='#' onclick='UserFilter.addReceiver(event)' class='hidden'>submit</a><a id='newReceiver" + UserFilter.Id + "' href='#' onclick='UserFilter.newReceiver(event)' >add</a></div>";
	html += "<br /><input type='button' id='btnOk" + UserFilter.Id + "' onclick='UserFilter.on_OkClick(event)' value='Ok' />";
	return html;
}

UserFilter.addReceiver = function(event) {
	var input = $(event.srcElement).parent().find('#userFilterReceiver' + UserFilter.Id).val();
	if (input != "") {
		$(event.srcElement).parent().find('#receiverList' + UserFilter.Id).append("<li><span>" + input + "</span>&nbsp;<a href='#' onclick='UserFilter.remove(event)'>remove</a></li>");
		$(event.srcElement).parent().find('#userFilterReceiver' + UserFilter.Id).val("");
		$(event.srcElement).parent().find('#userFilterReceiver' + UserFilter.Id).hide('fast');
		$(event.srcElement).parent().find('#newReceiver' + UserFilter.Id).show('fast');
		$(event.srcElement).hide('fast');
	}
}

UserFilter.addSender = function(event) {
	var input = $(event.srcElement).parent().find('#userFilterSender' + UserFilter.Id).val();
	if (input != "") {
		$(event.srcElement).parent().find('#senderList' + UserFilter.Id).append("<li><span>" + input + "</span>&nbsp;<a href='#' onclick='UserFilter.remove(event)'>remove</a></li>");
		$(event.srcElement).parent().find('#userFilterSender' + UserFilter.Id).val("");
		$(event.srcElement).parent().find('#userFilterSender' + UserFilter.Id).hide('fast');
		$(event.srcElement).parent().find('#newSender' + UserFilter.Id).show('fast');
		$(event.srcElement).hide('fast');
	}
}

UserFilter.remove = function(event) {
	$(event.srcElement).parent().remove();
}

UserFilter.on_OkClick = function(event){
	var senderFilter = new Array();
	$(event.srcElement).parent().find('#senderList' + UserFilter.Id).children().each(function(index, item){
		senderFilter[senderFilter.length] = $(item).children('span').text();
	});
	var receiverFilter = new Array();
	$(event.srcElement).parent().find('#receiverList' + UserFilter.Id).children().each(function(index, item){
		receiverFilter[receiverFilter.length] = $(item).children('span').text();
	});
	UserFilter.FilterData.Senders = senderFilter;
	UserFilter.FilterData.Receivers = receiverFilter;
	UserFilter.onOkClick(UserFilter.FilterData);
}

UserFilter.newSender = function(event) {
	$(event.srcElement).parent().find('#userFilterSender' + UserFilter.Id).show('fast');
	$(event.srcElement).parent().find('#userFilterSender' + UserFilter.Id).autocompleteArray(['test1','test2','test3'], { onItemSelect: UserFilter.onSenderSelected });
//	$(event.srcElement).parent().find('#userFilterSender' + UserFilter.Id).attr('autocomplete', 'on');
	$(event.srcElement).parent().find('#addSender' + UserFilter.Id).show('fast');
	$(event.srcElement).hide('fast');
}

UserFilter.newReceiver = function(event){
	$(event.srcElement).parent().find('#userFilterReceiver' + UserFilter.Id).show('fast');
	$(event.srcElement).parent().find('#userFilterReceiver' + UserFilter.Id).autocompleteArray(UserFilter.ReceiverAutoCompleteArray, { onItemSelect: UserFilter.onReceiverSelected });

//	$(event.srcElement).parent().find('#userFilterReceiver' + UserFilter.Id).attr('autocomplete', 'on');
	$(event.srcElement).parent().find('#addReceiver' + UserFilter.Id).show('fast');
	$(event.srcElement).hide('fast');
}



UserFilter.enableAutoComplete = function(receiverArray, senderArray, modalObj) {
	UserFilter.jQueryObj = modalObj;
	UserFilter.ReceiverAutoCompleteArray = receiverArray;
	UserFilter.SenderAutoCompleteArray = senderArray;
	$(modalObj).find('#userFilterSender' + UserFilter.Id).autocompleteArray(UserFilter.SenderAutoCompleteArray, { onItemSelect: UserFilter.onSenderSelected });
	$(modalObj).find('#userFilterReceiver' + UserFilter.Id).autocompleteArray(UserFilter.ReceiverAutoCompleteArray, { onItemSelect: UserFilter.onReceiverSelected });

}

UserFilter.onSenderSelected = function(listItem) {
	var userName = listItem.substring(listItem.lastIndexOf('('), listItem.lastIndexOf(')'));
	$(UserFilter.jQueryObj).find('#userFilterSender' + UserFilter.Id).val(userName);
}

UserFilter.onReceiverSelected = function(listItem) {
	var userName = listItem.substring(listItem.lastIndexOf('('), listItem.lastIndexOf(')'));
	$(UserFilter.jQueryObj).find('#userFilterReceiver' + UserFilter.Id).val(userName);
}
