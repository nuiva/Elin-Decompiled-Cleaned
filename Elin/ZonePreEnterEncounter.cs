using System;
using System.Collections.Generic;
using UnityEngine;

public class ZonePreEnterEncounter : ZonePreEnterEvent
{
	public override void Execute()
	{
		bool flag = EClass.pc.HasCondition<ConDrawMetal>();
		int lv = Mathf.Max(EClass._zone.DangerLv, EClass.pc.FameLv * Math.Min(this.roadDist * 20, 100) / 100);
		if (this.mob != null)
		{
			List<Chara> list = new List<Chara>();
			Chara leader = null;
			float num = Mathf.Clamp((float)(EClass.pc.FameLv + 8), 8f, 24f + Mathf.Sqrt((float)EClass.pc.FameLv));
			for (int i = 0; i < EClass.rndHalf((int)num); i++)
			{
				Point randomPointInRadius = EClass.pc.pos.GetRandomPointInRadius(2, 5, false, false, false, 2000);
				if (randomPointInRadius != null)
				{
					Chara chara = EClass._zone.SpawnMob(randomPointInRadius, SpawnSetting.Mob(this.mob.id, -1));
					chara.hostility = (chara.c_originalHostility = Hostility.Enemy);
					chara.enemy = EClass.pc.party.members.RandomItem<Chara>();
					leader = chara;
					list.Add(chara);
				}
			}
			if (leader != null)
			{
				Thing t;
				List<Thing> list3 = EClass.pc.things.List((Thing t) => t.Num < 10 && (t.trait.CanBeDestroyed && t.things.Count == 0 && t.invY != 1 && t.trait.CanBeStolen && !t.trait.CanOnlyCarry && !t.IsUnique) && !t.isEquipped, false);
				t = ((list3.Count > 0) ? list3.RandomItem<Thing>() : null);
				if (t == null)
				{
					GameLang.refDrama1 = (GameLang.refDrama2 = "mobPity".lang());
				}
				else
				{
					GameLang.refDrama1 = t.NameSimple;
					GameLang.refDrama2 = t.Name;
				}
				LayerDrama.refAction1 = delegate()
				{
					foreach (Chara chara3 in list)
					{
						chara3.ShowEmo(Emo.angry, 0f, true);
						if (EClass.rnd(6) == 0)
						{
							chara3.Talk((EClass.rnd(5) == 0) ? "rumor_bad" : ((EClass.rnd(5) == 0) ? "callGuards" : "disgust"), null, null, false);
						}
					}
				};
				LayerDrama.refAction2 = delegate()
				{
					if (t != null)
					{
						leader.AddCard(t);
					}
					foreach (Chara chara3 in list)
					{
						if (EClass.rnd(6) == 0)
						{
							chara3.Talk((EClass.rnd(5) == 0) ? "rumor_good" : ((EClass.rnd(3) == 0) ? "thanks3" : "thanks"), null, null, false);
						}
						chara3.ShowEmo(Emo.happy, 0f, true);
						chara3.hostility = (chara3.c_originalHostility = Hostility.Neutral);
						chara3.enemy = null;
					}
					EClass.player.ModKarma(1);
				};
				leader.ShowDialog("_chara", "encounter_mob", "");
			}
		}
		else
		{
			for (int j = 0; j < this.enemies; j++)
			{
				Point nearestPoint = (EClass.pc.pos.GetRandomPoint(4, true, true, false, 100) ?? EClass.pc.pos).GetNearestPoint(false, false, true, false);
				Chara chara2 = EClass._zone.SpawnMob(nearestPoint, SpawnSetting.Encounter(lv));
				chara2.hostility = (chara2.c_originalHostility = Hostility.Enemy);
				chara2.enemy = EClass.pc.party.members.RandomItem<Chara>();
			}
		}
		if (flag && EClass.rnd(EClass.debug.enable ? 1 : 3) == 0)
		{
			Point nearestPoint2 = (EClass.pc.pos.GetRandomPoint(4, true, true, false, 100) ?? EClass.pc.pos).GetNearestPoint(false, false, true, false);
			SpawnList list2 = SpawnListChara.Get("c_metal", (SourceChara.Row s) => s.race == "metal");
			EClass._zone.AddCard(CharaGen.CreateFromFilter(list2, EClass._zone.DangerLv, -1), nearestPoint2);
		}
		if ((EClass._zone.Tile.isRoad || EClass._zone.Tile.IsNeighborRoad) && EClass.rnd(2) == 0)
		{
			Point nearestPoint3 = (EClass.pc.pos.GetRandomPoint(4, true, true, false, 100) ?? EClass.pc.pos).GetNearestPoint(false, false, true, false);
			EClass._zone.AddCard(CharaGen.Create("guard", -1), nearestPoint3);
		}
	}

	public int enemies;

	public int roadDist;

	public Chara mob;
}
