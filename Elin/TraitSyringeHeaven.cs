public class TraitSyringeHeaven : Trait
{
	public override bool CanChangeHeight => false;

	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.Charas.ForEach(delegate(Chara c)
		{
			p.TrySetAct("actInject".lang("", c.Name), delegate
			{
				EClass.pc.PlaySound("syringe");
				EClass.pc.Say("syringe", EClass.pc, owner.NameOne, c.Name);
				c.PlayEffect("blood").SetParticleColor(EClass.Colors.matColors[c.material.alias].main).Emit(20);
				c.AddBlood(2 + EClass.rnd(2));
				c.AddCondition<ConHallucination>(50);
				if ((c.trait is TraitLittleOne) & !c.HasCondition<ConDeathSentense>())
				{
					EClass.player.ModKarma(3);
					c.AddCondition<ConDeathSentense>(100, force: true);
				}
				owner.ModNum(-1);
				return false;
			});
		});
	}
}
