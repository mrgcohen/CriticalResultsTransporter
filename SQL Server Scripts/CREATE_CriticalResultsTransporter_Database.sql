:setvar DatabasePath ""
:setvar DatabaseName "CriticalResultsTransporterDevDB"
:setvar DefaultDataPath "C:\ANCR\dev\data\"
:setvar DefaultLogPath "C:\ANCR\dev\data\"

USE [master]
GO
/****** Object:  Database [$(DatabaseName)]    Script Date: 02/06/2009 10:28:51 ******/
CREATE DATABASE [$(DatabaseName)] ON  PRIMARY 
( NAME = N'$(DatabaseName)', FILENAME = N'$(DefaultDataPath)$(DatabaseName).mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'$(DatabaseName)_log', FILENAME = N'$(DefaultLogPath)$(DatabaseName)_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
EXEC dbo.sp_dbcmptlevel @dbname=N'$(DatabaseName)', @new_cmptlevel=90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [$(DatabaseName)].[dbo].[sp_fulltext_database] @action = 'disable'
end
GO
ALTER DATABASE [$(DatabaseName)] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [$(DatabaseName)] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [$(DatabaseName)] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [$(DatabaseName)] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [$(DatabaseName)] SET ARITHABORT OFF 
GO
ALTER DATABASE [$(DatabaseName)] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [$(DatabaseName)] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [$(DatabaseName)] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [$(DatabaseName)] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [$(DatabaseName)] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [$(DatabaseName)] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [$(DatabaseName)] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [$(DatabaseName)] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [$(DatabaseName)] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [$(DatabaseName)] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [$(DatabaseName)] SET  ENABLE_BROKER 
GO
ALTER DATABASE [$(DatabaseName)] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [$(DatabaseName)] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [$(DatabaseName)] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [$(DatabaseName)] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [$(DatabaseName)] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [$(DatabaseName)] SET  READ_WRITE 
GO
ALTER DATABASE [$(DatabaseName)] SET RECOVERY FULL 
GO
ALTER DATABASE [$(DatabaseName)] SET  MULTI_USER 
GO
ALTER DATABASE [$(DatabaseName)] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [$(DatabaseName)] SET DB_CHAINING OFF 