using System;

public class TraitFloor : TraitTile
{
	public override TileRow source
	{
		get
		{
			return EClass.sources.floors.rows[this.owner.refVal];
		}
	}
}
