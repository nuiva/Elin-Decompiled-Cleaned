using System;

public class TraitContainerCompost : TraitBrewery
{
	public override bool CanChildDecay(Card c)
	{
		return false;
	}

	public override TraitBrewery.Type type
	{
		get
		{
			return TraitBrewery.Type.Fertilizer;
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
		return "fertilizer";
	}
}
