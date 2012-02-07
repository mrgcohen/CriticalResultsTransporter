var UserManager = {
	Users: null,
	TableHtml: null,
	isInitialized: false,
	initCallback: null,
	SelectedUser: null,
	UpdatedUser: null,
	Action: null,
	Filter: {
		UserName: "",
		Name: "",
		Role: "",
		SysAccount: ""
	},
	AuthExtAvailable: [	  
	    {
	        Name: "ANCR",  
	        Password: true
	    },
	    {
	        Name: "Centricity",
	        Password: false
	    },
	    {
	        Name: "Windows",
	        Password: false
	    }		
	]
}

var updatedTransportCount = 0;

var Actions = { CREATE: "create", UPDATE: "update" }
var MANUAL_TRANSPORT = "Manual";

UserManager.initialize = function(initCallback, transports) {
    UserManager.initCallback = initCallback;
    UserManager.queryUsers();
}

UserManager.queryUsers = function() {
    var queryString = "1 == 1";
    if (UserManager.Filter.UserName != "") {
        queryString += " AND it.UserName LIKE '%" + UserManager.Filter.UserName + "%'"
    }
    if (UserManager.Filter.Name != "") {
        if (UserManager.Filter.Name.lastIndexOf(",") != -1) {
            var names = UserManager.Filter.Name.replace(/ /g, "").split(",");
            queryString += " AND (it.LastName LIKE '%" + names[0] + "%' AND it.FirstName LIKE '%" + names[1] + "%')";
        }
        else {
            queryString += " AND(it.LastName LIKE '%" + UserManager.Filter.Name + "%' OR it.FirstName LIKE '%" + UserManager.Filter.Name + "%')";
        }
    }
    if (UserManager.Filter.Role != "") {
        queryString += " AND it.Roles.Name == '" + UserManager.Filter.Role + "'";
    }
    if (UserManager.Filter.SysAccount != "") {
        queryString += " AND it.IsHomoSapien == " + UserManager.Filter.SysAccount;
    }

    CRManager.WebClient.queryUser(queryString, null, null, UserManager.getUsersSuccess);
}

UserManager.onUserNameFilterChange = function() {
    UserManager.Filter.UserName = $('#txtUserNameFilter').val();
}
UserManager.onNameFilterChange = function() {
    UserManager.Filter.Name = $('#txtNameFilter').val();
}
UserManager.onRoleFilterChange = function() {
    UserManager.Filter.Role = $('#selRoleFilter option:selected').val();
}
UserManager.onSysAcctFilterChange = function() {
    UserManager.Filter.SysAccount = $('#selSystemAcctFilter option:selected').val();
}

UserManager.getUsersSuccess = function(data) {
    UserManager.Users = data.QueryUsers_JsonResult;
    if (UserManager.Users.length > 99) {
        $('#userMessage').html("<i>Only the first 100 users were returned, please provide filter information above.</i>");
    }
    else {
        $('#userMessage').html("");
    }
    UserManager.buildUsersTable();
}

UserManager.buildUsersTable = function() {
    UserManager.TableHtml = "";
    $(UserManager.Users).each(function(index, item) {
        UserManager.TableHtml += UserManager.buildUserRow(item);
    });
    $('#userBody').html(UserManager.TableHtml);
    $('#userBody tr:not(:hidden):nth-child(odd)').addClass("oddRow");
    $('#userBody tr:not(:hidden):nth-child(even)').addClass("evenRow");
    UserManager.isInitialized = true;
    UserManager.initCallback(UserManager);
}

UserManager.buildUserRow = function(data) {
	var row = "<tr>";
	row += "<td>" + data.UserName + "</td>";
	row += "<td>" + data.LastName;
	row += ", " + data.FirstName + "</td>";
	row += "<td>";
	$(data.Roles).each(function(index, item) {
		if (index == 0) {
			row += item.Description;
		}
		else {
			row += " - " + item.Description;
		}
	});
	row += "</td>";
	row += "<td><input type='checkbox' disabled='disabled'";
	if (data.IsSystemAccount === true) {
		row += "checked='checked'";
	}
	row += "/></td>";
	row += "<td><input type='checkbox' disabled='disabled'";
	if (data.Enabled === true) {
		row += "checked='checked'";
	}
	row += "/></td>";
	row += "<td>";
	row += "<input type='button' name='details' onclick='CRManager.WebClient.getUser(\"" + data.UserName + "\", UserManager.getUserSuccess)' value='Details' />";
	row += "</td></tr>";
	return row;
}


