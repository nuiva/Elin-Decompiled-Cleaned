public class TraitFoodSeasoning : TraitFood
{
	public override int DefaultStock => 2 + EClass.rnd(5);
}
