using System.Collections.Generic;
using Newtonsoft.Json;

public class CharaList : EClass
{
	[JsonProperty]
	public List<int> list = new List<int>();

	public void Add(Chara c)
	{
		if (!list.Contains(c.uid))
		{
			list.Add(c.uid);
		}
	}

	public void Remove(Chara c)
	{
		list.Remove(c.uid);
	}

	public List<Chara> Get()
	{
		List<Chara> list = new List<Chara>();
		foreach (int item in this.list)
		{
			Chara chara = RefChara.Get(item);
			if (chara == null)
			{
				chara = EClass._map.FindChara(item);
			}
			if (chara != null)
			{
				list.Add(chara);
			}
		}
		return list;
	}
}
