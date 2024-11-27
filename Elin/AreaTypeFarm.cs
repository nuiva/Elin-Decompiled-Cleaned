using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class AreaTypeFarm : AreaType
{
	public override bool IsWork
	{
		get
		{
			return true;
		}
	}

	public override AIAct GetAI()
	{
		Point plowPos = this.GetPlowPos();
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
		else
		{
			if (EClass.rnd(3) == 0)
			{
				return new AI_Farm
				{
					pos = this.owner.GetRandomFreePos()
				};
			}
			if (EClass.rnd(3) == 0)
			{
				return new AI_Water
				{
					pos = this.owner.GetRandomFreePos()
				};
			}
			return base.GetAI();
		}
	}

	public Point GetPlowPos()
	{
		List<Point> list = new List<Point>();
		foreach (Point point in this.owner.points)
		{
			if ((point.sourceFloor.ContainsTag("grass") || point.sourceFloor.tag.Contains("soil")) && (!point.IsFarmField || point.HasObj))
			{
				list.Add(point);
			}
		}
		if (list.Count > 0)
		{
			return list.RandomItem<Point>();
		}
		return null;
	}

	[JsonProperty]
	public int refSeed;
}
