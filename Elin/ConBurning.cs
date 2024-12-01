public class ConBurning : BadCondition
{
	public override int GetPhase()
	{
		return 0;
	}

	public override void Tick()
	{
		if (base.value > 10)
		{
			base.value = 10;
		}
		if (EClass.rnd(2) == 0)
		{
			owner.PlayEffect("fire_step");
			owner.PlaySound("fire_step");
			owner.DamageHP(1 + EClass.rnd(owner.MaxHP / (owner.IsPowerful ? 200 : 20) + 3), AttackSource.Condition);
		}
		Mod(-1);
	}
}
