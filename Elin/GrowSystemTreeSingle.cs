public class GrowSystemTreeSingle : GrowSystemTree
{
	public override int GetShadow(int index)
	{
		return 33;
	}

	public override void SetStageTile(Stage s)
	{
		s.renderData = source.renderData;
		s.SetTile(0, baseTiles);
	}
}