UserManager.getUserSuccess = function(data) {
    UserManager.Action = Actions.UPDATE;
    UserManager.SelectedUser = data;
    $('#txtUserName').val(data.UserName);
    $('#txtUserName').attr({ disabled: true });
    $('#txtFirstName').val(data.FirstName);
    $('#txtLastName').val(data.LastName);
    $('#txtTitle').val(data.Title);
    $('#txtCred').val(data.Credentials);
    $('#txtBaseAddress').val(data.Address);
    $('#txtCellPhone').val(data.CellPhone);
    $('#txtOfficePhone').val(data.OfficePhone);
    $('#txtHomePhone').val(data.HomePhone);
    $('#txtCellPhoneProvider').val(data.CellPhoneProvider);
    $('#txtCity').val(data.City);
    $('#txtState').val(data.State);
    $('#txtCountry').val(data.Country);
    $('#txtZip').val(data.Zip);
    $('#txtPager').val(data.Pager);
    $('#txtPagerId').val(data.PagerId);
    $('#txtPagerType').val(data.PagerType);
    $('#txtNPI').val(data.NPI);
    $('[name="chkRole"]').removeAttr('checked');
    $('[name="chkRole"]').removeAttr('defaultChecked');
    $('#chkSystemAccount').removeAttr('defaultChecked');
    $('#chkSystemAccount').removeAttr('checked');
    $('#chkSystemAccount').removeAttr('defaultChecked');
    $('#chkSystemAccount').attr('defaultChecked', data.IsSystemAccount);
    $('#chkSystemAccount').attr('checked', data.IsSystemAccount);
    $('#chkEnabled').attr('defaultChecked', data.Enabled);
    $('#chkEnabled').attr('checked', data.Enabled);
    $(data.Roles).each(function(i, role) {
        $('[name="chkRole"]').each(function(j, checkbox) {
            if (role.Name == $(checkbox).val()) {
                $(checkbox).attr('defaultChecked', 'true');
                $(checkbox).attr('checked', 'true');
            }
        });
    });

    $('#accessList').html("");
    //    $('#accessList').append("<li>ANCR&nbsp;as&nbsp;" + data.UserName + "</li>");
    $('#tbdAuthExt').html("");
    $(UserManager.AuthExtAvailable).each(function(j, authExt) {
        if (authExt.Password)
            $('#tbdAuthExt').append("<tr><td>" + authExt.Name + "</td><td><a id='txt" + authExt.Name + "' href='#' onclick='UserManager.generatePasswordAuthExt(\"" + authExt.Name + "\");return false;' >generate password</a></td><td></td></tr>");
        else
            $('#tbdAuthExt').append("<tr><td>" + authExt.Name + "</td><td><input type='text' id='txt" + authExt.Name + "' name='authExt' /></td><td></td></tr>");
    });

    $(data.UserEntries).each(function(i, entry) {
        if (entry.Type == "AuthExt") {
            $(UserManager.AuthExtAvailable).each(function(j, authExt) {
                if (authExt.Name == entry.Key) {
                    if (authExt.Password) {
                        $('#accessList').append("<li>" + entry.Key + "&nbsp;as&nbsp;" + data.UserName + "</li>");
                        if ($('#txt' + authExt.Name).val() == "") {
                            $('#txt' + authExt.Name).parent().next().html("<a href='#' onclick='UserManager.deleteAuthExt(\"" + entry.Key + "\",\"" + entry.Type + "\");return false;'>delete</a>");
                        }
                        else {
                            $('#tbdAuthExt').append("<tr><td>" + authExt.Name + "</td><td><a id='txt" + authExt.Name + j + "' href='#' onclick='UserManager.generatePasswordAuthExt(\"" + authExt.Name + "\");return false;' >generate password</a></td>" +
						    "<td><a href='#' onclick='UserManager.deleteAuthExt(\"" + authExt.Name + "\",\"" + entry.Value + "\");return false;' >delete</a></td></tr>");
                        }
                    }
                    else {
                        $('#accessList').append("<li>" + entry.Key + "&nbsp;as&nbsp;" + entry.Value + "</li>");
                        if ($('#txt' + authExt.Name).val() == "") {
                            $('#txt' + authExt.Name).val(entry.Value);
                            $('#txt' + authExt.Name).parent().next().html("<a href='#' onclick='UserManager.deleteAuthExt(\"" + entry.Key + "\",\"" + entry.Type + "\");return false;'>delete</a>");
                        }
                        else {
                            $('#tbdAuthExt').append("<tr><td>" + authExt.Name + "</td><td><input type='text' id='txt" + authExt.Name + j + "' name='authExt' value='" + entry.Value + "' /></td>" +
							"<td><a href='#' onclick='UserManager.deleteAuthExt(\"" + authExt.Name + "\",\"" + entry.Value + "\");return false;' >delete</a></td></tr>");
                        }
                    }

                }
            });
        }
    });

    var transportHtml = "";
    $(TransportManager.Transports).each(function(i, transport) {
        if (i != 0) {
            transportHtml += "<tr><td colspan='2'><hr /></td></tr>";
        }
        var selectedUserTransport = null;
        $(data.Transports).each(function(j, userTransport) {
            if (transport.Name == userTransport.Transport.Name) {
                selectedUserTransport = userTransport;
            }
        });
        if (selectedUserTransport != null) {
            transportHtml += "<tr><td><h4>Type</h4></td><td><h4>" + transport.FriendlyName + "</h4></td></tr>";
            if (transport.Name == MANUAL_TRANSPORT) {
                transportHtml += "<tr><td>Address</td><td><input type='input' class='text' name='txtAddress" + transport.Name + "' value='" + selectedUserTransport.Address + "' disabled='disabled'></td>";
            }
            else {
                transportHtml += "<tr><td>Address</td><td><input type='input' class='text' name='txtAddress" + transport.Name + "' value='" + selectedUserTransport.Address + "'></td>";
            }
            transportHtml += "<tr><td>Levels</td><td><ul class='slim'>";
            $(LevelManager.Levels).each(function(k, level) {
                var levelFound = false;
                $(selectedUserTransport.Levels).each(function(l, levelTransport) {
                    if (level.Name == levelTransport.Name) {
                        levelFound = true;
                    }
                });
                var isMandatory = false;
                $(level.MandatoryTransports).each(function(m, mTransport) {
                    if (mTransport.Name == transport.Name) {
                        isMandatory = true;
                    }
                });
                if (levelFound == true) {
                    if (isMandatory == false) {
                        transportHtml += "<li name='" + level.ColorValue + "'><input type='checkbox' name='" + transport.Name + "' value='" + level.Name + "' checked='checked' />" + level.Name + "</li>";
                    }
                    else {
                        transportHtml += "<li name='" + level.ColorValue + "'><input type='checkbox' name='" + transport.Name + "' value='" + level.Name + "' checked='checked' disabled='disabled' />" + level.Name + "&nbsp;(Mandatory)</li>";
                    }
                }
                else {
                    if (isMandatory == false) {
                        transportHtml += "<li name='" + level.ColorValue + "'><input type='checkbox' name='" + transport.Name + "' value='" + level.Name + "' />" + level.Name + "</li>";
                    }
                    else {
                        transportHtml += "<li name='" + level.ColorValue + "'><input type='checkbox' name='" + transport.Name + "' value='" + level.Name + "' checked='checked' disabled='disabled' />" + level.Name + "&nbsp;(Mandatory)</li>";
                    }
                }
            });
        }
        else {
            transportHtml += "<tr><td><h4>Type</h4></td><td><h4>" + transport.FriendlyName + "</h4></td></tr>";
            if (transport.Name == MANUAL_TRANSPORT) {
                transportHtml += "<tr><td>Address</td><td><input type='input' name='txtAddress" + transport.Name + "' disabled='disabled' class='text'></td>";
            }
            else {
                transportHtml += "<tr><td>Address</td><td><input type='input' name='txtAddress" + transport.Name + "' class='text'></td>";
            }
            transportHtml += "<tr><td>Levels</td><td><ul class='slim'>";

            $(LevelManager.Levels).each(function(k, level) {
                var isMandatory = false;
                $(level.MandatoryTransports).each(function(m, mTransport) {
                    if (mTransport.Name == transport.Name) {
                        isMandatory = true;
                    }
                });
                if (isMandatory == false) {
                    transportHtml += "<li name='" + level.ColorValue + "'><input type='checkbox' name='" + transport.Name + "' value='" + level.Name + "' />" + level.Name + "</li>";
                }
                else {
                    transportHtml += "<li name='" + level.ColorValue + "'><input type='checkbox' name='" + transport.Name + "' value='" + level.Name + "' checked='checked' disabled='disabled' />" + level.Name + "&nbsp;(Mandatory)</li>";
                }
            });
        }
    });
    $('#contactInfoBody').html(transportHtml);
    var x = $(document).width();
    x = Math.round(x * .9);
    var y = $(document).height();
    y = Math.round(y * .9);
    $('#userDetails').dialog({
        bgiframe: true,
        modal: true,
        width: x,
        height: y,
        title: "User Details",
        close: function(event, ui) {
            $('#userDetails').dialog('destroy');
        }
    });
    //$('#userDetails').attr({ width: '100%', height: '100%' });
    $('#userDetails li').each(function(index, item) {
        var colorCode = $(item).attr('name');
        if (colorCode) {
            Utility.setLevelColor(item, $(item).attr('name'));
        }
    });
}

