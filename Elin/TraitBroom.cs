public class TraitBroom : TraitTool
{
	public override bool CanHarvest => false;

	public override void TrySetHeldAct(ActPlan p)
	{
		if (p.pos.HasDirt || p.pos.cell.HasLiquid)
		{
			p.TrySetAct("actClean", delegate
			{
				EClass._map.SetDecal(p.pos.x, p.pos.z);
				EClass._map.SetLiquid(p.pos.x, p.pos.z, 0, 0);
				p.pos.PlayEffect("vanish");
				EClass.pc.Say("clean", owner);
				EClass.pc.PlaySound("clean_floor");
				EClass.pc.stamina.Mod(-1);
				EClass.pc.ModExp(293, 40);
				return true;
			});
		}
	}
}
