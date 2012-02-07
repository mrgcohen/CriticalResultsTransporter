var MobileUserAgents =
[
    "iPhone",
    "iPod",
    "BlackBerry",
    "Android",
    "Windows CE",
    "Palm"
];

var targetScreenWidth = 800;

function isMobile() {
    if(screen.width < targetScreenWidth){
        return true;
    }
    
    var userAgent = navigator.userAgent;
    for (var i = 0; i < MobileUserAgents.length; i++) {
        if(userAgent.lastIndexOf(MobileUserAgents[i]) != -1){
            return true;
        }
    }
    
    return false;
}

function isIE6() {
    if(navigator.userAgent.lastIndexOf("MSIE 6") != -1) {
        return true;
    }
    return false;
}