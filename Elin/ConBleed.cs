using System;
using UnityEngine;

public class ConBleed : BadCondition
{
	public override Emo2 EmoIcon
	{
		get
		{
			return Emo2.bleeding;
		}
	}

	public override void Tick()
	{
		this.owner.DamageHP(EClass.rnd(Mathf.Clamp(this.owner.hp * (1 + base.value / 4) / 100 + 3, 1, (int)Mathf.Sqrt((float)this.owner.MaxHP) + 100)), AttackSource.Condition, null);
		this.owner.AddBlood(1, -1);
		base.Mod(-1, false);
	}
}
