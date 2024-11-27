using System;
using Newtonsoft.Json;

public class NumLogCategory : NumLog
{
	public override string Name
	{
		get
		{
			return EClass.sources.categories.map[this.id].GetText("name", false);
		}
	}

	public override int Value
	{
		get
		{
			return EClass._map.Stocked.categoryMap[this.id].sum;
		}
	}

	[JsonProperty]
	public string id;
}
