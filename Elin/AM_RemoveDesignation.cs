using System;

public class AM_RemoveDesignation : AM_BaseTileSelect
{
	public override bool IsBuildMode
	{
		get
		{
			return true;
		}
	}

	public override BaseTileSelector.HitType hitType
	{
		get
		{
			return BaseTileSelector.HitType.Default;
		}
	}

	public override void OnUpdateCursor()
	{
		base.SetCursorOnMap(CursorSystem.Cancel);
	}

	public override MeshPass GetGuidePass(Point point)
	{
		TaskBuild taskBuild = base.Designations.mapAll.TryGetValue(point.index, null) as TaskBuild;
		if (taskBuild != null && !(taskBuild.recipe.source.row is SourceFloor.Row))
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
