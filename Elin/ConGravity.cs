public class ConGravity : BaseDebuff
{
	public override bool ShouldRefresh => true;

	public override void OnRefresh()
	{
		owner._isLevitating = false;
		if (EClass.core.IsGameStarted)
		{
			owner.RefreshSpeed();
		}
	}

	public override int GetPhase()
	{
		return 0;
	}
}
