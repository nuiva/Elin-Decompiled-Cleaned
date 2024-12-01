public class TraitFoodPrepared : TraitFood
{
	public override int DefaultStock => 5 + EClass.rnd(5);
}
