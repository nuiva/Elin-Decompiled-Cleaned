public class ConTransmute : BaseBuff
{
	public override bool IsToggle => true;

	public override void Tick()
	{
		if (owner.host == null)
		{
			base.Tick();
		}
	}

	public override void OnStart()
	{
		owner._CreateRenderer();
	}

	public override void OnRemoved()
	{
		owner._CreateRenderer();
	}
}
