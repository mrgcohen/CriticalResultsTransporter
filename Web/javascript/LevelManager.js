var LevelManager = {
    Levels: null,
    Action: null,
    isInitialized: false,
    initCallback: null,
    TableHtml: null,
    SelectedLevel: null,
    UpdatedLevel: null
}

var BASEDATE = new Date("1/1/1900");


var Actions = { CREATE: "create", UPDATE: "update" }

LevelManager.initialize = function(initCallback) {
    LevelManager.initCallback = initCallback;
    LevelManager.getAllLevels();
}

LevelManager.getAllLevels = function() {
    CRManager.WebClient.getAllLevels(LevelManager.getAllLevesSuccess);
}

LevelManager.getAllLevesSuccess = function(data) {
    LevelManager.Levels = data;
    LevelManager.buildLevelTable();
}

LevelManager.buildLevelTable = function() {
    LevelManager.TableHtml = "";
    $(LevelManager.Levels).each(function(i, level) {
        LevelManager.TableHtml += "<tr name='" + level.ColorValue + "'><td>" + level.Name + "</td><td>" + level.ShortDescription +
            "</td><td>" + level.ColorValue + "</td><td>" + LevelManager.formatTimespan(level.EscalationTimeout) + 
            "</td><td>" + LevelManager.formatTimespan(level.DueTimeout) + "</td><td><input type='button' value='Details' onclick='LevelManager.edit(\"" + level.Name + "\")' /></td></tr>";
    });
    LevelManager.isInitialized = true;
    LevelManager.initCallback(LevelManager);
}

LevelManager.edit = function(levelName) {
    LevelManager.Action = Actions.UPDATE;
    $(LevelManager.Levels).each(function(i, level) {
        if (level.Name == levelName) {
            LevelManager.SelectedLevel = level;
        }
    });

    var escTimespan = new Timespan();
    escTimespan.fromDates(Timespan.parseJsonDate(LevelManager.SelectedLevel.EscalationTimeout), BASEDATE);
    if (escTimespan.days > 0) {
        $('#txtLevelEscTime').val(escTimespan.totalDays());
        $('#escTimeInterval').val('0');
    }
    if (escTimespan.hours > 0) {
        $('#txtLevelEscTime').val(escTimespan.totalHours());
        $('#escTimeInterval').val('1');
    }
    if (escTimespan.minutes > 0) {
        $('#txtLevelEscTime').val(escTimespan.totalMinutes());
        $('#escTimeInterval').val('2');
    }

    var dueTimespan = new Timespan();
    dueTimespan.fromDates(Timespan.parseJsonDate(LevelManager.SelectedLevel.DueTimeout), BASEDATE);
    if (dueTimespan.days > 0) {
        $('#txtLevelDueTime').val(dueTimespan.totalDays());
        $('#dueTimeInterval').val('0');
    }
    if (dueTimespan.hours > 0) {
        $('#txtLevelDueTime').val(dueTimespan.totalHours());
        $('#dueTimeInterval').val('1');
    }
    if (dueTimespan.minutes > 0) {
        $('#txtLevelDueTime').val(dueTimespan.totalMinutes());
        $('#dueTimeInterval').val('2');
    }

    $('#txtLevelName').val(LevelManager.SelectedLevel.Name);
    $('#txtLevelSDesc').val(LevelManager.SelectedLevel.ShortDescription);
    $('#txtLevelLDesc').val(LevelManager.SelectedLevel.Description);
    $('#txtLevelColor').val(LevelManager.SelectedLevel.ColorValue);
    $('#chkLevelDirReq').attr({ 'checked': LevelManager.SelectedLevel.DirectContactRequired });
    $('#chkLevelDirReq').attr({ 'defaultChecked': LevelManager.SelectedLevel.DirectContactRequired });

    //Build Communication Details
    $('#levelDetailBody').html("");
    $(TransportManager.Transports).each(function(i, transport) {
        var tableBody = "<tr><td><h4>" + transport.FriendlyName + "</h4></td><td></td></tr>";
        var transportEnabled = false;
        $(LevelManager.SelectedLevel.Transports).each(function(j, ltransport) {
            if (ltransport.Name == transport.Name) {
                transportEnabled = true;
            }
        });
        var transportRequired = false;
        $(LevelManager.SelectedLevel.MandatoryTransports).each(function(j, mtransports) {
            if (transport.Name == mtransports.Name) {
                transportRequired = true;
            }
        });
        var enabledName = (levelName + "_" + transport.Name).replace(/ /g, "-");
        var reqName = ("req_" + levelName + "_" + transport.Name).replace(/ /g, "-");
        if (transportEnabled == true && transportRequired == false) {
            tableBody += "<tr><td><input type='checkbox' checked='checked' name='enabled' id='" + enabledName + "' />&nbsp;Enabled</td>";
        }
        else if (transportEnabled == true && transportRequired == true) {
            tableBody += "<tr><td><input type='checkbox' checked='checked' disabled='disabled' name='enabled' id='" + enabledName + "' />&nbsp;Enabled</td>";
        }
        else {
            tableBody += "<tr><td><input type='checkbox' name='enabled' id='" + enabledName + "' />&nbsp;Enabled</td>";
        }
        if (transportRequired == true) {
            tableBody += "<td><input type='checkbox' checked='checked' name='required' id='req_" + enabledName + "' onclick='LevelManager.tiedCb(\"" + enabledName + "\")' />&nbsp;Required</td></tr>";
        }
        else {
            tableBody += "<td><input type='checkbox' name='required' id='req_" + enabledName + "' onclick='LevelManager.tiedCb(\"" + enabledName + "\")' />&nbsp;Required</td></tr>";

        }
        $('#levelDetailBody').append(tableBody);
    });
    LevelManager.showDetail();
    document.getElementById('txtLevelColor').color.fromString($('#txtLevelColor').val());
}

