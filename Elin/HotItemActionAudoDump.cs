public class HotItemActionAudoDump : HotAction
{
	public override string Id => "AutoDump";

	public override void Perform()
	{
		TaskDump.TryPerform();
	}
}
