
/****** Object:  Table [dbo].[users]    Script Date: 04/04/2011 07:56:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_name] [varchar](100) NOT NULL,
	[first_name] [varchar](100) NOT NULL,
	[last_name] [varchar](100) NOT NULL,
	[middle_name] [varchar](100) NOT NULL,
	[title] [varchar](100) NULL,
	[is_homo_sapien] [bit] NOT NULL CONSTRAINT [DF_users_is_homo_sapien]  DEFAULT ((-1)),
	[credentials] [varchar](50) NULL,
	[enabled] [bit] NOT NULL CONSTRAINT [DF_users_enabled]  DEFAULT ((-1)),
	[email] [varchar](50) NULL,
	[address] [varchar](100) NULL,
	[cell_phone] [varchar](50) NULL,
	[office_phone] [varchar](50) NULL,
	[home_phone] [varchar](50) NULL,
	[city] [varchar](50) NULL,
	[state] [varchar](50) NULL,
	[zip] [varchar](50) NULL,
	[country] [varchar](50) NULL,
	[npi] [varchar](50) NULL,
	[cell_phone_provider] [varchar](50) NULL,
	[pager] [varchar](50) NULL,
	[pager_id] [varchar](50) NULL,
	[pager_type] [varchar](50) NULL,
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[context_types]    Script Date: 04/04/2011 07:56:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[context_types](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[uuid] [uniqueidentifier] NOT NULL,
	[namespace_uri] [varchar](max) NOT NULL,
	[name] [varchar](100) NOT NULL,
	[description] [varchar](max) NOT NULL,
	[xml_schema] [xml] NULL,
	[json_template] [varchar](max) NULL,
 CONSTRAINT [PK_context_types] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[roles]    Script Date: 04/04/2011 07:56:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[roles](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](25) NOT NULL,
	[description] [varchar](100) NOT NULL,
 CONSTRAINT [PK_roles] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[levels]    Script Date: 04/04/2011 07:56:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[levels](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[uuid] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_levels_uuid]  DEFAULT (newid()),
	[name] [varchar](100) NOT NULL,
	[description] [varchar](max) NOT NULL,
	[short_description] [varchar](50) NOT NULL,
	[color_value] [varchar](25) NOT NULL,
	[escalation_timeout] [datetime] NOT NULL,
	[due_timeout] [datetime] NOT NULL,
	[direct_contact_required] [bit] NOT NULL CONSTRAINT [DF_levels_direct_contact_required]  DEFAULT ((0)),
 CONSTRAINT [PK_level] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[transports]    Script Date: 04/04/2011 07:56:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[transports](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
	[transport_uri] [varchar](max) NOT NULL,
	[friendly_address_name] [varchar](50) NULL,
 CONSTRAINT [PK_transports] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[settings]    Script Date: 04/04/2011 07:56:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[settings](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[uuid] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_settings_uuid]  DEFAULT (newid()),
	[owner] [varchar](50) NOT NULL,
	[entry_key] [varchar](50) NOT NULL,
	[value] [varchar](max) NULL,
	[xml_value] [xml] NULL,
 CONSTRAINT [PK_settings] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[proxies]    Script Date: 04/04/2011 07:56:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[proxies](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[uuid] [uniqueidentifier] NOT NULL,
	[relationship_description] [varchar](128) NOT NULL,
	[user_id] [int] NOT NULL,
	[proxy_user_id] [int] NOT NULL,
 CONSTRAINT [PK_proxies] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[user_entries]    Script Date: 04/04/2011 07:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[user_entries](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[type] [varchar](50) NOT NULL,
	[entry_key] [varchar](50) NOT NULL,
	[value] [varchar](max) NULL,
	[xml_value] [xml] NULL,
	[restricted_access] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_credentials] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[user_id] ASC,
	[type] ASC,
	[entry_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[user_transports]    Script Date: 04/04/2011 07:56:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[user_transports](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[transport_id] [int] NOT NULL,
	[address] [varchar](max) NOT NULL,
 CONSTRAINT [PK_user_transports] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ratings]    Script Date: 04/04/2011 07:56:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ratings](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[result_id] [int] NOT NULL,
	[user_id] [int] NOT NULL,
	[value] [int] NOT NULL,
	[comments] [varchar](max) NOT NULL,
 CONSTRAINT [PK_ratings] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[user_results]    Script Date: 04/04/2011 07:56:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[user_results](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[uuid] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_user_results_uuid]  DEFAULT (newid()),
	[user_id] [int] NOT NULL,
	[result_id] [int] NOT NULL,
	[tag] [varchar](128) NOT NULL,
	[description] [varchar](max) NOT NULL,
 CONSTRAINT [PK_user_results] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[results]    Script Date: 04/04/2011 07:56:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[results](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[uuid] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_results_uuid]  DEFAULT (newid()),
	[message] [varchar](max) NOT NULL,
	[sender_user_id] [int] NOT NULL,
	[receiver_user_id] [int] NOT NULL,
	[level_id] [int] NOT NULL,
	[creation_time] [datetime] NOT NULL,
	[proxy_sender_user_id] [int] NULL,
	[escalation_time] [datetime] NOT NULL,
	[due_time] [datetime] NOT NULL,
	[acknowledgment_time] [datetime] NULL,
 CONSTRAINT [PK_results] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[user_roles]    Script Date: 04/04/2011 07:56:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_roles](
	[user_id] [int] NOT NULL,
	[role_id] [int] NOT NULL,
 CONSTRAINT [PK_user_roles] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC,
	[role_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tokens]    Script Date: 04/04/2011 07:56:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tokens](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[token] [uniqueidentifier] NOT NULL,
	[ipv4] [varchar](15) NOT NULL,
	[created_time] [datetime] NOT NULL,
	[updated_time] [datetime] NOT NULL,
 CONSTRAINT [PK_tokens] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[acknowledgments]    Script Date: 04/04/2011 07:56:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[acknowledgments](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[result_id] [int] NOT NULL,
	[creation_time] [datetime] NOT NULL,
	[notes] [varchar](max) NOT NULL,
 CONSTRAINT [PK_acknowledgments] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[result_contexts]    Script Date: 04/04/2011 07:56:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[result_contexts](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[result_id] [int] NOT NULL,
	[context_type_id] [int] NOT NULL,
	[json_value] [varchar](max) NULL,
	[xml_value] [xml] NULL,
	[exam_key] [varchar](64) NOT NULL CONSTRAINT [DF_result_contexts_exam_key]  DEFAULT (''),
	[patient_key] [varchar](64) NOT NULL CONSTRAINT [DF_result_contexts_patient_key]  DEFAULT (''),
 CONSTRAINT [PK_result_contexts] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[transport_levels]    Script Date: 04/04/2011 07:56:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[transport_levels](
	[transport_id] [int] NOT NULL,
	[level_id] [int] NOT NULL,
	[mandatory] [bit] NOT NULL CONSTRAINT [DF_transport_levels_mandatory]  DEFAULT ((0)),
 CONSTRAINT [PK_transport_levels] PRIMARY KEY CLUSTERED 
(
	[transport_id] ASC,
	[level_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_transport_levels]    Script Date: 04/04/2011 07:56:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_transport_levels](
	[user_transport_id] [int] NOT NULL,
	[level_id] [int] NOT NULL,
 CONSTRAINT [PK_user_transport_levels] PRIMARY KEY CLUSTERED 
(
	[user_transport_id] ASC,
	[level_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[notifications]    Script Date: 04/04/2011 07:56:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[notifications](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[uuid] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_notifications_uuid]  DEFAULT (newid()),
	[user_transport_id] [int] NOT NULL,
	[result_id] [int] NOT NULL,
	[creation_time] [datetime] NOT NULL,
	[notes] [varchar](max) NOT NULL,
	[state] [varchar](50) NOT NULL,
 CONSTRAINT [PK_notifications] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[list_views]    Script Date: 04/04/2011 07:56:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[list_views]
AS
SELECT     dbo.results.uuid AS result_uuid, dbo.results.message AS result_message, dbo.results.creation_time, sender.user_name AS sender_user_name, 
                      sender.title AS sender_title, sender.last_name AS sender_last_name, sender.first_name AS sender_first_name, 
                      receiver.user_name AS receiver_user_name, receiver.last_name AS receiver_last_name, receiver.first_name AS receiver_first_Name, 
                      receiver.title AS receiver_title, dbo.result_contexts.json_value AS result_context_json, dbo.result_contexts.xml_value AS result_context_xml, 
                      dbo.levels.uuid AS level_uuid, dbo.context_types.name AS context_type_name, dbo.context_types.namespace_uri AS context_type, 
                      dbo.results.escalation_time, dbo.results.due_time, dbo.result_contexts.exam_key, dbo.result_contexts.patient_key, 
                      dbo.results.acknowledgment_time
FROM         dbo.context_types INNER JOIN
                      dbo.result_contexts ON dbo.context_types.id = dbo.result_contexts.context_type_id RIGHT OUTER JOIN
                      dbo.levels INNER JOIN
                      dbo.results ON dbo.levels.id = dbo.results.level_id INNER JOIN
                      dbo.users AS receiver ON dbo.results.receiver_user_id = receiver.id INNER JOIN
                      dbo.users AS sender ON dbo.results.sender_user_id = sender.id ON dbo.result_contexts.result_id = dbo.results.id
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[59] 4[2] 2[14] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "levels"
            Begin Extent = 
               Top = 310
               Left = 620
               Bottom = 460
               Right = 799
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "results"
            Begin Extent = 
               Top = 16
               Left = 41
               Bottom = 133
               Right = 210
            End
            DisplayFlags = 280
            TopColumn = 7
         End
         Begin Table = "receiver"
            Begin Extent = 
               Top = 266
               Left = 404
               Bottom = 457
               Right = 569
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "sender"
            Begin Extent = 
               Top = 264
               Left = 131
               Bottom = 457
               Right = 296
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "context_types"
            Begin Extent = 
               Top = 14
               Left = 813
               Bottom = 163
               Right = 973
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "result_contexts"
            Begin Extent = 
               Top = 11
               Left = 526
               Bottom = 128
               Right = 694
            End
            DisplayFlags = 280
            TopColumn = 3
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'list_views'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'list_views'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'list_views'
GO
/****** Object:  ForeignKey [FK_acknowledgments_results]    Script Date: 04/04/2011 07:56:23 ******/
ALTER TABLE [dbo].[acknowledgments]  WITH CHECK ADD  CONSTRAINT [FK_acknowledgments_results] FOREIGN KEY([result_id])
REFERENCES [dbo].[results] ([id])
GO
ALTER TABLE [dbo].[acknowledgments] CHECK CONSTRAINT [FK_acknowledgments_results]
GO
/****** Object:  ForeignKey [FK_acknowledgments_users]    Script Date: 04/04/2011 07:56:24 ******/
ALTER TABLE [dbo].[acknowledgments]  WITH CHECK ADD  CONSTRAINT [FK_acknowledgments_users] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[acknowledgments] CHECK CONSTRAINT [FK_acknowledgments_users]
GO
/****** Object:  ForeignKey [FK_notifications_results]    Script Date: 04/04/2011 07:56:29 ******/
ALTER TABLE [dbo].[notifications]  WITH CHECK ADD  CONSTRAINT [FK_notifications_results] FOREIGN KEY([result_id])
REFERENCES [dbo].[results] ([id])
GO
ALTER TABLE [dbo].[notifications] CHECK CONSTRAINT [FK_notifications_results]
GO
/****** Object:  ForeignKey [FK_notifications_user_transports]    Script Date: 04/04/2011 07:56:29 ******/
ALTER TABLE [dbo].[notifications]  WITH CHECK ADD  CONSTRAINT [FK_notifications_user_transports] FOREIGN KEY([user_transport_id])
REFERENCES [dbo].[user_transports] ([id])
GO
ALTER TABLE [dbo].[notifications] CHECK CONSTRAINT [FK_notifications_user_transports]
GO
/****** Object:  ForeignKey [FK_proxies_users]    Script Date: 04/04/2011 07:56:30 ******/
ALTER TABLE [dbo].[proxies]  WITH CHECK ADD  CONSTRAINT [FK_proxies_users] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[proxies] CHECK CONSTRAINT [FK_proxies_users]
GO
/****** Object:  ForeignKey [FK_proxies_users_proxy]    Script Date: 04/04/2011 07:56:30 ******/
ALTER TABLE [dbo].[proxies]  WITH CHECK ADD  CONSTRAINT [FK_proxies_users_proxy] FOREIGN KEY([proxy_user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[proxies] CHECK CONSTRAINT [FK_proxies_users_proxy]
GO
/****** Object:  ForeignKey [FK_ratings_results]    Script Date: 04/04/2011 07:56:31 ******/
ALTER TABLE [dbo].[ratings]  WITH CHECK ADD  CONSTRAINT [FK_ratings_results] FOREIGN KEY([result_id])
REFERENCES [dbo].[results] ([id])
GO
ALTER TABLE [dbo].[ratings] CHECK CONSTRAINT [FK_ratings_results]
GO
/****** Object:  ForeignKey [FK_ratings_users]    Script Date: 04/04/2011 07:56:31 ******/
ALTER TABLE [dbo].[ratings]  WITH CHECK ADD  CONSTRAINT [FK_ratings_users] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[ratings] CHECK CONSTRAINT [FK_ratings_users]
GO
/****** Object:  ForeignKey [FK_result_contexts_context_types]    Script Date: 04/04/2011 07:56:33 ******/
ALTER TABLE [dbo].[result_contexts]  WITH CHECK ADD  CONSTRAINT [FK_result_contexts_context_types] FOREIGN KEY([context_type_id])
REFERENCES [dbo].[context_types] ([id])
GO
ALTER TABLE [dbo].[result_contexts] CHECK CONSTRAINT [FK_result_contexts_context_types]
GO
/****** Object:  ForeignKey [FK_result_contexts_results]    Script Date: 04/04/2011 07:56:33 ******/
ALTER TABLE [dbo].[result_contexts]  WITH CHECK ADD  CONSTRAINT [FK_result_contexts_results] FOREIGN KEY([result_id])
REFERENCES [dbo].[results] ([id])
GO
ALTER TABLE [dbo].[result_contexts] CHECK CONSTRAINT [FK_result_contexts_results]
GO
/****** Object:  ForeignKey [FK_results_levels]    Script Date: 04/04/2011 07:56:35 ******/
ALTER TABLE [dbo].[results]  WITH CHECK ADD  CONSTRAINT [FK_results_levels] FOREIGN KEY([level_id])
REFERENCES [dbo].[levels] ([id])
GO
ALTER TABLE [dbo].[results] CHECK CONSTRAINT [FK_results_levels]
GO
/****** Object:  ForeignKey [FK_results_users]    Script Date: 04/04/2011 07:56:36 ******/
ALTER TABLE [dbo].[results]  WITH CHECK ADD  CONSTRAINT [FK_results_users] FOREIGN KEY([proxy_sender_user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[results] CHECK CONSTRAINT [FK_results_users]
GO
/****** Object:  ForeignKey [FK_results_users_receiver]    Script Date: 04/04/2011 07:56:36 ******/
ALTER TABLE [dbo].[results]  WITH CHECK ADD  CONSTRAINT [FK_results_users_receiver] FOREIGN KEY([receiver_user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[results] CHECK CONSTRAINT [FK_results_users_receiver]
GO
/****** Object:  ForeignKey [FK_results_users_sender]    Script Date: 04/04/2011 07:56:36 ******/
ALTER TABLE [dbo].[results]  WITH CHECK ADD  CONSTRAINT [FK_results_users_sender] FOREIGN KEY([sender_user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[results] CHECK CONSTRAINT [FK_results_users_sender]
GO
/****** Object:  ForeignKey [FK_tokens_users]    Script Date: 04/04/2011 07:56:39 ******/
ALTER TABLE [dbo].[tokens]  WITH CHECK ADD  CONSTRAINT [FK_tokens_users] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[tokens] CHECK CONSTRAINT [FK_tokens_users]
GO
/****** Object:  ForeignKey [FK_transport_levels_levels]    Script Date: 04/04/2011 07:56:40 ******/
ALTER TABLE [dbo].[transport_levels]  WITH CHECK ADD  CONSTRAINT [FK_transport_levels_levels] FOREIGN KEY([level_id])
REFERENCES [dbo].[levels] ([id])
GO
ALTER TABLE [dbo].[transport_levels] CHECK CONSTRAINT [FK_transport_levels_levels]
GO
/****** Object:  ForeignKey [FK_transport_levels_transports]    Script Date: 04/04/2011 07:56:40 ******/
ALTER TABLE [dbo].[transport_levels]  WITH CHECK ADD  CONSTRAINT [FK_transport_levels_transports] FOREIGN KEY([transport_id])
REFERENCES [dbo].[transports] ([id])
GO
ALTER TABLE [dbo].[transport_levels] CHECK CONSTRAINT [FK_transport_levels_transports]
GO
/****** Object:  ForeignKey [FK_credentials_users]    Script Date: 04/04/2011 07:56:43 ******/
ALTER TABLE [dbo].[user_entries]  WITH CHECK ADD  CONSTRAINT [FK_credentials_users] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[user_entries] CHECK CONSTRAINT [FK_credentials_users]
GO
/****** Object:  ForeignKey [FK_user_results_results]    Script Date: 04/04/2011 07:56:44 ******/
ALTER TABLE [dbo].[user_results]  WITH CHECK ADD  CONSTRAINT [FK_user_results_results] FOREIGN KEY([result_id])
REFERENCES [dbo].[results] ([id])
GO
ALTER TABLE [dbo].[user_results] CHECK CONSTRAINT [FK_user_results_results]
GO
/****** Object:  ForeignKey [FK_user_results_users]    Script Date: 04/04/2011 07:56:44 ******/
ALTER TABLE [dbo].[user_results]  WITH CHECK ADD  CONSTRAINT [FK_user_results_users] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[user_results] CHECK CONSTRAINT [FK_user_results_users]
GO
/****** Object:  ForeignKey [FK_user_roles_roles]    Script Date: 04/04/2011 07:56:45 ******/
ALTER TABLE [dbo].[user_roles]  WITH CHECK ADD  CONSTRAINT [FK_user_roles_roles] FOREIGN KEY([role_id])
REFERENCES [dbo].[roles] ([id])
GO
ALTER TABLE [dbo].[user_roles] CHECK CONSTRAINT [FK_user_roles_roles]
GO
/****** Object:  ForeignKey [FK_user_roles_users]    Script Date: 04/04/2011 07:56:45 ******/
ALTER TABLE [dbo].[user_roles]  WITH CHECK ADD  CONSTRAINT [FK_user_roles_users] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[user_roles] CHECK CONSTRAINT [FK_user_roles_users]
GO
/****** Object:  ForeignKey [FK_user_transport_levels_levels]    Script Date: 04/04/2011 07:56:46 ******/
ALTER TABLE [dbo].[user_transport_levels]  WITH CHECK ADD  CONSTRAINT [FK_user_transport_levels_levels] FOREIGN KEY([level_id])
REFERENCES [dbo].[levels] ([id])
GO
ALTER TABLE [dbo].[user_transport_levels] CHECK CONSTRAINT [FK_user_transport_levels_levels]
GO
/****** Object:  ForeignKey [FK_user_transport_levels_user_transports]    Script Date: 04/04/2011 07:56:46 ******/
ALTER TABLE [dbo].[user_transport_levels]  WITH CHECK ADD  CONSTRAINT [FK_user_transport_levels_user_transports] FOREIGN KEY([user_transport_id])
REFERENCES [dbo].[user_transports] ([id])
GO
ALTER TABLE [dbo].[user_transport_levels] CHECK CONSTRAINT [FK_user_transport_levels_user_transports]
GO
/****** Object:  ForeignKey [FK_user_transports_transports]    Script Date: 04/04/2011 07:56:47 ******/
ALTER TABLE [dbo].[user_transports]  WITH CHECK ADD  CONSTRAINT [FK_user_transports_transports] FOREIGN KEY([transport_id])
REFERENCES [dbo].[transports] ([id])
GO
ALTER TABLE [dbo].[user_transports] CHECK CONSTRAINT [FK_user_transports_transports]
GO
/****** Object:  ForeignKey [FK_user_transports_users]    Script Date: 04/04/2011 07:56:47 ******/
ALTER TABLE [dbo].[user_transports]  WITH CHECK ADD  CONSTRAINT [FK_user_transports_users] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[user_transports] CHECK CONSTRAINT [FK_user_transports_users]
GO


