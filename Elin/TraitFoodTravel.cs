public class TraitFoodTravel : TraitFood
{
	public override int DefaultStock => 5 + EClass.rnd(10);
}
