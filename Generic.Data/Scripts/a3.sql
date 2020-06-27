
create proc Post_RevenuHeadParty_Session
(
	        @RvCode varchar(30)
           ,@PartyId varchar(30)
           ,@PartyValue varchar(20)
           ,@PartyAccountId varchar(350)
           ,@CreateDate datetime
           ,@UserId varchar(50)
)
as
declare @RespCode int,@RespMessage varchar(50)
DECLARE @CNT INT = 0


		INSERT INTO SM_RevenuHeadParty_Sess
           (--[ID]
            RvCode
		   ,PartyId
		   ,PartyValue
		   ,PartyAccountId
           ,CreateDate
           ,UserId
		   )
     VALUES
           (
		    @RvCode
           ,@PartyId
           ,@PartyValue
           ,@PartyAccountId
           ,@CreateDate
           ,@UserId
		   )
		   if @@ROWCOUNT > 0
		   begin
				   
				select @RespCode = 0 ,@RespMessage = 'Success'
				select @RespCode as RespCode, @RespMessage as RespMessage
			end
			else
			begin
				   
				select @RespCode = 1 ,@RespMessage = 'Problem Processing Request'
				select @RespCode as RespCode, @RespMessage as RespMessage
			end
