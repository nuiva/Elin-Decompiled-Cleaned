public class TraitFoodDough : TraitFood
{
	public override int DefaultStock => 2 + EClass.rnd(5);
}
