function createModalDialog(id, path) {
    var html = "";
    html += '<div class="modalBackground"></div>';
    html += '<div class="modalContainer">';
    html += '<iframe src="' + path + '" scrolling="auto" class="modal" />';
    html += '</div>';
    $('#' + id).append(html);
}

function showModalDialog(id, height, width) {
	var size = getPageSize();
	$('.modalBackground').css({ "height": size.y + size.scrollY, "width": size.x + size.scrollX });
	$('.modalContainer').css({ "top": (size.y / 2) });
	$('#' + id + ' .modal').css({ "height": height, "width": width, "top": -((height / 2)), "left": -(width / 2), "display": "block" });
	$('#' + id).css({ "display": "block" });
}

function hideModalDialog(id) {
	$('#' + id).css("display", "none");
}

function removeModalDialog(id) {
	$('#' + id).css("display", "none");
	$('#' + id + ' div').remove();
}

function getPageSize() {
	var size = {};
	if (window.pageXOffset) { //Everyone else
		size.y = window.pageYOffset;
		size.x = window.pageXOffset;
	}
	else { //IE
		size.scrollY = document.body.scrollHeight; // || document;
		size.scrollX = document.body.scrollLeft || document.documentElement.scrollLeft;
		size.y = document.body.offsetHeight;
		size.x = document.body.offsetWidth;
	}
	return size;
}

