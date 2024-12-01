public class ConInvisibility : BaseBuff
{
	public override bool SyncRide => true;

	public override bool ShouldRefresh => true;

	public override void OnRefresh()
	{
		owner.isHidden = true;
	}
}
