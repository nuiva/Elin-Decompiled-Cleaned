using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TraitSeed : Trait
{
	public SourceObj.Row row
	{
		get
		{
			return EClass.sources.objs.map[this.owner.refVal];
		}
	}

	public override bool CanExtendBuild
	{
		get
		{
			return true;
		}
	}

	public override bool CanChangeHeight
	{
		get
		{
			return false;
		}
	}

	public override bool CanName
	{
		get
		{
			return true;
		}
	}

	public override int DefaultStock
	{
		get
		{
			return 3 + EClass.rnd(10);
		}
	}

	public override void OnCreate(int lv)
	{
		TraitSeed.ApplySeed(this.owner.Thing, TraitSeed.GetRandomSeedObj().id);
		this.owner.c_seed = EClass.rnd(10000);
	}

	public override void SetName(ref string s)
	{
		s = "_of".lang(this.row.GetName(), s, null, null, null);
	}

	public override void WriteNote(UINote n, bool identified)
	{
		base.WriteNote(n, identified);
		int num = 1;
		if (!this.row._growth.IsEmpty() && this.row._growth.Length >= 4)
		{
			if (this.row._growth.Length >= 5)
			{
				num = this.row._growth[4].ToInt();
			}
			n.AddText("isHarvestCrop".lang(num.ToString() ?? "", null, null, null, null), FontColor.DontChange);
		}
		n.AddText("isConsumeFertility".lang((0.1f * (float)this.row.costSoil).ToString() ?? "", null, null, null, null), FontColor.DontChange);
		if (this.row.tag.Contains("flood"))
		{
			n.AddText("isWaterCrop", FontColor.DontChange);
		}
		if (this.row.growth != null && this.row.growth.NeedSunlight)
		{
			n.AddText("isNeedSun", FontColor.DontChange);
		}
	}

	public void TrySprout(bool force = false, bool sucker = false, VirtualDate date = null)
	{
		Point pos = this.owner.pos;
		if (!pos.HasObj && pos.cell.CanGrow(this.row, date ?? new VirtualDate(0)))
		{
			pos.SetObj(this.row.id, 1, 0);
			EClass._map.AddPlant(pos, this.owner.Thing);
			if (sucker)
			{
				Zone.Suckers.Add(this.owner.Thing);
				return;
			}
			this.owner.Destroy();
		}
	}

	public static Thing MakeSeed(SourceObj.Row obj, PlantData plant = null)
	{
		Thing thing = (plant != null) ? plant.seed : null;
		if (EClass._zone.IsUserZone)
		{
			thing = null;
		}
		Thing thing2 = ThingGen.Create("seed", -1, -1);
		TraitSeed.ApplySeed(thing2, obj.id);
		if (thing != null)
		{
			foreach (Element element in thing.elements.dict.Values)
			{
				if (element.IsFoodTrait)
				{
					thing2.elements.SetTo(element.id, element.Value);
				}
			}
			thing2.SetEncLv(thing.encLV);
			thing2.elements.SetBase(2, EClass.curve(thing2.encLV, 50, 10, 80), 0);
			thing2.c_refText = thing.c_refText;
			thing2.c_seed = thing.c_seed;
			int num = (plant != null) ? plant.water : 0;
			int num2 = (plant != null) ? plant.fert : 0;
			int num3 = 220 / (Mathf.Clamp(EClass.pc.Evalue(286) - thing.LV, 0, 50) * 2 + 10 + num * 2 + ((num2 > 0) ? 20 : 0) + (EClass.pc.HasElement(1325, 1) ? 25 : 0));
			if (EClass.player.isAutoFarming)
			{
				num3 = 2 + num3 * 2;
			}
			if (EClass.rnd(num3) == 0)
			{
				int num4 = Mathf.Max(5, EClass.pc.Evalue(286)) - thing2.encLV;
				if (num4 <= 0)
				{
					if (!EClass.player.isAutoFarming)
					{
						Msg.Say("seedLvLimit", thing2, null, null, null);
					}
				}
				else
				{
					int num5 = Mathf.Clamp(EClass.rnd(num4) - 5, 1, EClass.player.isAutoFarming ? 3 : 10);
					TraitSeed.LevelSeed(thing2, obj, num5);
					EClass.pc.PlaySound("seed_level", 1f, true);
				}
			}
			Rand.SetSeed(-1);
		}
		thing2.SetBlessedState(BlessedState.Normal);
		return thing2;
	}

	public static void LevelSeed(Thing t, SourceObj.Row obj, int num)
	{
		for (int i = 0; i < num; i++)
		{
			if (obj == null || obj.objType == "crop")
			{
				if (t.encLV == 0)
				{
					CraftUtil.AddRandomFoodEnc(t);
				}
				else
				{
					Rand.SetSeed(t.c_seed);
					CraftUtil.ModRandomFoodEnc(t);
				}
			}
			t.ModEncLv(1);
		}
	}

	public static Thing MakeSeed(string idSource)
	{
		return TraitSeed.MakeSeed(EClass.sources.objs.alias[idSource]);
	}

	public static Thing ApplySeed(Thing t, int refval)
	{
		t.refVal = refval;
		SourceObj.Row row = EClass.sources.objs.map.TryGetValue(refval, null);
		if (row != null && row.vals.Length != 0)
		{
			t.idSkin = row.vals[0].ToInt();
		}
		return t;
	}

	public static Thing MakeSeed(SourceObj.Row obj)
	{
		Thing thing = ThingGen.Create("seed", -1, -1);
		TraitSeed.ApplySeed(thing, obj.id);
		return thing;
	}

	public static Thing MakeRandomSeed(bool enc = false)
	{
		Thing thing = ThingGen.Create("seed", null);
		SourceObj.Row randomSeedObj = TraitSeed.GetRandomSeedObj();
		TraitSeed.ApplySeed(thing, randomSeedObj.id);
		return thing;
	}

	public static SourceObj.Row GetRandomSeedObj()
	{
		if (TraitSeed.listSeeds == null)
		{
			TraitSeed.listSeeds = (from s in EClass.sources.objs.rows
			where s.HasTag(CTAG.seed) && !s.HasTag(CTAG.rareSeed)
			select s).ToList<SourceObj.Row>();
		}
		return TraitSeed.listSeeds.RandomItemWeighted((SourceObj.Row a) => (float)a.chance);
	}

	public static List<SourceObj.Row> listSeeds;
}
