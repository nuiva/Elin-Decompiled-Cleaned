using UnityEngine;

public class ConBleed : BadCondition
{
	public override Emo2 EmoIcon => Emo2.bleeding;

	public override void Tick()
	{
		owner.DamageHP(EClass.rnd(Mathf.Clamp(owner.hp * (1 + base.value / 4) / 100 + 3, 1, (int)Mathf.Sqrt(owner.MaxHP) + 100)), AttackSource.Condition);
		owner.AddBlood();
		Mod(-1);
	}
}