UserManager.editAuthExt = function() {
    $('#divAuthExt').dialog({
        bgiframe: true,
        draggable: false,
        resizable: false,
        close: function(event, ui) {
            $(this).dialog('destroy');
        },
        title: "Edit Access Accounts"
    });
    if ($("[name='authExt']").length > UserManager.AuthExtAvailable.length) {
        alert("User has multiple Access defined for a single system.\n\rANCR only allows one account per integrated system.\n\rPlease delete the duplicates.");
    }
}

UserManager.verifyAuthExt = function() {
    UserManager.verifyAuthExtCallCount = 0;
    UserManager.authExtValidationFailed = false;
    $(UserManager.AuthExtAvailable).each(function(i, item) {
        if (!item.Password) {
            $("#txt" + item.Name).each(function(j, textBox) {
                if ($(textBox).val() != "") {
                    UserManager.verifyAuthExtCallCount++;
                    CRManager.WebClient.getUserByAuthExt(item.Name, $(textBox).val(), UserManager.verifyAuthExtSuccess, UserManager.verifyAuthExtFail, UserManager.verifyAuthExtCallCount);
                }
            });
        }
    });

}

UserManager.verifyAuthExtCallCount = 0;
UserManager.authExtValidationFailed = false;
UserManager.verifyAuthExtSuccess = function(data, callId) {
	UserManager.verifyAuthExtCallCount--;
	if (data.CheckAuthExtUsageResult != null) {
		if (data.CheckAuthExtUsageResult.UserName != UserManager.SelectedUser.UserName) {
			alert("A user (" + data.CheckAuthExtUsageResult.UserName + ") already exists with one of the Authorization Extension you have tried to configure.  This Authorization Extension must be removed before it can be added to another ANCR account.");
			UserManager.authExtValidationFailed = true;
		}
	}
	if (UserManager.verifyAuthExtCallCount == 0 && UserManager.authExtValidationFailed == false) {
		UserManager.updateAuthExt();
	}
}

