using System;

public class TileTypeNone : TileTypeFloor
{
	public override bool EditorTile
	{
		get
		{
			return true;
		}
	}

	public override bool CastShadowSelf
	{
		get
		{
			return false;
		}
	}
}
