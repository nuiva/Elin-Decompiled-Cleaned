public class InvOwnerBlend : InvOwnerDraglet
{
	public override bool CanTargetAlly => true;

	public override string langTransfer => "invBlend";

	public override bool ShouldShowGuide(Thing t)
	{
		if (t != owner)
		{
			return owner.trait.CanBlend(t);
		}
		return false;
	}

	public InvOwnerBlend(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.None)
		: base(owner, container, _currency)
	{
	}

	public override void _OnProcess(Thing t)
	{
		EClass.pc.Say("dip", EClass.pc, t, owner.GetName(NameStyle.Full, 1));
		SE.Change();
		owner.trait.OnBlend(t, t.GetRootCard().Chara ?? EClass.pc);
	}
}
