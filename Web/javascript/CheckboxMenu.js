

function CheckboxMenu(linkId, data, menuId, callback) {
    this.linkId = linkId;
    this.data = data;
    this.menuId = menuId;
    this.callback = callback;
    this.html = '';
    
    this.hide = function(menuId) {
        $('#' + menuId).remove();
    }

    this.show = function(menuId) {
        $('#' + menuId).css("display", "block");
    }

    this.genHtml = function() {
        this.html = '<div class="menu" id="' + this.menuId + '" onblur="menu.hide(\'' + this.menuId + '\')">';
        for (var i = 0; i < this.data.length; i++) {
            this.html += '<input type="checkbox" id="' + this.data[i].name + '"';
            if (this.data[i].value === "true" || this.data[i].value === true) {
                this.html += 'checked="checked"';
            }
            this.html += '">' + this.data[i].name + '</input>';
            this.html += '<br/>';
        }
        this.html += '<input type="button" onclick="submit(\'' + this.menuId + '\',' + callback + ')" value="submit" />';
        this.html += '<input type="button" onclick="CheckboxMenu.hide(\'' + this.menuId + '\')" value="cancel" /></div>';
    }

}

//this is a static method attached to the object
CheckboxMenu.click = function(event, menuObj, callback, data) {
	if (data) {
		menuObj.data = data;
	}
	if (callback != null) {
		menuObj.callback = callback;
	}
	if (event.srcElement) {
		menuObj.genHtml();
		event.srcElement.outerHTML += menuObj.html; //per microsoft
	}
	else if (event.target) {
		menuObj.genHtml();
		event.target.parentNode.innerHTML += menuObj.html; //per everyone else
	}

	menuObj.show(menuObj.menuId);
}

CheckboxMenu.isFiltered = function(data) {
	var filtered = false;
	for (var i = 0; i < data.length; i++) {
		if (data[i].value === false) {
			filtered = true;
		}
	}
	return filtered;
}

CheckboxMenu.hide = function(menuId) {
    $('#' + menuId).remove();
}

CheckboxMenu.show = function(menuId) {
    $('#' + menuId).css({"display":"block"});
}

function submit(menuId, callback) {
    var data = new menuState();
    $("#" + menuId + " input:checkbox").each(function(index, item) { data.addState(index, item) });
    var atLeastOneChecked = false;
    for (var i = 0; i < data.state.length; i++) {
    	if (data.state[i].value === true) {
    		atLeastOneChecked = true;
    	}
    }
    if (atLeastOneChecked == true) {
    	callback(data.state);
    	CheckboxMenu.hide(menuId);
    }
    else {
    	alert("You must choose at least one selection to display.");
    }
    
}

function menuState() {
    this.state = [];

    this.addState = function(index, item) {
        var s = { "name": item.id, "value": item.checked };
        this.state[this.state.length] = s;
    }
}