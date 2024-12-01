using Newtonsoft.Json;

public class ZoneInstanceBout : ZoneInstance
{
	[JsonProperty]
	public int uidTarget;

	[JsonProperty]
	public int targetX;

	[JsonProperty]
	public int targetZ;

	public override ZoneTransition.EnterState ReturnState => ZoneTransition.EnterState.Exact;

	public override void OnLeaveZone()
	{
		Chara chara = EClass.game.cards.Find(uidTarget);
		if (chara != null)
		{
			if (chara.isDead)
			{
				chara.Revive();
			}
			chara.HealAll();
			Hostility c_originalHostility = (chara.hostility = Hostility.Friend);
			chara.c_originalHostility = c_originalHostility;
			chara.SetEnemy();
			chara.SetAI(new NoGoal());
			chara.MoveZone(EClass.game.spatials.Find(uidZone), new ZoneTransition
			{
				state = ZoneTransition.EnterState.Exact,
				x = targetX,
				z = targetZ
			});
		}
	}
}
