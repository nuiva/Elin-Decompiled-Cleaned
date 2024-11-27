using System;

public class CellEffectManager : LogicalPointManager
{
	public override LogicalPoint Create()
	{
		return new LogicalFire();
	}

	public override bool AllowBlock
	{
		get
		{
			return true;
		}
	}
}