LevelManager.tiedCb = function(name) {
    var check = $('#req_' + name).attr('checked');
    if (check == true) {
        $('#' + name).attr({ 'checked': true, 'disabled': true });
    }
    else {
        $('#' + name).removeAttr('disabled');
    }
}

LevelManager.formatTimespan = function(jsonDateTime) {
    var escTime = "";
    var e = Timespan.parseJsonDate(jsonDateTime);
    var escTimespan = new Timespan();
    escTimespan.fromDates(e, BASEDATE);
    if (escTimespan.days > 0)
        escTime += escTimespan.days + " d ";
    if (escTimespan.hours > 0)
        escTime += escTimespan.hours + " h ";
    if (escTimespan.minutes > 0)
        escTime += escTimespan.minutes + " m ";
    return escTime;
}
    

LevelManager.setLevelColor = function() {
    $("#levelBody tr").each(function(i, row) {
        Utility.setLevelColor(row, $(row).attr('Name'));
    });
}

LevelManager.closeDetails = function() {
    $('#levelDetails').dialog('close');
}

LevelManager.Submit = function() {
    var updatedLevel = {
        Uuid: "",
        Name: $('#txtLevelName').val(),
        ShortDescription: $('#txtLevelSDesc').val(),
        Description: $('#txtLevelLDesc').val(),
        ColorValue: '#' + $('#txtLevelColor').val(),
        EscalationTimeout: "",
        DueTimeout: "",
        DirectContactRequired: $('#chkLevelDirReq').attr('checked'),
        Transports: new Array(),
        MandatoryTransports: new Array()
    }

    var escTime = null;
    var timespan = new Timespan();
    var escInterval = $('#escTimeInterval option:selected').text();
    var time = $('#txtLevelEscTime').val();
    if (escInterval == "Minutes") {
        timespan.addMinutes(time);
    }
    else if (escInterval == "Hours") {
        timespan.addHours(time);
    }
    else if (escInterval == "Days") {
        timespan.addDays(time);
    }
    escTime = timespan.base + BASEDATE.getTime();
    updatedLevel.EscalationTimeout = "\/Date(" + escTime + "-0000)\/";

    var dueTime = null;
    var timespan2 = new Timespan();
    var dueInterval = $('#dueTimeInterval option:selected').text();
    var time = $('#txtLevelDueTime').val();
    if (dueInterval == "Minutes") {
        timespan2.addMinutes(time);
    }
    else if (dueInterval == "Hours") {
        timespan2.addHours(time);
    }
    else if (dueInterval == "Days") {
        timespan2.addDays(time);
    }
    dueTime = timespan2.base + BASEDATE.getTime();
    updatedLevel.DueTimeout = "\/Date(" + dueTime + "-0000)\/";

    var transports = new Array();
    var mandatoryTransports = new Array();
    
    $("[name='enabled']").each(function(i, item){
        if($(item).attr('checked') == true){
            var spl_item = item.id.split('_');
            var transport = spl_item[1].replace(/-/g, " ");
            $(TransportManager.Transports).each(function(j, tport){
                if(tport.Name == transport){
                    updatedLevel.Transports[updatedLevel.Transports.length] = tport;
                }
            });
        }
    }); 
    
    $("[name='required']").each(function(i, item){
        if($(item).attr('checked') == true){
            var spl_item = item.id.split('_');
            var transport = spl_item[2].replace(/-/g, " ");
            $(TransportManager.Transports).each(function(j, tport){
                if(tport.Name == transport){
                    updatedLevel.MandatoryTransports[updatedLevel.MandatoryTransports.length] = tport;
                }
            });
        }
    });
    
    LevelManager.UpdatedLevel = updatedLevel;
    
    if (LevelManager.Action == Actions.CREATE) {
        updatedLevel.Uuid = "00000000-0000-0000-0000-000000000000";
        CRManager.WebClient.createLevel($.toJSON(updatedLevel), LevelManager.updateCreateSuccess);
    }
    else if (LevelManager.Action == Actions.UPDATE) {
        updatedLevel.Uuid = LevelManager.SelectedLevel.Uuid;
        CRManager.WebClient.updateLevel($.toJSON(updatedLevel), LevelManager.updateCreateSuccess);
    }
}

