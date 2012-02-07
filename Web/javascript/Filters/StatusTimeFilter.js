var StatusTimeFilter = {
	html: null,
	TimeFilterData: null,
	StatusFilterData: null,
	Id: null,
	Name: null,
	onOkClick: null,
	onCancelClick: null,	CRManager: null}

var LAST_COMPLETE_FILTER_SETTING = "lastCompleteFilter";

StatusTimeFilter.initialize = function(manager) {
	StatusTimeFilter.CRManager = manager;
	var storedCompleteFilter = "";
	$(manager.AuthenticatedUser.UserEntries).each(function(index, setting) {
		if (setting.Key == LAST_COMPLETE_FILTER_SETTING && setting.Type == "FilterSetting") {
			storedCompleteFilter = setting.Value;
		}
	});

	StatusTimeFilter.html = '<table style="border: none">' +
	'<tr><td>Pending</td><td><select id="selPending" style="width:175px;">' +
	'		<option value="none">None</option>' +
	'		<option value="" selected="selected">All</option>' +
	'		<option value="3600000">Due Next 1 hour</option>' +
	'		<option value="10800000">Due Next 3 hours</option>' +
	'		<option value="43200000">Due Next 12 hours</option>' +
	'		<option value="86400000">Due Next 24 hours</option>' +
	'		<option value="259200000">Due Next 3 days</option>' +
	'	</select></td></tr>' +
	'<tr><td>Overdue</td><td><select id="selOverdue" style="width:175px;">' +
	'		<option value="" selected="selected">All</option>' +
	'	</select></td></tr>' +
	'<tr><td>Acknowledged</td><td><select id="selComplete" style="width:175px;">' +
	'		<option value="none" selected="selected">None</option>' +
	'		<option value="-3600000">Last 1 hour</option>' +
	'		<option value="-10800000">Last 3 hours</option>' +
	'		<option value="-43200000">Last 12 hours</option>' +
	'		<option value="-86400000">Last 24 hours</option>' +
	'		<option value="-259200000">Last 3 days</option>' +
	'		<option value="-604800000">Last 7 days</option>' +
	'	</select></td></tr>' +
	'<tr><td colspan="2"><input type="button" id="btnFilter" value="Filter" onclick="StatusTimeFilter.on_OkClick(event);" /></td></tr>' +
	'</table>';

}

StatusTimeFilter.getHtml = function(statusFilterData, timeFilterData, id, name, okClick, cancelClick) {
	StatusTimeFilter.StatusFilterData = statusFilterData;
	StatusTimeFilter.TimeFilterData = timeFilterData;
	StatusTimeFilter.onOkClick = okClick;
	StatusTimeFilter.onCancelClick = cancelClick;
	//StatusTimeFilter.initialize();
	return StatusTimeFilter.html;
}

StatusTimeFilter.applySavedFilter = function(obj, statusFilterData, timeFilterData) {
	StatusTimeFilter.StatusFilterData = statusFilterData;
	StatusTimeFilter.TimeFilterData = timeFilterData;
	$(StatusTimeFilter.TimeFilterData).each(function(i, filter) {
		$(obj).find(' #sel' + filter.name).val(filter.value);
	});
}

StatusTimeFilter.on_OkClick = function(event) {
	StatusTimeFilter.TimeFilterData = new Array();
	StatusTimeFilter.StatusFilterData = new Array();
	StatusTimeFilter.TimeFilterData[0] = {
		name: "Complete",
		value: $(event.srcElement).parent().parent().parent().find('#selComplete').val()
	};
	var data = {
		type: "FilterSetting",
		key: LAST_COMPLETE_FILTER_SETTING,
		value: { value: StatusTimeFilter.TimeFilterData[0].value, name: $(event.srcElement).parent().parent().parent().find('#selComplete :selected').text() },
		xmlValue: ""
	};
	data.value = $.toJSON(data.value);
	StatusTimeFilter.CRManager.createUserEntry($.toJSON(data));
	StatusTimeFilter.TimeFilterData[1] = {
		name: "Pending",
		value: $(event.srcElement).parent().parent().parent().find('#selPending').val()
	};
	StatusTimeFilter.TimeFilterData[2] = {
		name: "Overdue",
		value: $(event.srcElement).parent().parent().parent().find('#selOverdue').val()
	};
	var bComplete = true;
	if ($(event.srcElement).parent().parent().parent().find('#selComplete').val() == "none") {
		bComplete = false;
	}
	StatusTimeFilter.StatusFilterData[0] = {
		name: "Complete",
		value: bComplete,
		text: $(event.srcElement).parent().parent().parent().find('#selComplete :selected').text()
	};
	var bPending = true;
	if ($(event.srcElement).parent().parent().parent().find('#selPending').val() == "none") {
		bPending = false;
	}
	StatusTimeFilter.StatusFilterData[1] = {
		name: "Pending",
		value: bPending,
		text: $(event.srcElement).parent().parent().parent().find('#selPending :selected').text()
	};
	var bOverdue = true;
	if ($(event.srcElement).parent().parent().parent().find('#selOverdue').val() == "none") {
		bOverdue = false;
	}
	StatusTimeFilter.StatusFilterData[2] = {
		name: "Overdue",
		value: bOverdue,
		text: $(event.srcElement).parent().parent().parent().find('#selOverdue :selected').text()
	};
	StatusTimeFilter.onOkClick({ TimeFilter: StatusTimeFilter.TimeFilterData, StatusFilter: StatusTimeFilter.StatusFilterData });
}