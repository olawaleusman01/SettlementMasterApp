
create PROC [dbo].[Get_RevenueHeadParty_Sess]
@UserId VARCHAR(50),
@RvCode varchar(30)
AS
select a.*,
       p.Party_Desc PartyName,
	   pa.Deposit_AccountNo + '-' + pa.Deposit_AcctName as PartyAccountName
from
(
	SELECT  * 
	FROM SM_RevenuHeadParty_Sess 
	WHERE USERID = @USERID 
	AND RvCode = @RvCode
) a
join SM_Party p on a.PartyId = p.ItbId
join SM_PARTYACCOUNT pa on a.PartyAccountId = pa.ItbId
