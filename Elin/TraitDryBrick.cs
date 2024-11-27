using System;

public class TraitDryBrick : TraitBrewery
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
			return "driedFood";
		}
	}

	public override string GetProductID(Card c)
	{
		return "jerky";
	}

	public override void OnProduce(Card c)
	{
		c.elements.SetTo(72, -10);
	}
}
