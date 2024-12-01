using System.Collections.Generic;

public class TraitLightSun : TraitLight
{
	public override int radius
	{
		get
		{
			if (EClass._map.IsIndoor)
			{
				if (!(owner.parent is Zone))
				{
					if (EClass._zone.electricity < -Electricity)
					{
						return 1;
					}
					return 6;
				}
				if (!owner.isOn)
				{
					return 1;
				}
				return 6;
			}
			return 6;
		}
	}

	public override bool CanUseRoomRadius => false;

	public override List<Point> ListPoints(Point center = null, bool onlyPassable = true)
	{
		Trait.listRadiusPoints.Clear();
		if (center == null)
		{
			center = owner.pos;
		}
		EClass._map.ForeachSphere(center.x, center.z, radius + 1, delegate(Point p)
		{
			Trait.listRadiusPoints.Add(p.Copy());
		});
		if (Trait.listRadiusPoints.Count == 0)
		{
			Trait.listRadiusPoints.Add(center.Copy());
		}
		return Trait.listRadiusPoints;
	}

	public override void OnChangePlaceState(PlaceState state)
	{
		Map.isDirtySunMap = true;
	}

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		base.OnRenderTile(point, result, dir);
		EClass.screen.tileMap.screenHighlight = BaseTileMap.ScreenHighlight.SunMap;
	}
}
