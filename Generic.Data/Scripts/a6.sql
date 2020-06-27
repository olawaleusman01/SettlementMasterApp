create proc Get_RVHead_Party
@P_RvItbId int
as
begin

	select a.*,
       p.Party_Desc PartyName,
	   pa.Deposit_AccountNo + '-' + pa.Deposit_AcctName as PartyAccountName
	from
	(
		SELECT  * 
		FROM SM_RevenuHeadParty 
		where RvCodeItbId = @P_RvItbId
	) a
	join SM_Party p on a.PartyId = p.ItbId
	join SM_PARTYACCOUNT pa on a.PartyAccountId = pa.ItbId

end