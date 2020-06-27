
create PROC [dbo].[Purge_RevenueHeadParty_Sess]
@RvCode VARCHAR(50),
@USERID VARCHAR(50)
AS
DELETE FROM SM_RevenuHeadParty_sess 
WHERE RvCode = @RvCode and USERID = @USERID 

