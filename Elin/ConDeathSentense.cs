public class ConDeathSentense : BaseDebuff
{
	public override void Tick()
	{
		Mod(-1);
		if (base.value <= 0 && owner.IsAliveInCurrentZone)
		{
			owner.Die(null, null, AttackSource.DeathSentense);
		}
	}
}
