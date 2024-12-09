using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MATERIAL : EClass
{
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

	public static SourceMaterial.Row sourceWaterSea => EClass.sources.materials.rows[88];

	public static SourceMaterial.Row GetRandomMaterial(int lv, string materialGroup = null, bool tryLevelMatTier = false)
	{
		if (materialGroup == null)
		{
			materialGroup = ((EClass.rnd(2) == 0) ? "metal" : "leather");
		}
		if (!(materialGroup == "metal"))
		{
			if (materialGroup == "leather" && EClass.rnd(15) == 0)
			{
				materialGroup = "metal";
			}
		}
		else if (EClass.rnd(15) == 0)
		{
			materialGroup = "leather";
		}
		SourceMaterial.TierList tierListOfMaterialGroup = SourceMaterial.tierMap[materialGroup];
		int chosenMaterialTier = 0;
		if (tryLevelMatTier) // This targets a specific material tier, and never gives low rolls for high levels
		{
			chosenMaterialTier = Mathf.Clamp(lv / 15, 0, tierListOfMaterialGroup.tiers.Length - 1);
			chosenMaterialTier = Mathf.Clamp(chosenMaterialTier + EClass.rnd(2) - EClass.rnd(2), 0, tierListOfMaterialGroup.tiers.Length - 1);
		}
		else // This is a more random tier selection
		{
			int minimumMaterialTier = ((lv >= 60) ? 2 : ((lv >= 25) ? 1 : 0));
			chosenMaterialTier = Mathf.Clamp(EClass.rnd(EClass.rnd(EClass.rnd(lv / 10 + 2) + 1) + 1), minimumMaterialTier, tierListOfMaterialGroup.tiers.Length - 1);
		}
		SourceMaterial.Tier obj = tierListOfMaterialGroup.tiers[chosenMaterialTier];
		SourceMaterial.Row result = obj.Select();
		if (obj.list.Count == 0)
		{
			Debug.Log(lv + "/" + materialGroup + "/" + chosenMaterialTier + "/");
		}
		return result;
	}

	public static SourceMaterial.Row GetRandomMaterialFromCategory(int lv, string cat, SourceMaterial.Row fallback)
	{
		return GetRandomMaterialFromCategory(lv, new string[1] { cat }, fallback);
	}

	public static SourceMaterial.Row GetRandomMaterialFromCategory(int lv, string[] cat, SourceMaterial.Row fallback)
	{
		int min = ((lv >= 60) ? 2 : ((lv >= 25) ? 1 : 0));
		int a2 = lv / 5 + 2;
		int idTier = Mathf.Clamp(EClass.rnd(EClass.rnd(EClass.rnd(a2) + 1) + 1), min, 4);
		List<SourceMaterial.Row> list = EClass.sources.materials.rows.Where((SourceMaterial.Row m) => cat.Contains(m.category) && m.tier <= idTier).ToList();
		if (list.Count > 0)
		{
			return list.RandomItemWeighted((SourceMaterial.Row a) => a.chance * ((a.tier != idTier) ? 1 : 5));
		}
		return fallback;
	}
}
