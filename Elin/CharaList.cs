using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CharaList : EClass
{
	public void Add(Chara c)
	{
		if (!this.list.Contains(c.uid))
		{
			this.list.Add(c.uid);
		}
	}

	public void Remove(Chara c)
	{
		this.list.Remove(c.uid);
	}

	public List<Chara> Get()
	{
		List<Chara> list = new List<Chara>();
		foreach (int uid in this.list)
		{
			Chara chara = RefChara.Get(uid);
			if (chara == null)
			{
				chara = EClass._map.FindChara(uid);
			}
			if (chara != null)
			{
				list.Add(chara);
			}
		}
		return list;
	}

	[JsonProperty]
	public List<int> list = new List<int>();
}
