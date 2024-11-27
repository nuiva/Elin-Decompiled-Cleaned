using System;
using System.Collections.Generic;
using UnityEngine;

public class TraitFoodEgg : TraitFood
{
	public override bool HaveUpdate
	{
		get
		{
			return true;
		}
	}

	public override int DecaySpeed
	{
		get
		{
			return 20;
		}
	}

	public override void Update()
	{
		if (this.owner.pos.IsHotSpring)
		{
			this.timer++;
			if (this.timer > 5 && EClass.rnd(2) == 0)
			{
				this.owner.PlaySound("cook_micro_finish", 1f, true);
				this.owner.PlayEffect("heal_tick", true, 0f, default(Vector3));
				if (this is TraitFoodEggFertilized)
				{
					TraitFoodEggFertilized.Incubate(this.owner.Thing, this.owner.pos, null);
					this.owner.ModNum(-1, true);
					this.timer = 0;
					return;
				}
				Thing thing = ThingGen.Create("onsentamago", -1, -1).SetNum(this.owner.Num);
				CraftUtil.MakeDish(thing, new List<Thing>
				{
					this.owner.Thing
				}, 999, null);
				thing.elements.ModBase(756, EClass._zone.elements.Has(3701) ? 50 : 30);
				EClass._zone.AddCard(thing, this.owner.pos);
				this.owner.Destroy();
			}
		}
	}

	protected int timer;
}
