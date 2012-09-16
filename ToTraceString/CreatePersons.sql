USE [School]
GO



CREATE TABLE [dbo].[Person](
	[PersonID] [int] IDENTITY(1,1) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](20) NULL,
	[HireDate] [datetime] NULL,
	[EnrollmentDate] [datetime] NULL,
	[Image] [varbinary](50) NULL,
	[BigIntfield] [bigint] NULL,
	[Floatfield] [float] NULL,
	[xmlfield] [xml] NULL,
	[bitfield] [bit] NULL,
	[charfield] [char](10) NULL,
	[datefield] [date] NULL,
	[datetime2field] [datetime2](7) NULL,
	[datetimeoffsetfield] [datetimeoffset](7) NULL,
	[geographyfield] [geography] NULL,
	[geometryfield] [geometry] NULL,
	[hierarchyfield] [hierarchyid] NULL,
	[moneyfield] [money] NULL,
	[ncharfield] [nchar](10) NULL,
	[numericfield] [numeric](18, 0) NULL,
	[realfield] [real] NULL,
	[smalldatetimefield] [smalldatetime] NULL,
	[smallintfield] [smallint] NULL,
	[smallmoneyfield] [smallmoney] NULL,
	[guidfield] [uniqueidentifier] NULL,
	[decimalfield] [decimal](18, 4) NULL,
 CONSTRAINT [PK_School.Student] PRIMARY KEY CLUSTERED 
(
	[PersonID] ASC
)
GO


