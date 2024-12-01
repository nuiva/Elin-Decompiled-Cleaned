using System.Collections.Generic;

public class TraitFoodEgg : TraitFood
{
	protected int timer;

	public override bool HaveUpdate => true;

	public override int DecaySpeed => 20;

	public override void Update()
	{
		if (!owner.pos.IsHotSpring)
		{
			return;
		}
		timer++;
		if (timer > 5 && EClass.rnd(2) == 0)
		{
			owner.PlaySound("cook_micro_finish");
			owner.PlayEffect("heal_tick");
			if (this is TraitFoodEggFertilized)
			{
				TraitFoodEggFertilized.Incubate(owner.Thing, owner.pos);
				owner.ModNum(-1);
				timer = 0;
				return;
			}
			Thing thing = ThingGen.Create("onsentamago").SetNum(owner.Num);
			CraftUtil.MakeDish(thing, new List<Thing> { owner.Thing }, 999);
			thing.elements.ModBase(756, EClass._zone.elements.Has(3701) ? 50 : 30);
			EClass._zone.AddCard(thing, owner.pos);
			owner.Destroy();
		}
	}
}
