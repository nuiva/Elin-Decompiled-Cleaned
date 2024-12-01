public class AM_StateEditor : AM_BaseTileSelect
{
	public override bool IsBuildMode => true;

	public override BaseTileMap.CardIconMode cardIconMode => BaseTileMap.CardIconMode.State;

	public override BaseTileSelector.SelectType selectType => BaseTileSelector.SelectType.Single;

	public override BaseTileSelector.HitType hitType => BaseTileSelector.HitType.Default;

	public override void OnUpdateCursor()
	{
		SetCursorOnMap(CursorSystem.Select);
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (GetDestState(point).HasValue)
		{
			return HitResult.Valid;
		}
		return base.HitTest(point, start);
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		PlaceState? destState = GetDestState(point);
		foreach (Card item in point.ListCards())
		{
			if (item.isThing && destState.HasValue && item.placeState != destState)
			{
				item.SetPlaceState(destState.Value);
			}
		}
		SE.ClickGeneral();
	}

	public PlaceState? GetDestState(Point point)
	{
		PlaceState? result = null;
		foreach (Card item in point.ListCards())
		{
			if (item.isThing && !item.isNPCProperty)
			{
				if (item.placeState == PlaceState.installed)
				{
					result = PlaceState.roaming;
				}
				else if (item.placeState == PlaceState.roaming)
				{
					result = PlaceState.installed;
				}
				if (result.HasValue)
				{
					break;
				}
			}
		}
		return result;
	}
}
