using System;

public class TraitBroom : TraitTool
{
	public override bool CanHarvest
	{
		get
		{
			return false;
		}
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		if (p.pos.HasDirt || p.pos.cell.HasLiquid)
		{
			p.TrySetAct("actClean", delegate()
			{
				EClass._map.SetDecal(p.pos.x, p.pos.z, 0, 1, true);
				EClass._map.SetLiquid(p.pos.x, p.pos.z, 0, 0);
				p.pos.PlayEffect("vanish");
				EClass.pc.Say("clean", this.owner, null, null);
				EClass.pc.PlaySound("clean_floor", 1f, true);
				EClass.pc.stamina.Mod(-1);
				EClass.pc.ModExp(293, 40);
				return true;
			}, null, 1);
		}
	}
}
