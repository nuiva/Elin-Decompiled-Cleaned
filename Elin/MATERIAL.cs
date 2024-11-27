using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MATERIAL : EClass
{
	public static SourceMaterial.Row sourceWaterSea
	{
		get
		{
			return EClass.sources.materials.rows[88];
		}
	}

	public static SourceMaterial.Row GetRandomMaterial(int lv, string group = null, bool tryLevelMatTier = false)
	{
		if (group == null)
		{
			group = ((EClass.rnd(2) == 0) ? "metal" : "leather");
		}
		if (!(group == "metal"))
		{
			if (group == "leather")
			{
				if (EClass.rnd(15) == 0)
				{
					group = "metal";
				}
			}
		}
		else if (EClass.rnd(15) == 0)
		{
			group = "leather";
		}
		SourceMaterial.TierList tierList = SourceMaterial.tierMap[group];
		int num;
		if (tryLevelMatTier)
		{
			num = Mathf.Clamp(lv / 15, 0, tierList.tiers.Length - 1);
			num = Mathf.Clamp(num + EClass.rnd(2) - EClass.rnd(2), 0, tierList.tiers.Length - 1);
		}
		else
		{
			int min = (lv >= 60) ? 2 : ((lv >= 25) ? 1 : 0);
			num = Mathf.Clamp(EClass.rnd(EClass.rnd(EClass.rnd(lv / 10 + 2) + 1) + 1), min, tierList.tiers.Length - 1);
		}
		SourceMaterial.Tier tier = tierList.tiers[num];
		SourceMaterial.Row result = tier.Select();
		if (tier.list.Count == 0)
		{
			Debug.Log(string.Concat(new string[]
			{
				lv.ToString(),
				"/",
				group,
				"/",
				num.ToString(),
				"/"
			}));
		}
		return result;
	}

	public static SourceMaterial.Row GetRandomMaterialFromCategory(int lv, string cat, SourceMaterial.Row fallback)
	{
		return MATERIAL.GetRandomMaterialFromCategory(lv, new string[]
		{
			cat
		}, fallback);
	}

	public static SourceMaterial.Row GetRandomMaterialFromCategory(int lv, string[] cat, SourceMaterial.Row fallback)
	{
		int min = (lv >= 60) ? 2 : ((lv >= 25) ? 1 : 0);
		int a2 = lv / 5 + 2;
		int idTier = Mathf.Clamp(EClass.rnd(EClass.rnd(EClass.rnd(a2) + 1) + 1), min, 4);
		List<SourceMaterial.Row> list = (from m in EClass.sources.materials.rows
		where cat.Contains(m.category) && m.tier <= idTier
		select m).ToList<SourceMaterial.Row>();
		if (list.Count > 0)
		{
			return list.RandomItemWeighted((SourceMaterial.Row a) => (float)(a.chance * ((a.tier == idTier) ? 5 : 1)));
		}
		return fallback;
	}

	public const byte oak = 1;

	public const byte granite = 3;

	public const byte mud = 4;

	public const byte sand = 8;

	public const byte soil = 45;

	public const byte snow = 48;

	public const byte water = 66;

	public const byte water_fresh = 67;

	public const byte ice = 61;

	public const byte gold = 12;

	public const byte water_sea = 88;

	public const byte process = 94;

	public const byte sand_sea = 97;

	public static SourceMaterial.Row sourceSnow = EClass.sources.materials.rows[48];

	public static SourceMaterial.Row sourceIce = EClass.sources.materials.rows[61];

	public static SourceMaterial.Row sourceGold = EClass.sources.materials.rows[12];

	public static SourceMaterial.Row sourceOak = EClass.sources.materials.rows[1];
}
