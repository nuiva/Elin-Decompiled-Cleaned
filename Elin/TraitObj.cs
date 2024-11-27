using System;

public class TraitObj : TraitTile
{
	public override TileRow source
	{
		get
		{
			return EClass.sources.objs.rows[this.owner.refVal];
		}
	}
}
