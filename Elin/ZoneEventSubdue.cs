using System;
using Newtonsoft.Json;

public class ZoneEventSubdue : ZoneEventQuest
{
	public override bool CountEnemy
	{
		get
		{
			return true;
		}
	}

	public override bool WarnBoss
	{
		get
		{
			return true;
		}
	}

	public override void OnVisit()
	{
		if (EClass.game.isLoading)
		{
			return;
		}
		EClass._zone._dangerLv = base.quest.DangerLv;
		base.Spawn(4 + base.quest.difficulty * 2 + EClass.rnd(5));
		base.AggroEnemy(15);
		EClass._zone.SetBGM(102, true);
		this.max = this.enemies.Count;
	}

	public override void OnCharaDie(Chara c)
	{
		this.CheckClear();
	}

	public override void _OnTickRound()
	{
		base.AggroEnemy(100);
		this.CheckClear();
	}

	public void CheckClear()
	{
		if (EClass._zone.instance.status == ZoneInstance.Status.Success || EClass._zone.instance.status == ZoneInstance.Status.Fail)
		{
			return;
		}
		this.enemies.ForeachReverse(delegate(int id)
		{
			Chara chara = EClass._map.FindChara(id);
			if (chara == null || !chara.IsAliveInCurrentZone || !EClass.pc.IsHostile(chara))
			{
				this.enemies.Remove(id);
			}
		});
		if (this.enemies.Count == 0)
		{
			EClass._zone.instance.status = ZoneInstance.Status.Success;
			Msg.Say("subdue_complete");
			EClass._zone.SetBGM(-1, true);
			SE.Play("Jingle/fanfare");
		}
	}

	[JsonProperty]
	public int max;
}
