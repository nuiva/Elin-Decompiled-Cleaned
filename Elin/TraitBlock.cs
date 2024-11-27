using System;

public class TraitBlock : TraitTile
{
	public override TileRow source
	{
		get
		{
			return EClass.sources.blocks.rows[this.owner.refVal];
		}
	}
}