UserManager.verifyAuthExtFail = function(msg, callId) {
	UserManager.verifyAuthExtCallCount--;
	alert("An error occured communicating with the server.  If this persists please contact a System Administrator.");
	CRManager.log(msg.responseText);
}

UserManager.updateAuthExtCallCount = 0;
UserManager.updateAuthExt = function() {
    $(UserManager.AuthExtAvailable).each(function(i, item) {
        if (!item.Password) {
            $("#txt" + item.Name).each(function(j, textBox) {
                if ($(textBox).val() != "") {
                    UserManager.updateAuthExtCallCount++;
                    var data = {
                        type: "AuthExt",
                        key: item.Name,
                        value: $(textBox).val(),
                        xmlValue: ""
                    }
                    CRManager.WebClient.createUpdateUserEntry(UserManager.SelectedUser.UserName, $.toJSON(data), UserManager.updateAuthExtSuccess, null, i);
                }
            });
        }
    });
}

UserManager.updateAuthExtSuccess = function(data) {
	UserManager.updateAuthExtCallCount--;
	if (UserManager.updateAuthExtCallCount == 0) {
		$('#divAuthExt').dialog('close');
		CRManager.WebClient.getUser(UserManager.SelectedUser.UserName, UserManager.getUserSuccess)
	}
}

