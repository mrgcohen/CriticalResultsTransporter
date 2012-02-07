
var StatusFilter = {
	FilterData: null,
	Id: null,
	Name: null,
	onOkClick: null,
	onCancelClick: null	}

StatusFilter.getFilter = function(statsFilterData, id, name, okClick, cancelClick) {
	StatusFilter.FilterData = statsFilterData;
	StatusFilter.Id = id;
	StatusFilter.Name = name;
	StatusFilter.onOkClick = okClick;
	StatusFilter.onCancelClick = cancelClick;

	var html = "<div id='div" + StatusFilter.Id + "' name='div" + StatusFilter.Name + "'><ul id='ul" + StatusFilter.Id + "' name='ul" + StatusFilter.Name + "'>";
	for (var i = 0; i < StatusFilter.FilterData.length; i++) {
		if (StatusFilter.FilterData[i].value === true || StatusFilter.FilterData[i].value === "true") {
			html += "<li id='li" + StatusFilter.Id + i + "' name='li" + StatusFilter.Name + "'><input type='checkbox' id='chk" + StatusFilter.Id + i + "' name='chk" + StatusFilter.Name + "' checked='checked' /><label for=chk" + StatusFilter.Id + i + "'>" + StatusFilter.FilterData[i].name + "</label></li>";
		}
		else {
			html += "<li id='li" + StatusFilter.Id + i + "' name='li" + StatusFilter.Name + "'><input type='checkbox' id='chk" + StatusFilter.Id + i + "' name='chk" + StatusFilter.Name + "' /><label for=chk" + StatusFilter.Id + i + "'>" + StatusFilter.FilterData[i].name + "</label></li>";
		}
	}
	html += "</ul><input type='button' id='btnOk" + StatusFilter.Id + "' onclick='StatusFilter.on_OkClick(event);' value='Filter' /></div>"
	return html;
}

StatusFilter.on_OkClick = function(event) {
	var checkboxes = $(event.srcElement).prev().children();
	StatusFilter.FilterData = new Array();
	$(checkboxes).each(function(index, item) {
		var checkbox = $(item).children()[0];
		var label = $(item).children()[1];
		var labelname = $(label).text();
		StatusFilter.FilterData[StatusFilter.FilterData.length] = { name: labelname, value: checkbox.checked };
	});
	StatusFilter.onOkClick(StatusFilter.FilterData);
}