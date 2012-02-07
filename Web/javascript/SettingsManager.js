var SettingsManager = {
    Settings: null,
    isInitialized: false,
    onInitCallback: null,
    SystemSettingsOwner: "System",
    tableHtml: ""
}

SettingsManager.initialize = function(initCallback) {
    SettingsManager.onInitCallback = initCallback;
    CRManager.WebClient.getAllSettings(SettingsManager.getSettingsSuccess);
}

SettingsManager.getSettingsSuccess = function(data) {
    SettingsManager.Settings = data;
    SettingsManager.buildTableHtml();
}

SettingsManager.buildTableHtml = function() {
    SettingsManager.tableHtml = "";
    $(SettingsManager.Settings).each(function(i, setting) {
        if (i % 2 == 0)
            SettingsManager.tableHtml += "<tr class='evenRow'>";
        else
            SettingsManager.tableHtml += "<tr class='oddRow'>";           
        SettingsManager.tableHtml += "<td>" + setting.Owner + 
            "</td><td>" + setting.EntryKey + "</td><td>" +
            "<textarea id='" + setting.Uuid + "' class='text'>" + setting.Value + "</textarea></td><td align='center'>" +
            "<input type='button' value='Save' id='btn_" + setting.Uuid + "' onclick='SettingsManager.submit(\"" + setting.Uuid + "\")' /></td></tr>";
    });
    $('#settingsBody').html(SettingsManager.tableHtml);

    $('#settingsBody tr:not(:hidden):nth-child(odd)').addClass("oddRow");
    $('#settingsBody tr:not(:hidden):nth-child(even)').addClass("evenRow");
    SettingsManager.isInitialized = true;
    SettingsManager.onInitCallback(this);
}

SettingsManager.submit = function(uuid) {
    var ok = confirm("This will change settings necassary for ANCR to function, are you sure?");
    if (ok == true) {
        var data = {
            uuid: uuid,
            value: $('#' + uuid).val()
        }
        CRManager.WebClient.updateSetting(SettingsManager.SystemSettingsOwner, $.toJSON(data), SettingsManager.onUpdateSuccess);
    }
}

SettingsManager.onUpdateSuccess = function(data) {
    SettingsManager.initialize(SettingsManager.onInitCallback);
}
