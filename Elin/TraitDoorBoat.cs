using System;

public class TraitDoorBoat : Trait
{
	public override bool ShouldRefreshTile
	{
		get
		{
			return true;
		}
	}

	public override bool IsOpenPath
	{
		get
		{
			return true;
		}
	}

	public override bool IsChangeFloorHeight
	{
		get
		{
			return true;
		}
	}
}
