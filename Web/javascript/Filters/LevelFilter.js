
var LevelFilter = {
	FilterData: null,	Id: null,	Name: null,	onOkClick: null,	onCancelClick: null}

LevelFilter.getFilter = function(levelFilterData, id, name, okClick, cancelClick) {
	LevelFilter.FilterData = levelFilterData;
	LevelFilter.Id = id;
	LevelFilter.Name = name;
	LevelFilter.onOkClick = okClick;
	LevelFilter.onCancelClick = cancelClick;
	var html = "<div id='div" + LevelFilter.Id + "' name='div" + LevelFilter.Name + "'><ul id='ul" + LevelFilter.Id + "' name='ul" + LevelFilter.Name + "'>";
	for (var i = 0; i < LevelFilter.FilterData.length; i++) {
		if (LevelFilter.FilterData[i].value === true) {
			html += "<li id='li" + LevelFilter.Id + i + "' name='li" + LevelFilter.Name + "'><input type='checkbox' id='chk" + LevelFilter.Id + i + "' name='chk" + LevelFilter.Name + "' checked='checked' /><label for=chk" + LevelFilter.Id + i + "'>" + LevelFilter.FilterData[i].name + "</label></li>";
		}
		else {
			html += "<li id='li" + LevelFilter.Id + i + "' name='li" + LevelFilter.Name + "'><input type='checkbox' id='chk" + LevelFilter.Id + i + "' name='chk" + LevelFilter.Name + "'  /><label for=chk" + LevelFilter.Id + i + "'>" + LevelFilter.FilterData[i].name + "</label></li>";
		}
	}
	html += "</ul>";
	html += "<input type='button' id='btnOk" + LevelFilter.Id + "' onclick='LevelFilter.on_OkClick(event);' value='Filter' /></div>"
	return html;
}

LevelFilter.on_OkClick = function(event) {
	var checkboxes = $(event.srcElement).prev().children();
	LevelFilter.FilterData = new Array();
	$(checkboxes).each(function(index, item) {
		var label = $(item).find('label')[0];
		var labelname = $(label).text();
		var checked = $(item).find('input').attr('checked');
		LevelFilter.FilterData[LevelFilter.FilterData.length] = { name: labelname, value: checked };
	});
	LevelFilter.onOkClick(LevelFilter.FilterData);
}

