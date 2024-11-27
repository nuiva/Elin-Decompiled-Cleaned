using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneEventMusic : ZoneEventQuest
{
	public QuestMusic questMusic
	{
		get
		{
			return base.quest as QuestMusic;
		}
	}

	public override string TextWidgetDate
	{
		get
		{
			return "eventMusic".lang((this.TimeLimit - this.minElapsed <= 30) ? "end_soon".lang() : "", this.questMusic.score.ToString() ?? "", this.questMusic.destScore.ToString() ?? "", null, null);
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
		if (EClass.debug.enable && Input.GetKey(KeyCode.LeftShift))
		{
			return;
		}
		int difficulty = this.questMusic.difficulty;
		int maxLv = difficulty * 5 + 2;
		if (difficulty > 2)
		{
			maxLv += (difficulty - 2) * 10;
		}
		GenBounds genBounds = GenBounds.Create(EClass._zone);
		genBounds.marginPartial = 1;
		genBounds.FuncCheckEmpty = ((Cell cell) => cell.sourceFloor.id == 76);
		int i;
		Action<PartialMap, GenBounds> <>9__2;
		int k;
		for (i = 0; i < 25; i = k + 1)
		{
			GenBounds genBounds2 = genBounds;
			MapPiece.Type type = MapPiece.Type.Concert;
			float ruin = 0f;
			string tags = "";
			Action<PartialMap, GenBounds> onCreate;
			if ((onCreate = <>9__2) == null)
			{
				Chara c3;
				onCreate = (<>9__2 = delegate(PartialMap p, GenBounds b)
				{
					int num = 1 + i * 3;
					if (num > maxLv)
					{
						num = EClass.rnd(maxLv);
					}
					List<Point> list = b.ListEmptyPoint();
					int num2 = 0;
					while (num2 < EClass.rndHalf(list.Count) && list.Count != 0)
					{
						Point point = list.RandomItem<Point>();
						Chara c3 = CharaGen.CreateFromFilter("c_neutral", num, 5);
						this.Spawn(c3, point);
						list.Remove(point);
						num2++;
					}
				});
			}
			genBounds2.TryAddMapPiece(type, ruin, tags, onCreate);
			k = i;
		}
		foreach (SourceChara.Row row in from c in EClass.sources.charas.rows
		where c.tag.Contains("party")
		select c)
		{
			if (row.LV < maxLv * 2)
			{
				Point randomPoint = EClass._map.bounds.GetRandomPoint();
				if (!randomPoint.HasChara && !randomPoint.HasBlock)
				{
					bool flag = true;
					using (List<Chara>.Enumerator enumerator2 = EClass.pc.party.members.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.id == row.id)
							{
								flag = false;
							}
						}
					}
					if (flag)
					{
						Chara c3 = CharaGen.Create(row.id, -1);
						this.Spawn(c3, randomPoint);
					}
				}
			}
		}
		for (int j = 0; j < 50; j++)
		{
			Point randomPoint2 = EClass._map.bounds.GetRandomPoint();
			if (!randomPoint2.HasChara && !randomPoint2.HasBlock)
			{
				Chara c2 = CharaGen.CreateFromFilter("c_neutral", maxLv / 2, -1);
				this.Spawn(c2, randomPoint2);
			}
		}
		foreach (Thing thing in EClass._map.things)
		{
			thing.isNPCProperty = true;
		}
	}

	public void Spawn(Chara c, Point p)
	{
		EClass._zone.AddCard(c, p);
		if (c.hostility < Hostility.Neutral)
		{
			c.c_originalHostility = (c.hostility = Hostility.Neutral);
		}
	}

	public override ZoneInstance.Status OnReachTimeLimit()
	{
		Msg.Say("party_end", this.questMusic.score.ToString() ?? "", null, null, null);
		Msg.Say("party_end2", this.questMusic.sumMoney.ToString() ?? "", null, null, null);
		if (this.questMusic.score < this.questMusic.destScore)
		{
			return ZoneInstance.Status.Fail;
		}
		return ZoneInstance.Status.Success;
	}
}