UserManager.generatePasswordAuthExt = function(key) {
    var data = {        
        key: key
    }
    CRManager.WebClient.generateNewPassword(UserManager.SelectedUser.UserName, UserManager.generatePasswordAuthExtSuccess);
}

UserManager.generatePasswordAuthExtSuccess = function(data) {
    CRManager.WebClient.getUser(UserManager.SelectedUser.UserName, UserManager.getUserSuccess);
}


UserManager.deleteAuthExt = function(key, type) {
    var data = {
        type: type,
        key: key
    }
    CRManager.WebClient.deleteUserEntry(UserManager.SelectedUser.UserName, data, UserManager.deleteAuthExtSuccess);
}

UserManager.deleteAuthExtSuccess = function(data) {
    CRManager.WebClient.getUser(UserManager.SelectedUser.UserName, UserManager.getUserSuccess);
}

UserManager.closeDetails = function() {
    UserManager.SelectedUser = null;
    UserManager.UpdatedUser = null;
    $('#userDetails').dialog('close');
}

UserManager.createUser = function() {
	UserManager.SelectedUser = null;
	UserManager.Action = Actions.CREATE;
	$('#txtUserName').val("");
	$('#txtUserName').removeAttr('disabled');
	$('#txtFirstName').val("");
	$('#txtLastName').val("");
	$('#txtTitle').val("");
	$('#txtCred').val("");	
	$('#txtBaseAddress').val("");
	$('#txtCellPhone').val("");
	$('#txtOfficePhone').val("");
	$('#txtHomePhone').val("");
	$('#txtCellPhoneProvider').val("");
	$('#txtCity').val("");
	$('#txtState').val("");
	$('#txtCountry').val("");
	$('#txtZip').val("");
	$('#txtPager').val("");
	$('#txtPagerId').val("");
	$('#txtPagerType').val("");
	$('#txtNPI').val("");
	$('#lstRoles input').removeAttr('checked');
	$('#chkEnabled').attr("defaultChecked", true);
	$('#chkEnabled').attr("checked", true);

	var transportHtml = "";
	$(TransportManager.Transports).each(function(i, transport) {
		transportHtml += "<tr><td><h4>Type</h4></td><td><h4>" + transport.FriendlyName + "</h4></td></tr>";
		if (transport.Name == MANUAL_TRANSPORT) {
			transportHtml += "<tr><td>Address</td><td><input type='input' class='text' name='txtAddress" + transport.Name + "' disabled='disabled'></td>";
		}
		else {
			transportHtml += "<tr><td>Address</td><td><input type='input' class='text' name='txtAddress" + transport.Name + "'></td>";
		}
		transportHtml += "<tr><td>Levels</td><td><ul class='slim'>";
		$(LevelManager.Levels).each(function(k, level) {
			var transportMandatory = false;
			$(level.MandatoryTransports).each(function(l, mTransport) {
				if (mTransport.Name == transport.Name) {
					transportMandatory = true;
				}
			});
			if (transportMandatory == true) {
				transportHtml += "<li name='" + level.ColorValue + "'><input type='checkbox' name='" + transport.Name + "' value='" + level.Name + "' checked='checked' disabled='disabled' />" + level.Name + "&nbsp;(Mandatory)</li>";
			}
			else {
				transportHtml += "<li name='" + level.ColorValue + "'><input type='checkbox' name='" + transport.Name + "' value='" + level.Name + "' />" + level.Name + "</li>";
			}
		});
	});
	$('#contactInfoBody').html(transportHtml);
	var x = $(document).width();
	x = Math.round(x * .9);
	var y = $(document).height();
	y = Math.round(y * .9);
	$('#userDetails').dialog({
		bgiframe: true,
		modal: true,
		width: x,
		height: y,
		title: "User Details",
		close: function(event, ui) {
			$('#userDetails').dialog('destroy');
		}
	});

	$('#userDetails li').each(function(index, item) {
		var colorCode = $(item).attr('name');
		if (colorCode) {
			Utility.setLevelColor(item, $(item).attr('name'));
		}
	});
}

