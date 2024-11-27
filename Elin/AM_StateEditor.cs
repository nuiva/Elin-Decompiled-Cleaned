using System;

public class AM_StateEditor : AM_BaseTileSelect
{
	public override bool IsBuildMode
	{
		get
		{
			return true;
		}
	}

	public override BaseTileMap.CardIconMode cardIconMode
	{
		get
		{
			return BaseTileMap.CardIconMode.State;
		}
	}

	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			return BaseTileSelector.SelectType.Single;
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
		base.SetCursorOnMap(CursorSystem.Select);
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (this.GetDestState(point) != null)
		{
			return HitResult.Valid;
		}
		return base.HitTest(point, start);
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		PlaceState? destState = this.GetDestState(point);
		foreach (Card card in point.ListCards(false))
		{
			if (card.isThing && destState != null)
			{
				PlaceState placeState = card.placeState;
				PlaceState? placeState2 = destState;
				if (!(placeState == placeState2.GetValueOrDefault() & placeState2 != null))
				{
					card.SetPlaceState(destState.Value, false);
				}
			}
		}
		SE.ClickGeneral();
	}

	public PlaceState? GetDestState(Point point)
	{
		PlaceState? result = null;
		foreach (Card card in point.ListCards(false))
		{
			if (card.isThing && !card.isNPCProperty)
			{
				if (card.placeState == PlaceState.installed)
				{
					result = new PlaceState?(PlaceState.roaming);
				}
				else if (card.placeState == PlaceState.roaming)
				{
					result = new PlaceState?(PlaceState.installed);
				}
				if (result != null)
				{
					break;
				}
			}
		}
		return result;
	}
}
