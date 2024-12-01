public class ConExcommunication : BaseDebuff
{
	public override bool ShouldRefresh => true;

	public override void OnRefresh()
	{
		owner.RefreshFaithElement();
	}

	public override void OnRemoved()
	{
		owner.RefreshFaithElement();
	}
}
