public class ActDrawWater : Act
{
	public TraitToolWaterCan waterCan;

	public override TargetType TargetType => TargetType.Ground;

	public override CursorInfo CursorIcon => CursorSystem.Hand;

	public override bool CanPerform()
	{
		if (HasWaterSource(Act.TP) && waterCan != null)
		{
			return waterCan.owner.c_charges < waterCan.MaxCharge;
		}
		return false;
	}

	public override bool Perform()
	{
		Act.CC.PlaySound("water_draw");
		waterCan.owner.SetCharge(waterCan.MaxCharge);
		Act.CC.Say("water_draw", Act.CC, waterCan.owner);
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
}
