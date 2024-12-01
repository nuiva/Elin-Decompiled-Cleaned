public class ConLevitate : BaseBuff
{
	public override bool SyncRide => true;

	public override bool ShouldRefresh => true;

	public override void OnRefresh()
	{
		owner._isLevitating = true;
	}
}
