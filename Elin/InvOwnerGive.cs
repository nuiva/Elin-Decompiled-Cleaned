public class InvOwnerGive : InvOwnerDraglet
{
	public Chara chara;

	public override string langTransfer => "invGive";

	public override ProcessType processType => ProcessType.Consume;

	public override bool DenyImportant => true;

	public InvOwnerGive(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.Money)
		: base(owner, container, _currency)
	{
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return !t.trait.CanOnlyCarry;
	}

	public override void _OnProcess(Thing t)
	{
		SE.Play("offering");
	}
}
