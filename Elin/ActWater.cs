public class ActWater : Act
{
	public TraitToolWaterCan waterCan;

	public override CursorInfo CursorIcon => CursorSystem.Hand;

	public override TargetType TargetType => TargetType.SelfAndNeighbor;

	public override bool CanPerform()
	{
		return IsWaterCanValid(msg: false);
	}

	public override bool Perform()
	{
		Act.CC.Say("water_ground", Act.CC);
		if (!Act.TP.cell.IsTopWater && !Act.TP.cell.IsSnowTile)
		{
			Act.TP.cell.isWatered = true;
		}
		foreach (Chara chara in Act.TP.Charas)
		{
			if (chara.HasCondition<ConBurning>())
			{
				chara.Talk("thanks");
			}
			else if (!chara.IsPCParty && EClass.rnd(2) == 0)
			{
				chara.Say("water_evade", chara);
				if (!chara.IsHostile())
				{
					chara.Talk("scold");
				}
				continue;
			}
			chara.AddCondition<ConWet>();
			Act.CC.DoHostileAction(chara);
		}
		Act.CC.PlaySound("water_farm");
		waterCan.owner.ModCharge(-1);
		return base.Perform();
	}

	public bool IsWaterCanValid(bool msg = true)
	{
		bool num = waterCan != null && waterCan.owner.c_charges > 0;
		if (!num && msg)
		{
			Msg.Say("water_deplete");
		}
		return num;
	}
}
