var TimeFilter = {
	FilterData: null,
	Id: null,
	Name: null,
	onOkClick: null,
	onCancelClick: null,
	FilterOptions: null
}

TimeFilter.getFilter = function(timeFilterOptions, filterData, id, name, okClick, cancelClick) {
	TimeFilter.FilterData = filterData;
	TimeFilter.FilterOptions = timeFilterOptions;
	TimeFilter.Id = id;
	TimeFilter.Name = name;
	TimeFilter.onOkClick = okClick;
	TimeFilter.onCancelClick = cancelClick;

	var createOptions = "";
	$(TimeFilter.FilterOptions.createTimeFilterOptions).each(function(index, item) {
	if (item.selected && item.selected === true) {
			createOptions += "<option class='" + escape(item.value) + "' value='" + item.value + "' selected='selected'>" + item.name + "</option>";
		}
		else {
			createOptions += "<option class='" + escape(item.value) + "' value='" + item.value + "'>" + item.name + "</option>";
		}
	});

	var dueOptions = "";
	$(TimeFilter.FilterOptions.dueTimeFilterOptions).each(function(index, item) {
	if (item.selected && item.selected === true) {
			dueOptions += "<option class='" + escape(item.value) + "' value='" + item.value + "' selected='selected'>" + item.name + "</option>";
		}
		else {
			dueOptions += "<option class='" + escape(item.value) + "' value='" + item.value + "'>" + item.name + "</option>";
		}
	});

	var ackOptions = "";
	$(TimeFilter.FilterOptions.ackTimeFilterOptions).each(function(index, item) {
		if (item.selected && item.selected === true) {
			ackOptions += "<option class='" + escape(item.value) + "' value='" + item.value + "' selected='selected'>" + item.name + "</option>";
		}
		else {
			ackOptions += "<option class='" + escape(item.value) + "' value='" + item.value + "'>" + item.name + "</option>";
		}
	});

	var html = "<div id='div" + TimeFilter.Id + "' name='div" + TimeFilter.Name + "'>";
	html += "<table><tr><td>Created</td>";
	html += "<td><select id='sel_created" + TimeFilter.Id + "' name='sel" + TimeFilter.Name + "'>";
	html += createOptions + "</select></td></tr>";
	html += "<tr><td>Due</td>";
	html += "<td><select id='sel_due" + TimeFilter.Id + "'name='sel" + TimeFilter.Name + "'>";
	html += dueOptions + "</select></td></tr>";
	html += "<tr><td>Acknowledged</td>";
	html += "<td><select id='sel_ack" + TimeFilter.Id + "'name='sel" + TimeFilter.Name + "'>";
	html += ackOptions + "</select></td></tr></table>";
	html += "<input type='button' id='btnOk" + TimeFilter.Id + "' onclick='TimeFilter.on_OkClick(event);' value='Filter' />&nbsp;"
	return html;
}

TimeFilter.setCurrentChoices = function(menuObject) {
	if (TimeFilter.FilterData && TimeFilter.FilterData.length > 0) {

		$(TimeFilter.FilterData).each(function(index, item) {

			$(menuObject).find("select").each(function(selIndex, select) {
				var className = null;
				if (select.id.lastIndexOf(item.name) != -1) {
					$(select).children().each(function(optIndex, option) {
						if (item.html.lastIndexOf($(option).val()) != -1) {
							className = escape($(option).val());
						}
					});
					if (className) {
						$(select).find("'." + className + "'").remove();
					}
					$(select).append(item.html);
				}
			});

		});
	}
}

TimeFilter.on_OkClick = function(event) {
	var filterData = new Array();
	var selects = $(event.srcElement).prev().find('select');
	$(selects).each(function(index, item) {
		if (item.id == 'sel_ack' + TimeFilter.Id ||
				item.id == 'sel_created' + TimeFilter.Id ||
				item.id == 'sel_due' + TimeFilter.Id) {

			for (var i = 0; i < item.length; i++) {
				if (item[i].selected == true) {
					var _name = item.id.replace("sel_", "");
					_name = _name.replace(TimeFilter.Id, "");
					var filter = { name: _name, value: $(item).val(), html: item[i].outerHTML }
					filterData[filterData.length] = filter;
				}
			}
		}
	});

	TimeFilter.FilterData = filterData;
	TimeFilter.onOkClick(filterData);

}
