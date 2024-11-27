using System;

public class TraitPlatform : TraitTile
{
	public override bool IsHomeItem
	{
		get
		{
			return true;
		}
	}

	public override TileRow source
	{
		get
		{
			return EClass.sources.floors.rows[this.owner.refVal];
		}
	}

	public override string suffix
	{
		get
		{
			return "-b";
		}
	}
}
