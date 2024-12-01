public class InvOwnerRefuel : InvOwnerDraglet
{
	public override ProcessType processType => ProcessType.Consume;

	public override string langTransfer => "invRefuel";

	public override bool DenyImportant => true;

	public override bool AllowStockIngredients
	{
		get
		{
			if (!EClass._zone.IsPCFaction)
			{
				return EClass._zone is Zone_Tent;
			}
			return true;
		}
	}

	public InvOwnerRefuel(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.None)
		: base(owner, container, _currency)
	{
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return owner.trait.IsFuel(t);
	}

	public override void _OnProcess(Thing t)
	{
		int fuelValue = owner.trait.GetFuelValue(t);
		int num = (owner.trait.MaxFuel - owner.c_charges) / fuelValue;
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
		owner.trait.Refuel(t2);
	}
}
