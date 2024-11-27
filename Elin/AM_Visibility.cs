using System;

public class AM_Visibility : AM_BaseTileSelect
{
	public override BaseTileMap.CardIconMode cardIconMode
	{
		get
		{
			return BaseTileMap.CardIconMode.Visibility;
		}
	}

	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			return BaseTileSelector.SelectType.Single;
		}
	}

	public override bool IsBuildMode
	{
		get
		{
			return true;
		}
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (this.GetTarget(point) != null)
		{
			return HitResult.Valid;
		}
		return base.HitTest(point, start);
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		Card target = this.GetTarget(point);
		if (target != null)
		{
			SE.Click();
			target.isMasked = !target.isMasked;
		}
	}

	public Card GetTarget(Point point)
	{
		Thing lastThing = point.LastThing;
		if (lastThing == null || lastThing.trait.CanOnlyCarry)
		{
			return null;
		}
		return lastThing;
	}
}
