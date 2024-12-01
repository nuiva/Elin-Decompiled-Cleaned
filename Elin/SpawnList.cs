using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpawnList : EClass
{
	public static SpawnList Get(string id, string parent = null, CardFilter filter = null)
	{
		SpawnList spawnList = SpawnList.allList.TryGetValue(id, null);
		if (spawnList != null)
		{
			if (spawnList.filter != filter && filter != null)
			{
				spawnList.CreateMaster(filter, parent);
			}
			return spawnList;
		}
		spawnList = new SpawnList(id);
		if (filter != null)
		{
			spawnList.CreateMaster(filter, parent);
		}
		SpawnList.allList.Add(id, spawnList);
		return spawnList;
	}

	public SpawnList(string id = null)
	{
		if (!id.IsEmpty())
		{
			SourceSpawnList.Row row = EClass.sources.spawnLists.map.TryGetValue(id, null);
			if (row != null)
			{
				bool flag = row.type == "chara";
				CardFilter cardFilter = flag ? new CharaFilter() : new ThingFilter();
				cardFilter.isChara = flag;
				cardFilter.strTag = row.tag;
				cardFilter.strFilter = row.filter;
				cardFilter.filterCategory = row.category;
				cardFilter.idCard = row.idCard;
				this.CreateMaster(cardFilter, row.parent);
			}
			this.id = id;
		}
	}

	public void Add(CardRow row)
	{
		this.rows.Add(row);
		this.totalChance += row.chance;
	}

	public void CreateMaster(CardFilter _filter, string parent = null)
	{
		this.rows.Clear();
		this.totalChance = 0;
		this.filter = _filter;
		List<CardRow> list = parent.IsEmpty() ? EClass.sources.cards.rows : SpawnList.Get(parent, null, null).rows;
		for (int i = 0; i < list.Count; i++)
		{
			CardRow cardRow = list[i];
			if (cardRow.chance > 0 && this.filter.Pass(cardRow))
			{
				this.Add(cardRow);
			}
		}
		if (!this.filter.idCard.IsEmpty())
		{
			foreach (string key in this.filter.idCard)
			{
				CardRow row = EClass.sources.cards.map[key];
				this.Add(row);
			}
		}
	}

	public SpawnList Filter(int lv, int levelRange = -1)
	{
		SpawnList.tempList.rows.Clear();
		SpawnList.tempList.totalChance = 0;
		SpawnList.tempList.filter = this.filter;
		int i = 0;
		while (i < this.rows.Count)
		{
			CardRow cardRow = this.rows[i];
			if (levelRange != -1)
			{
				if (cardRow.isChara && (cardRow as SourceChara.Row).mainElement.Length >= 2)
				{
					SourceChara.Row row = cardRow as SourceChara.Row;
					bool flag = false;
					string[] mainElement = row.mainElement;
					for (int j = 0; j < mainElement.Length; j++)
					{
						string[] array = mainElement[j].Split('/', StringSplitOptions.None);
						SourceElement.Row row2 = EClass.sources.elements.alias["ele" + array[0]];
						if (Mathf.Abs(cardRow.LV * row2.eleP / 100 - lv) < levelRange)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						goto IL_F1;
					}
				}
				else if (Mathf.Abs(cardRow.LV - lv) < levelRange)
				{
					goto IL_F1;
				}
			}
			else if (cardRow.LV <= lv)
			{
				goto IL_F1;
			}
			IL_118:
			i++;
			continue;
			IL_F1:
			SpawnList.tempList.rows.Add(cardRow);
			SpawnList.tempList.totalChance += cardRow.chance;
			goto IL_118;
		}
		if (SpawnList.tempList.rows.Count == 0)
		{
			Debug.Log("list contains no item: " + this.id + "/" + lv.ToString());
			return this;
		}
		return SpawnList.tempList;
	}

	public CardRow Select(int lv = -1, int levelRange = -1)
	{
		if (lv != -1)
		{
			if (levelRange != -1)
			{
				for (int i = 0; i < 50; i++)
				{
					SpawnList spawnList = this.Filter(lv + i - i * i, levelRange + i * i);
					if (spawnList.rows.Count > 5)
					{
						return spawnList.Select(-1, -1);
					}
				}
			}
			lv = lv + 2 + EClass.rnd(EClass.rnd(EClass.rnd(EClass.rnd(Mathf.Min(lv * 2, 20)) + 1) + 1) + 1);
			return this.Filter(lv, -1).Select(-1, -1);
		}
		if (this.filter != null && this.filter.categoriesInclude.Count > 0)
		{
			SourceCategory.Row r = this.filter.categoriesInclude.RandomItem<SourceCategory.Row>();
			for (int j = 0; j < 100; j++)
			{
				CardRow cardRow = this.<Select>g___Select|11_0();
				if (cardRow != null && cardRow.Category.IsChildOf(r))
				{
					return cardRow;
				}
			}
		}
		return this.<Select>g___Select|11_0();
	}

	public CardRow GetRandom()
	{
		return this.rows[EClass.rnd(this.rows.Count)];
	}

	public CardRow GetFirst()
	{
		return this.rows[0];
	}

	[CompilerGenerated]
	private CardRow <Select>g___Select|11_0()
	{
		int num = EClass.rnd(this.totalChance);
		int num2 = 0;
		foreach (CardRow cardRow in this.rows)
		{
			num2 += cardRow.chance;
			if (num < num2)
			{
				return cardRow;
			}
		}
		if (this.rows.Count == 0)
		{
			Debug.Log("no item:" + this.id);
			foreach (CardFilter.FilterItem filterItem in this.filter.tags)
			{
				Debug.Log(filterItem.name + "/" + filterItem.exclude.ToString());
			}
			return null;
		}
		return this.rows[EClass.rnd(this.rows.Count)];
	}

	public static Dictionary<string, SpawnList> allList = new Dictionary<string, SpawnList>();

	public static SpawnList tempList = new SpawnList(null);

	public string id;

	public List<CardRow> rows = new List<CardRow>();

	public CardFilter filter;

	public int totalChance;
}
