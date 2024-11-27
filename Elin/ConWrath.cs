using System;

public class ConWrath : BaseDebuff
{
	public override int GetPhase()
	{
		return 0;
	}

	public override bool AllowMultipleInstance
	{
		get
		{
			return true;
		}
	}

	public override bool CanStack(Condition c)
	{
		return false;
	}

	public override void OnRemoved()
	{
		Thing thing = this.owner.things.Find<TraitPunishBall>();
		if (thing != null)
		{
			thing.Destroy();
		}
	}
}
