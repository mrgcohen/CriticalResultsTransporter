var TransportManager = {
    Action: null,
    Transports: null,
    initCallback: null,
    isInitialized: false,
    TableHtml: "",
    SelectedTransport: null,
    UpdatedTransport: null
}

var Actions = { UPDATE: "update", CREATE: "create" }

TransportManager.initialize = function(initCallback) {
    TransportManager.initCallback = initCallback;
    TransportManager.getAllTransports();
}

TransportManager.getAllTransports = function() {
    CRManager.WebClient.getAllTransports(TransportManager.getAllTransportsSuccess);
}

TransportManager.getAllTransportsSuccess = function(data) {
    TransportManager.Transports = data;
    TransportManager.buildTableHtml();
}

TransportManager.buildTableHtml = function() {
    TransportManager.TableHtml = "";
    $(TransportManager.Transports).each(function(i, transport) {
        if(i % 2 == 0)
            TransportManager.TableHtml += "<tr class='oddRow'>";
        else
         TransportManager.TableHtml += "<tr class='evenRow'>";
        TransportManager.TableHtml += "<td>" + transport.Name + "</td><td>" + transport.FriendlyName + "</td><td>" +
            transport.TransportUri + "</td><td>" + "<input type='button' value='Details' onclick='TransportManager.showDetails(\"" + transport.Name + "\")' /></td></tr>";
    });
    $('#transportBody').html(TransportManager.TableHtml);

    TransportManager.isInitialized = true;
    TransportManager.initCallback(TransportManager);
}

TransportManager.showDetails = function(transportName) {
    TransportManager.Action = Actions.UPDATE;
    $(TransportManager.Transports).each(function(i, transport) {
        if (transport.Name == transportName) {
            TransportManager.SelectedTransport = transport;
        }
    });

    $('#txtTranName').val(TransportManager.SelectedTransport.Name);
    $('#txtTranFName').val(TransportManager.SelectedTransport.FriendlyName);
    $('#txtTranUri').val(TransportManager.SelectedTransport.TransportUri);

    $('#transportDetailBody').html("");

    $(TransportManager.SelectedTransport.Levels).each(function(i, level) {
        var mandatory = false;
        $(TransportManager.SelectedTransport.MandatoryLevels).each(function(j, mLevel) {
            if (mLevel.Name == level.Name) {
                mandatory = true;
            }
        });
        if (mandatory == true) {
            $('#transportDetailBody').append("<tr name='transportLevelRow' id='" + level.ColorValue + "'><td>" + level.Name + "&nbsp;(Mandatory)</td></tr>")
        }
        else {
            $('#transportDetailBody').append("<tr name='transportLevelRow' id='" + level.ColorValue + "'><td>" + level.Name + "</td></tr>")
        }
    });
    $('[name="transportLevelRow"]').each(function(index, row) {
        Utility.setLevelColor(row, $(row).attr('Id'));
    });

    TransportManager.showDetailDialog();
}

TransportManager.createTransport = function() {
    TransportManager.Action = Actions.CREATE;
    $('#txtTranName').val("");
    $('#txtTranFName').val("");
    $('#txtTranUri').val("");

    TransportManager.showDetailDialog();
}

TransportManager.showDetailDialog = function() {
    var x = $(document).width();
    x = Math.round(x - (x * .07));
    var y = $(document).height();
    y = Math.round(y - (x * .07));
    $('#transportDetails').dialog({
        modal: true,
        width: x,
        height: y,
        title: "Transport Details",
        close: function(event, ui) {
            $('#transportDetails').dialog('destroy');
        }
    });
}

TransportManager.closeDetails = function() {
    $('#transportDetails').dialog('close');
}

TransportManager.submit = function() {
    if (TransportManager.Action == Actions.UPDATE) {
        TransportManager.UpdatedTransport = {
            origName: TransportManager.SelectedTransport.Name,
            origUri: TransportManager.SelectedTransport.TransportUri,
            newName: $('#txtTranName').val(),
            newUri: $('#txtTranUri').val(),
            friendlyName: $('#txtTranFName').val()
        }
        CRManager.WebClient.updateTransport($.toJSON(TransportManager.UpdatedTransport), TransportManager.onUpdateCreateSuccess);
    }
    if (TransportManager.Action == Actions.CREATE) {
        var transport = {
            transportUri: $('#txtTranUri').val(),
            friendlyName: $('#txtTranFName').val()
        }
        CRManager.WebClient.createTransport($('#txtTranName').val(), $.toJSON(transport), TransportManager.onUpdateCreateSuccess);
    }
}


TransportManager.onUpdateCreateSuccess = function(data) {
    $('#transportDetails').dialog('close');
    TransportManager.initialize(TransportManager.initCallback);
}