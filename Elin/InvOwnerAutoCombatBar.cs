public class InvOwnerAutoCombatBar : InvOwner
{
	public int index;

	public virtual bool ShowFuel => false;

	public virtual string langWhat => "";

	public override bool AllowTransfer => true;

	public override bool AllowAutouse => false;

	public override bool AllowContext => false;

	public override bool UseGuide => true;

	public override bool CopyOnTransfer => true;

	public override bool AllowHold(Thing t)
	{
		return false;
	}

	public InvOwnerAutoCombatBar(Card owner, Card container = null, CurrencyType _currency = CurrencyType.None)
		: base(owner, container, _currency)
	{
	}
}
