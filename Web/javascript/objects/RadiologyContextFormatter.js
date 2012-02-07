////////////////////////////////////////
// ContextFormatters
//	A context formatter is used to format a CRT Context in HTML.
//
//	*** Context Formatter Interface ***
//	string format(var contextObject): accepts the context object, returns html markup as a string
//
//	*** Notes ***
//	A context formatter may be dependent on CSS.  Dependencies should be documented.
//
//	Dependencies:
//
//	Created: 2009-06-18, Jeremy Richardson
////////////////////////////////////////
var ContextFormatters = {
	formatters : [],
	getFormatter : null
}
ContextFormatters.getFormatter = function(contextTypeName, contextTypeUri) {
	for (var i = 0; i < this.formatters.length; i++) {
		if (this.formatters[i].name == contextTypeName && this.formatters[i].uri == contextTypeUri)
			return this.formatters[i];
	}
	return null;
}
ContextFormatters.format = function(contextTypeName, contextTypeUri, context) {
	var formatter = getFormatter(contextTypeName, contextTypeUri);
	return formatter.format(context);
}


////////////////////////////////////////
// RadiologyContextFormatter
//	Dependencies:
//		ContextFormatters.js
//		StyleSheet.css
//
//	Created: 2009-06-18, Jeremy Richardson
//	Modified: 2009-07-06, John Morgan
//	Modified: 2009-07-13, Jeremy Richardson
//		-Removed applyStyle - added css classes into markup.
////////////////////////////////////////
var RadiologyContextFormatter = {
	name: 'Radiology',
	uri: 'http://partners.org/brigham/criticalresults/v0/context/radiology/',
	format: null		
}
RadiologyContextFormatter.format = function(ctx) {

	var context = RadiologyContextFormatter.template;
	var markup = '<h5>' + 'Radiology Exam' + '</h5><div class=\"context\">';

	//markup += Utility.format("<span>{0} [ MRN: <b>{1}</b> ]<br><i>{2}</i><br>{3} [ Acc: <b>{4}</b> ]<br><i>{5}</i><br><i>{6}</i>", ctx.PatientName.value, ctx.MRN.value, ctx.DOB.value, ctx.ExamDescription.value, ctx.Accession.value, ctx.ExamTime.value, ctx.ExamDate.value);
	markup += Utility.format("<span>Patient: {0}<br>MRN: <b>{1}</b><br>DOB: <i>{2}</i><br>Exam ID: <b>{4}</b><br>Exam Time: <i>{5}</i><br>Exam Date: <i>{6}</i><br>Description: {3}", ctx.PatientName.value, ctx.MRN.value, ctx.DOB.value, ctx.ExamDescription.value, ctx.Accession.value, ctx.ExamTime.value, ctx.ExamDate ? ctx.ExamDate.value : '');

	markup += '</div>';

	return markup;
}

var RadiologyTabularContextFormatter = {
	name: 'RadiologyTabular',
	uri: 'http://partners.org/brigham/criticalresults/v0/context/radiology/',
	format: null
}

RadiologyTabularContextFormatter.format = function(ctx) {

	var context = RadiologyContextFormatter.template;
	var markup = '<table style="width: 100%;">';
	markup += '<tr><td class="ResultData">Patient Name</td><td class="ResultData ResultContent">{0}</td></tr>';
	markup += '<tr><td class="ResultData">Patient DOB</td><td class="ResultData ResultContent">{1}</td></tr>';
	markup += '<tr><td class="ResultData">Patient MRN</td><td class="ResultData ResultContent">{2}</td></tr>';
	markup += '<tr><td class="ResultData">Exam Description</td><td class="ResultData ResultContent">{3}</td></tr>';
	markup += '<tr><td class="ResultData">Exam Time</td><td class="ResultData ResultContent">{4}</td></tr>';
	markup += '<tr><td class="ResultData">Exam Date</td><td class="ResultData ResultContent">{5}</td></tr>';
	markup += '<tr><td class="ResultData">Exam Id</td><td class="ResultData ResultContent">{6}</td></tr>';
	markup += '</table>';

	return Utility.format(markup, ctx.PatientName.value, ctx.DOB.value, ctx.MRN.value, ctx.ExamDescription.value, ctx.ExamTime.value, ctx.ExamDate?ctx.ExamDate.value:'', ctx.Accession.value);
}

ContextFormatters.formatters[ContextFormatters.formatters.length] = RadiologyContextFormatter;
ContextFormatters.formatters[ContextFormatters.formatters.length] = RadiologyTabularContextFormatter;