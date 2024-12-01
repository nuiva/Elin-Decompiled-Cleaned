public class AI_Water : TaskPoint
{
	public Thing well;

	public override int LeftHand => -1;

	public override int RightHand => 1105;

	public override bool HasProgress => true;

	public override void OnProgress()
	{
		owner.SetTempHand(1106, -1);
		owner.PlaySound("Material/mud");
	}

	public override void OnProgressComplete()
	{
		owner.PlaySound("water_farm");
		EClass._map.SetLiquid(pos.x, pos.z, 1, 2);
		pos.cell.isWatered = true;
	}
}
