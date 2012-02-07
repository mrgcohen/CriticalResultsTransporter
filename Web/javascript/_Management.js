Management = {
	CRManager: null,	UserIsAdmin: false}
Management.initialize = function(crManager) {
	Management.CRManager = crManager;
	$(Management.CRManager.AuthenticatedUser.Roles).each(function(index, item) {
		if (item.Name == Management.CRManager.ROLE_SYS_ADMIN) {
			Management.UserIsAdmin = true;
			var showManagementLnk = $("<a href='#'>Management</a>");
			$(showManagementLnk).bind("click", function() { Management.showManagement(); });
			Management.CRManager.addMenuObject(showManagementLnk);
		}
	});
}
/// Requires manager object containing addModal function as parameter
Management.showManagement = function() {
	if (Management.UserIsAdmin == true) {
		var width = $(window).width() - Math.round($(window).width() * .2);
		var height = parent.document.body.clientHeight - Math.round(parent.document.body.clientHeight * .20);
		var divHeight = height - 50;
		var iframeHgt = divHeight - 100;
		var html = "<div id='managementTabs' style='height:" + divHeight + ";border:none; '><ul><li><a href='#usersTab'>Users</a></li><li><a href='#levelsTab'>Levels</a></li><li><a href='#transportsTab'>Transports</a></li></ul>";
		html += "<div id='usersTab'><iframe id='usersFrame' height='" + iframeHgt + "' width='100%' src='Manager/UserManager.htm' frameborder=0 /></div>";
		html += "<div id='levelsTab'><iframe id='usersFrame' height='" + iframeHgt + "' width='100%' src='Manager/LevelManager.htm' frameborder=0 /></div>";
		html += "<div id='transportsTab'><iframe id='usersFrame' height='" + iframeHgt + "' width='100%' src='Manager/TransportManager.htm' frameborder=0 /></div>";
		html += "</div>";
		Management.CRManager.addModal("Management", html, "Critical Results Management", "", null, null, width, height, null);
		$('#managementTabs').tabs();
	}
	else {
		Management.CRManager.showAlert("Authenticated user is not an administrator.<br /><input type='button' onclick='Manager.closeAlert()' style='float:right' />");
	}
}