DELETE FROM dbo.[acknowledgments]
DELETE FROM dbo.[notifications]
DELETE FROM dbo.[result_contexts]
DELETE FROM dbo.[results]
DELETE FROM dbo.[user_transport_levels]
DELETE FROM dbo.[transport_levels]
DELETE FROM dbo.[user_roles]
DELETE FROM dbo.[user_transports]
DELETE FROM dbo.[levels]
DELETE FROM dbo.[roles]
DELETE FROM dbo.[transports]
DELETE FROM dbo.[user_entries]
DELETE FROM dbo.[proxies]
DELETE FROM dbo.[tokens]
DELETE FROM dbo.[users]
DELETE FROM dbo.[context_types]
DELETE FROM dbo.[settings]
DELETE FROM dbo.[ratings]
DELETE FROM dbo.[user_results]

/*Add Global System Settings*/
INSERT INTO [settings] ([uuid],[owner],[entry_key],[value]) VALUES ( NEWID(), 'System', 'Session_Timeout', '60')
INSERT INTO [settings] ([uuid],[owner],[entry_key],[value]) VALUES ( NEWID(), 'System', 'DefaultAccountDomain', 'DefaultDomain')
INSERT INTO [settings] ([uuid],[owner],[entry_key],[value]) VALUES ( NEWID(), 'System', 'DefaultTransport', '')

/*Add System Settings for SMTP notification for system management*/
insert into settings(uuid, [owner], entry_key, [value]) values (newid(), 'System', 'SMTP_AccountName', '') 
insert into settings(uuid, [owner], entry_key, [value]) values (newid(), 'System', 'SMTP_AccountPassword', '')
insert into settings(uuid, [owner], entry_key, [value]) values (newid(), 'System', 'SMTP_AccountDomain', '')
insert into settings(uuid, [owner], entry_key, [value]) values (newid(), 'System', 'SMTP_UseSSL', 'false')
insert into settings(uuid, [owner], entry_key, [value]) values (newid(), 'System', 'SMTP_ServerIP', '')
insert into settings(uuid, [owner], entry_key, [value]) values (newid(), 'System', 'SMTP_ServerPort', '25')

/*Create Settings for Email for system notification (not related to email transport)*/
insert into settings(uuid, [owner], entry_key, [value]) values (newid(), 'System', 'SMTP_FromAddress', 'ancr@example.com')
insert into settings(uuid, [owner], entry_key, [value]) values (newid(), 'System', 'FromName', 'Alert Notification of Critical Radiology Results (ANCR)')
insert into settings(uuid, [owner], entry_key, [value]) values (newid(), 'System', 'AccountConfirmationMessage', 'An account has been created for you in the Alert Notification of Critical Radiology Results (ANCR) System.  You can access the system at http://defaultdomain/CriticalResultsTransporter/Web/\r\nYour username is {0}.')

/*Create default Clinical Admin Account Setting */
insert into settings(uuid, [owner], entry_key, [value]) values (newid(), 'System', 'MasterClinicalAdminAccount', '')

/*Create Authorization Extension Settings for Login*/
INSERT INTO [settings] ([uuid],[owner],[entry_key],[value],[xml_value]) VALUES ( NEWID(), 'AuthExt', 'default', 'DefaultLogin.htm', NULL)
INSERT INTO [settings] ([uuid],[owner],[entry_key],[value],[xml_value]) VALUES ( NEWID(), 'AuthExt', 'Windows', 'WindowsLogin.htm', NULL)
INSERT INTO [settings] ([uuid],[owner],[entry_key],[value],[xml_value]) VALUES ( NEWID(), 'AuthExt', 'Centricity', 'CentricityLogin.htm', NULL)


/*Create Default Level(s)*/
DECLARE @RedAlertLevel int
INSERT INTO dbo.levels (uuid, [name], short_description, [description], color_value, escalation_timeout, due_timeout, direct_contact_required) VALUES (NEWID(), 'Red Alert', 'Immediately life-threatening', 'Findings that are <b>potentially immediately life-threatening</b>.  <i>Requires "face-to-face" or "telephone" contact</i>', '#CF0E21', '1-1-1900 00:30:00', '1-1-1900 00:10:00', -1)
SET @RedAlertLevel = SCOPE_IDENTITY()

