function Timespan() {
    this.constants = {};
    this.constants.week = 1000 * 60 * 60 * 24 * 7;
    this.constants.day = 1000 * 60 * 60 * 24;
    this.constants.hour = 1000 * 60 * 60;
    this.constants.minute = 1000 * 60;
    this.constants.second = 1000;
    this.constants.basedate = new Date("1/1/1900");
    this.base = 0;
    this.days = 0;
    this.hours = 0;
    this.minutes = 0;
    this.seconds = 0;
    this.milliseconds = 0;

    this.totalMilliseconds = function() { return this.base; }
    this.totalSeconds = function() { return this.base / this.constants.second; }
    this.totalMinutes = function() { return this.base / this.constants.minute; }
    this.totalHours = function() { return this.base / this.constants.hour; }
    this.totalDays = function() { return this.base / this.constants.day; }

    this.addDays = function(days) {
        this.base += (days * this.constants.day);
        this.fromMilliseconds(this.base);
    }
    this.addHours = function(hours) {
        this.base += (hours * this.constants.hour);
        this.fromMilliseconds(this.base);
    }
    
    this.addMinutes = function(minutes) {
        this.base += (minutes * this.constants.minute);
        this.fromMilliseconds(this.base);
    }
    
    this.addSeconds = function(seconds) {
        this.base += (seconds * this.constants.second);
        this.fromMilliseconds(this.base);
    }
    this.addMilliseconds = function(milliseconds) {
        this.base += milliseconds;
        this.fromMilliseconds(this.base);
    }


    this.fromDates = function(date1, date2) {
        if (date2 == null) {
            date2 = this.constants.basedate;
        }
        var milliseconds = date1 - date2;
        this.fromMilliseconds(milliseconds);
    }

    this.fromMilliseconds = function(milliseconds) {
        this.constants = {};
        this.constants.week = 1000 * 60 * 60 * 24 * 7;
        this.constants.day = 1000 * 60 * 60 * 24;
        this.constants.hour = 1000 * 60 * 60;
        this.constants.minute = 1000 * 60;
        this.constants.second = 1000;
        this.base = milliseconds;

        this.days = 0;
        var d = milliseconds / this.constants.day;
        if (d >= 1) {
            this.days = Math.floor(d);
            milliseconds = milliseconds - (this.days * this.constants.day);
        }

        this.hours = 0;
        var h = milliseconds / this.constants.hour;
        if (h >= 1) {
            this.hours = Math.floor(h);
            milliseconds = milliseconds - (this.hours * this.constants.hour);
        }

        this.minutes = 0;
        var m = milliseconds / this.constants.minute;
        if (m >= 1) {
            this.minutes = Math.floor(m);
            milliseconds = milliseconds - (this.minutes * this.constants.minute);
        }

        this.seconds = 0;
        var s = milliseconds / this.constants.second;
        if (s >= 0) {
            this.seconds = Math.floor(s);
            milliseconds = milliseconds - (this.seconds * this.constants.second);
        }

        this.milliseconds = milliseconds;
    }

    this.toString = function() {
        var rtnString = this.days + "." + this.hours + ":" + this.minutes + ":" + this.seconds + "." + this.milliseconds;
        return rtnString;
    }

    this.toFormattedString = function() {
        var rtnString = "";
        if (this.days > 0) {
            rtnString += this.days + "d ";
        }
        if (this.hours > 0) {
            rtnString += this.hours + "h ";
        }
        if (this.minutes > 0) {
            rtnString += this.minutes + "m ";
        }
        if (this.seconds > 0) {
            rtnString += this.seconds + "s ";
        }
        if (this.milliseconds > 0) {
            rtnString += this.milliseconds + "ms ";
        }
        return rtnString;
    }

    this.parseFormattedString = function(string) {
        var values = string.split(" ");
        var milliseconds = 0;
        for (var i = 0; i < values.length; i++) {
            if (values[i].indexOf("w") != -1) {
                var x = values[i].replace("w", "").replace(" ", "");
                milliseconds += x * this.constants.week;
            }
            if (values[i].indexOf("d") != -1) {
                var x = values[i].replace("d", "").replace(" ", "");
                milliseconds += x * this.constants.day;
            }
            if (values[i].indexOf("h") != -1) {
                var x = values[i].replace("h", "").replace(" ", "");
                milliseconds += x * this.constants.hour;
            }
            if (values[i].indexOf("m") != -1) {
                var x = values[i].replace("m", "").replace(" ", "");
                milliseconds += x * this.constants.minute;
            }
            if (values[i].indexOf("ms") != -1) {
                var x = values[i].replace("ms", "").replace(" ", "");
                milliseconds += x;
            }
            if (values[i].indexOf("s") != -1) {
                var x = values[i].replace("s", "").replace(" ", "");
                milliseconds += x * this.constants.second;
            }
        }
        this.fromMilliseconds(milliseconds);
    }
    
    this.addToDate = function(date) {
        if(date == null){
            date = this.constants.basedate;
        }
        var ms = date.getTime();
        ms += this.base;
        return date.setTime(ms);
    }

    this.subtractFromDate = function(date) {
        if(date == null){
            date = this.constants.basedate;
        }
        var ms = date.getTime();
        ms += this.base;
        return date.setTime(ms);
    }
}

Timespan.parseJsonDate = function(jsonDate) {
    var date = jsonDate.replace("/Date(", "");
    date = date.replace(")/", "");
    var dt = date.substring(0, date.lastIndexOf("-"));  
    var d = new Date();
    d.setTime(dt);
    return d;
}
