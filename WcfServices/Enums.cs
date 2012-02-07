using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CriticalResults
{
	enum AuditEvents
	{
		CreateResult,
		UpdateResultReceiver,
		CreateLevel,
		UpdateLevel,
		CreateTransportLevel,
		ModifyTransportLevel,
		RemoveTransportFromLevel,
		GetUser,
		CreateResultNotification,
		UpdateNotification,
		CreateResultAcknowledgment,
		CreateTransport,
		CreateUser,
		SendAccountConfirmation,
		AddTransportsToUser,
		UpdateUser,
		UpdateUserRoles,
		AddUserToRole,
		UpdateTransport,
		CreateUserEntry,
		CreateUpdateUserEntry,
		DeleteUserEntry,
		CreateUserTransport,
		AddUsersToTransport,
		UpdateUserTransport,
		SetPassword,
		GeneratePassword,
		LeaveFeedback,
	}
	enum LogEvents
	{

	}
}