DECLARE @OrangeAlertLevel int
INSERT INTO dbo.levels (uuid, [name], short_description, [description], color_value, escalation_timeout, due_timeout, direct_contact_required) VALUES (NEWID(), 'Orange Alert', 'Urgent Treatment', 'Findings that <i>could result in mortality or significant morbidity if not appropriately treated</i><b>urgently.</b>  <i>Requires "face-to-face" or "telephone" contact</i>', '#FF9100', '1-1-1900 03:00:00', '1-1-1900 00:30:00', -1)
SET @OrangeAlertLevel = SCOPE_IDENTITY()

DECLARE @YellowAlertLevel int
INSERT INTO dbo.levels (uuid, [name], short_description, [description], color_value, escalation_timeout, due_timeout, direct_contact_required) VALUES (NEWID(), 'Yellow Alert', 'Not immediate or urgent', 'Findings that <i>could result in mortality or significant morbidity if not appropriately treated</i>, but are not immediately life-threatening or urgent. Requires "face-to-face", "telephone", or other verifiable contact', '#EEFF05', '1-2-1900 12:00:00', '1-3-1900 00:00:00', 0)
SET @YellowAlertLevel = SCOPE_IDENTITY()

DECLARE @GrayAlertLevel int
INSERT INTO dbo.levels (uuid, [name], short_description, [description], color_value, escalation_timeout, due_timeout, direct_contact_required) VALUES (NEWID(), 'Non-Critical Alert', 'Not immediate or urgent', '', '#888888', '1-2-1900 12:00:00', '1-3-1900 00:00:00', 0)
SET @GrayAlertLevel = SCOPE_IDENTITY()

/*Create Email Transport*/
DECLARE @EmailTransport int
INSERT INTO dbo.transports ([name], [transport_uri], [friendly_address_name]) VALUES ('SMTP Transport', 'http://localhost/CriticalResultsTransporter/SmtpTransportService/SmtpTransport.svc', 'Email')
SET @EmailTransport = SCOPE_IDENTITY()

INSERT INTO [transport_levels] (level_id, transport_id, mandatory) VALUES(@RedAlertLevel, @EmailTransport, 0)
INSERT INTO [transport_levels] (level_id, transport_id, mandatory) VALUES(@OrangeAlertLevel, @EmailTransport, 0)
INSERT INTO [transport_levels] (level_id, transport_id, mandatory) VALUES(@YellowAlertLevel, @EmailTransport, 0)
INSERT INTO [transport_levels] (level_id, transport_id, mandatory) VALUES(@GrayAlertLevel, @EmailTransport, 0)

/*Create Default Context Type(s)*/
DECLARE @GenericContext int
INSERT INTO dbo.context_types (uuid, namespace_uri, name, description, xml_schema, json_template) VALUES 
(
	'6629949E-A451-4bae-80AF-D287343FF7B1', 
	'http://partners.org/brigham/criticalresults/v0/context/generic/', 
	'Generic', 
	'Context for Generic Message', 
	'', 
	'{Fields:[]}'
)
SET @GenericContext = SCOPE_IDENTITY()

DECLARE @RadiologyContext int
INSERT INTO dbo.context_types (uuid, namespace_uri, name, description, xml_schema, json_template) VALUES 
(
	'6629949E-A451-4bae-80AF-D287343FF7B0', 
	'http://partners.org/brigham/criticalresults/v0/context/radiology/', 
	'Radiology', 
	'Context for Radiology data', 
	'', 
	'{Fields:[
	{"Name":"PatientName", "DisplayName":"Patient Name", "Required":"false", "ValidationPatterns":[], "Enabled":"true"},
	{"Name":"DOB", "DisplayName":"Patient DOB", "Required":"false", "ValidationPatterns":[], "Enabled":"true"},
	{"Name":"ORG", "DisplayName":"ORG", "Required":"false", "ValidationPatterns":[], "Enabled":"false"},
	{"Name":"MRN", "DisplayName":"Patient MRN", "Required":"false", "ValidationPatterns":[], "Enabled":"true"},
	{"Name":"Accession", "DisplayName":"Exam ID", "Required":"false", "ValidationPatterns":[], "Enabled":"true"},
	{"Name":"ExamDescription", "DisplayName":"Description", "Required":"falscv wee", "ValidationPatterns":[], "Enabled":"true"},
	{"Name":"ExamTime", "DisplayName":"Exam Time", "Required":"false", "ValidationPatterns":[], "Enabled":"true"},
	{"Name":"ExamDate", "DisplayName":"Exam Date", "Required":"false", "ValidationPatterns":[], "Enabled":"true"}
	]}'
)
SET @RadiologyContext = SCOPE_IDENTITY()

