//dependencies
/// <reference path="../Utility.js" />
/// <reference path="../WebClient.js" />


var Manager = {
	//Constants
	ROLE_SENDER: "sender",
	ROLE_RECEIVER: "receiver",
	ROLE_SYS_ADMIN: "sysAdmin",
	ROLE_CLIN_ADMIN: "clinAdmin",
	MANUAL_TRANSPORT: "Manual",

	///Properties
	AuthenticatedUser: null,
	Token: null,
	ActiveRole: null,
	ProxyView: false,
	Levels: null,
	Users: null,
	AutoCompleteSenders: null,
	AutoCompleteReceivers: null,
	DashboardResults: null,
	ListResults: null,
	QueryString: null,
	Filter: {
		levelFilter: null,
		statusFilter: [{ "name": "complete", "value": "true" }, { "name": "pending", "value": "true" }, { "name": "overdue", "value": "true"}],
		dueFilter: "",
		contextFilter: "",
		messageFilter: "",
		senderFilter: "",
		receiverFilter: "",
		createdFilter: "",
		userFilter: "",
		acknowledgedFilter: "",
		userFilter: {
			Senders: null,
			Receivers: null
		},
		timeFilter: null,
		timeFilterOptions: {
			createTimeFilterOptions: [
							{ name: "all", value: "all" },
							{ name: "today", value: "today" },
							{ name: "last hour", value: "-3600000" },
							{ name: "last 3 hours", value: "-10800000" },
							{ name: "last 6 hours", value: "-21600000" },
							{ name: "last 12 hours", value: "-43200000" },
							{ name: "last 24 hours", value: "-86400000" },
							{ name: "last 3 days", value: "-259200000" },
							{ name: "last 7 days", value: "-604800000" },
							{ name: "last 30 days", value: "-2592000000", selected: true}],
			dueTimeFilterOptions: [
							{ name: "all", value: "all", selected: true },
							{ name: "today", value: "today" },
							{ name: "next hour", value: "3600000" },
							{ name: "next 3 hours", value: "10800000" },
							{ name: "next 6 hours", value: "21600000" },
							{ name: "next 12 hours", value: "43200000" },
							{ name: "next 24 hours", value: "86400000" },
							{ name: "next 3 days", value: "259200000" },
							{ name: "next 7 days", value: "604800000" },
							{ name: "next 30 days", value: "2592000000"}],

			ackTimeFilterOptions: [
							{ name: "unacknowledged", value: "unacknowledged", selected: true },
							{ name: "all", value: "all" },
							{ name: "today", value: "today" },
							{ name: "last hour", value: "-3600000" },
							{ name: "last 3 hours", value: "-10800000" },
							{ name: "last 6 hours", value: "-21600000" },
							{ name: "last 12 hours", value: "-43200000" },
							{ name: "last 24 hours", value: "-86400000" },
							{ name: "last 3 days", value: "-259200000" },
							{ name: "last 7 days", value: "-604800000" },
							{ name: "last 30 days", value: "-2592000000"}]
		}
	},

	///Methods
	getFullUserName: null,
	setActiveRole: null,
	setAuthenticatedUser: null,
	refresh: null,
	initialize: null,
	initialized: null,

	queryResults: null,
	onQueryResultsSuccess: null,
	log: function(message) {
		
	}
}
