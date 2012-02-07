ALTER TABLE [dbo].[users] ADD [email] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[users] ADD [address] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[users] ADD [cell_phone] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[users] ADD [office_phone] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[users] ADD [home_phone] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[users] ADD [city] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[users] ADD [state] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[users] ADD [zip] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[users] ADD [country] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[users] ADD [npi] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[users] ADD [cell_phone_provider] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[users] ADD [pager] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[users] ADD [pager_id] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[users] ADD [pager_type] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

CREATE NONCLUSTERED INDEX [PK_token_guids] ON [dbo].[tokens]
	([token] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

update users set office_phone=(select address from user_transports where user_id=users.id and transport_id=(select id from transports where name='Manual'))
go

delete FROM user_transport_levels
WHERE (user_transport_id IN (SELECT id FROM user_transports WHERE (transport_id IN (SELECT id FROM transports WHERE name = 'Manual'))))
go

delete FROM transport_levels WHERE (transport_id IN (SELECT id FROM transports WHERE name = 'Manual'))
go

delete FROM user_transports WHERE (transport_id IN (SELECT id FROM transports WHERE name = 'Manual')) 
go

delete FROM transports WHERE name = 'Manual'
go

delete from user_entries where entry_key='Default'
go

INSERT INTO [settings] ([uuid],[owner],[entry_key],[value]) VALUES ( NEWID(), 'System', 'Session_Timeout', '60')
go

update dbo.context_types set json_template=
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
where uuid='6629949E-A451-4bae-80AF-D287343FF7B0'
go
