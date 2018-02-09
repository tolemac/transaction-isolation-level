using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace IsolationLevel
{
    public static class DropAndCreate
    {
        public static void DoIt(SqlConnection conn)
        {
            var dropCreate = @"
USE [master]
GO

/****** Object:  Database [IsolationTest]    Script Date: 09/02/2018 20:06:01 ******/
IF EXISTS(select * from sys.databases where name='IsolationTest')
DROP DATABASE [IsolationTest]
GO

/****** Object:  Database [IsolationTest]    Script Date: 09/02/2018 20:06:01 ******/
CREATE DATABASE [IsolationTest]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'IsolationTest', FILENAME = N'C:\Users\jros\IsolationTest.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'IsolationTest_log', FILENAME = N'C:\Users\jros\IsolationTest_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO

ALTER DATABASE [IsolationTest] SET COMPATIBILITY_LEVEL = 130
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [IsolationTest].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [IsolationTest] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [IsolationTest] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [IsolationTest] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [IsolationTest] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [IsolationTest] SET ARITHABORT OFF 
GO

ALTER DATABASE [IsolationTest] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [IsolationTest] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [IsolationTest] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [IsolationTest] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [IsolationTest] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [IsolationTest] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [IsolationTest] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [IsolationTest] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [IsolationTest] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [IsolationTest] SET  DISABLE_BROKER 
GO

ALTER DATABASE [IsolationTest] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [IsolationTest] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [IsolationTest] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [IsolationTest] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [IsolationTest] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [IsolationTest] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [IsolationTest] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [IsolationTest] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [IsolationTest] SET  MULTI_USER 
GO

ALTER DATABASE [IsolationTest] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [IsolationTest] SET DB_CHAINING OFF 
GO

ALTER DATABASE [IsolationTest] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [IsolationTest] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [IsolationTest] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [IsolationTest] SET QUERY_STORE = OFF
GO

USE [IsolationTest]
GO

ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO

ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO

ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO

ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO

ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO

ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO

ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO

ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO

ALTER DATABASE [IsolationTest] SET  READ_WRITE 
GO

USE [IsolationTest]
GO

/****** Object:  Table [dbo].[TestTable]    Script Date: 09/02/2018 20:22:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TestTable](
	[Id] [bigint] NOT NULL,
	[Number] [bigint] NOT NULL,
 CONSTRAINT [PK_TestTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


insert into dbo.TestTable values (1, 1)
GO
";

            try
            {

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }


                string script = dropCreate;

                // split script on GO command
                IEnumerable<string> commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                foreach (string commandString in commandStrings)
                {
                    if (commandString.Trim() != "")
                    {
                        new SqlCommand(commandString, conn).ExecuteNonQuery();
                    }
                }

            }
            finally
            {
                conn.Close();
            }

        }
    }
}
