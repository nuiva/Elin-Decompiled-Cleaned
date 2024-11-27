using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class HomeResourceWorth : HomeResourceRate
{
	public override bool IsAvailable
	{
		get
		{
			return false;
		}
	}

	public void UpdateRank()
	{
		int rank = EClass.game.spatials.ranks.GetRank(this.branch.owner);
		if (this.bestRank == 0)
		{
			this.bestRank = rank;
		}
		if (rank < this.bestRank)
		{
			this.bestRank = rank;
			EClass.Sound.Play((rank <= 50) ? "clap3" : ((rank <= 100) ? "clap2" : "clap1"));
			Msg.Say("homerank_up", EClass.game.spatials.ranks.GetRankText(this.branch.owner), null, null, null);
		}
	}

	public override void Refresh()
	{
		base.Refresh();
		this.UpdateRank();
	}

	public int GetPrice(Thing t, bool top = false)
	{
		int num = t.GetPrice(CurrencyType.Money, false, PriceType.Default, null);
		if (t.noSell)
		{
			num /= 50;
		}
		if (top && this.branch.policies.IsActive(2821, -1))
		{
			num = num * (150 + (int)Mathf.Sqrt((float)this.branch.Evalue(2821)) * 5) / 100;
		}
		return num;
	}

	public override int GetDestValue()
	{
		List<Thing> list = this.ListHeirloom();
		int num = 0;
		foreach (Thing thing in list)
		{
			num += this.GetPrice(thing, list[0] == thing);
		}
		return num;
	}

	public List<Thing> ListHeirloom()
	{
		List<Thing> list = new List<Thing>();
		List<Thing> list2 = new List<Thing>();
		HashSet<string> hashSet = new HashSet<string>();
		int[] array = new int[EClass._map.SizeXZ];
		int num = 0;
		int num2 = this.branch.Evalue(2814);
		int num3 = this.branch.Evalue(2823);
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.IsInstalled)
			{
				if (thing.HasTag(CTAG.tourism))
				{
					bool flag = thing.trait is TraitFigure;
					if (flag)
					{
						if (array[thing.pos.index] != 0)
						{
							continue;
						}
						array[thing.pos.index]++;
					}
					int num4 = 1;
					string item;
					if (flag)
					{
						item = "figure_" + thing.c_idRefCard;
						num4 = 2;
					}
					else
					{
						item = thing.id + "_" + thing.idSkin.ToString();
					}
					if (!hashSet.Contains(item))
					{
						int num5 = this.GetPrice(thing, false) * num4;
						if (num3 > 0)
						{
							num5 = num5 * (110 + (int)Mathf.Sqrt((float)num3) * 4) / 100;
						}
						num += num5;
						hashSet.Add(item);
					}
					else if (num2 > 0)
					{
						int num6 = this.GetPrice(thing, false) * num4 / Mathf.Max(20, 30 - (int)Mathf.Sqrt((float)num2));
						if (num6 > 0)
						{
							num += num6;
						}
					}
				}
				if (thing.IsFurniture || thing.trait is TraitToolMusic)
				{
					list2.Add(thing);
				}
			}
		}
		int num7 = this.branch.Evalue(3780) + this.branch.Evalue(3781) + this.branch.Evalue(3782) + this.branch.Evalue(3783) + this.branch.Evalue(3784);
		this.branch.tourism = (100 + num) * (100 + num7 * 15) / 100;
		list2.Sort((Thing a, Thing b) => this.GetPrice(b, false) - this.GetPrice(a, false));
		int num8 = 0;
		while (num8 < this.branch.NumHeirloom && num8 < list2.Count)
		{
			list.Add(list2[num8]);
			num8++;
		}
		return list;
	}

	public override void WriteNote(UINote n)
	{
		n.Clear();
		n.AddHeader(base.Name, null);
		n.AddTopic("TopicLeft", "vCurrent".lang(), this.value.ToFormat() ?? "");
		n.AddTopic("TopicLeft", "tourism_value".lang(), this.branch.tourism.ToFormat() ?? "");
		n.Space(0, 1);
		n.AddHeader("HeaderTopic", "heirloom_list".lang(this.branch.NumHeirloom.ToString() ?? "", null, null, null, null), null);
		n.Space(1, 1);
		List<Thing> list = this.ListHeirloom();
		for (int i = 0; i < list.Count; i++)
		{
			Thing thing = list[i];
			string str = (i + 1).ToString();
			string str2 = ": ";
			string name = thing.Name;
			object obj = EClass.debug.showExtra ? this.GetPrice(thing, i == 0) : "";
			n.AddText(str + str2 + name + ((obj != null) ? obj.ToString() : null), FontColor.DontChange);
		}
		n.Build();
	}

	[JsonProperty]
	public int bestRank;
}
