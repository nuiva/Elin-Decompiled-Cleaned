using System;
using UnityEngine;

public class ConBurning : BadCondition
{
	public override int GetPhase()
	{
		return 0;
	}

	public override void Tick()
	{
		if (base.value > 10)
		{
			base.value = 10;
		}
		if (EClass.rnd(2) == 0)
		{
			this.owner.PlayEffect("fire_step", true, 0f, default(Vector3));
			this.owner.PlaySound("fire_step", 1f, true);
			this.owner.DamageHP(1 + EClass.rnd(this.owner.MaxHP / (this.owner.IsPowerful ? 200 : 20) + 3), AttackSource.Condition, null);
		}
		base.Mod(-1, false);
	}
}
