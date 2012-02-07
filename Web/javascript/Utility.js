var Utility = {};

Utility.format = function(text) {
	for (var i = 1; i < arguments.length; i++){
		var pattern = "\\{" + (i - 1) + "\\}";
		var regexp = new RegExp(pattern, 'g');
		text = text.replace(regexp, arguments[i]);
	}
	return text;
}

Utility.queryString = function(decode) {
	var raw = window.location.search.substring(1);

	if (decode)
		raw = decodeURI(raw);

	var components = raw.split('&');
	var values = {};
	$.each(components, function(item, component) {
		var keyValue = component.split('=');
		values[keyValue[0]] = keyValue[1];
	});
	return values;
}

/// 
/// Description:
///		Parses a json date string and returns a date object
///	Notes:
///		Validates that a timezone exists, but does not account for timezone when returning date
/// Future:
///		Check the timezone of the local machine versus the timezone in the returned json string
///		Confirm that this works with other serialized json time formats
Utility.parseJsonDate = function(jsonDateString) {
	var validateDateRegExp = /Date\(\d+\-\d+\)/;
	var matchDateRegExp = /\d+/;

	var ok = validateDateRegExp.exec(jsonDateString);
	if (!ok)
		return null;

	var dateString = jsonDateString.match(matchDateRegExp.source);

	var date = new Date();
	date.setTime(dateString[0]);

	return date;
}

///
/// Usage: 
///	var pos = new Utility.ClientPosition(myElement);
///	alert( pos.x + "," + pos.y );
///
Utility.ClientPosition = function(element) {
	this.x = 0;
	this.y = 0;

	do {
		this.x += element.offsetX;
		this.y += element.offsetY;
	} while (element = element.offsetParent);
}

Utility.setLevelColor = function(object, hexColorCode) {
	var color = hexColorCode.replace('#', '');
	var red = parseInt(color.substring(0, 2), 16);
	var blue = parseInt(color.substring(2, 4), 16);
	var yellow = parseInt(color.substring(4), 16);
	var average = ((red / 255) + (blue / 255) + (yellow / 255)) / 3;
	average = average * 100;
	var foreColor = '#000000';
	if(average <= 50){
		foreColor = '#FFFFFF';
	}
	$(object).css({ 'background-color': hexColorCode, 'color': foreColor });
}

Utility.escapeHtmlPunctuation = function(text) {
	text = encodeURIComponent(text);
	return text;
}

Utility.unescapeHtmlPunctuation = function(text) {
	text = decodeURIComponent(text);
	return text;
}

Utility.htmlEscapeCharLookup = [
	["!","&#33;"],	["\"","&#34;"],	["\"","&quot;"],	["#","&#35;"],	["$","&#36;"],	["%","&#37;"],	["&","&#38"],	["&","&amp;"],	["\'","&#39;"],	["\\x28", "&#40;"],	["\\x29", "&#41;"],	["*","&#42;"],	["+","&#43;"],	[",","&#44;"],	["-","&#45;"],		[".","&#46;"],	["/","&#47;"],	[":","&#58;"],	[";","&#59;"],		["<","&#60;"],		["<","&lt;"],
	["=","&#61;"],		[">","&#62;"],	[">","&gt;"],
	["\\","&#92;"],	["\\x5B","&#91;"],	["\\x5D","&#93;"],	["^","&#94;"],	["_","&#95;"],	["`","&#96;"]	]



