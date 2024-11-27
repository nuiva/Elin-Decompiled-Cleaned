using System;

public class InvOwnerGacha : InvOwnerDraglet
{
	public override string langTransfer
	{
		get
		{
			return "gacha";
		}
	}

	public InvOwnerGacha(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.None) : base(owner, container, _currency)
	{
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return t.id == this.gacha.GetIdCoin();
	}

	public override bool SingleTarget
	{
		get
		{
			return true;
		}
	}

	public override InvOwnerDraglet.ProcessType processType
	{
		get
		{
			return InvOwnerDraglet.ProcessType.Consume;
		}
	}

	public override void _OnProcess(Thing t)
	{
		SE.Play("gacha");
		this.gacha.PlayGacha(1);
		t.Destroy();
	}

	public TraitGacha gacha;
}
