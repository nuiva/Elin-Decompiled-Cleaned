public class TileTypeWaterfall : TileTypePillar
{
	public override float RepeatSize => 0.6f;

	public override bool IsBlockPass => false;

	public override bool IsBlockSight => false;
}
