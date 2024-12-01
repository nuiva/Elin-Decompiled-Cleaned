using System.Collections.Generic;
using UnityEngine;

public class ElementSelecter : EClass
{
	public List<SourceElement.Row> listSource = new List<SourceElement.Row>();

	public List<SourceElement.Row> list = new List<SourceElement.Row>();

	public string type = "B";

	public bool useDomain;

	public int lvMod;

	public int Select(int lv)
	{
		if (listSource.Count == 0 || Application.isEditor)
		{
			listSource.Clear();
			foreach (SourceElement.Row row2 in EClass.sources.elements.rows)
			{
				if (row2.thing.Contains(type) && row2.chance != 0 && !(row2.aliasRef == "mold"))
				{
					listSource.Add(row2);
				}
			}
		}
		list.Clear();
		foreach (SourceElement.Row item in listSource)
		{
			if (item.LV > lv + lvMod)
			{
				continue;
			}
			if (!item.aliasRef.IsEmpty() && item.tag.Contains("domain"))
			{
				if (!useDomain && EClass.rnd(4) != 0)
				{
					continue;
				}
				SourceElement.Row row = EClass.sources.elements.alias[item.aliasRef];
				if ((useDomain && !EClass.player.domains.Contains(row.id) && EClass.rnd(10) > 1) || !row.tag.Contains(item.alias.Split('_')[0]))
				{
					continue;
				}
			}
			list.Add(item);
		}
		if (list.Count == 0)
		{
			return listSource.RandomItemWeighted((SourceElement.Row r) => r.chance).id;
		}
		return list.RandomItemWeighted((SourceElement.Row r) => r.chance).id;
	}
}
