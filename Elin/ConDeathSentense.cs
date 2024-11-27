using System;

public class ConDeathSentense : BaseDebuff
{
	public override void Tick()
	{
		base.Mod(-1, false);
		if (base.value <= 0 && this.owner.IsAliveInCurrentZone)
		{
			this.owner.Die(null, null, AttackSource.DeathSentense);
		}
	}
}
