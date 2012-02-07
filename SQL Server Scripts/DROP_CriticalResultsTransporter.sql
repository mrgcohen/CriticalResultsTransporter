USE [master]
GO
/****** Object:  Database [ClinicalMessaging]    Script Date: 02/06/2009 10:36:27 ******/
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'CriticalResultsTransporter')
DROP DATABASE [CriticalResultsTransporter]