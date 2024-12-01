using System.Collections.Generic;
using Newtonsoft.Json;

public class FactionGroup : EClass
{
	[JsonProperty]
	public List<Faction> list = new List<Faction>();

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
		foreach (Faction item in list)
		{
			num += item.GetHappiness();
		}
		return num / (float)list.Count;
	}

	public void Add(Faction f)
	{
		list.Add(f);
	}

	public void Remove(Faction f)
	{
		list.Remove(f);
	}
}
