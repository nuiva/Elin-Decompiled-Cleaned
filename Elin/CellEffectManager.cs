public class CellEffectManager : LogicalPointManager
{
	public override bool AllowBlock => true;

	public override LogicalPoint Create()
	{
		return new LogicalFire();
	}
}
