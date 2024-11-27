using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class FactionGroup : EClass
{
	public int CountMemebers()
	{
		return 0;
	}

	public int CountMemebersInZone()
	{
		return 0;
	}

	public float GetHappiness()
	{
		float num = 0f;
		foreach (Faction faction in this.list)
		{
			num += faction.GetHappiness();
		}
		num /= (float)this.list.Count;
		return num;
	}

	public void Add(Faction f)
	{
		this.list.Add(f);
	}

	public void Remove(Faction f)
	{
		this.list.Remove(f);
	}

	[JsonProperty]
	public List<Faction> list = new List<Faction>();
}
