using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharaAbility : EClass
{
	public CharaAbility(Chara _owner)
	{
		this.owner = _owner;
		this.Refresh();
	}

	public void Refresh()
	{
		this.list.items.Clear();
		string[] actCombat = this.owner.source.actCombat;
		for (int i = 0; i < actCombat.Length; i++)
		{
			string[] array = actCombat[i].Split('/', StringSplitOptions.None);
			this.list.items.Add(new ActList.Item
			{
				act = ACT.dict[this.<Refresh>g__ConvertID|4_0(array[0])],
				chance = ((array.Length > 1) ? array[1].ToInt() : 100),
				pt = (array.Length > 2)
			});
		}
		if (this.owner.trait.UseRandomAbility && this.owner._listAbility == null)
		{
			int num = 3 + EClass.rnd(2) - this.list.items.Count;
			if (num > 1)
			{
				if (CharaAbility.randomAbilities.Count == 0)
				{
					this.BuildRandomAbilityList();
				}
				this.owner._listAbility = new List<int>();
				for (int j = 0; j < num; j++)
				{
					this.owner._listAbility.Add(CharaAbility.randomAbilities.RandomItemWeighted((SourceElement.Row e) => (float)e.chance).id);
				}
			}
		}
		if (this.owner._listAbility != null)
		{
			foreach (int num2 in this.owner._listAbility)
			{
				string alias = EClass.sources.elements.map[Mathf.Abs(num2)].alias;
				this.list.items.Add(new ActList.Item
				{
					act = ACT.dict[alias],
					chance = 50,
					pt = (num2 < 0)
				});
			}
		}
	}

	public void BuildRandomAbilityList()
	{
		foreach (SourceElement.Row row in EClass.sources.elements.rows)
		{
			if (row.abilityType.Length != 0 && !(row.aliasRef == "mold"))
			{
				int id = row.id;
				if (id <= 5040)
				{
					if (id - 5000 <= 1 || id == 5005 || id == 5040)
					{
						continue;
					}
				}
				else if (id <= 6400)
				{
					if (id == 5048 || id == 6400)
					{
						continue;
					}
				}
				else if (id == 6410 || id == 8200)
				{
					continue;
				}
				CharaAbility.randomAbilities.Add(row);
			}
		}
	}

	public void Add(int id, int chance, bool pt)
	{
		if (this.owner._listAbility == null)
		{
			this.owner._listAbility = new List<int>();
		}
		this.owner._listAbility.Add(id * (pt ? -1 : 1));
		this.Refresh();
	}

	public void AddRandom()
	{
		if (this.owner._listAbility == null)
		{
			this.owner._listAbility = new List<int>();
		}
		if (CharaAbility.randomAbilities.Count == 0)
		{
			this.BuildRandomAbilityList();
		}
		this.owner._listAbility.Add(CharaAbility.randomAbilities.RandomItemWeighted((SourceElement.Row e) => (float)e.chance).id);
		this.Refresh();
	}

	public void Remove(int id)
	{
		this.owner._listAbility.Remove(id);
		if (this.owner._listAbility.Count == 0)
		{
			this.owner._listAbility = null;
		}
		this.Refresh();
	}

	[CompilerGenerated]
	private string <Refresh>g__ConvertID|4_0(string s)
	{
		if (this.owner.MainElement == Element.Void)
		{
			return s;
		}
		if (EClass.sources.elements.alias[s].aliasRef == "mold")
		{
			return s + this.owner.MainElement.source.alias.Replace("ele", "");
		}
		return s;
	}

	public static List<SourceElement.Row> randomAbilities = new List<SourceElement.Row>();

	public Chara owner;

	public ActList list = new ActList();
}
