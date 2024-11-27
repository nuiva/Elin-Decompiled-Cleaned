using System;

public class GrowSystemTreeSingle : GrowSystemTree
{
	public override int GetShadow(int index)
	{
		return 33;
	}

	public override void SetStageTile(GrowSystem.Stage s)
	{
		s.renderData = this.source.renderData;
		s.SetTile(0, this.baseTiles);
	}
}
