public class TraitContainerCompost : TraitBrewery
{
	public override Type type => Type.Fertilizer;

	public override string idMsg => "agedFood";

	public override bool CanChildDecay(Card c)
	{
		return false;
	}

	public override string GetProductID(Card c)
	{
		return "fertilizer";
	}
}
