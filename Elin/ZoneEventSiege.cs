using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ZoneEventSiege : ZoneEvent
{
	public override string id
	{
		get
		{
			return "trial_siege";
		}
	}

	public override Playlist playlist
	{
		get
		{
			return EClass.Sound.playlistBattle;
		}
	}

	public virtual Chara CreateChara()
	{
		return CharaGen.CreateFromFilter("c_wilds", -1, -1);
	}

	public override void OnFirstTick()
	{
		EClass.player.stats.sieges++;
		Msg.Say("startSiege");
		EClass._zone.RefreshBGM();
		Point randomEdge = EClass._map.GetRandomEdge(3);
		for (int i = 0; i < 10; i++)
		{
			Chara chara = this.CreateChara();
			EClass._zone.AddCard(chara, EClass._map.GetRandomSurface(randomEdge.x, randomEdge.z, 6, true, false));
			chara.hostility = Hostility.Enemy;
			this.members.Add(chara);
			this.uids.Add(chara.uid);
		}
		Thing t = ThingGen.Create("torch", -1, -1);
		EClass._zone.AddCard(t, randomEdge);
		if (this.members.Count == 0)
		{
			foreach (int num in this.uids)
			{
				foreach (Chara chara2 in EClass._map.charas)
				{
					if (chara2.uid == num)
					{
						this.members.Add(chara2);
					}
				}
			}
		}
	}

	public override void OnTickRound()
	{
		bool flag = true;
		foreach (Chara chara in this.members)
		{
			if (chara.IsAliveInCurrentZone)
			{
				if (chara.ai is GoalIdle)
				{
					chara.SetAI(new GoalSiege());
				}
				flag = false;
			}
		}
		if (flag || EClass.Branch.IsAllDead())
		{
			base.Kill();
		}
	}

	public override void OnKill()
	{
		Msg.Say("endSiege");
		EClass._zone.RefreshBGM();
	}

	[JsonProperty]
	public List<int> uids = new List<int>();

	public List<Chara> members = new List<Chara>();
}
