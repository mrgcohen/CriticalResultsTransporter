function Timespan() {
	this.constants = {
		MS_PER_WEEK: 1000 * 60 * 60 * 24 * 7,
		MS_PER_DAY: 1000 * 60 * 60 * 24,
		MS_PER_HOUR : 1000 * 60 * 60,
		MS_PER_MIN : 1000 * 60,
		MS_PER_SEC : 1000
	};
	this.days = 0;
	this.hours = 0;
	this.minutes = 0;
	this.seconds = 0;
	this.milliseconds = 0;
	this.base = 0;

	this.fromDates = function(date1, date2) {
		var milliseconds = date1 - date2;
		this.fromMilliseconds(milliseconds);
	}

	this.fromMilliseconds = function(milliseconds) {
		
		this.base = milliseconds;

		if (milliseconds >= 0)
			this.sign = 1;
		else
			this.sign = -1;

		milliseconds = Math.abs(milliseconds);

		this.weeks = 0;
		var w = milliseconds / this.constants.MS_PER_WEEK;
		if (w >= 1) {
			this.weeks = Math.floor(w);
			milliseconds = milliseconds - (this.weeks * this.constants.MS_PER_WEEK);
		}
		this.days = 0;
		var d = milliseconds / this.constants.MS_PER_DAY;
		if (d >= 1) {
			this.days = Math.floor(d);
			milliseconds = milliseconds - (this.days * this.constants.MS_PER_DAY);
		}

		this.hours = 0;
		var h = milliseconds / this.constants.MS_PER_HOUR;
		if (h >= 1) {
			this.hours = Math.floor(h);
			milliseconds = milliseconds - (this.hours * this.constants.MS_PER_HOUR);
		}

		this.minutes = 0;
		var m = milliseconds / this.constants.MS_PER_MIN;
		if (m >= 1) {
			this.minutes = Math.floor(m);
			milliseconds = milliseconds - (this.minutes * this.constants.MS_PER_MIN);
		}

		this.seconds = 0;
		var s = milliseconds / this.constants.MS_PER_SEC;
		if (s >= 0) {
			this.seconds = Math.floor(s);
			milliseconds = milliseconds - (this.seconds * this.constants.MS_PER_SEC);
		}

		this.milliseconds = milliseconds;
	}

	this.totalMilliseconds = function() {
		return this.base;
	}
	this.totalSeconds = function() {
		return this.base / this.constants.MS_PER_SEC;
	}
	this.totalMinutes = function() {
		return this.base / this.constants.MS_PER_MIN;
	}
	this.totalHours = function() {
		return this.base / this.constants.MS_PER_HOUR;
	}
	this.totalDays = function() {
		return this.base / this.constants.MS_PER_DAY;
	}

	this.addDays = function(days) {
		var ms = this.totalMilliseconds() + (days * this.constants.MS_PER_DAY);
		this.fromMilliseconds(ms);
	}
	this.addHours = function(hours) {
		var ms = this.totalMilliseconds() + (hours * this.constants.MS_PER_HOUR);
		this.fromMilliseconds(ms);
	}

	this.addMinutes = function(minutes) {
		var ms = this.totalMilliseconds() + (minutes * this.constants.MS_PER_MIN);
		this.fromMilliseconds(ms);
	}

	this.addSeconds = function(seconds) {
		var ms = this.totalMilliseconds() + (seconds * this.constants.MS_PER_SEC);
		this.fromMilliseconds(ms);
	}
	this.addMilliseconds = function(milliseconds) {
		this.milliseconds += milliseconds;
		this.fromMilliseconds(this.milliseconds);
	}

	this.toString = function() {
		var value = "";
		if (this.sign < 0)
			value = "-";

		if (this.days > 0) {
			value += this.days + ".";
		}
		if (this.hours > 0) {
			value += this.hours + ":";
		}
		if (this.minutes > 0) {
			value += this.minutes + ":";
		}
		if (this.seconds > 0) {
			value += this.seconds + ".";
		}
		if (this.milliseconds > 0) {
			value += this.milliseconds;
		}
		return value;
	}

//	this.parse = function(string) {
//		var values = string.split(" ");
//		var milliseconds = 0;
//		for (var i = 0; i < values.length; i++) {
//			if (values[i].indexOf("w") != -1) {
//				var x = values[i].replace("w", "").replace(" ", "");
//				milliseconds += x * this.constants.MS_PER_WEEK;
//			}
//			if (values[i].indexOf("d") != -1) {
//				var x = values[i].replace("d", "").replace(" ", "");
//				milliseconds += x * this.constants.MS_PER_DAY;
//			}
//			if (values[i].indexOf("h") != -1) {
//				var x = values[i].replace("h", "").replace(" ", "");
//				milliseconds += x * this.constants.MS_PER_HOUR;
//			}
//			if (values[i].indexOf("m") != -1) {
//				var x = values[i].replace("m", "").replace(" ", "");
//				milliseconds += x * this.constants.MS_PER_MIN;
//			}
//			if (values[i].indexOf("ms") != -1) {
//				var x = values[i].replace("ms", "").replace(" ", "");
//				milliseconds += x;
//			}
//			if (values[i].indexOf("s") != -1) {
//				var x = values[i].replace("s", "").replace(" ", "");
//				milliseconds += x * this.constants.MS_PER_SEC;
//			}
//		}
//		this.fromMilliseconds(milliseconds);
//	}
	
}

Timespan.parseJsonDate = function(jsonDate) {
	var date = jsonDate.replace("/Date(", "");
	date = date.replace(")/", "");
	var dt = date.substring(0, date.lastIndexOf("-"));
	var d = new Date();
	d.setTime(dt);
	return d;
}