/*Create Default Role(s)*/
DECLARE @SysRole int
INSERT INTO roles ([name],[description]) VALUES ('sysAdmin', 'System Administrator')
SET @SysRole = SCOPE_IDENTITY()
DECLARE @ClinRole int
INSERT INTO roles ([name],[description]) VALUES ('clinAdmin', 'Clinical Administrator')
SET @ClinRole = SCOPE_IDENTITY()
DECLARE @SenderRole int
INSERT INTO roles ([name],[description]) VALUES ('sender', 'Sender')
SET @SenderRole = SCOPE_IDENTITY()
DECLARE @ReceiverRole int
INSERT INTO roles ([name],[description]) VALUES ('receiver', 'Receiver')
SET @ReceiverRole = SCOPE_IDENTITY()

/*Create test user set*/

--Modified: 2009-08-07, John Morgan
--	- added password to test user
--Modified: 2009-12-10, Jeremy Richardson
--	-Removed SIIM users.
--	-Removed QueryStringSecurityException(s)
DECLARE @SystemAdmin int
INSERT INTO [users] 
([user_name],[first_name],[last_name],[middle_name],[credentials],[title],[is_homo_sapien]) 
VALUES ( 'root','System','Administrator','','','',-1 )
SET @SystemAdmin = SCOPE_IDENTITY()
INSERT INTO user_roles ([user_id], [role_id]) VALUES (@SystemAdmin, @SysRole)
INSERT INTO user_roles ([user_id], [role_id]) VALUES (@SystemAdmin, @SenderRole)
INSERT INTO user_roles ([user_id], [role_id]) VALUES (@SystemAdmin, @ReceiverRole)

-- all users have password: 'password'

INSERT INTO user_entries([user_id], [type], [entry_key], [value], [restricted_access]) VALUES (@SystemAdmin, 'AuthExt', 'ANCR', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 0)
INSERT INTO user_transports([user_id], [transport_id], [address]) VALUES (@SystemAdmin, @EmailTransport, '')

--Added: 2009-10-21, Jeremy Richardson
DECLARE @RefUser int
INSERT INTO [users] 
([user_name],[first_name],[last_name],[middle_name],[credentials],[title],[is_homo_sapien]) 
VALUES ( 'ref','Test','Referring','','MD','',-1 )
SET @RefUser = SCOPE_IDENTITY()
INSERT INTO user_roles ([user_id], [role_id]) VALUES (@RefUser, @ReceiverRole)
INSERT INTO user_entries([user_id], [type], [entry_key], [value], [restricted_access]) VALUES (@RefUser, 'AuthExt', 'ANCR', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 0)
INSERT INTO user_transports([user_id], [transport_id], [address]) VALUES (@RefUser, @EmailTransport, '')

--Added: 2009-10-21, Jeremy Richardson
DECLARE @RadUser int
INSERT INTO [users] 
([user_name],[first_name],[last_name],[middle_name],[credentials],[title],[is_homo_sapien]) 
VALUES ( 'rad','Test','Radiologist','','MD','',-1 )
SET @RadUser = SCOPE_IDENTITY()
INSERT INTO user_roles ([user_id], [role_id]) VALUES (@RadUser, @ReceiverRole)
INSERT INTO user_entries([user_id], [type], [entry_key], [value], [restricted_access]) VALUES (@RadUser, 'AuthExt', 'ANCR', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 0)
INSERT INTO user_transports([user_id], [transport_id], [address]) VALUES (@RadUser, @EmailTransport, '')

