using System;
using System.Collections.Generic;
using UnityEngine;

public class ElementSelecter : EClass
{
	public int Select(int lv)
	{
		if (this.listSource.Count == 0 || Application.isEditor)
		{
			this.listSource.Clear();
			foreach (SourceElement.Row row in EClass.sources.elements.rows)
			{
				if (row.thing.Contains(this.type) && row.chance != 0 && !(row.aliasRef == "mold"))
				{
					this.listSource.Add(row);
				}
			}
		}
		this.list.Clear();
		foreach (SourceElement.Row row2 in this.listSource)
		{
			if (row2.LV <= lv + this.lvMod)
			{
				if (!row2.aliasRef.IsEmpty() && row2.tag.Contains("domain"))
				{
					if (!this.useDomain && EClass.rnd(4) != 0)
					{
						continue;
					}
					SourceElement.Row row3 = EClass.sources.elements.alias[row2.aliasRef];
					if ((this.useDomain && !EClass.player.domains.Contains(row3.id) && EClass.rnd(10) > 1) || !row3.tag.Contains(row2.alias.Split('_', StringSplitOptions.None)[0]))
					{
						continue;
					}
				}
				this.list.Add(row2);
			}
		}
		if (this.list.Count == 0)
		{
			return this.listSource.RandomItemWeighted((SourceElement.Row r) => (float)r.chance).id;
		}
		return this.list.RandomItemWeighted((SourceElement.Row r) => (float)r.chance).id;
	}

	public List<SourceElement.Row> listSource = new List<SourceElement.Row>();

	public List<SourceElement.Row> list = new List<SourceElement.Row>();

	public string type = "B";

	public bool useDomain;

	public int lvMod;
}
