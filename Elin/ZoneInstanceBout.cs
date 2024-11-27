using System;
using Newtonsoft.Json;

public class ZoneInstanceBout : ZoneInstance
{
	public override ZoneTransition.EnterState ReturnState
	{
		get
		{
			return ZoneTransition.EnterState.Exact;
		}
	}

	public override void OnLeaveZone()
	{
		Chara chara = EClass.game.cards.Find(this.uidTarget);
		if (chara == null)
		{
			return;
		}
		if (chara.isDead)
		{
			chara.Revive(null, false);
		}
		chara.HealAll();
		chara.c_originalHostility = (chara.hostility = Hostility.Friend);
		chara.SetEnemy(null);
		chara.SetAI(new NoGoal());
		chara.MoveZone(EClass.game.spatials.Find(this.uidZone), new ZoneTransition
		{
			state = ZoneTransition.EnterState.Exact,
			x = this.targetX,
			z = this.targetZ
		});
	}

	[JsonProperty]
	public int uidTarget;

	[JsonProperty]
	public int targetX;

	[JsonProperty]
	public int targetZ;
}
