using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SlaverData : EClass
{
	public void TryRefresh(Chara owner)
	{
		if (!EClass.world.date.IsExpired(this.dateRefresh))
		{
			return;
		}
		this.list.Clear();
		int num = 4 + EClass.rnd(4);
		for (int i = 0; i < num; i++)
		{
			Chara item;
			if (owner.trait.SlaverType == SlaverType.Animal)
			{
				if (EClass._zone is Zone_Yowyn && EClass.rnd(3) != 0)
				{
					item = CharaGen.Create(SpawnListChara.Get("slaver_horse", (SourceChara.Row s) => s.id == "horse_yowyn").Select(-1, -1).id, -1);
				}
				else
				{
					item = CharaGen.Create(SpawnListChara.Get("slaver_animal", (SourceChara.Row s) => EClass.sources.races.map[s.race].tag.Contains("animal")).Select(-1, -1).id, -1);
				}
			}
			else
			{
				item = CharaGen.Create(SpawnListChara.Get("slaver_human", (SourceChara.Row s) => EClass.sources.races.map[s.race].tag.Contains("human")).Select(-1, -1).id, -1);
			}
			this.list.Add(item);
		}
		this.dateRefresh = EClass.world.date.GetRaw(168);
		owner.isRestocking = true;
	}

	[JsonProperty]
	public List<Chara> list = new List<Chara>();

	[JsonProperty]
	public int dateRefresh;
}
