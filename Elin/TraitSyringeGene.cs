using System;
using UnityEngine;

public class TraitSyringeGene : Trait
{
	public override bool CanChangeHeight
	{
		get
		{
			return false;
		}
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.Charas.ForEach(delegate(Chara c)
		{
			p.TrySetAct("actInject".lang("", c.Name, null, null, null), delegate()
			{
				EClass.pc.PlaySound("syringe", 1f, true);
				EClass.pc.Say("syringe", EClass.pc, this.owner.NameOne, c.Name);
				c.PlayEffect("blood", true, 0f, default(Vector3)).SetParticleColor(EClass.Colors.matColors[c.material.alias].main).Emit(20);
				c.AddBlood(2 + EClass.rnd(2), -1);
				c.AddCondition<ConHallucination>(50, false);
				TraitGeneMachine traitGeneMachine = c.pos.FindThing<TraitGeneMachine>();
				if (traitGeneMachine != null && traitGeneMachine.GetTarget() == c)
				{
					int num = EClass.world.date.GetRemainingHours(c.conSuspend.dateFinish);
					num = num * 2 / 3 - 10;
					if (num < 0)
					{
						num = 0;
					}
					c.conSuspend.dateFinish = EClass.world.date.GetRaw(num);
					c.PlayEffect("heal_tick", true, 0f, default(Vector3));
					c.PlaySound("heal_tick", 1f, true);
				}
				else
				{
					c.ModCorruption(-50);
				}
				this.owner.ModNum(-1, true);
				return false;
			}, null, 1);
		});
	}
}
