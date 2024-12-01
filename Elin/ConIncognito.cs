public class ConIncognito : BaseBuff
{
	public override void OnStart()
	{
		EClass._zone.ResetHostility();
		EClass._zone.RefreshCriminal();
	}

	public override void OnRemoved()
	{
		EClass._zone.RefreshCriminal();
	}
}
