using System;

public class ActWater : Act
{
	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.Hand;
		}
	}

	public override TargetType TargetType
	{
		get
		{
			return TargetType.SelfAndNeighbor;
		}
	}

	public override bool CanPerform()
	{
		return this.IsWaterCanValid(false);
	}

	public override bool Perform()
	{
		Act.CC.Say("water_ground", Act.CC, null, null);
		if (!Act.TP.cell.IsTopWater && !Act.TP.cell.IsSnowTile)
		{
			Act.TP.cell.isWatered = true;
		}
		foreach (Chara chara in Act.TP.Charas)
		{
			if (chara.HasCondition<ConBurning>())
			{
				chara.Talk("thanks", null, null, false);
			}
			else if (!chara.IsPCParty && EClass.rnd(2) == 0)
			{
				chara.Say("water_evade", chara, null, null);
				if (!chara.IsHostile())
				{
					chara.Talk("scold", null, null, false);
					continue;
				}
				continue;
			}
			chara.AddCondition<ConWet>(100, false);
			Act.CC.DoHostileAction(chara, false);
		}
		Act.CC.PlaySound("water_farm", 1f, true);
		this.waterCan.owner.ModCharge(-1, false);
		return base.Perform();
	}

	public bool IsWaterCanValid(bool msg = true)
	{
		bool flag = this.waterCan != null && this.waterCan.owner.c_charges > 0;
		if (!flag && msg)
		{
			Msg.Say("water_deplete");
		}
		return flag;
	}

	public TraitToolWaterCan waterCan;
}
