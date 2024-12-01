public class AM_RemoveDesignation : AM_BaseTileSelect
{
	public override bool IsBuildMode => true;

	public override BaseTileSelector.HitType hitType => BaseTileSelector.HitType.Default;

	public override void OnUpdateCursor()
	{
		SetCursorOnMap(CursorSystem.Cancel);
	}

	public override MeshPass GetGuidePass(Point point)
	{
		if (base.Designations.mapAll.TryGetValue(point.index) is TaskBuild taskBuild && !(taskBuild.recipe.source.row is SourceFloor.Row))
		{
			return EClass.screen.guide.passGuideBlock;
		}
		return base.GetGuidePass(point);
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (base.Designations.CanRemoveDesignation(point))
		{
			return HitResult.Valid;
		}
		return base.HitTest(point, start);
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		base.Designations.TryRemoveDesignation(point);
	}
}