UserManager.Submit = function() {
    UserManager.UpdatedUser = null;
    var user = {
        Id: null,
        UserName: $('#txtUserName').val(),
        FirstName: $('#txtFirstName').val(),
        LastName: $('#txtLastName').val(),
        MiddleName: $('#txtMiddleName').val(),
        Title: $('#txtTitle').val(),
        Credentials: $('#txtCred').val(),
        Address: $('#txtBaseAddress').val(),
        CellPhone: $('#txtCellPhone').val(),
        OfficePhone: $('#txtOfficePhone').val(),
        HomePhone: $('#txtHomePhone').val(),
        CellPhoneProvider: $('#txtCellPhoneProvider').val(),
        City: $('#txtCity').val(),
        State: $('#txtState').val(),
        Country: $('#txtCountry').val(),
        Zip: $('#txtZip').val(),
        Pager: $('#txtPager').val(),
        PagerId: $('#txtPagerId').val(),
        PagerType: $('#txtPagerType').val(),
        NPI: $('#txtNPI').val(),
        Roles: {},
        IsSystemAccount: $('#chkSystemAccount').attr('checked'),
        Enabled: $('#chkEnabled').attr('checked'),
        Transports: null
    }
    var roles = new Array();
    $('[name="chkRole"]').each(function(i, checkbox) {
        if ($(checkbox).attr('checked') == true) {
            roles[roles.length] = { Name: $(checkbox).val() }
        }
    });
    user.Roles = roles;

    var transports = new Array();

    $('[name*="txtAddress"]').each(function(i, address) {
        var transportName = $(address).attr('name').replace('txtAddress', '');
        var transport = { Name: transportName };
        var levels = new Array();
        $('[name="' + transportName + '"]').each(function(i, level) {
            if ($(level).attr('checked') == true) {
                var level = { Name: $(level).val() };
                levels[levels.length] = level;
            }
        });
        transports[transports.length] = { Transport: transport, Address: $(address).val(), Levels: levels };
    });

    user.Transports = transports;

    UserManager.UpdatedUser = user;

    if (UserManager.Action == Actions.UPDATE) {
        UserManager.showStatusMessage("Updating User");
        CRManager.WebClient.updateUser($.toJSON(user), UserManager.onCreateUpdateSuccess);
    }
    else if (UserManager.Action == Actions.CREATE) {
        UserManager.showStatusMessage("Creating User");
        CRManager.WebClient.createUser($.toJSON(user), UserManager.onCreateUpdateSuccess);
    }
}

UserManager.onCreateUpdateSuccess = function(data) {
    UserManager.hideStatusMessage();
    UserManager.showStatusMessage("Updating Roles");
    var roles = new Array();
    $('[name="chkRole"]').each(function(i, checkbox) {
        if ($(checkbox).attr('checked') == true) {
            roles[roles.length] = $(checkbox).val();
        }
    });
    CRManager.WebClient.updateUserRoles($('#txtUserName').val(), roles, UserManager.onRolesAddedSuccess);
}

