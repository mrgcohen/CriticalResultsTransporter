// ===================================================================
// Author: David Lionetti <david.lionetti@poiesisinformatics.com>
// File:
// Description:
// Note:
// ===================================================================



/////////////////////////////////////////////
///////////       Variables        //////////


var AnimObj;



/////////////////////////////////////////////
///////////       Functions         //////////


//Function:
//Description:
//
//Arguments:
//	none
//Returns:
//	none
//Author:
//	David Lionetti, Poiesis Informatics
//	<date>
function ContextMenu (  ) {
	this.Menu = window.createPopup();
		
	this.currentHeight = 0;
	this.increment = 15;
	this.delay=2;
}

function __PrepMenu ( MenuContent, width, maxHeight) {
	this.w = width;
	this.Menu.document.close();
	this.Menu.document.write ( MenuContent ); // this is an ajax call
	this.Menu.show(0, 0, width, 0);
	if (maxHeight) {
		this.h = Math.min (maxHeight, this.Menu.document.body.scrollHeight+10);	
	}
	else {
		this.h = this.Menu.document.body.scrollHeight+10;
	}
	return this.h;
}

function __ShowMenu ( x, y ) {
	this.Menu.hide();
   	this.Menu.show(x, y, this.w, this.h);
}


function __Animate (x, y, Direction) {
	this.x = x;
	this.y = y;
	this.AnimateDirection = Direction

	AnimObj = this;
	__MyAnimation(  );
}

function __MyAnimation (  ) {
	if ( AnimObj.currentHeight >= AnimObj.h ) {
		AnimObj.currentHeight = 0;
		AnimObj.Menu.hide();
		AnimObj.Menu.show(AnimObj.x, AnimObj.y, AnimObj.w, AnimObj.h);
	}
	else {
		AnimObj.HideMenu();
		if ( AnimObj.AnimateDirection == "UP" )
			AnimObj.Menu.show(AnimObj.x, AnimObj.y+(AnimObj.h -AnimObj.currentHeight), AnimObj.w, AnimObj.currentHeight);
		else
			AnimObj.Menu.show(AnimObj.x, AnimObj.y, AnimObj.w, AnimObj.currentHeight);
		
		AnimObj.currentHeight = Math.min ( AnimObj.h, (AnimObj.currentHeight + AnimObj.increment));
		setTimeout ("__MyAnimation ();", AnimObj.delay);
	}
}

function __IsOpen () {
	return this.Menu.isOpen;
}

function __HideMenu () {
	this.Menu.hide();
}


function HideContextMenu () {
	_ContextMenu.HideMenu();
}


function GenerateContextMenuItem ( ItemID, GraphicURL, MenuText, Disabled ) {
	if (ItemID == null) {
		return "<tr><td bgcolor='#C0C0C0' width='24' align='middle' ondragstart='return false;' unselectable='on'></td><td><hr size='0'></td></tr>"
	}
	
	else {
		if (Disabled == true) {
			GraphicURL = GraphicURL.replace (".gif", "_disabled.gif");
			return "<tr id='" + ItemID + "'><td width='24' align='middle' ondragstart='return false;' unselectable='on' bgcolor='#c0c0c0'><img src='" + GraphicURL + "' width='16' height='16'></td><td class='disabled' unselectable='on'>" + MenuText + "</td></tr>";
		}
		else {
			return "<tr id='" + ItemID + "'><td width='24' align='middle' ondragstart='return false;' unselectable='on' bgcolor='#c0c0c0'><img src='" + GraphicURL + "' width='16' height='16'></td><td class='shortcut' onClick='MenuItemChosen();' onMouseOver='hover();' onMouseOut='unhover();' unselectable='on'>" + MenuText + "</td></tr>";
		}
	}
}

// function prototypes
ContextMenu.prototype.ShowMenu=__ShowMenu;
ContextMenu.prototype.HideMenu=__HideMenu;
ContextMenu.prototype.PrepMenu=__PrepMenu;
ContextMenu.prototype.Animate=__Animate;
ContextMenu.prototype.IsOpen=__IsOpen;