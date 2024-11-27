using System;

public class TraitContainerCurrency : TraitContainer
{
	public override InvGridSize InvGridSize
	{
		get
		{
			return InvGridSize.Purse;
		}
	}

	public override ContainerType ContainerType
	{
		get
		{
			return ContainerType.Currency;
		}
	}

	public override void Prespawn(int lv)
	{
		this.owner.AddCard(ThingGen.Create("money", -1, -1).SetNum(3 + EClass.rnd(5)));
	}
}
