using System;

public class TraitAgingShelf : TraitBrewery
{
	public override TraitBrewery.Type type
	{
		get
		{
			return TraitBrewery.Type.Food;
		}
	}

	public override string idMsg
	{
		get
		{
			return "agedFood";
		}
	}

	public override string GetProductID(Card c)
	{
		return "cheese";
	}

	public override void OnProduce(Card c)
	{
		c.elements.SetTo(73, -10);
	}
}
