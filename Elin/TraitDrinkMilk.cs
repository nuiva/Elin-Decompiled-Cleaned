public class TraitDrinkMilk : TraitDrink
{
	public override EffectId IdEffect => EffectId.DrinkMilk;

	public override int DefaultStock => 3 + EClass.rnd(5);
}
