public class ConWrath : BaseDebuff
{
	public override bool AllowMultipleInstance => true;

	public override int GetPhase()
	{
		return 0;
	}

	public override bool CanStack(Condition c)
	{
		return false;
	}

	public override void OnRemoved()
	{
		owner.things.Find<TraitPunishBall>()?.Destroy();
	}
}
