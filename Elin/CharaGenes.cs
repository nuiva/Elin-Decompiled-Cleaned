using System.Collections.Generic;
using Newtonsoft.Json;

public class CharaGenes : EClass
{
	[JsonProperty]
	public List<DNA> items = new List<DNA>();

	[JsonProperty]
	public int inferior;

	public void Remove(Chara c, DNA item)
	{
		items.Remove(item);
		c.feat += item.cost;
		item.Apply(c, reverse: true);
		c.Refresh();
	}

	public int GetTotalCost()
	{
		int num = 0;
		foreach (DNA item in items)
		{
			num += item.cost;
		}
		return num;
	}

	public int GetGeneSlot(Chara c)
	{
		int num = 0;
		foreach (DNA item in items)
		{
			int num2 = item.slot;
			if (num2 > 1 && c.HasElement(1237))
			{
				num2--;
			}
			num += num2;
		}
		return num;
	}
}
