using System;

public class ActDrawWater : Act
{
	public override TargetType TargetType
	{
		get
		{
			return TargetType.Ground;
		}
	}

	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.Hand;
		}
	}

	public override bool CanPerform()
	{
		return ActDrawWater.HasWaterSource(Act.TP) && this.waterCan != null && this.waterCan.owner.c_charges < this.waterCan.MaxCharge;
	}

	public override bool Perform()
	{
		Act.CC.PlaySound("water_draw", 1f, true);
		this.waterCan.owner.SetCharge(this.waterCan.MaxCharge);
		Act.CC.Say("water_draw", Act.CC, this.waterCan.owner, null, null);
		return true;
	}

	public static bool HasWaterSource(Point p)
	{
		foreach (Thing thing in p.Things)
		{
			if (thing.trait is TraitWell && thing.c_charges > 0)
			{
				return true;
			}
		}
		return p.cell.IsTopWaterAndNoSnow;
	}

	public TraitToolWaterCan waterCan;
}
