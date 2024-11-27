using System;

public class TileTypeObjHuge : TileTypeObjBig
{
	public override bool IsBlockSight
	{
		get
		{
			return true;
		}
	}
}
