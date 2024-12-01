public class ConTelepathy : BaseBuff
{
	public override bool ShouldRefresh => true;

	public override void OnRefresh()
	{
		owner.hasTelepathy = true;
	}
}
