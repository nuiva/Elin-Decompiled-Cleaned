using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class ResearchPlan : EClass
{
	[JsonProperty]
	public string id;

	[JsonProperty]
	public int exp;

	[JsonProperty]
	public int lastExp;

	[JsonProperty]
	public int rank = 1;

	public FactionBranch branch;

	public SourceResearch.Row source => EClass.sources.researches.map[id];

	public string Name => source.GetName() + ((source.maxLv == 1) ? "" : (" " + rank));

	public int MaxExp => 100;

	public bool IsComplete => exp == MaxExp;

	public void SetOwner(FactionBranch _branch)
	{
		branch = _branch;
	}

	public void GainExp(int a)
	{
		exp += a;
		if (exp > MaxExp)
		{
			exp = MaxExp;
		}
	}

	public int GetSortVal(UIList.SortMode m)
	{
		if (IsComplete)
		{
			return 0;
		}
		return -10;
	}

	public void WriteNote(UINote n)
	{
		n.Clear();
		n.AddHeader(Name);
		n.AddText("vRank".lang() + rank + " (" + Mathf.Clamp(exp * 100 / MaxExp, 0, 100) + "%)");
		n.AddText("vMaxRank".lang() + source.maxLv);
		n.AddHeader("researchEffect");
		ParseReward(n);
		n.Build();
	}

	public void OnComplete()
	{
		ParseReward();
	}

	public void ParseReward(UINote n = null)
	{
		bool flag = n == null;
		string reward = source.reward;
		if (reward.IsEmpty())
		{
			return;
		}
		string[] array = reward.Split(Environment.NewLine.ToCharArray());
		string text = array[0];
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			if (text2.Split(',')[0].ToInt() <= rank)
			{
				text = text2;
			}
		}
		List<string> list = text.Split(',').ToList();
		list.RemoveAt(0);
		foreach (string item in list)
		{
			string[] array3 = item.Split('/');
			string text3 = "";
			switch (array3[0])
			{
			case "r":
				text3 = "rewardRecipe".lang(EClass.sources.cards.map[array3[1]].GetName());
				if (flag)
				{
					EClass.player.recipes.Add(array3[1]);
				}
				break;
			case "p":
				text3 = "rewardPolicy".lang(EClass.sources.elements.alias[array3[1]].GetName());
				if (flag)
				{
					branch.policies.AddPolicy(array3[1]);
				}
				break;
			case "e":
			{
				Element element = Element.Create(array3[1], array3[2].ToInt());
				text3 = "rewardElement".lang(EClass.sources.elements.alias[array3[1]].GetName(), array3[2]);
				if (flag)
				{
					branch.AddFeat(element.id, element.Value);
				}
				break;
			}
			case "department":
				text3 = "rewardDepartment".lang(source.GetName());
				break;
			}
			if (n != null)
			{
				n.AddText(text3);
			}
		}
	}

	public static ResearchPlan Create(string id)
	{
		return new ResearchPlan
		{
			id = id
		};
	}
}
