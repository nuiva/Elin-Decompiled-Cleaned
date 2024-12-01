public class TraitDryBrick : TraitBrewery
{
	public override Type type => Type.Food;

	public override string idMsg => "driedFood";

	public override string GetProductID(Card c)
	{
		return "jerky";
	}

	public override void OnProduce(Card c)
	{
		c.elements.SetTo(72, -10);
	}
}
