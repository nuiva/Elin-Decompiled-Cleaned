using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class ResearchPlan : EClass
{
	public SourceResearch.Row source
	{
		get
		{
			return EClass.sources.researches.map[this.id];
		}
	}

	public string Name
	{
		get
		{
			return this.source.GetName() + ((this.source.maxLv == 1) ? "" : (" " + this.rank.ToString()));
		}
	}

	public int MaxExp
	{
		get
		{
			return 100;
		}
	}

	public bool IsComplete
	{
		get
		{
			return this.exp == this.MaxExp;
		}
	}

	public void SetOwner(FactionBranch _branch)
	{
		this.branch = _branch;
	}

	public void GainExp(int a)
	{
		this.exp += a;
		if (this.exp > this.MaxExp)
		{
			this.exp = this.MaxExp;
		}
	}

	public int GetSortVal(UIList.SortMode m)
	{
		if (this.IsComplete)
		{
			return 0;
		}
		return -10;
	}

	public void WriteNote(UINote n)
	{
		n.Clear();
		n.AddHeader(this.Name, null);
		n.AddText(string.Concat(new string[]
		{
			"vRank".lang(),
			this.rank.ToString(),
			" (",
			Mathf.Clamp(this.exp * 100 / this.MaxExp, 0, 100).ToString(),
			"%)"
		}), FontColor.DontChange);
		n.AddText("vMaxRank".lang() + this.source.maxLv.ToString(), FontColor.DontChange);
		n.AddHeader("researchEffect", null);
		this.ParseReward(n);
		n.Build();
	}

	public void OnComplete()
	{
		this.ParseReward(null);
	}

	public void ParseReward(UINote n = null)
	{
		bool flag = n == null;
		string reward = this.source.reward;
		if (reward.IsEmpty())
		{
			return;
		}
		string[] array = reward.Split(Environment.NewLine.ToCharArray());
		string text = array[0];
		foreach (string text2 in array)
		{
			if (text2.Split(',', StringSplitOptions.None)[0].ToInt() <= this.rank)
			{
				text = text2;
			}
		}
		List<string> list = text.Split(',', StringSplitOptions.None).ToList<string>();
		list.RemoveAt(0);
		foreach (string text3 in list)
		{
			string[] array3 = text3.Split('/', StringSplitOptions.None);
			string text4 = "";
			string a = array3[0];
			if (!(a == "r"))
			{
				if (!(a == "p"))
				{
					if (!(a == "e"))
					{
						if (a == "department")
						{
							text4 = "rewardDepartment".lang(this.source.GetName(), null, null, null, null);
						}
					}
					else
					{
						Element element = Element.Create(array3[1], array3[2].ToInt());
						text4 = "rewardElement".lang(EClass.sources.elements.alias[array3[1]].GetName(), array3[2], null, null, null);
						if (flag)
						{
							this.branch.AddFeat(element.id, element.Value);
						}
					}
				}
				else
				{
					text4 = "rewardPolicy".lang(EClass.sources.elements.alias[array3[1]].GetName(), null, null, null, null);
					if (flag)
					{
						this.branch.policies.AddPolicy(array3[1]);
					}
				}
			}
			else
			{
				text4 = "rewardRecipe".lang(EClass.sources.cards.map[array3[1]].GetName(), null, null, null, null);
				if (flag)
				{
					EClass.player.recipes.Add(array3[1], true);
				}
			}
			if (n != null)
			{
				n.AddText(text4, FontColor.DontChange);
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

	[JsonProperty]
	public string id;

	[JsonProperty]
	public int exp;

	[JsonProperty]
	public int lastExp;

	[JsonProperty]
	public int rank = 1;

	public FactionBranch branch;
}
