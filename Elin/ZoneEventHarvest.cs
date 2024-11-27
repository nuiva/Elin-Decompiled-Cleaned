using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneEventHarvest : ZoneEventQuest
{
	public QuestHarvest questHarvest
	{
		get
		{
			return base.quest as QuestHarvest;
		}
	}

	public override string TextWidgetDate
	{
		get
		{
			return "eventHarvest".lang((this.TimeLimit - this.minElapsed <= 30) ? "end_soon".lang() : "", Lang._weight(this.questHarvest.weightDelivered, true, 0), Lang._weight(this.questHarvest.destWeight, true, 0), null, null);
		}
	}

	public override int TimeLimit
	{
		get
		{
			return 180;
		}
	}

	public override void OnVisit()
	{
		if (EClass.game.isLoading)
		{
			return;
		}
		EClass._zone.SetBGM(17, true);
		Point centerPos = EClass._map.GetCenterPos();
		PartialMap.Apply("Special/farm_chest.mp", centerPos);
		GenBounds genBounds = GenBounds.Create(EClass._zone);
		genBounds.marginPartial = 2;
		genBounds.FuncCheckEmpty = ((Cell cell) => cell.sourceFloor.id == 42);
		List<SourceObj.Row> crops = (from o in EClass.sources.objs.rows
		where o.tag.Contains("harvest")
		select o).ToList<SourceObj.Row>();
		Action<PartialMap, GenBounds> <>9__2;
		for (int i = 0; i < 50; i++)
		{
			GenBounds genBounds2 = genBounds;
			MapPiece.Type type = MapPiece.Type.Farm;
			float ruin = 0f;
			string tags = "";
			Action<PartialMap, GenBounds> onCreate;
			if ((onCreate = <>9__2) == null)
			{
				onCreate = (<>9__2 = delegate(PartialMap p, GenBounds b)
				{
					List<Point> list = b.ListEmptyPoint();
					SourceObj.Row row = crops.RandomItemWeighted((SourceObj.Row o) => (float)o.chance);
					int num = 1 + EClass.rnd(5 + this.quest.difficulty * 2);
					foreach (Point point in list)
					{
						if (point.sourceFloor.id == 4 && EClass.rnd(4) != 0)
						{
							point.SetObj(row.id, 1, 0);
							int num2 = point.growth.HarvestStage - EClass.rnd(4);
							point.growth.SetStage(num2, false);
							if (num2 == point.growth.HarvestStage)
							{
								EClass._map.AddPlant(point, null).size = Mathf.Clamp(num + EClass.rnd(2) - EClass.rnd(2), 0, 9) + 1;
							}
							point.cell.isClearSnow = true;
						}
					}
				});
			}
			genBounds2.TryAddMapPiece(type, ruin, tags, onCreate);
		}
		foreach (Thing thing in EClass._map.things)
		{
			thing.isNPCProperty = true;
		}
		for (int j = 0; j < 12; j++)
		{
			EClass._zone.SpawnMob(null, null);
		}
		for (int k = 0; k < 30; k++)
		{
			EClass._zone.SpawnMob(null, SpawnSetting.HomeWild(1));
		}
	}

	public override ZoneInstance.Status OnReachTimeLimit()
	{
		Msg.Say("harvest_end", Lang._weight(this.questHarvest.weightDelivered, true, 0), null, null, null);
		if (this.questHarvest.weightDelivered < this.questHarvest.destWeight)
		{
			return ZoneInstance.Status.Fail;
		}
		return ZoneInstance.Status.Success;
	}

	public override void OnLeaveZone()
	{
		if (EClass._zone.instance.status == ZoneInstance.Status.Running)
		{
			EClass._zone.instance.status = this.OnReachTimeLimit();
		}
		List<Thing> list = new List<Thing>();
		Action<Thing> <>9__0;
		foreach (Chara chara in EClass.pc.party.members)
		{
			ThingContainer things = chara.things;
			Action<Thing> action;
			if ((action = <>9__0) == null)
			{
				action = (<>9__0 = delegate(Thing t)
				{
					if (t.GetBool(115) && EClass.rnd(2) != 0)
					{
						list.Add(t);
					}
				});
			}
			things.Foreach(action, true);
		}
		if (list.Count > 0)
		{
			Msg.Say("harvest_confiscate", list.Count.ToString() ?? "", null, null, null);
			foreach (Thing thing in list)
			{
				thing.Destroy();
			}
			EClass.player.ModKarma(-1);
		}
	}
}
