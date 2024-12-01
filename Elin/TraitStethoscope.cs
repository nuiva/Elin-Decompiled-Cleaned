public class TraitStethoscope : TraitTool
{
	public override bool HasCharges => true;

	public override void OnCreate(int lv)
	{
		owner.c_charges = EClass.rnd(6) + 2;
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.ListCards().ForEach(delegate(Card a)
		{
			Chara c = a.Chara;
			if (c != null && p.IsSelfOrNeighbor && EClass.pc.CanSee(a) && c.IsPCFaction)
			{
				p.TrySetAct("actInvestigate", delegate
				{
					EClass.pc.Say("use_scope", c, owner);
					EClass.pc.Say("use_scope2", c);
					c.Talk("pervert2");
					EClass.ui.AddLayer<LayerChara>().SetChara(c);
					owner.ModCharge(-1);
					if (owner.c_charges <= 0)
					{
						EClass.pc.Say("spellbookCrumble", owner);
						owner.Destroy();
					}
					return false;
				}, c);
			}
		});
	}
}
