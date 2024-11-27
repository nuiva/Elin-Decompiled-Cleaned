using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CharaGenes : EClass
{
	public void Remove(Chara c, DNA item)
	{
		this.items.Remove(item);
		c.feat += item.cost;
		item.Apply(c, true);
		c.Refresh(false);
	}

	public int GetTotalCost()
	{
		int num = 0;
		foreach (DNA dna in this.items)
		{
			num += dna.cost;
		}
		return num;
	}

	[JsonProperty]
	public List<DNA> items = new List<DNA>();

	[JsonProperty]
	public int inferior;
}
