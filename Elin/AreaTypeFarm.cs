using System.Collections.Generic;
using Newtonsoft.Json;

public class AreaTypeFarm : AreaType
{
	[JsonProperty]
	public int refSeed;

	public override bool IsWork => true;

	public override AIAct GetAI()
	{
		Point plowPos = GetPlowPos();
		if (plowPos != null)
		{
			if (plowPos.HasObj)
			{
				return new TaskCut
				{
					pos = plowPos.Copy()
				};
			}
			return new AI_Farm
			{
				pos = plowPos.Copy()
			};
		}
		if (EClass.rnd(3) == 0)
		{
			return new AI_Farm
			{
				pos = owner.GetRandomFreePos()
			};
		}
		if (EClass.rnd(3) == 0)
		{
			return new AI_Water
			{
				pos = owner.GetRandomFreePos()
			};
		}
		return base.GetAI();
	}

	public Point GetPlowPos()
	{
		List<Point> list = new List<Point>();
		foreach (Point point in owner.points)
		{
			if ((point.sourceFloor.ContainsTag("grass") || point.sourceFloor.tag.Contains("soil")) && (!point.IsFarmField || point.HasObj))
			{
				list.Add(point);
			}
		}
		if (list.Count > 0)
		{
			return list.RandomItem();
		}
		return null;
	}
}
