
create table SM_RevenuHeadParty_Sess
(
    [Pid] [varchar](150) NOT NULL default (newid()) primary key,
	[RvCode] [varchar](50) NULL,
	[PartyId] int NULL,
	[PartyValue] decimal(15,2) NULL,
	[PartyAccountId] int NULL,
	[CreateDate] [datetime] NULL,
	[UserId] [varchar](50) NULL
)


create table SM_RevenuHeadParty
(
    [ItbId] [int] IDENTITY(1,1) NOT NULL primary key,
	[PartyId] int NULL,
	[PartyValue] decimal(15,2) NULL,
	[PartyAccountId] int NULL,
	[RvCodeItbId] [int] references SM_REVENUEHEAD(ITBID),
	[CreateDate] [datetime] NULL,
	[UserId] [varchar](50) NULL
)

create table SM_RevenuHeadPartyTemp
(
    [ItbId] bigint IDENTITY(1,1) NOT NULL primary key,
	[PartyId] int NULL,
	[PartyValue] decimal(15,2) NULL,
	[PartyAccountId] int NULL,
	[RvCodeItbId] [int] references SM_REVENUEHEAD(ITBID),
	[CreateDate] [datetime] NULL,
	[UserId] [varchar](50) NULL,
	BatchId varchar(30)
)

