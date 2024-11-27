using System;

public class InvOwnerRefuel : InvOwnerDraglet
{
	public override InvOwnerDraglet.ProcessType processType
	{
		get
		{
			return InvOwnerDraglet.ProcessType.Consume;
		}
	}

	public override string langTransfer
	{
		get
		{
			return "invRefuel";
		}
	}

	public override bool DenyImportant
	{
		get
		{
			return true;
		}
	}

	public override bool AllowStockIngredients
	{
		get
		{
			return true;
		}
	}

	public InvOwnerRefuel(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.None) : base(owner, container, _currency)
	{
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return this.owner.trait.IsFuel(t);
	}

	public override void _OnProcess(Thing t)
	{
		int fuelValue = this.owner.trait.GetFuelValue(t);
		int num = (this.owner.trait.MaxFuel - this.owner.c_charges) / fuelValue;
		if (num == 0)
		{
			SE.BeepSmall();
			Msg.Say("fuelFull");
			return;
		}
		if (t.Num < num)
		{
			num = t.Num;
		}
		Thing t2 = t.Split(num);
		this.owner.trait.Refuel(t2);
	}
}
