using System;
using UnityEngine;

public class TraitSyringeHeaven : Trait
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
				if (c.trait is TraitLittleOne & !c.HasCondition<ConDeathSentense>())
				{
					EClass.player.ModKarma(3);
					c.AddCondition<ConDeathSentense>(100, true);
				}
				this.owner.ModNum(-1, true);
				return false;
			}, null, 1);
		});
	}
}
