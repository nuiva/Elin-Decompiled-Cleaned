public class TraitFloor : TraitTile
{
	public override TileRow source => EClass.sources.floors.rows[owner.refVal];
}
