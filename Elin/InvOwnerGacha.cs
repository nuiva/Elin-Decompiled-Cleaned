public class InvOwnerGacha : InvOwnerDraglet
{
	public TraitGacha gacha;

	public override string langTransfer => "gacha";

	public override bool SingleTarget => true;

	public override ProcessType processType => ProcessType.Consume;

	public InvOwnerGacha(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.None)
		: base(owner, container, _currency)
	{
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return t.id == gacha.GetIdCoin();
	}

	public override void _OnProcess(Thing t)
	{
		SE.Play("gacha");
		gacha.PlayGacha(1);
		t.Destroy();
	}
}