UserManager.onRolesAddedSuccess = function(data) {
    UserManager.hideStatusMessage();
    UserManager.showStatusMessage("Updating Contact Information");
    updatedTransportCount = 0;
    $(UserManager.UpdatedUser.Transports).each(function(i, transport) {

        var origAddress = "";
        if (UserManager.SelectedUser) {
            $(UserManager.SelectedUser.Transports).each(function(j, oTransport) {
                if (oTransport.Transport.Name == transport.Transport.Name) {
                    origAddress = oTransport.Address;
                }
            });
        }
        if (origAddress != transport.Address) {
            CRManager.WebClient.updateUserTransport(
                UserManager.UpdatedUser.UserName,
                transport.Transport.Name,
                origAddress,
                transport.Address,
                UserManager.updateUserTransports);
        }
        else {
            UserManager.updateUserTransports(transport);
        }
    });

}

UserManager.updateUserTransports = function(data) {
    UserManager.hideStatusMessage();
    UserManager.showStatusMessage("Updating Levels");
    var levels = new Array();
    $(UserManager.UpdatedUser.Transports).each(function(i, transport) {
        if (transport.Transport.Name == data.Transport.Name) {
            $(transport.Levels).each(function(j, level) {
                levels[levels.length] = level.Name;
            });
        }
    });

    CRManager.WebClient.addLevelsToUserTransport(
            UserManager.UpdatedUser.UserName,
            data.Transport.Name,
            data.Address,
            levels, UserManager.updateTransportSuccess);
}

UserManager.updateTransportSuccess = function(data) {
    updatedTransportCount++;
    /*if (UserManager.Action == Actions.CREATE) {
        UserManager.hideStatusMessage();
        if (updatedTransportCount == UserManager.UpdatedUser.Transports.length) {
            UserManager.showStatusMessage("Sending new account email.");
            CRManager.WebClient.timeout = 100000;
            CRManager.WebClient.initialize();
            CRManager.WebClient.sendAccountConfirmation(UserManager.UpdatedUser.UserName, UserManager.accountConfirmSuccess, UserManager.accountConfirmFail);
        }
    }
    else */
    {
        UserManager.hideStatusMessage();
        $('#userDetails').dialog('close');
        UserManager.initialize(UserManager.initCallback);
    }
}

UserManager.accountConfirmSuccess = function(data) {
    UserManager.hideStatusMessage();
    if (data === false) {
        alert("An error occurred sending emails, please verify system settings.");
    }
    UserManager.showStatusMessage("Creating/Emailing password.");
    CRManager.WebClient.generateNewPassword(UserManager.UpdatedUser.UserName, UserManager.newPasswordSuccess, UserManager.newPasswordFail);
}

UserManager.accountConfirmFail = function(data) {
    UserManager.hideStatusMessage();
    var message = "Sending account confirmation failed: ";
    if (data.responseText) {
        message += data.responseText;
    }
    else {
        message += "Unknown Server Error.";
    }
    alert(message);
    UserManager.accountConfirmSuccess(true);
}

UserManager.newPasswordSuccess = function(data) {
    UserManager.hideStatusMessage();
    $('#userDetails').dialog('close');
    UserManager.initialize(UserManager.initCallback);
}

UserManager.newPasswordFail = function(data) {
    UserManager.hideStatusMessage();
    var message = "Password Generation Failed: ";
    if (data.responseText) {
        message == data.responseText;
    }
    else {
        message += "Unknown Server Error.";
    }
    alert(message);
    UserManager.hideStatusMessage();
    $('#userDetails').dialog('close');
    UserManager.initialize(UserManager.initCallback);
}

UserManager.showStatusMessage = function(message) {
    $("statusMessage").html(message);
    $("statusMessage").removeClass("hidden");
    $("statusMessage").addClass("status");
}

UserManager.hideStatusMessage = function() {
    $("statusMessage").html("");
    $("statusMessage").removeClass("status");
    $("statusMessage").addClass("hidden");
}
