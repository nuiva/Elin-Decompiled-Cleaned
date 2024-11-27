using System;

public class TraitStethoscope : TraitTool
{
	public override bool HasCharges
	{
		get
		{
			return true;
		}
	}

	public override void OnCreate(int lv)
	{
		this.owner.c_charges = EClass.rnd(6) + 2;
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.ListCards(false).ForEach(delegate(Card a)
		{
			Chara c = a.Chara;
			if (c == null)
			{
				return;
			}
			if (p.IsSelfOrNeighbor && EClass.pc.CanSee(a) && c.IsPCFaction)
			{
				p.TrySetAct("actInvestigate", delegate()
				{
					EClass.pc.Say("use_scope", c, this.owner, null, null);
					EClass.pc.Say("use_scope2", c, null, null);
					c.Talk("pervert2", null, null, false);
					EClass.ui.AddLayer<LayerChara>().SetChara(c);
					this.owner.ModCharge(-1, false);
					if (this.owner.c_charges <= 0)
					{
						EClass.pc.Say("spellbookCrumble", this.owner, null, null);
						this.owner.Destroy();
					}
					return false;
				}, c, null, 1, false, true, false);
			}
		});
	}
}
