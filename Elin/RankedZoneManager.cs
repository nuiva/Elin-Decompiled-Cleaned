using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RankedZoneManager : EClass
{
	public int GetIncome(Zone z)
	{
		List<FactionBranch> children = EClass.pc.faction.GetChildren();
		children.Sort((FactionBranch a, FactionBranch b) => this.GetRank(a.owner) - this.GetRank(b.owner));
		int num = children.IndexOf(z.branch);
		return this.<GetIncome>g__CalcIncome|0_0(z) * 100 / (100 + num * 40);
	}

	public string GetRankText(Zone z)
	{
		int rank = this.GetRank(z);
		string[] list = Lang.GetList("num_rank");
		int num = rank % 10;
		if (num >= list.Length)
		{
			num = 0;
		}
		return rank.ToString() + list[num];
	}

	public int GetRank(Zone z)
	{
		foreach (RankedZone rankedZone in this.GetList())
		{
			if (rankedZone.z == z)
			{
				return rankedZone.rank;
			}
		}
		return 1;
	}

	public List<RankedZone> GetList()
	{
		List<RankedZone> list = new List<RankedZone>();
		using (Dictionary<int, Spatial>.ValueCollection.Enumerator enumerator = EClass.game.spatials.map.Values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Spatial z = enumerator.Current;
				if ((z.source.value > 0 && z.lv == 0) || z.IsPlayerFaction)
				{
					int v = 0;
					if (z.IsPlayerFaction)
					{
						v = (z as Zone).branch.Worth;
					}
					else
					{
						v = z.source.value;
						Rand.UseSeed(z.uid, delegate
						{
							v = z.source.value + EClass.rnd(z.source.value / 10);
						});
					}
					list.Add(new RankedZone
					{
						z = (z as Zone),
						value = v
					});
				}
			}
		}
		list.Sort((RankedZone a, RankedZone b) => b.Value - a.Value);
		int num = 0;
		int num2 = 0;
		foreach (RankedZone rankedZone in list)
		{
			int num3 = num2 - rankedZone.value;
			int num4;
			if (rankedZone.value >= 100000000)
			{
				num4 = 100000000;
			}
			else if (rankedZone.value >= 10000000)
			{
				num4 = 5000000;
			}
			else if (rankedZone.value >= 5000000)
			{
				num4 = 1000000;
			}
			else if (rankedZone.value >= 1000000)
			{
				num4 = 500000;
			}
			else if (rankedZone.value >= 500000)
			{
				num4 = 50000;
			}
			else if (rankedZone.value >= 100000)
			{
				num4 = 10000;
			}
			else if (rankedZone.value >= 50000)
			{
				num4 = 5000;
			}
			else if (rankedZone.value >= 10000)
			{
				num4 = 2000;
			}
			else
			{
				num4 = 1000;
			}
			num += Mathf.Max(num3 / num4, 1);
			rankedZone.rank = num;
			num2 = rankedZone.Value;
		}
		return list;
	}

	[CompilerGenerated]
	private int <GetIncome>g__CalcIncome|0_0(Zone _z)
	{
		int rank = this.GetRank(_z);
		if (rank > 125)
		{
			return 0;
		}
		return (126 - rank) * 100;
	}
}
