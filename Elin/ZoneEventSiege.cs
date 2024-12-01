using System.Collections.Generic;
using Newtonsoft.Json;

public class ZoneEventSiege : ZoneEvent
{
	[JsonProperty]
	public List<int> uids = new List<int>();

	public List<Chara> members = new List<Chara>();

	public override string id => "trial_siege";

	public override Playlist playlist => EClass.Sound.playlistBattle;

	public virtual Chara CreateChara()
	{
		return CharaGen.CreateFromFilter("c_wilds");
	}

	public override void OnFirstTick()
	{
		EClass.player.stats.sieges++;
		Msg.Say("startSiege");
		EClass._zone.RefreshBGM();
		Point randomEdge = EClass._map.GetRandomEdge();
		for (int i = 0; i < 10; i++)
		{
			Chara chara = CreateChara();
			EClass._zone.AddCard(chara, EClass._map.GetRandomSurface(randomEdge.x, randomEdge.z, 6));
			chara.hostility = Hostility.Enemy;
			members.Add(chara);
			uids.Add(chara.uid);
		}
		Thing t = ThingGen.Create("torch");
		EClass._zone.AddCard(t, randomEdge);
		if (members.Count != 0)
		{
			return;
		}
		foreach (int uid in uids)
		{
			foreach (Chara chara2 in EClass._map.charas)
			{
				if (chara2.uid == uid)
				{
					members.Add(chara2);
				}
			}
		}
	}

	public override void OnTickRound()
	{
		bool flag = true;
		foreach (Chara member in members)
		{
			if (member.IsAliveInCurrentZone)
			{
				if (member.ai is GoalIdle)
				{
					member.SetAI(new GoalSiege());
				}
				flag = false;
			}
		}
		if (flag || EClass.Branch.IsAllDead())
		{
			Kill();
		}
	}

	public override void OnKill()
	{
		Msg.Say("endSiege");
		EClass._zone.RefreshBGM();
	}
}