LevelManager.updateCreateSuccess = function() {
    
    $('#levelDetails').dialog('close');
    LevelManager.initialize(LevelManager.initCallback);
}

LevelManager.createLevel = function() {
    LevelManager.Action = Actions.CREATE;
    $('#txtLevelName').val("");
    $('#txtLevelSDesc').val("");
    $('#txtLevelLDesc').val("");
    $('#txtLevelColor').val("FFFFFF");
    $('#txtLevelEscTime').val("");
    $('#escTimeInterval').val("0");
    $('#txtLevelDueTime').val("");
    $('#dueTimeInterval').val("0");
    $('#levelDetailBody').html("");
    $(TransportManager.Transports).each(function(i, transport) {
        var tableBody = "<tr><td><h4>" + transport.FriendlyName + "</h4></td><td></td></tr>";
        var enabledName = ("New" + "_" + transport.Name).replace(/ /g, "-");
        var reqName = ("req_New" + "_" + transport.Name).replace(/ /g, "-");
        tableBody += "<tr><td><input type='checkbox' name='enabled' id='" + enabledName + "' />&nbsp;Enabled</td>";
        tableBody += "<td><input type='checkbox' name='required' id='" + reqName + "' onclick='LevelManager.tiedCb(\"" + enabledName + "\")' />&nbsp;Required</td></tr>";
        $('#levelDetailBody').append(tableBody);
    });
    LevelManager.showDetail();
    document.getElementById('txtLevelColor').color.fromString($('#txtLevelColor').val());
}

LevelManager.showDetail = function() {
    var x = $(document).width();
    x = x - (x * .05);
    var y = $(document).height();
    y = y - (x * .05);
    $('#levelDetails').dialog({
        modal: true,
        width: x,
        height: y,
        title: "Level Details",
        close: function(event, ui) {
            $('#levelDetails').dialog('destroy');
        }
    });
}