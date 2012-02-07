var SavedFilters = {
	SAVED_FILTER_TYPE: "SavedFilter",
	Filters: null,
	Manager: null,
	onApplyFilter: null,
	onClosed: null
}

SavedFilters.showSavedFiltersDialog = function(userEntries, manager, onApplyFilter, onClosed) {
	SavedFilters.onClosed = onClosed;
	SavedFilters.Manager = manager;
	SavedFilters.onApplyFilter = onApplyFilter;
	SavedFilters.Filters = new Array();
	SavedFilters.refreshSavedFilters(SavedFilters.buildList);
}

SavedFilters.refreshSavedFilters = function(onComplete) {
	WebClient.queryUserEntry(" it.User.UserName = '" + SavedFilters.Manager.AuthenticatedUser.UserName + "' AND it.Type = '" + SavedFilters.SAVED_FILTER_TYPE + "'", null, null, onComplete)
}

SavedFilters.buildList = function(userEntries) {
	SavedFilters.Filters = new Array();
	if (userEntries && userEntries.length > 0) {
		$(userEntries).each(function(index, item) {
			if (item.Type == SavedFilters.SAVED_FILTER_TYPE) {
				SavedFilters.Filters[SavedFilters.Filters.length] = item;
			}
		});
	}

	var html = "<div id='divSavedFilters'>";
	html += "<select id='selSavedFilters' onchange='SavedFilters.applyFilter()'>";
	html += SavedFilters.buildSelectOptions();
	html += "</select>&nbsp;";
	//	html += "<input type='button' onclick='SavedFilters.applyFilter()' value='Apply' />&nbsp;";
	html += "<input type='button' onclick='SavedFilters.deleteFilter()' value='Delete' />";
	html += "<br />";
	html += "<hr /><br />";
	html += "<input type='text' id='txtSavedFilterName' name='addNew' class='hidden'></input>";
	html += "<input type='button' id='btnSaveFilter' onclick='SavedFilters.save()' name='addNew' value='Save' class='hidden' />&nbsp;";
	html += "<input type='button' id='btnSaveCurrent' onclick='SavedFilters.showSave()' value='Save Current Filter' />";
	html += "</div>";

	$(top.document.body).append(html);
	$('#divSavedFilters').dialog({ zIndex: 4000, title: 'My Saved Filters', close: SavedFilters.onMenuClosed });
}

SavedFilters.onRefresh = function(userEntries) {
	SavedFilters.Filters = new Array();
	if (userEntries && userEntries.length > 0) {
		$(userEntries).each(function(index, item) {
			if (item.Type == SavedFilters.SAVED_FILTER_TYPE) {
				SavedFilters.Filters[SavedFilters.Filters.length] = item;
			}
		});
	}
	$('#selSavedFilters').html("");
	$('#selSavedFilters').html(SavedFilters.buildSelectOptions());
}


SavedFilters.buildSelectOptions = function() {
	var html = "<option value='empty'></option>";
	if (SavedFilters.Filters && SavedFilters.Filters.length > 0) {
		$(SavedFilters.Filters).each(function(index, item) {
			html += "<option value='" + item.Key + "'>" + item.Key + "</option>";
		});
	}
	return html;
}

SavedFilters.applyFilter = function() {
	var selectedFilter = $('#selSavedFilters').val();
	if (selectedFilter != "empty") {
		$(SavedFilters.Filters).each(function(index, item) {
			if (item.Key == selectedFilter) {
				var filter = $.evalJSON(item.Value);
				Manager.Filter = filter;
			}        });
        if (SavedFilters.onApplyFilter) {
            SavedFilters.onApplyFilter();
        }
	}
}

SavedFilters.deleteFilter = function() {
	var selectedFilter = $('#selSavedFilters').val();
	if (selectedFilter != "empty") {
		$(SavedFilters.Filters).each(function(index, item) {
			if (item.Key == selectedFilter) {
				var data = { "type": item.Type, "key": item.Key }
				WebClient.deleteUserEntry(Manager.AuthenticatedUser.UserName, data, SavedFilters.onDeleteSuccess);
			}
		});
	}
}

SavedFilters.onDeleteSuccess = function(data) {
	SavedFilters.refreshSavedFilters(SavedFilters.onRefresh);
}

SavedFilters.showSave = function() {
	$('#divSavedFilters').find('[name="addNew"]').show('fast');
	$('#divSavedFilters').find('#btnSaveCurrent').hide('fast');
}

SavedFilters.save = function() {
	var filterName = $('#txtSavedFilterName').val();
	var data = { "type": "SavedFilter", "key": filterName, "value": $.toJSON(Manager.Filter), "xmlValue": "" };
	WebClient.createUserEntry(Manager.AuthenticatedUser.UserName, $.toJSON(data), SavedFilters.onSaveComplete);
}

SavedFilters.onSaveComplete = function(data) {
	SavedFilters.Filters = new Array();
	$(data.UserEntries).each(function(index, item) {
		if (item.Type == SavedFilters.SAVED_FILTER_TYPE) {
			SavedFilters.Filters[SavedFilters.Filters.length] = item;
		}
	});
	$('#selSavedFilters').html(SavedFilters.buildSelectOptions());
	var newFilterName = $('#divSavedFilters').find('#txtSavedFilterName').val();
	$('#selSavedFilters option:contains(' + newFilterName + ')').attr('selected', 'selected');
	
	$('#divSavedFilters').find('[name="addNew"]').hide('fast');
	$('#divSavedFilters').find('#txtSavedFilterName').val("");
	$('#divSavedFilters').find('#btnSaveCurrent').show('fast');

}

SavedFilters.onMenuClosed = function(event, ui) {
	$('#divSavedFilters').dialog('destroy');
	$('#divSavedFilters').remove();
	SavedFilters.onClosed();
}
